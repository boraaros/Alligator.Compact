using System.Diagnostics;

namespace Alligator.Solver.Algorithms
{
    internal class SearchManager : ISearchManager
    {
        private readonly Stopwatch stopwatch = new();
        private long timeLimitMs;
        private int nodeCount;

        public int DepthLimit { get; set; }
        public bool IsAborted { get; private set; }

        public SearchManager(int depthLimit)
        {
            DepthLimit = depthLimit;
        }

        public void StartTimedSearch(long timeBudgetMs)
        {
            timeLimitMs = timeBudgetMs;
            nodeCount = 0;
            IsAborted = false;
            if (timeBudgetMs > 0)
            {
                stopwatch.Restart();
            }
        }

        public void CheckTimeBudget()
        {
            if (timeLimitMs > 0 && (++nodeCount & 0x3FF) == 0
                && stopwatch.ElapsedMilliseconds >= timeLimitMs)
            {
                IsAborted = true;
            }
        }
    }
}