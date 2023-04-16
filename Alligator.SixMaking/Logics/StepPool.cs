using Alligator.SixMaking.Model;

namespace Alligator.SixMaking.Logics
{
    public sealed class StepPool : IStepPool
    {
        private readonly Step[] placingSteps;
        private readonly Step[][][] movingSteps;

        public StepPool()
        {
            int totalSize = Constants.BoardSize * Constants.BoardSize;

            placingSteps = new Step[totalSize];
            CreatePlacementPool();

            movingSteps = new Step[totalSize][][];
            CreateMovementPool();
        }

        private void CreatePlacementPool()
        {
            for (int i = 0; i < placingSteps.Length; i++)
            {
                placingSteps[i] = new Placement(i);
            }
        }

        private void CreateMovementPool()
        {
            int totalSize = Constants.BoardSize * Constants.BoardSize;

            for (int from = 0; from < totalSize; from++)
            {
                movingSteps[from] = new Step[totalSize][];

                for (int to = 0; to < totalSize; to++)
                {
                    if (!IsValidMove(from, to))
                    {
                        continue;
                    }
                    movingSteps[from][to] = new Step[Constants.WinnerHeight];

                    for (int count = 1; count < Constants.WinnerHeight; count++)
                    {
                        movingSteps[from][to][count] = new Movement(from, to, count);
                    }
                }
            }
        }

        private bool IsValidMove(int from, int to)
        {
            return from != to;
        }

        public Step GetPlacement(int to)
        {
            return placingSteps[to];
        }

        public Step GetMovement(int from, int to, int count)
        {
            return movingSteps[from][to][count];
        }
    }
}