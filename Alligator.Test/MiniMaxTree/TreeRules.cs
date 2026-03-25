using Alligator.Solver;

namespace Alligator.Test.MiniMaxTree
{
    internal class TreeRules : IRules<TreePosition, int>
    {
        private readonly TreeNode root;

        public TreeRules(TreeNode root)
        {
            this.root = root ?? throw new ArgumentNullException(nameof(root));
        }

        public TreePosition InitialPosition()
        {
            return new TreePosition(root);
        }

        public IEnumerable<int> LegalStepsAt(TreePosition position)
        {
            if (position.Current.IsLeaf)
            {
                if (!position.Current.IsGoal)
                    yield return 0;
                yield break;
            }
            for (int i = 0; i < position.Current.Children.Count; i++)
            {
                yield return i;
            }
        }

        public bool IsGoal(TreePosition position)
        {
            return position.Current.IsGoal;
        }
    }
}
