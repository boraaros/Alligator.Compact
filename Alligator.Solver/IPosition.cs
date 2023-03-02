namespace Alligator.Solver
{
    /// <summary>
    /// Represents the game board.
    /// </summary>
    /// <typeparam name="TMove">type of moves in the specified game</typeparam>
    public interface IPosition<TMove>
    {
        /// <summary>
        /// Unique identifier or very strong hash, e.g. <see href="https://en.wikipedia.org/wiki/Zobrist_hashing">Zobrist hashing</see>.
        /// </summary>
        ulong Identifier { get; }

        /// <summary>
        /// The <see href="https://en.wikipedia.org/wiki/Evaluation_function">static evaluation value</see> from the solver's point of view.
        /// </summary>
        sbyte Value { get; }

        /// <summary>
        /// Updates the position with the specified move.
        /// </summary>
        /// <param name="move">specified move</param>
        void Take(TMove move);

        /// <summary>
        /// Withdraws the last move.
        /// </summary>
        void TakeBack();
    }
}