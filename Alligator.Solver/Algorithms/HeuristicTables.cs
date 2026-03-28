namespace Alligator.Solver.Algorithms
{
    internal class HeuristicTables<TMove> : IHeuristicTables<TMove>
    {
        private readonly Dictionary<int, IList<TMove>> killerSteps;
        private readonly Dictionary<TMove, int> historyScores;

        private const int StoredKillerStepsLimitPerDepth = 2;

        public HeuristicTables()
        {
            killerSteps = new Dictionary<int, IList<TMove>>();
            historyScores = new Dictionary<TMove, int>();
        }

        public void StoreBetaCutOff(TMove move, int depth)
        {
            UpdateKillerSteps(move, depth);
            RecordHistorySuccess(move, depth);
        }

        public IEnumerable<TMove> GetKillerSteps(int depth)
        {
            if (killerSteps.TryGetValue(depth, out var killers))
            {
                return killers;
            }
            return Enumerable.Empty<TMove>();
        }

        public int GetHistoryScore(TMove move)
        {
            return historyScores.TryGetValue(move, out int score) ? score : 0;
        }

        private void RecordHistorySuccess(TMove move, int depth)
        {
            int bonus = depth * depth;
            if (historyScores.TryGetValue(move, out int current))
            {
                historyScores[move] = current + bonus;
            }
            else
            {
                historyScores[move] = bonus;
            }
        }

        private void UpdateKillerSteps(TMove move, int depth)
        {
            if (killerSteps.TryGetValue(depth, out var killers))
            {
                if (killers[0]!.Equals(move))
                {
                    return;
                }
                killers.Insert(0, move);
                if (killers.Count > StoredKillerStepsLimitPerDepth)
                {
                    killers.RemoveAt(StoredKillerStepsLimitPerDepth);
                }
            }
            else
            {
                killerSteps.Add(depth, new List<TMove> { move });
            }
        }
    }
}