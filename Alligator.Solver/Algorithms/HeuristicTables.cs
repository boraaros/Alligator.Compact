namespace Alligator.Solver.Algorithms
{
    internal class HeuristicTables<TMove> : IHeuristicTables<TMove>
    {
        private readonly IDictionary<int, IList<TMove>> killerSteps;

        private const int StoredKillerStepsLimitPerDepth = 2;

        public HeuristicTables()
        {
            killerSteps = new Dictionary<int, IList<TMove>>();
        }

        public void StoreBetaCutOff(TMove move, int depth)
        {
            UpdateKillerSteps(move, depth);
        }

        public IEnumerable<TMove> GetKillerSteps(int depth)
        {
            IList<TMove> killers;
            if (killerSteps.TryGetValue(depth, out killers))
            {
                return killers;
            }
            return Enumerable.Empty<TMove>();
        }

        private void UpdateKillerSteps(TMove move, int depth)
        {
            IList<TMove> killers;
            if (killerSteps.TryGetValue(depth, out killers))
            {
                if (killers[0].Equals(move))
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