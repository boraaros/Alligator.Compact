namespace Alligator.Solver.Algorithms
{
    internal interface ISearchManager
    {
        int MaxDepth { get; }
        int DepthLimit { get; set; }
        bool IsAborted { get; }

        void StartSearch();
        void CheckTimeBudget();
    }
}