namespace Alligator.Solver.Algorithms
{
    internal class CacheTables<TPosition, TMove> : ICacheTables<TPosition, TMove>
    {
        // TODO: cache implementations!

        public void AddTransposition(TPosition position, Transposition<TMove> transposition)
        {
        }

        public void AddValue(TPosition position, int value)
        {
        }

        public bool TryGetTransposition(TPosition position, out Transposition<TMove> transposition)
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