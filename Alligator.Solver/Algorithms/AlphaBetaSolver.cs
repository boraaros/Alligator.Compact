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

            for (int i = 2; i < 7; i += 2) // TODO: magic number (7)!
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
            int alpha = -int.MaxValue;
            int beta = int.MaxValue;

            IList<TStep> candidates = rules.LegalStepsAt(position).ToList();

            while (alpha + 1 < beta && candidates.Count > 1)
            {
                var newCandidates = new List<TStep>();

                foreach (var move in candidates)
                {
                    int value = NullWindowTest(position, move, guess);

                    if (value >= guess)
                    {
                        newCandidates.Add(move);
                    }
                }

                if (newCandidates.Count > 0)
                {
                    candidates = newCandidates;
                    alpha = guess;
                }
                else
                {
                    beta = guess;
                }

                guess = NextGuess(alpha, beta, candidates.Count);
            }

            return (candidates, guess);
        }

        private int NullWindowTest(TPosition position, TStep step, int guess)
        {
            position.Take(step);
            var value = -alphaBetaPruning.Search(position, -guess, -(guess - 1));
            position.TakeBack();
            return value;
        }

        private int NextGuess(int alpha, int beta, int count)
        {
            if (alpha <= 0)
            {
                beta = Math.Min(beta, int.MaxValue / 2);
            }

            if (beta >= 0)
            {
                alpha = Math.Max(alpha, -int.MaxValue / 2);
            }

            var guess = (int)(alpha + (count - 1.0) / count * (beta - alpha));

            if (guess == alpha)
            {
                return guess + 1;
            }
            else if (guess == beta)
            {
                return guess - 1;
            }
            return guess;
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