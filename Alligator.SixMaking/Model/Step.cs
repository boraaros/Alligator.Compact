namespace Alligator.SixMaking.Model
{
    public abstract class Step
    {
        protected readonly int from;
        protected readonly int to;
        protected readonly int count;

        private int index;

        protected Step(int from, int to, int count)
        {
            this.from = from;
            this.to = to;
            this.count = count;
            index = 125 * (from + 1) + 5 * to + (count - 1);
        }

        public int From
        {
            get { return from; }
        }

        public int To
        {
            get { return to; }
        }

        public int Count
        {
            get { return count; }
        }

        public override bool Equals(object obj)
        {
            if (obj is not Step other)
            {
                return false;
            }
            return index == other.index;
        }

        public override int GetHashCode()
        {
            return index;
        }

        protected static string CellToString(int cell)
        {
            return string.Format("[{0},{1}]", cell / Constants.BoardSize, cell % Constants.BoardSize);
        }
    }
}