using Alligator.Solver;

namespace Alligator.ConnectFour
{
    internal class Configuration : IConfiguration
    {
        public int MaxDepth => 43;
        public TimeSpan? TimeBudget => TimeSpan.FromSeconds(5);
    }
}
