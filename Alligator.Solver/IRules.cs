namespace Alligator.Solver
{
    /// <summary>
    /// Defines the rules of the specified game.
    /// </summary>
    /// <typeparam name="TPosition">type of positions in the specified game</typeparam>
    /// <typeparam name="TStep">type of steps in the specified game</typeparam>
    public interface IRules<TPosition, TStep>
    {
        /// <summary>
        /// Creates the initial position of the game.
        /// </summary>
        /// <returns>the initial position of the game</returns>
        TPosition InitialPosition();

        /// <summary>
        /// Enumerates the legal steps at the specified game position. If the game is already over, there is no legal step.
        /// </summary>
        /// <param name="position">specified game position</param>
        /// <returns>the legal steps</returns>
        IEnumerable<TStep> LegalStepsAt(TPosition position);

        /// <summary>
        /// Defines the result of the game.
        /// </summary>
        /// <param name="position">specified game position</param>
        /// <returns>true if the game is ended but did not finish with a tie</returns>
        bool IsGoal(TPosition position);
    }
}