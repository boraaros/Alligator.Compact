using Alligator.Solver;

namespace Alligator.TicTacToe
{
    public class Rules : IRules<Position, Placement>
    {
        public Position InitialPosition()
        {
            return new Position();
        }

        public IEnumerable<Placement> LegalStepsAt(Position position)
        {
            if (position == null)
            {
                throw new ArgumentNullException(nameof(position));
            }

            if (IsGoal(position))
            {
                yield break;
            }

            for (int i = 0; i < Position.BoardSize; i++)
            {
                for (int j = 0; j < Position.BoardSize; j++)
                {
                    if (position.GetSymbolAt(i, j) == Symbol.Empty)
                    {
                        yield return new Placement(i, j);
                    }
                }
            }
        }

        public bool IsGoal(Position position)
        {
            if (position == null)
            {
                throw new ArgumentNullException(nameof(position));
            }

            if (!position.History.Any())
            {
                return false;
            }

            var lastMove = position.History[position.History.Count - 1];

            if (IsHorizontalLine(position, lastMove.Row) || IsVerticalLine(position, lastMove.Column))
            {
                return true;
            }
            if (position.GetSymbolAt(1, 1) == Symbol.Empty)
            {
                return false;
            }
            if (HasDiagonalLine(position) || HasReverseDiagonalLine(position))
            {
                return true;
            }

            return false;
        }

        private static bool IsHorizontalLine(Position position, int row)
        {
            return Enumerable.Range(0, Position.BoardSize)
                .Select(t => position.GetSymbolAt(row, t)).Distinct().Count() == 1;
        }

        private static bool IsVerticalLine(Position position, int column)
        {
            return Enumerable.Range(0, Position.BoardSize)
                .Select(t => position.GetSymbolAt(t, column)).Distinct().Count() == 1;
        }

        private static bool HasDiagonalLine(Position position)
        {
            return Enumerable.Range(0, Position.BoardSize)
                .Select(t => position.GetSymbolAt(t, t)).Distinct().Count() == 1;
        }

        private static bool HasReverseDiagonalLine(Position position)
        {
            return Enumerable.Range(0, Position.BoardSize)
                .Select(t => position.GetSymbolAt(t, Position.BoardSize - 1 - t)).Distinct().Count() == 1;
        }
    }
}