using Alligator.SixMaking.Model;
using System.Reflection;

namespace Alligator.SixMaking.Logics
{
    public sealed class StepPool : Singleton<StepPool>, IStepPool
    {
        private int poolSize = 0;

        private readonly Step[] placingSteps;
        private readonly Step[][][] movingSteps;

        public int Size
        {
            get { return poolSize; }
        }

        private StepPool()
        {
            int totalSize = Constants.BoardSize * Constants.BoardSize;

            placingSteps = new Step[totalSize];
            CreatePlacementPool();

            movingSteps = new Step[totalSize][][];
            CreateMovementPool();
        }

        private void CreatePlacementPool()
        {
            // the types of the constructor parameters, in order
            Type[] paramTypes = new Type[] { typeof(int), typeof(int) };

            for (int i = 0; i < placingSteps.Length; i++)
            {
                // the values of the constructor parameters, in order
                object[] paramValues = new object[] { i, poolSize++ };
                placingSteps[i] = Construct<Placement>(paramTypes, paramValues);
            }
        }

        private void CreateMovementPool()
        {
            int totalSize = Constants.BoardSize * Constants.BoardSize;

            // the types of the constructor parameters, in order
            Type[] paramTypes = new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) };

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
                        // the values of the constructor parameters, in order
                        object[] paramValues = new object[] { from, to, count, poolSize++ };
                        movingSteps[from][to][count] = Construct<Movement>(paramTypes, paramValues);
                    }
                }
            }
        }

        private bool IsValidMove(int from, int to)
        {
            return from != to;
        }

        private T Construct<T>(Type[] paramTypes, object[] paramValues)
        {
            Type t = typeof(T);
            ConstructorInfo ci = t.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, paramTypes, null);
            return (T)ci.Invoke(paramValues);
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