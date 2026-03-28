namespace Alligator.Solver.Algorithms
{
    internal class AlphaBetaPruning<TPosition, TStep> : IAlphaBetaPruning<TPosition>
        where TPosition : IPosition<TStep>
    {
        private readonly IRules<TPosition, TStep> rules;
        private readonly ICacheTables<TPosition, TStep> cacheTables;
        private readonly IHeuristicTables<TStep> heuristicTables;
        private readonly ISearchManager searchManager;

        private const int MaxSearchDepth = 16;
        private readonly List<TStep>[] orderedStepBuffers;

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

            orderedStepBuffers = new List<TStep>[MaxSearchDepth];
            for (int i = 0; i < MaxSearchDepth; i++)
            {
                orderedStepBuffers[i] = new List<TStep>();
            }
        }

        public int Search(TPosition position, int alpha, int beta)
        {
            return SearchRecursively(position, searchManager.DepthLimit, alpha, beta);
        }

        private int SearchRecursively(TPosition position, int depth, int alpha, int beta)
        {
            if (depth <= 0)
            {
                if (rules.IsGoal(position))
                {
                    return -(sbyte.MaxValue + depth);
                }
                return -HeuristicValue(position, depth);
            }

            var originalAlpha = alpha;

            bool hasTransposition = cacheTables.TryGetTransposition(position, out Transposition<TStep> transposition);

            if (hasTransposition && depth <= transposition.Depth)
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

            var orderedSteps = GetOrderedLegalSteps(position, depth, hasTransposition, transposition.OptimalStep);

            if (orderedSteps.Count == 0)
            {
                return -(rules.IsGoal(position) ? sbyte.MaxValue + depth : 0);
            }

            var bestValue = -int.MaxValue;
            TStep bestStep = orderedSteps[0];

            for (int i = 0; i < orderedSteps.Count; i++)
            {
                var step = orderedSteps[i];
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
            if (depth > 1)
            {
                var newTransposition = new Transposition<TStep>(GetEvaluationMode(bestValue, originalAlpha, beta), bestValue, depth, bestStep);
                cacheTables.AddTransposition(position, newTransposition);
            }
            return bestValue;
        }

        private List<TStep> GetOrderedLegalSteps(TPosition position, int depth, bool hasTransposition, TStep transpositionStep)
        {
            var result = orderedStepBuffers[depth];
            result.Clear();

            var killers = heuristicTables.GetKillerSteps(depth);
            int killerCount = 0;
            bool transpositionStepIsLegal = false;

            foreach (var move in rules.LegalStepsAt(position))
            {
                if (hasTransposition && move!.Equals(transpositionStep))
                {
                    transpositionStepIsLegal = true;
                    continue;
                }
                if (killers.Contains(move))
                {
                    result.Insert(killerCount, move);
                    killerCount++;
                }
                else
                {
                    result.Add(move);
                }
            }

            if (transpositionStepIsLegal)
            {
                result.Insert(0, transpositionStep);
            }

            return result;
        }

        private bool IsBetaCutOff(int alpha, int beta)
        {
            return alpha >= beta;
        }

        private void HandleBetaCutOff(TStep step, int depth)
        {
            heuristicTables.StoreBetaCutOff(step, depth);
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
            if (!cacheTables.TryGetValue(position, out var value))
            {
                value = position.Value;
                cacheTables.AddValue(position, value);
            }
            return IsOpponentsTurn(depth) ? -value : value;
        }

        private bool IsOpponentsTurn(int depth)
        {
            int distanceFromRoot = searchManager.DepthLimit - depth;
            return distanceFromRoot % 2 != 0;
        }
    }
}