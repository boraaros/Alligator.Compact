using Alligator.SixMaking.Model;
using Alligator.Solver.Algorithms;

namespace Alligator.Benchmark
{
    internal class MeasurableHeuristicTables : IHeuristicTables<Step>
    {
        public int GetKillerStepsCallCount = 0;
        public int StoreBetaCutOffCallCount = 0;

        private readonly IHeuristicTables<Step> heuristicTables;

        public MeasurableHeuristicTables(IHeuristicTables<Step> heuristicTables)
        {
            this.heuristicTables = heuristicTables ?? throw new ArgumentNullException(nameof(heuristicTables));
        }

        public IEnumerable<Step> GetKillerSteps(int depth)
        {
            GetKillerStepsCallCount++;
            return heuristicTables.GetKillerSteps(depth);
        }

        public void StoreBetaCutOff(Step move, int depth)
        {
            StoreBetaCutOffCallCount++;
            heuristicTables.StoreBetaCutOff(move, depth);
        }

        public void ClearCounters()
        {
            GetKillerStepsCallCount = 0;
            StoreBetaCutOffCallCount = 0;
        }
    }
}