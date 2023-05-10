using Alligator.SixMaking.Logics;
using Alligator.SixMaking.Model;
using Alligator.Solver;
using Alligator.Solver.Algorithms;
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
            var rules = new MeasurableRules(new Rules(new StepPool(), new MovingRules()));
            IConfiguration solverConfiguration = new Configuration();
            SolverProvider<IPosition, Step> solverFactory = new SolverProvider<IPosition, Step>(rules, solverConfiguration, SolverLog);
            var cacheTables = new MeasurableCacheTables(new CacheTables<IPosition, Step>());
            var heuristicTables = new MeasurableHeuristicTables(new HeuristicTables<Step>());
            ISolver<Step> solver = solverFactory.Create(cacheTables, heuristicTables);

            Console.WriteLine("Performance testing");
            int counter = 0;

            foreach (var example in EnumerateExamples())
            {
                Console.WriteLine($"#{++counter} TEST");
                sw.Restart();
                var result = solver.OptimizeNextStep(example.History);
                Console.WriteLine($"Computation time: {sw.ElapsedMilliseconds} ms [result: {result}]");
                PrintAndResetStats(rules);
                PrintAndResetStats(cacheTables);
                PrintAndResetStats(heuristicTables);
            }

            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        private static void PrintAndResetStats(MeasurableRules rules)
        {
            Console.WriteLine($"#InitPosCalls={rules.InitialPositionCallCount}, #LegalStepsCalls={rules.LegalStepsCallCount}, #IsGoalCalls={rules.IsGoalCallCount}");
            rules.ClearCounters();
        }

        private static void PrintAndResetStats(MeasurableCacheTables cacheTables)
        {
            Console.WriteLine($"#AddTranspositionCalls={cacheTables.AddTranspositionCallCount}, #AddValueCalls={cacheTables.AddValueCallCount}");
            Console.WriteLine($"#TryGetTranspositionCalls={cacheTables.TryGetTranspositionCallCount}, #TryGetValueCalls={cacheTables.TryGetValueCallCount}");
            Console.WriteLine($"TranspositionHitRatio={cacheTables.TranspositionHitRatio}, ValueHitRatio={cacheTables.ValueHitRatio}");
            cacheTables.ClearCounters();
        }

        private static void PrintAndResetStats(MeasurableHeuristicTables heuristicTables)
        {
            Console.WriteLine($"#GetKillerStepsCalls={heuristicTables.GetKillerStepsCallCount}, #StoreBetaCutOffCalls={heuristicTables.StoreBetaCutOffCallCount}");
            heuristicTables.ClearCounters();
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
            yield return Example6();
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
            position.Take(new Placement(12));
            position.Take(new Placement(13));
            position.Take(new Placement(11));
            position.Take(new Placement(17));
            return position;
        }

        private static IPosition Example3()
        {
            IPosition position = new Position();
            for (int i = 6; i < 18; i++)
            {
                position.Take(new Placement(i));
            }
            return position;
        }

        private static IPosition Example4()
        {
            IPosition position = new Position();
            for (int i = 6; i < 18; i++)
            {
                position.Take(new Placement(i));
            }
            position.Take(new Movement(6, 7, 1));
            position.Take(new Movement(9, 8, 1));
            position.Take(new Movement(10, 11, 1));
            position.Take(new Movement(14, 13, 1));
            position.Take(new Movement(17, 12, 1));
            position.Take(new Movement(12, 11, 1));
            position.Take(new Placement(2));
            position.Take(new Placement(18));
            position.Take(new Movement(16, 11, 1));
            position.Take(new Movement(11, 15, 1));
            return position;
        }

        private static IPosition Example5()
        {
            IPosition position = new Position();
            position.Take(new Placement(0));
            position.Take(new Placement(12));
            position.Take(new Placement(1));
            position.Take(new Placement(11));
            position.Take(new Placement(2));
            position.Take(new Placement(17));
            position.Take(new Placement(3));
            position.Take(new Placement(13));
            position.Take(new Placement(4));
            position.Take(new Placement(10));
            position.Take(new Placement(5));
            position.Take(new Placement(15));
            position.Take(new Placement(20));
            position.Take(new Placement(16));
            position.Take(new Movement(5, 10, 1));
            position.Take(new Movement(15, 10, 1));
            position.Take(new Placement(5));
            position.Take(new Placement(6));
            position.Take(new Movement(1, 6, 1));
            position.Take(new Movement(12, 11, 1));
            position.Take(new Movement(10, 17, 1));
            position.Take(new Placement(12));
            position.Take(new Placement(7));
            position.Take(new Placement(22));
            position.Take(new Placement(1));
            position.Take(new Movement(11, 16, 2));
            position.Take(new Placement(21));
            position.Take(new Placement(18));
            return position;
        }

        private static IPosition Example6()
        {
            IPosition position = new Position();
            position.Take(new Placement(0));
            position.Take(new Placement(12));
            position.Take(new Placement(1));
            position.Take(new Placement(7));
            position.Take(new Placement(2));
            position.Take(new Placement(13));
            position.Take(new Placement(11));
            position.Take(new Placement(17));
            position.Take(new Movement(2, 7, 1));
            position.Take(new Placement(8));
            position.Take(new Placement(16));
            position.Take(new Placement(22));
            position.Take(new Movement(7, 12, 2));
            position.Take(new Movement(12, 1, 1));
            position.Take(new Movement(11, 12, 1));
            position.Take(new Movement(12, 1, 1));
            position.Take(new Movement(1, 8, 3));
            position.Take(new Movement(13, 8, 1));
            return position;
        }

        #endregion
    }
}