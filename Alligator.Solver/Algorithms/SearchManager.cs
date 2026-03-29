using System.Diagnostics;

namespace Alligator.Solver.Algorithms
{
    internal class SearchManager : ISearchManager
    {
        private readonly Stopwatch stopwatch = new();
        private readonly long timeLimitMs;
        private int nodeCount;

        public int MaxDepth { get; }
        public int DepthLimit { get; set; }
        public bool IsAborted { get; private set; }

        public SearchManager(int maxDepth, TimeSpan? timeBudget = null)
        {
            MaxDepth = maxDepth;
            timeLimitMs = (long)(timeBudget?.TotalMilliseconds ?? 0);
        }

        public void StartSearch()
        {
            nodeCount = 0;
            IsAborted = false;
            if (timeLimitMs > 0)
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