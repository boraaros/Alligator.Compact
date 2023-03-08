namespace Alligator.Solver.Algorithms
{
    internal class AlphaBetaPruning<TPosition, TMove> : IAlphaBetaPruning<TPosition>
        where TPosition : IPosition<TMove>
    {
        private readonly IRules<TPosition, TMove> rules;
        private readonly ICacheTables<TPosition, TMove> cacheTables;
        private readonly ISearchManager searchManager;

        public AlphaBetaPruning(
            IRules<TPosition, TMove> rules, 
            ICacheTables<TPosition, TMove> cacheTables, 
            ISearchManager searchManager)
        {
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
            this.cacheTables = cacheTables ?? throw new ArgumentNullException(nameof(cacheTables));
            this.searchManager = searchManager ?? throw new ArgumentNullException(nameof(searchManager));
        }

        public int Search(TPosition position, int alpha, int beta)
        {
            return SearchRecursively(position, searchManager.DepthLimit, alpha, beta);
        }

        private int SearchRecursively(TPosition position, int depth, int alpha, int beta)
        {
            var originalAlpha = alpha;

            if (cacheTables.TryGetTransposition(position, out Transposition<TMove> transposition) 
                && depth <= transposition.Depth)
            {
                switch (transposition.EvaluationMode)
                {
                    case EvaluationMode.ExactValue:
                        return transposition.Value;
                    case EvaluationMode.LowerBound:
                        alpha = Math.Max(alpha, transposition.Value);
                        break;
                    case EvaluationMode.UpperBound:
                        beta = Math.Min(beta, transposition.Value);
                        break;
                }
                if (IsBetaCutOff(alpha, beta))
                {
                    HandleBetaCutOff(transposition.BestStrategy, depth);
                    return transposition.Value;
                }
            }
            if (IsLeaf(position, depth))
            {
                return -HeuristicValue(position, depth);
            }
            var bestValue = -int.MaxValue;
            TMove besTMove = default;

            foreach (var move in rules.LegalMovesAt(position))
            {
                position.Take(move);
                var value = -SearchRecursively(position, depth - 1, -beta, -alpha);
                position.TakeBack();

                if (value > bestValue)
                {
                    bestValue = value;
                    besTMove = move;
                }
                alpha = Math.Max(alpha, value);
                if (IsBetaCutOff(alpha, beta))
                {
                    HandleBetaCutOff(move, depth);
                    break;
                }
            }
            if (depth > 0)
            {
                EvaluationMode evaluationMode = GetEvaluationMode(bestValue, originalAlpha, beta);
                transposition = new Transposition<TMove>(evaluationMode, bestValue, depth, besTMove);
                cacheTables.AddTransposition(position, transposition);
            }
            return bestValue;
        }

        private bool IsBetaCutOff(int alpha, int beta)
        {
            return alpha >= beta;
        }

        private void HandleBetaCutOff(TMove move, int depth)
        {
            // TODO: it could help at move ordering!
        }

        private bool IsLeaf(TPosition position, int depth)
        {
            return depth <= 0 || !rules.LegalMovesAt(position).Any();
        }

        private EvaluationMode GetEvaluationMode(int value, int alpha, int beta)
        {
            if (value <= alpha)
            {
                return EvaluationMode.UpperBound;
            }
            else if (value >= beta)
            {
                return EvaluationMode.LowerBound;
            }
            else
            {
                return EvaluationMode.ExactValue;
            }
        }

        private int HeuristicValue(TPosition position, int depth)
        {
            if (rules.LegalMovesAt(position).Any())
            {
                if (!cacheTables.TryGetValue(position, out var value))
                {
                    value = position.Value;
                    cacheTables.AddValue(position, value);
                }
                return IsOpponentsTurn(depth) ? -value : value; // TODO: check if the sign is required!
            }
            return rules.IsGoal(position) ? sbyte.MaxValue + depth : 0;
        }

        private bool IsOpponentsTurn(int depth)
        {
            int distanceFromRoot = searchManager.DepthLimit - depth;
            return distanceFromRoot % 2 != 0;
        }
    }
}