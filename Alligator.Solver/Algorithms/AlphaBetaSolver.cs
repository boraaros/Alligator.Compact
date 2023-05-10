using System.Diagnostics;

namespace Alligator.Solver.Algorithms
{
    internal class AlphaBetaSolver<TPosition, TStep> : ISolver<TStep>
        where TPosition : IPosition<TStep>
    {
        private readonly AlphaBetaPruning<TPosition, TStep> alphaBetaPruning;
        private readonly IRules<TPosition, TStep> rules;
        private readonly ISearchManager searchManager;
        private readonly Action<string> logger;

        private const int maxDepth = 7; // TODO: magic number!

        public AlphaBetaSolver(
            AlphaBetaPruning<TPosition, TStep> alphaBetaPruning,
            IRules<TPosition, TStep> rules,
            ISearchManager searchManager,
            Action<string> logger)
        {
            this.alphaBetaPruning = alphaBetaPruning ?? throw new ArgumentNullException(nameof(alphaBetaPruning));
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
            this.searchManager = searchManager ?? throw new ArgumentNullException(nameof(searchManager));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public TStep OptimizeNextStep(IList<TStep> history)
        {
            var sw = new Stopwatch();
            sw.Restart();
            var position = CreatePosition(history);

            var bestStep = default(TStep);
            var guess = 0;

            for (int i = 2; i < maxDepth; i += 2) 
            {
                searchManager.DepthLimit = i;
                var (OptimalSteps, Value) = BestNodeSearch(position, guess);
                bestStep = OptimalSteps.First();
                guess = Value;
                logger($"Iteration has been completed in {sw.ElapsedMilliseconds} ms (value: {Value}, step: {bestStep}, depth: {i})");
            }

            return bestStep;
        }

        private (ICollection<TStep> OptimalSteps, int Value) BestNodeSearch(TPosition position, int guess)
        {
            int alpha = -sbyte.MaxValue - maxDepth;
            int beta = sbyte.MaxValue + maxDepth;

            IList<TStep> candidates = rules.LegalStepsAt(position).ToList();

            var optimalValue = 0;

            while (alpha + 1 < beta && candidates.Count > 1)
            {
                var newCandidates = new List<TStep>();

                foreach (var move in candidates)
                {
                    if (newCandidates.Count > 1)
                    {
                        newCandidates.Add(move);
                        continue;
                    }

                    int value = NullWindowTest(position, move, guess);

                    if (value >= guess)
                    {
                        newCandidates.Add(move);
                    }
                }

                if (newCandidates.Count > 0)
                {
                    optimalValue = guess;
                    candidates = newCandidates;
                    alpha = guess;
                }
                else
                {
                    beta = guess;
                }

                guess = newCandidates.Count > 0 ? guess + 1 : guess - 1;
            }

            return (candidates, optimalValue);
        }

        private int NullWindowTest(TPosition position, TStep step, int guess)
        {
            position.Take(step);
            var value = -alphaBetaPruning.Search(position, -guess, -(guess - 1));
            position.TakeBack();
            return value;
        }

        private TPosition CreatePosition(IEnumerable<TStep> history)
        {
            var position = rules.InitialPosition();
            foreach (var step in history)
            {
                position.Take(step);
            }

            if (!rules.LegalStepsAt(position).Any())
            {
                throw new InvalidOperationException("Next step calculation failed because the game is already over");
            }

            return position;
        }
    }
}