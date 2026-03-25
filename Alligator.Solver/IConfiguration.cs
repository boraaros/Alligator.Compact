namespace Alligator.Solver
{
    /// <summary>
    /// Can be used to configure the solver.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Maximum search depth for iterative deepening (exclusive upper bound).
        /// The solver searches at even depths: 2, 4, 6, ... up to the largest even less than this value.
        /// </summary>
        int MaxDepth => 7;
    }
}