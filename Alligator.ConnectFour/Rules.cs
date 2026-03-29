using Alligator.Solver;

namespace Alligator.ConnectFour
{
    public class Rules : IRules<Position, Drop>
    {
        public Position InitialPosition()
        {
            return new Position();
        }

        public IEnumerable<Drop> LegalStepsAt(Position position)
        {
            if (position.HasWinner() || position.IsBoardFull)
            {
                yield break;
            }

            int center = Position.Columns / 2;

            if (position.HeightAt(center) < Position.Rows)
            {
                yield return Drop.At(center);
            }

            for (int offset = 1; offset <= Position.Columns / 2; offset++)
            {
                int left = center - offset;
                int right = center + offset;

                if (left >= 0 && position.HeightAt(left) < Position.Rows)
                {
                    yield return Drop.At(left);
                }
                if (right < Position.Columns && position.HeightAt(right) < Position.Rows)
                {
                    yield return Drop.At(right);
                }
            }
        }

        public bool IsGoal(Position position)
        {
            return position.HasWinner();
        }
    }
}
