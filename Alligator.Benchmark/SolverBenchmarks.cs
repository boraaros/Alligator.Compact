using Alligator.SixMaking.Logics;
using Alligator.SixMaking.Model;
using Alligator.Solver;
using BenchmarkDotNet.Attributes;

namespace Alligator.Benchmark
{
    [MemoryDiagnoser]
    [SimpleJob(warmupCount: 1, iterationCount: 5)]
    public class SolverBenchmarks
    {
        private ISolver<Step> solver = null!;
        private IList<Step> example1History = null!;
        private IList<Step> example2History = null!;
        private IList<Step> example3History = null!;
        private IList<Step> example5History = null!;

        [GlobalSetup]
        public void Setup()
        {
            var rules = new Rules(new StepPool(), new MovingRules());
            var solverConfiguration = new Configuration();
            var solverFactory = new SolverProvider<IPosition, Step>(rules, solverConfiguration);
            solver = solverFactory.Create();

            example1History = BuildHistory(Example1());
            example2History = BuildHistory(Example2());
            example3History = BuildHistory(Example3());
            example5History = BuildHistory(Example5());
        }

        [Benchmark(Description = "Example1 - Empty board")]
        public Step Example1_EmptyBoard() => solver.OptimizeNextStep(example1History);

        [Benchmark(Description = "Example2 - Four pieces")]
        public Step Example2_FourPieces() => solver.OptimizeNextStep(example2History);

        [Benchmark(Description = "Example3 - Twelve pieces")]
        public Step Example3_TwelvePieces() => solver.OptimizeNextStep(example3History);

        [Benchmark(Description = "Example5 - Complex midgame")]
        public Step Example5_ComplexMidgame() => solver.OptimizeNextStep(example5History);

        private static IList<Step> BuildHistory(IPosition position)
        {
            return position.History.ToList();
        }

        private static IPosition Example1()
        {
            return new Position();
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
    }
}
