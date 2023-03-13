using Alligator.SixMaking.Model;
using Alligator.Solver;

namespace Alligator.SixMaking.Logics
{
    public class Rules : IRules<IPosition, Step>
    {
        protected readonly IStepPool stepPool;
        protected readonly IMovingRules movingRules;

        public Rules(IStepPool stepPool, IMovingRules movingRules)
        {
            this.stepPool = stepPool ?? throw new ArgumentNullException(nameof(stepPool));
            this.movingRules = movingRules ?? throw new ArgumentNullException(nameof(movingRules));
        }

        public IPosition InitialPosition()
        {
            return new Position();
        }

        public IEnumerable<Step> LegalStepsAt(IPosition position)
        {
            var result = new List<Step>();
            if (IsGoal(position))
            {
                return result;
            }

            for (int cell = 0; cell < Constants.BoardSize * Constants.BoardSize; cell++)
            {
                var columnHeight = position.ColumnHeightAt(cell);

                if (columnHeight == 0)
                {
                    result.Add(stepPool.GetPlacement(cell));
                }
                else
                {
                    var isOwnTower = position.Next == position.DiskAt(cell, columnHeight - 1);

                    foreach (var to in movingRules.LegalMovementsAt(position, cell))
                    {
                        for (int diskCount = columnHeight; diskCount > 0; diskCount--)
                        {
                            var move = stepPool.GetMovement(cell, to, diskCount);

                            if (movingRules.AreInverses(position.LastStep, move))
                            {
                                continue;
                            }

                            if (position.ColumnHeightAt(to) < Constants.WinnerHeight - diskCount)
                            {
                                result.Add(move);
                            }
                            else if (isOwnTower)
                            {
                                return new List<Step> { move };
                            }
                        }
                    }
                }
            }
            return result;
        }       

        public bool IsGoal(IPosition position)
        {
            return Enumerable.Range(0, Constants.BoardSize * Constants.BoardSize)
                .Any(t => position.ColumnHeightAt(t) >= Constants.WinnerHeight);
        }
    }
}