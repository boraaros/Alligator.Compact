using Alligator.SixMaking.Model;
using Alligator.Solver.Algorithms;

namespace Alligator.Benchmark
{
    internal class MeasurableCacheTables : ICacheTables<IPosition, Step>
    {
        public int AddTranspositionCallCount = 0;
        public int AddValueCallCount = 0;
        public int TryGetTranspositionCallCount = 0;
        public int TryGetValueCallCount = 0;

        public int TranspositionHitCount = 0;
        public int ValueHitCount = 0;

        private readonly ICacheTables<IPosition, Step> cacheTables;

        public double TranspositionHitRatio => (double)TranspositionHitCount / TryGetTranspositionCallCount;
        public double ValueHitRatio => (double)ValueHitCount / TryGetValueCallCount;

        public MeasurableCacheTables(ICacheTables<IPosition, Step> cacheTables)
        {
            this.cacheTables = cacheTables ?? throw new ArgumentNullException(nameof(cacheTables));
        }

        public void AddTransposition(IPosition position, Transposition<Step> transposition)
        {
            AddTranspositionCallCount++;
            cacheTables.AddTransposition(position, transposition);
        }

        public void AddValue(IPosition position, int value)
        {
            AddValueCallCount++;
            cacheTables.AddValue(position, value);
        }

        public bool TryGetTransposition(IPosition position, out Transposition<Step> transposition)
        {
            TryGetTranspositionCallCount++;
            if (cacheTables.TryGetTransposition(position, out transposition))
            {
                TranspositionHitCount++;
                return true;
            }
            return false;
        }

        public bool TryGetValue(IPosition position, out int value)
        {
            TryGetValueCallCount++;
            if (cacheTables.TryGetValue(position, out value))
            {
                ValueHitCount++;
                return true;
            }
            return false;
        }

        public void ClearCounters()
        {
            AddTranspositionCallCount = 0;
            AddValueCallCount = 0;
            TryGetTranspositionCallCount = 0;
            TryGetValueCallCount = 0;
            TranspositionHitCount = 0;
            ValueHitCount = 0;
        }
    }
}