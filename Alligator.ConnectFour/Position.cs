using Alligator.Solver;

namespace Alligator.ConnectFour
{
    /// <summary>
    /// Connect Four board — 7 columns × 6 rows, discs drop from the top.
    /// Uses Zobrist hashing for fast incremental hash updates.
    /// </summary>
    public class Position : IPosition<Drop>
    {
        public const int Columns = 7;
        public const int Rows = 6;
        public const int WinLength = 4;

        private readonly Disk[,] board;   // [row, col] — row 0 is the bottom
        private readonly int[] heights;   // heights[col] = number of discs in that column

        private Disk nextDisk;
        private ulong identifier;

        private readonly Stack<(int Column, ulong PrevIdentifier)> history;

        // Zobrist: 2 colors × Rows × Columns random values
        private static readonly ulong[,,] zobristTable;
        private static readonly ulong zobristTurn;

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
        /// Heuristic evaluation: counts open windows of 2 and 3.
        /// Returns value from current player's perspective.
        /// </summary>
        private sbyte Evaluate()
        {
            int score = 0;

            // Center column preference — discs in center have more potential
            for (int r = 0; r < Rows; r++)
            {
                if (board[r, Columns / 2] == Disk.Red) score += 3;
                else if (board[r, Columns / 2] == Disk.Yellow) score -= 3;
            }

            // Evaluate all windows of 4
            score += EvaluateAllWindows();

            // Clamp to sbyte range (excluding MaxValue which is reserved for wins)
            score = Math.Clamp(score, -126, 126);

            // Return from current player's perspective
            return nextDisk == Disk.Red ? (sbyte)score : (sbyte)-score;
        }

        private int EvaluateAllWindows()
        {
            int score = 0;

            // Horizontal windows
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c <= Columns - WinLength; c++)
                {
                    score += EvaluateWindow(r, c, 0, 1);
                }
            }

            // Vertical windows
            for (int c = 0; c < Columns; c++)
            {
                for (int r = 0; r <= Rows - WinLength; r++)
                {
                    score += EvaluateWindow(r, c, 1, 0);
                }
            }

            // Diagonal (bottom-left to top-right)
            for (int r = 0; r <= Rows - WinLength; r++)
            {
                for (int c = 0; c <= Columns - WinLength; c++)
                {
                    score += EvaluateWindow(r, c, 1, 1);
                }
            }

            // Anti-diagonal (top-left to bottom-right)
            for (int r = WinLength - 1; r < Rows; r++)
            {
                for (int c = 0; c <= Columns - WinLength; c++)
                {
                    score += EvaluateWindow(r, c, -1, 1);
                }
            }

            return score;
        }

        private int EvaluateWindow(int startRow, int startCol, int dRow, int dCol)
        {
            int red = 0, yellow = 0;

            for (int i = 0; i < WinLength; i++)
            {
                Disk d = board[startRow + i * dRow, startCol + i * dCol];
                if (d == Disk.Red) red++;
                else if (d == Disk.Yellow) yellow++;
            }

            if (red > 0 && yellow > 0) return 0; // Blocked window

            if (red == 3) return 5;
            if (red == 2) return 2;
            if (yellow == 3) return -5;
            if (yellow == 2) return -2;

            return 0;
        }

        private static ulong NextRandomULong(Random rng)
        {
            byte[] buffer = new byte[8];
            rng.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }
    }
}
