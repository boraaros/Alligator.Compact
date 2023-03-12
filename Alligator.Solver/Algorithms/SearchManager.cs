namespace Alligator.Solver.Algorithms
{
    internal class SearchManager : ISearchManager
    {
        public int DepthLimit { get; private set; }

        public SearchManager(int depthLimit)
        {
            DepthLimit = depthLimit;
        }
    }
}