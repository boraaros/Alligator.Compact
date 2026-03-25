namespace Alligator.Test.MiniMaxTree
{
    internal class TreeNode
    {
        public sbyte? LeafValue { get; }
        public bool IsGoal { get; }
        public List<TreeNode> Children { get; } = [];

        private TreeNode(sbyte? leafValue, bool isGoal, IEnumerable<TreeNode> children)
        {
            LeafValue = leafValue;
            IsGoal = isGoal;
            Children.AddRange(children);
        }

        public bool IsLeaf => Children.Count == 0;

        public static TreeNode Leaf(sbyte value) => new(value, isGoal: false, []);

        public static TreeNode Goal() => new(leafValue: null, isGoal: true, []);

        public static TreeNode Inner(params TreeNode[] children)
        {
            if (children.Length == 0)
                throw new ArgumentException("Inner node must have at least one child.");
            return new(leafValue: null, isGoal: false, children);
        }
    }
}
