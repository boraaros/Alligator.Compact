namespace Alligator.SixMaking.Model
{
    public abstract class Step
    {
        protected readonly int index;
        protected readonly int from;
        protected readonly int to;
        protected readonly int count;

        protected Step(int from, int to, int count, int index)
        {
            this.from = from;
            this.to = to;
            this.count = count;
            this.index = index;
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
            return index == other.GetHashCode();
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