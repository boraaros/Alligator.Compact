namespace Alligator.ConnectFour
{
    /// <summary>
    /// Represents a move in Connect Four — dropping a disc into a column.
    /// </summary>
    public class Drop
    {
        public int Column { get; }

        private static readonly Drop[] instances;

        static Drop()
        {
            instances = new Drop[Position.Columns];
            for (int c = 0; c < Position.Columns; c++)
            {
                instances[c] = new Drop(c);
            }
        }

        private Drop(int column)
        {
            Column = column;
        }

        /// <summary>
        /// Returns the cached Drop instance for the given column.
        /// </summary>
        public static Drop At(int column) => instances[column];

        public override bool Equals(object? obj)
        {
            return obj is Drop other && Column == other.Column;
        }

        public override int GetHashCode() => Column;

        public override string ToString() => $"Col({Column})";
    }
}
