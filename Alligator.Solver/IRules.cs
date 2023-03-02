namespace Alligator.Solver
{
    /// <summary>
    /// Defines the rules of the specified game.
    /// </summary>
    /// <typeparam name="TPosition">type of positions in the specified game</typeparam>
    /// <typeparam name="TMove">type of moves in the specified game</typeparam>
    public interface IRules<TPosition, TMove>
    {
        /// <summary>
        /// Creates the initial position of the game.
        /// </summary>
        /// <returns>the initial position of the game</returns>
        TPosition InitialPosition();

        /// <summary>
        /// Enumerates the legal moves at the specified game position. If the game is already over, there is no legal move.
        /// </summary>
        /// <param name="position">specified game position</param>
        /// <returns>the legal moves</returns>
        IEnumerable<TMove> LegalMovesAt(TPosition position);

        /// <summary>
        /// Defines the result of the game.
        /// </summary>
        /// <param name="position">specified game position</param>
        /// <returns>true if the game is ended but did not finish with a tie</returns>
        bool IsGoal(TPosition position);
    }
}