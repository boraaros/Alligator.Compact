namespace Alligator.Solver.Algorithms
{
    internal interface IHeuristicTables<TMove>
    {
        IEnumerable<TMove> GetKillerSteps(int depth);
        void StoreBetaCutOff(TMove move, int depth);
    }
}