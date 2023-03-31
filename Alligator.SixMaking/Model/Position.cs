namespace Alligator.SixMaking.Model
{
    public class Position : IPosition
    {
        private readonly Disk[,] board;
        private readonly int[] heights;
        private Disk winner;
        
        public Disk Next { get; private set; }
        public IList<Step> History { get; }

        private const int HashParamsLength = 251;
        private readonly IHashing hashing = new ZobristHashing(HashParamsLength);

        public Position()
        {
            board = new Disk[Constants.BoardSize * Constants.BoardSize, (Constants.WinnerHeight - 1) * (Constants.WinnerHeight - 1)];
            heights = new int[Constants.BoardSize * Constants.BoardSize];
            winner = Disk.None;
            History = new List<Step>();
            Next = Disk.Red;
        }

        public Position(IList<Step> history)
            : this()
        {
            foreach (Step step in history)
            {
                Take(step);
            }
        }

        public bool IsOver => winner != Disk.None;

        public Step LastStep => History.LastOrDefault();

        public sbyte Value => StaticEvaluate();

        public ulong Identifier
        {
            get { return hashing.HashCode + /*(LastStep != null ? (ulong)LastStep.Identifier :*/ 0UL; } // TODO: resolve this!
        }

        public int ColumnHeightAt(int position)
        {
            return heights[position];
        }

        public Disk DiskAt(int position, int height)
        {
            return board[position, height];
        }

        public void Take(Step step)
        {
            if (winner != Disk.None)
            {
                throw new InvalidOperationException("The game is already over");
            }
            if (step.From == -1)
            {
                board[step.To, 0] = Next;
                heights[step.To] = 1;
                hashing.Modify(ZobristIndex(step.To, 0, Next));
            }
            else
            {
                Move(step.From, step.To, step.Count);
            }
            History.Add(step);
            ChangeNext();
        }

        public void TakeBack()
        {
            if (History.Count == 0)
            {
                throw new InvalidOperationException("There is no step in history yet");
            }
            ChangeNext();
            var lastStep = History[History.Count - 1];
            if (lastStep.From == -1)
            {
                board[lastStep.To, 0] = Disk.None;
                heights[lastStep.To] = 0;
                hashing.Modify(ZobristIndex(lastStep.To, 0, Next));
            }
            else
            {
                Move(lastStep.To, lastStep.From, lastStep.Count);
            }
            History.RemoveAt(History.Count - 1);   
        }

        private void Move(int from, int to, int count)
        {
            IList<int> indices = new List<int>();
            for (int i = heights[from] - count; i < heights[from]; i++)
            {
                var zi1 = ZobristIndex(to, heights[to], board[from, i]);
                if (zi1 != -1)
                    indices.Add(zi1);

                var zi2 = ZobristIndex(from, i, board[from, i]);
                if (zi2 != -1)
                    indices.Add(zi2);

                board[to, heights[to]++] = board[from, i];
                board[from, i] = Disk.None;
            }
            heights[from] -= count;
            winner = heights[to] > 5 ? board[to, heights[to] - 1] : Disk.None;
            hashing.Modify(indices.ToArray());
        }

        private void ChangeNext()
        {
            Next = 3 - Next;
            hashing.Modify(HashParamsLength - 1);
        }

        private int ZobristIndex(int position, int height, Disk disk)
        {
            if (height > 5)
            {
                return -1;
            }
            if (disk == Disk.Red)
            {
                return 5 * position + height;
            }
            if (disk == Disk.Yellow)
            {
                return 125 + 5 * position + height;
            }
            return -1;
        }

        public sbyte StaticEvaluate()
        {
            int value = 0;

            for (int i = 0; i < 25; i++)
            {
                var h = ColumnHeightAt(i);

                if (h == 0) continue;

                if (DiskAt(i, h - 1) == Disk.Red)
                {
                    value += h;
                }
                else
                {
                    value -= h;
                }
            }

            if (value > sbyte.MaxValue || value < -sbyte.MaxValue)
            {
                throw new InvalidOperationException("invalid value");
            }

            return (sbyte)value;
        }
    }
}