namespace Alligator.Solver
{
    /// <summary>
    /// The main interface of the abstract solver component.
    /// </summary>
    /// <typeparam name="TStep">type of steps in the specified game</typeparam>
    public interface ISolver<TStep>
    {
        /// <summary>
        /// Calculates the optimal next step from the game position defined by step history.
        /// </summary>
        /// <param name="history">ordered list of all previous steps in the current game</param>
        /// <returns>the optimal next step</returns>
        TStep OptimizeNextStep(IList<TStep> history);
    }
}