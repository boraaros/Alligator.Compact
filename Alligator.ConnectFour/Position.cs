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

        // Positional weights: columnWeight[c] + (Rows - 1 - row).
        // Column component = number of horizontal 4-windows through that column: {3,4,5,7,5,4,3}.
        // Gravity component = lower rows score higher because they are filled first
        // and threats there are more immediate.
        // This avoids the "stacking penalty" where the opponent benefits from placing
        // on top of your center disc (upper cells no longer outweigh lower ones).
        private static readonly int[,] cellWeights =
        {
            {  8,  9, 10, 12, 10,  9,  8 },   // row 0 (bottom)
            {  7,  8,  9, 11,  9,  8,  7 },
            {  6,  7,  8, 10,  8,  7,  6 },
            {  5,  6,  7,  9,  7,  6,  5 },
            {  4,  5,  6,  8,  6,  5,  4 },
            {  3,  4,  5,  7,  5,  4,  3 }    // row 5 (top)
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

        /// <summary>
        /// Static evaluation from the current player's perspective.
        /// Positive = good for the player to move.
        /// </summary>
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

        /// <summary>
        /// Checks if the last move created a 4-in-a-row.
        /// </summary>
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

        /// <summary>
        /// Heuristic evaluation: counts open windows of 2 and 3, with threat awareness.
        /// A 3-in-a-row where the completing cell is immediately playable scores much higher.
        /// Returns value always from Red's perspective (absolute convention expected by solver).
        /// </summary>
        private sbyte Evaluate()
        {
            int score = 0;

            // Positional quality: weight each disc by how many winning lines pass through its cell.
            // A disc at the center (weight 13) is far more valuable than one in the corner (weight 3).
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Columns; c++)
                {
                    if (board[r, c] == Disk.Red) score += cellWeights[r, c];
                    else if (board[r, c] == Disk.Yellow) score -= cellWeights[r, c];
                }
            }

            // Evaluate all windows of 4 with threat detection
            int redThreats = 0, yellowThreats = 0;
            score += EvaluateAllWindows(ref redThreats, ref yellowThreats);

            // Double threat bonus: two simultaneous immediate threats = opponent can only block one = forced win
            if (redThreats >= 2) score += 80;
            if (yellowThreats >= 2) score -= 80;

            // Clamp to sbyte range (excluding MaxValue which is reserved for wins)
            score = Math.Clamp(score, -126, 126);

            // Return from Red's perspective (absolute value convention)
            return (sbyte)score;
        }

        private int EvaluateAllWindows(ref int redThreats, ref int yellowThreats)
        {
            int score = 0;

            // Horizontal windows
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c <= Columns - WinLength; c++)
                {
                    score += EvaluateWindow(r, c, 0, 1, ref redThreats, ref yellowThreats);
                }
            }

            // Vertical windows
            for (int c = 0; c < Columns; c++)
            {
                for (int r = 0; r <= Rows - WinLength; r++)
                {
                    score += EvaluateWindow(r, c, 1, 0, ref redThreats, ref yellowThreats);
                }
            }

            // Diagonal (bottom-left to top-right)
            for (int r = 0; r <= Rows - WinLength; r++)
            {
                for (int c = 0; c <= Columns - WinLength; c++)
                {
                    score += EvaluateWindow(r, c, 1, 1, ref redThreats, ref yellowThreats);
                }
            }

            // Anti-diagonal (top-left to bottom-right)
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

            if (red > 0 && yellow > 0) return 0; // Blocked window

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
                0 => 50,    // Immediately completable — huge threat
                1 => 20,    // One disc away — very dangerous, hard to prevent
                2 => 8,     // Two discs away — significant potential
                _ => 3      // Far away — minor long-term potential
            };
        }

        private static int ScoreTwoInRow(int distanceToPlayable)
        {
            return distanceToPlayable switch
            {
                0 => 8,     // Can extend immediately
                1 => 4,     // Close to extending
                _ => 2      // Far from relevant
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
