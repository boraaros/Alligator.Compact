namespace Alligator.Solver.Algorithms
{
    internal class SearchManager : ISearchManager
    {
        public int DepthLimit { get; set; }

        public SearchManager(int depthLimit)
        {
            DepthLimit = depthLimit;
        }
    }
}