namespace Alligator.SixMaking.Model
{
    public sealed class Movement : Step
    {
        public Movement(int from, int to, int count)
            : base(from, to, count)
        {
        }

        public override string ToString()
        {
            return string.Format("{1}--{0}->{2}", count, CellToString(from), CellToString(to));
        }
    }
}