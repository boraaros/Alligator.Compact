namespace Alligator.Solver.Algorithms
{
    internal class CacheTables<TPosition, TStep> : ICacheTables<TPosition, TStep>
    {
        // TODO: cache implementations!

        public void AddTransposition(TPosition position, Transposition<TStep> transposition)
        {
        }

        public void AddValue(TPosition position, int value)
        {
        }

        public bool TryGetTransposition(TPosition position, out Transposition<TStep> transposition)
        {
            transposition = null;
            return false;
        }

        public bool TryGetValue(TPosition position, out int value)
        {
            value = 0;
            return false;
        }
    }
}