using Alligator.Solver;

namespace Alligator.Test
{
    internal class UniformTreeRules : IRules<UniformTreePosition, int>
    {
        private readonly IDictionary<ulong, sbyte> valuesById;
        private readonly uint degree;

        public UniformTreeRules(IDictionary<ulong, sbyte> valuesById, uint degree)
        {
            this.valuesById = valuesById ?? throw new ArgumentNullException(nameof(valuesById));
            this.degree = degree;
        }

        public UniformTreePosition InitialPosition()
        {
            return new UniformTreePosition(valuesById, degree);
        }

        public bool IsGoal(UniformTreePosition position)
        {
            return position.Value == sbyte.MinValue || position.Value == sbyte.MaxValue;
        }

        public IEnumerable<int> LegalStepsAt(UniformTreePosition position)
        {
            for (int i = 0; i < degree; i++)
            {
                position.Take(i);
                if (valuesById.ContainsKey(position.Identifier))
                {
                    position.TakeBack();
                    yield return i;
                    
                }
                else
                {
                    position.TakeBack();
                }
            }
        }
    }
}