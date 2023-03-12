using Alligator.Solver;

namespace Alligator.Test
{
    internal class UniformTreePosition : IPosition<int>
    {
        private readonly IDictionary<ulong, sbyte> valuesById;
        private readonly uint degree;

        public ulong Identifier { get; private set; }
        public sbyte Value => valuesById[Identifier];

        public UniformTreePosition(IDictionary<ulong, sbyte> valuesById, uint degree)
        {
            this.valuesById = valuesById ?? throw new ArgumentNullException(nameof(valuesById));
            this.degree = degree;
            Identifier = 0L;
        }

        public void Take(int move)
        {
            Identifier = degree * Identifier + (ulong)move + 1;
        }

        public void TakeBack()
        {
            Identifier = (Identifier - 1) / degree;
        }
    }
}