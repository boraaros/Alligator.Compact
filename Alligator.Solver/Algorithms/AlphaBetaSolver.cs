namespace Alligator.Solver.Algorithms
{
    internal class AlphaBetaSolver<TPosition, TMove> : ISolver<TMove>
        where TPosition : IPosition<TMove>
    {
        private readonly AlphaBetaPruning<TPosition, TMove> alphaBetaPruning;
        private readonly IRules<TPosition, TMove> rules;

        public AlphaBetaSolver(
            AlphaBetaPruning<TPosition, TMove> alphaBetaPruning, 
            IRules<TPosition, TMove> rules)
        {
            this.alphaBetaPruning = alphaBetaPruning ?? throw new ArgumentNullException(nameof(alphaBetaPruning));
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        public TMove OptimizeNextMove(IList<TMove> moveHistory)
        {
            var position = CreatePosition(moveHistory);

            var bestMove = default(TMove);
            var bestValue = -sbyte.MaxValue;

            foreach (var move in rules.LegalMovesAt(position))
            {
                position.Take(move);
                var result = -alphaBetaPruning.Search(position, bestValue, sbyte.MaxValue);
                if (result > bestValue)
                {
                    bestValue = result;
                    bestMove = move;
                }
                position.TakeBack();
            }
            return bestMove;
        }

        private TPosition CreatePosition(IEnumerable<TMove> moveHistory)
        {
            var position = rules.InitialPosition();
            foreach (var move in moveHistory)
            {
                position.Take(move);
            }

            if (!rules.LegalMovesAt(position).Any())
            {
                throw new InvalidOperationException("Next move calculation failed because the game is already over");
            }

            return position;
        }
    }
}