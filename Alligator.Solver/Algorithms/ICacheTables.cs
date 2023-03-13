namespace Alligator.Solver.Algorithms
{
    internal interface ICacheTables<TPosition, TStep>
    {
        void AddValue(TPosition position, int value);
        bool TryGetValue(TPosition position, out int value);
        void AddTransposition(TPosition position, Transposition<TStep> transposition);
        bool TryGetTransposition(TPosition position, out Transposition<TStep> transposition);
    }
}