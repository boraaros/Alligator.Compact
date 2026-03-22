using Alligator.Solver;

namespace Alligator.Test.MiniMaxTree
{
    internal class TreePosition : IPosition<int>
    {
        private readonly Stack<(TreeNode Node, int Step, ulong PrevIdentifier)> path = new();
        private TreeNode current;

        public TreePosition(TreeNode root)
        {
            current = root ?? throw new ArgumentNullException(nameof(root));
        }

        public TreeNode Current => current;

        public ulong Identifier { get; private set; }

        public sbyte Value => current.LeafValue ?? 0;

        public void Take(int step)
        {
            path.Push((current, step, Identifier));
            if (!current.IsLeaf)
                current = current.Children[step];
            Identifier = unchecked(Identifier * 7919 + (ulong)step + 1);
        }

        public void TakeBack()
        {
            var (parent, _, prevId) = path.Pop();
            current = parent;
            Identifier = prevId;
        }
    }
}
