namespace Alligator.Solver
{
    /// <summary>
    /// Represents the game board.
    /// </summary>
    /// <typeparam name="TStep">type of steps in the specified game</typeparam>
    public interface IPosition<TStep>
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
        /// Updates the position with the specified step.
        /// </summary>
        /// <param name="step">specified step</param>
        void Take(TStep step);

        /// <summary>
        /// Withdraws the last step.
        /// </summary>
        void TakeBack();
    }
}