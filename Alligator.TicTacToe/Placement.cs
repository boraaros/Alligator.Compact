namespace Alligator.TicTacToe
{
    public class Placement
    {
        public readonly int Row;
        public readonly int Column;

        public Placement(int row, int column)
        {
            if (row < 0 || row >= Position.BoardSize || column < 0 || column >= Position.BoardSize)
            {
                throw new ArgumentOutOfRangeException(string.Format("Invalid row ({0}) or column ({1}) index", row, column));
            }
            Row = row;
            Column = column;
        }

        public override int GetHashCode()
        {
            return Position.BoardSize * Row + Column;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is not Placement move)
            {
                return false;
            }
            return Row == move.Row && Column == move.Column;
        }

        public override string ToString()
        {
            return string.Format("[{0}:{1}]", Row, Column);
        }
    }
}