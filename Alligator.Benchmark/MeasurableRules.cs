using Alligator.SixMaking.Model;
using Alligator.Solver;

namespace Alligator.Benchmark
{
    internal class MeasurableRules : IRules<IPosition, Step>
    {
        public int InitialPositionCallCount = 0;
        public int LegalStepsCallCount = 0;
        public int IsGoalCallCount = 0;

        private readonly IRules<IPosition, Step> rules;

        public MeasurableRules(IRules<IPosition, Step> rules)
        {
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        public IPosition InitialPosition()
        {
            InitialPositionCallCount++;
            return rules.InitialPosition();
        }

        public IEnumerable<Step> LegalStepsAt(IPosition position)
        {
            LegalStepsCallCount++;
            return rules.LegalStepsAt(position);
        }

        public bool IsGoal(IPosition position)
        {
            IsGoalCallCount++;
            return rules.IsGoal(position);
        }
        
        public void ClearCounters()
        {
            InitialPositionCallCount = 0;
            LegalStepsCallCount = 0;
            IsGoalCallCount = 0;
        }
    }
}