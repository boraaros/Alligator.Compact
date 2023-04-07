using Alligator.SixMaking.Logics;
using Alligator.SixMaking.Model;
using Alligator.Solver;
using System.Diagnostics;

namespace Alligator.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Hello Six Making Benchmark!");

            var sw = new Stopwatch();
            var rules = new MeasurableRules(new Rules(StepPool.Instance, new MovingRules()));
            IConfiguration solverConfiguration = new Configuration();
            SolverProvider<IPosition, Step> solverFactory = new SolverProvider<IPosition, Step>(rules, solverConfiguration, SolverLog);
            ISolver<Step> solver = solverFactory.Create();

            Console.WriteLine("Performance testing");
            int counter = 0;

            foreach (var example in EnumerateExamples())
            {
                Console.WriteLine($"#{++counter} TEST");
                sw.Restart();
                var result = solver.OptimizeNextStep(example.History);
                Console.WriteLine($"Computation time: {sw.ElapsedMilliseconds} ms [result: {result}]");
                PrintAndResetCallCounts(rules);
            }

            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        private static void PrintAndResetCallCounts(MeasurableRules rules)
        {
            Console.WriteLine($"#InitPosCalls={rules.InitialPositionCallCount}, #LegalStepsCalls={rules.LegalStepsCallCount}, #IsGoalCalls={rules.IsGoalCallCount}");
            rules.ClearCounters();
        }

        private static void SolverLog(string message)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[SolverLog] {message}");
            Console.ForegroundColor = prevColor;
        }

        private static IEnumerable<IPosition> EnumerateExamples()
        {
            yield return Example1();
            yield return Example2();
            yield return Example3();
            yield return Example4();
            yield return Example5();
        }

        #region Examles

        private static IPosition Example1()
        {
            IPosition position = new Position();
            return position;
        }

        private static IPosition Example2()
        {
            IPosition position = new Position();
            position.Take(StepPool.Instance.GetPlacement(12));
            position.Take(StepPool.Instance.GetPlacement(13));
            position.Take(StepPool.Instance.GetPlacement(11));
            position.Take(StepPool.Instance.GetPlacement(17));
            return position;
        }

        private static IPosition Example3()
        {
            IPosition position = new Position();
            for (int i = 6; i < 18; i++)
            {
                position.Take(StepPool.Instance.GetPlacement(i));
            }
            return position;
        }

        private static IPosition Example4()
        {
            IPosition position = new Position();
            for (int i = 6; i < 18; i++)
            {
                position.Take(StepPool.Instance.GetPlacement(i));
            }
            position.Take(StepPool.Instance.GetMovement(6, 7, 1));
            position.Take(StepPool.Instance.GetMovement(9, 8, 1));
            position.Take(StepPool.Instance.GetMovement(10, 11, 1));
            position.Take(StepPool.Instance.GetMovement(14, 13, 1));
            position.Take(StepPool.Instance.GetMovement(17, 12, 1));
            position.Take(StepPool.Instance.GetMovement(12, 11, 1));
            position.Take(StepPool.Instance.GetPlacement(2));
            position.Take(StepPool.Instance.GetPlacement(18));
            position.Take(StepPool.Instance.GetMovement(16, 11, 1));
            position.Take(StepPool.Instance.GetMovement(11, 15, 1));
            return position;
        }

        private static IPosition Example5()
        {
            IPosition position = new Position();
            position.Take(StepPool.Instance.GetPlacement(0));
            position.Take(StepPool.Instance.GetPlacement(12));
            position.Take(StepPool.Instance.GetPlacement(1));
            position.Take(StepPool.Instance.GetPlacement(11));
            position.Take(StepPool.Instance.GetPlacement(2));
            position.Take(StepPool.Instance.GetPlacement(17));
            position.Take(StepPool.Instance.GetPlacement(3));
            position.Take(StepPool.Instance.GetPlacement(13));
            position.Take(StepPool.Instance.GetPlacement(4));
            position.Take(StepPool.Instance.GetPlacement(10));
            position.Take(StepPool.Instance.GetPlacement(5));
            position.Take(StepPool.Instance.GetPlacement(15));
            position.Take(StepPool.Instance.GetPlacement(20));
            position.Take(StepPool.Instance.GetPlacement(16));
            position.Take(StepPool.Instance.GetMovement(5, 10, 1));
            position.Take(StepPool.Instance.GetMovement(15, 10, 1));
            position.Take(StepPool.Instance.GetPlacement(5));
            position.Take(StepPool.Instance.GetPlacement(6));
            position.Take(StepPool.Instance.GetMovement(1, 6, 1));
            position.Take(StepPool.Instance.GetMovement(12, 11, 1));
            position.Take(StepPool.Instance.GetMovement(10, 17, 1));
            position.Take(StepPool.Instance.GetPlacement(12));
            position.Take(StepPool.Instance.GetPlacement(7));
            position.Take(StepPool.Instance.GetPlacement(22));
            position.Take(StepPool.Instance.GetPlacement(1));
            position.Take(StepPool.Instance.GetMovement(11, 16, 2));
            position.Take(StepPool.Instance.GetPlacement(21));
            position.Take(StepPool.Instance.GetPlacement(18));
            return position;
        }

        #endregion
    }
}