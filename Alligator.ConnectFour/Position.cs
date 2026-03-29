using Alligator.Solver;

namespace Alligator.ConnectFour
{
    public class Position : IPosition<Drop>
    {
        public const int Columns = 7;
        public const int Rows = 6;
        public const int WinLength = 4;

        private readonly Disk[,] board;   // [row, col] — row 0 is the bottom
        private readonly int[] heights;

        private Disk nextDisk;
        private ulong identifier;

        private readonly Stack<(int Column, ulong PrevIdentifier)> history;

        private static readonly ulong[,,] zobristTable;
        private static readonly ulong zobristTurn;

        private static readonly int[,] cellWeights =
        {
            {  8,  9, 10, 12, 10,  9,  8 },
            {  7,  8,  9, 11,  9,  8,  7 },
            {  6,  7,  8, 10,  8,  7,  6 },
            {  5,  6,  7,  9,  7,  6,  5 },
            {  4,  5,  6,  8,  6,  5,  4 },
            {  3,  4,  5,  7,  5,  4,  3 }
        };

        static Position()
        {
            var rng = new Random(42);
            zobristTable = new ulong[2, Rows, Columns];
            for (int color = 0; color < 2; color++)
            {
                for (int r = 0; r < Rows; r++)
                {
                    for (int c = 0; c < Columns; c++)
                    {
                        zobristTable[color, r, c] = NextRandomULong(rng);
                    }
                }
            }
            zobristTurn = NextRandomULong(rng);
        }

        public Position()
        {
            board = new Disk[Rows, Columns];
            heights = new int[Columns];
            nextDisk = Disk.Red;
            identifier = 0UL;
            history = new Stack<(int, ulong)>();
        }

        public ulong Identifier => identifier;

        public Disk Next => nextDisk;

        public int MoveCount => history.Count;

        // Always from Red's perspective (absolute convention expected by solver)
        public sbyte Value => Evaluate();

        public void Take(Drop step)
        {
            int col = step.Column;
            int row = heights[col];

            history.Push((col, identifier));

            board[row, col] = nextDisk;
            heights[col] = row + 1;

            int colorIndex = nextDisk == Disk.Red ? 0 : 1;
            identifier ^= zobristTable[colorIndex, row, col];
            identifier ^= zobristTurn;

            nextDisk = nextDisk == Disk.Red ? Disk.Yellow : Disk.Red;
        }

        public void TakeBack()
        {
            var (col, prevId) = history.Pop();

            int row = heights[col] - 1;
            board[row, col] = Disk.None;
            heights[col] = row;

            identifier = prevId;
            nextDisk = nextDisk == Disk.Red ? Disk.Yellow : Disk.Red;
        }

        public int HeightAt(int col) => heights[col];

        public Disk DiskAt(int row, int col) => board[row, col];

        public bool IsBoardFull => history.Count == Rows * Columns;

        public bool HasWinner()
        {
            if (history.Count == 0)
            {
                return false;
            }

            var (col, _) = history.Peek();
            int row = heights[col] - 1;
            Disk disk = board[row, col];

            return CountDirection(row, col, disk, 0, 1) + CountDirection(row, col, disk, 0, -1) >= WinLength - 1
                || CountDirection(row, col, disk, 1, 0) + CountDirection(row, col, disk, -1, 0) >= WinLength - 1
                || CountDirection(row, col, disk, 1, 1) + CountDirection(row, col, disk, -1, -1) >= WinLength - 1
                || CountDirection(row, col, disk, 1, -1) + CountDirection(row, col, disk, -1, 1) >= WinLength - 1;
        }

        private int CountDirection(int row, int col, Disk disk, int dRow, int dCol)
        {
            int count = 0;
            int r = row + dRow;
            int c = col + dCol;

            while (r >= 0 && r < Rows && c >= 0 && c < Columns && board[r, c] == disk)
            {
                count++;
                r += dRow;
                c += dCol;
            }

            return count;
        }

        private sbyte Evaluate()
        {
            int score = 0;

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    if (board[r, c] == Disk.Red) score += cellWeights[r, c];
                    else if (board[r, c] == Disk.Yellow) score -= cellWeights[r, c];
                }
            }

            int redThreats = 0, yellowThreats = 0;
            score += EvaluateAllWindows(ref redThreats, ref yellowThreats);

            if (redThreats >= 2) score += 80;
            if (yellowThreats >= 2) score -= 80;

            // sbyte.MaxValue is reserved for wins
            score = Math.Clamp(score, -126, 126);

            return (sbyte)score;
        }

        private int EvaluateAllWindows(ref int redThreats, ref int yellowThreats)
        {
            int score = 0;

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c <= Columns - WinLength; c++)
                {
                    score += EvaluateWindow(r, c, 0, 1, ref redThreats, ref yellowThreats);
                }
            }

            for (int c = 0; c < Columns; c++)
            {
                for (int r = 0; r <= Rows - WinLength; r++)
                {
                    score += EvaluateWindow(r, c, 1, 0, ref redThreats, ref yellowThreats);
                }
            }

            for (int r = 0; r <= Rows - WinLength; r++)
            {
                for (int c = 0; c <= Columns - WinLength; c++)
                {
                    score += EvaluateWindow(r, c, 1, 1, ref redThreats, ref yellowThreats);
                }
            }

            for (int r = WinLength - 1; r < Rows; r++)
            {
                for (int c = 0; c <= Columns - WinLength; c++)
                {
                    score += EvaluateWindow(r, c, -1, 1, ref redThreats, ref yellowThreats);
                }
            }

            return score;
        }

        private int EvaluateWindow(int startRow, int startCol, int dRow, int dCol,
            ref int redThreats, ref int yellowThreats)
        {
            int red = 0, yellow = 0;
            int minDistance = int.MaxValue;

            for (int i = 0; i < WinLength; i++)
            {
                int r = startRow + i * dRow;
                int c = startCol + i * dCol;
                Disk d = board[r, c];
                if (d == Disk.Red) red++;
                else if (d == Disk.Yellow) yellow++;
                else
                {
                    int distance = r - heights[c];
                    if (distance < minDistance) minDistance = distance;
                }
            }

            if (red > 0 && yellow > 0) return 0;

            if (red == 3)
            {
                if (minDistance == 0) redThreats++;
                return ScoreThreeInRow(minDistance);
            }
            if (yellow == 3)
            {
                if (minDistance == 0) yellowThreats++;
                return -ScoreThreeInRow(minDistance);
            }
            if (red == 2) return ScoreTwoInRow(minDistance);
            if (yellow == 2) return -ScoreTwoInRow(minDistance);

            return 0;
        }

        private static int ScoreThreeInRow(int distanceToPlayable)
        {
            return distanceToPlayable switch
            {
                0 => 50,
                1 => 20,
                2 => 8,
                _ => 3
            };
        }

        private static int ScoreTwoInRow(int distanceToPlayable)
        {
            return distanceToPlayable switch
            {
                0 => 8,
                1 => 4,
                _ => 2
            };
        }

        private static ulong NextRandomULong(Random rng)
        {
            byte[] buffer = new byte[8];
            rng.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }
    }
}
