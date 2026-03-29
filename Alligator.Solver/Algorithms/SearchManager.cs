using System.Diagnostics;

namespace Alligator.Solver.Algorithms
{
    internal class SearchManager : ISearchManager
    {
        private const int TimeBudgetCheckInterval = 1024;

        private readonly Stopwatch stopwatch = new();
        private readonly TimeSpan timeBudget;
        private int nodeCount;

        public int MaxDepth { get; }
        public int DepthLimit { get; set; }
        public bool IsAborted { get; private set; }

        public SearchManager(int maxDepth, TimeSpan? timeBudget = null)
        {
            MaxDepth = maxDepth;
            this.timeBudget = timeBudget ?? TimeSpan.Zero;
        }

        public void StartSearch()
        {
            nodeCount = 0;
            IsAborted = false;
            if (timeBudget > TimeSpan.Zero)
            {
                stopwatch.Restart();
            }
        }

        public void CheckTimeBudget()
        {
            if (timeBudget > TimeSpan.Zero && ++nodeCount % TimeBudgetCheckInterval == 0
                && stopwatch.Elapsed >= timeBudget)
            {
                IsAborted = true;
            }
        }
    }
}