namespace Alligator.Solver.Algorithms
{
    internal class AlphaBetaSolver<TPosition, TStep> : ISolver<TStep>
        where TPosition : IPosition<TStep>
    {
        private readonly AlphaBetaPruning<TPosition, TStep> alphaBetaPruning;
        private readonly IRules<TPosition, TStep> rules;

        public AlphaBetaSolver(
            AlphaBetaPruning<TPosition, TStep> alphaBetaPruning, 
            IRules<TPosition, TStep> rules)
        {
            this.alphaBetaPruning = alphaBetaPruning ?? throw new ArgumentNullException(nameof(alphaBetaPruning));
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        public TStep OptimizeNextStep(IList<TStep> history)
        {
            var position = CreatePosition(history);

            var bestStep = default(TStep);
            var bestValue = -sbyte.MaxValue;

            foreach (var step in rules.LegalStepsAt(position))
            {
                position.Take(step);
                var result = -alphaBetaPruning.Search(position, bestValue, sbyte.MaxValue);
                if (result > bestValue)
                {
                    bestValue = result;
                    bestStep = step;
                }
                position.TakeBack();
            }
            return bestStep;
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