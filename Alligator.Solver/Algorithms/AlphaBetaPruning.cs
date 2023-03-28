namespace Alligator.Solver.Algorithms
{
    internal class AlphaBetaPruning<TPosition, TStep> : IAlphaBetaPruning<TPosition>
        where TPosition : IPosition<TStep>
    {
        private readonly IRules<TPosition, TStep> rules;
        private readonly ICacheTables<TPosition, TStep> cacheTables;
        private readonly IHeuristicTables<TStep> heuristicTables;
        private readonly ISearchManager searchManager;

        public AlphaBetaPruning(
            IRules<TPosition, TStep> rules, 
            ICacheTables<TPosition, TStep> cacheTables, 
            IHeuristicTables<TStep> heuristicTables,
            ISearchManager searchManager)
        {
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
            this.cacheTables = cacheTables ?? throw new ArgumentNullException(nameof(cacheTables));
            this.heuristicTables = heuristicTables ?? throw new ArgumentNullException(nameof(heuristicTables));
            this.searchManager = searchManager ?? throw new ArgumentNullException(nameof(searchManager));
        }

        public int Search(TPosition position, int alpha, int beta)
        {
            return SearchRecursively(position, searchManager.DepthLimit, alpha, beta);
        }

        private int SearchRecursively(TPosition position, int depth, int alpha, int beta)
        {
            var originalAlpha = alpha;

            if (cacheTables.TryGetTransposition(position, out Transposition<TStep> transposition) 
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
                    HandleBetaCutOff(transposition.OptimalStep, depth);
                    return transposition.Value;
                }
            }
            if (IsLeaf(position, depth))
            {
                return -HeuristicValue(position, depth);
            }
            var bestValue = -int.MaxValue;
            TStep bestStep = default;

            foreach (var step in OrderedLegalStepsAt(position, depth))
            {
                position.Take(step);
                var value = -SearchRecursively(position, depth - 1, -beta, -alpha);
                position.TakeBack();

                if (value > bestValue)
                {
                    bestValue = value;
                    bestStep = step;
                }
                alpha = Math.Max(alpha, value);
                if (IsBetaCutOff(alpha, beta))
                {
                    HandleBetaCutOff(step, depth);
                    break;
                }
            }
            if (depth > 0)
            {
                EvaluationMode evaluationMode = GetEvaluationMode(bestValue, originalAlpha, beta);
                transposition = new Transposition<TStep>(evaluationMode, bestValue, depth, bestStep);
                cacheTables.AddTransposition(position, transposition);
            }
            return bestValue;
        }

        private IEnumerable<TStep> OrderedLegalStepsAt(TPosition position, int depth)
        {
            TStep prevOptimalStep = default(TStep);
            if (cacheTables.TryGetTransposition(position, out Transposition<TStep> transposition))
            {
                yield return transposition.OptimalStep;
                prevOptimalStep = transposition.OptimalStep;
            }
            var prevKillerSteps = heuristicTables.GetKillerSteps(depth);
            var otherSteps = new List<TStep>();
            foreach (var move in rules.LegalStepsAt(position).Where(t => !t.Equals(prevOptimalStep)))
            {
                if (prevKillerSteps.Contains(move))
                {
                    yield return move;
                }
                else
                {
                    otherSteps.Add(move);
                }
            }
            foreach (var move in otherSteps)
            {
                yield return move;
            }
        }

        private bool IsBetaCutOff(int alpha, int beta)
        {
            return alpha >= beta;
        }

        private void HandleBetaCutOff(TStep step, int depth)
        {
            heuristicTables.StoreBetaCutOff(step, depth);
        }

        private bool IsLeaf(TPosition position, int depth)
        {
            return depth <= 0 || !rules.LegalStepsAt(position).Any();
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
            if (rules.LegalStepsAt(position).Any())
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