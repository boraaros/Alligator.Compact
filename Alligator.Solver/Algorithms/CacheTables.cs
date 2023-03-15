namespace Alligator.Solver.Algorithms
{
    internal class CacheTables<TPosition, TStep> : ICacheTables<TPosition, TStep>
        where TPosition : IPosition<TStep>
    {
        private readonly IDictionary<ulong, int> evaluationTable;
        private readonly IDictionary<ulong, Transposition<TStep>> transpositionTable;

        public CacheTables()
        {
            evaluationTable = new Dictionary<ulong, int>();
            transpositionTable = new Dictionary<ulong, Transposition<TStep>>();
        }

        public void AddTransposition(TPosition position, Transposition<TStep> transposition)
        {
            transpositionTable.TryAdd(position.Identifier, transposition);
        }

        public void AddValue(TPosition position, int value)
        {
            evaluationTable.TryAdd(position.Identifier, value);
        }

        public bool TryGetTransposition(TPosition position, out Transposition<TStep> transposition)
        {
            return transpositionTable.TryGetValue(position.Identifier, out transposition);
        }

        public bool TryGetValue(TPosition position, out int value)
        {
            return evaluationTable.TryGetValue(position.Identifier, out value);
        }
    }
}