namespace Alligator.SixMaking.Model
{
    public sealed class Placement : Step
    {
        private const int FromIndex = -1;
        private const int DiskCount = 1;

        public Placement(int to)
            : base(FromIndex, to, DiskCount)
        {
        }

        public override string ToString()
        {
            return string.Format("-->{0}", CellToString(to));
        }
    }
}