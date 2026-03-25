namespace Alligator.Solver.Algorithms
{
    internal interface ISearchManager
    {
        int DepthLimit { get; set; }
        bool IsAborted { get; }

        void StartTimedSearch(long timeBudgetMs);
        void CheckTimeBudget();
    }
}