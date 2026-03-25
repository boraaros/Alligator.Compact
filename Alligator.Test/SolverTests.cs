using Alligator.Solver.Algorithms;
using Alligator.Test.MiniMaxTree;
using static Alligator.Test.MiniMaxTree.TreeNode;

namespace Alligator.Test
{
    [TestClass]
    public class SolverTests
    {
        /// <summary>
        /// Runs the full solver (iterative deepening + MTD(f)) on the given tree
        /// and returns the index of the root's chosen child.
        /// </summary>
        private static int Solve(TreeNode root, int maxDepth = 7)
        {
            var rules = new TreeRules(root);
            var cacheTables = new CacheTables<TreePosition, int>();
            var heuristicTables = new HeuristicTables<int>();
var searchManager = new SearchManager(maxDepth - 1);
var alphaBeta = new AlphaBetaPruning<TreePosition, int>(
    rules, cacheTables, heuristicTables, searchManager);
var solver = new AlphaBetaSolver<TreePosition, int>(
    alphaBeta, rules, searchManager, _ => { }, maxDepth);

            return solver.OptimizeNextStep([]);
        }

        //
        //        root (max)
        //       /    \
        //     A(min)  B(min)
        //    / \      / \
        //  [3] [-1] [2] [5]
        //
        // A = min(3, -1) = -1
        // B = min(2, 5)  =  2
        // root = max(-1, 2) = 2 → step 1 (B)
        //
        [TestMethod]
        public void Simple_two_level_tree_picks_optimal_branch()
        {
            var tree = Inner(
                Inner(Leaf(3), Leaf(-1)),
                Inner(Leaf(2), Leaf(5)));

            Assert.AreEqual(1, Solve(tree));
        }

        //
        //        root (max)
        //       /    \
        //     A(min)  B(min)
        //    / \      / \
        //  [5] [3]  [1] [3]
        //
        // A = min(5, 3) = 3
        // B = min(1, 3) = 1
        // root = max(3, 1) = 3 → step 0 (A)
        //
        [TestMethod]
        public void Symmetric_minimax_values_picks_left()
        {
            var tree = Inner(
                Inner(Leaf(5), Leaf(3)),
                Inner(Leaf(1), Leaf(3)));

            Assert.AreEqual(0, Solve(tree));
        }

        //
        //        root (max)
        //       /    \
        //     A(min)  B(min)
        //    / \      / \
        //  [4] [4]  [4] [4]
        //
        // Both branches equal → either is acceptable.
        //
        [TestMethod]
        public void Equal_branches_does_not_crash()
        {
            var tree = Inner(
                Inner(Leaf(4), Leaf(4)),
                Inner(Leaf(4), Leaf(4)));

            int step = Solve(tree);
            Assert.IsTrue(step == 0 || step == 1);
        }

        //
        //           root (max)
        //         /   |   \
        //       A     B     C
        //      / \   / \   /|\
        //    [1] [9] [5][3] [2][7][4]
        //
        // A = min(1, 9) = 1
        // B = min(5, 3) = 3
        // C = min(2, 7, 4) = 2
        // root = max(1, 3, 2) = 3 → step 1 (B)
        //
        [TestMethod]
        public void Uneven_branching_factor()
        {
            var tree = Inner(
                Inner(Leaf(1), Leaf(9)),
                Inner(Leaf(5), Leaf(3)),
                Inner(Leaf(2), Leaf(7), Leaf(4)));

            Assert.AreEqual(1, Solve(tree));
        }

        //
        //             root (max)
        //            /          \
        //          A (min)      B (min)
        //         / \           / \
        //       A1   A2       B1   B2
        //      / \   / \     / \   / \
        //    [1] [8][5][2] [7][3] [6][4]
        //
        // A1 = min(1,8)=1, A2 = min(5,2)=2 → A = max(1,2) = 2
        // B1 = min(7,3)=3, B2 = min(6,4)=4 → B = max(3,4) = 4
        // root = max(2, 4) = 4 → step 1 (B)
        //
        // Depth-2 iteration would see different values, depth-4 corrects.
        //
        [TestMethod]
        public void Four_level_tree_requires_deep_search()
        {
            var tree = Inner(
                Inner(
                    Inner(Leaf(1), Leaf(8)),
                    Inner(Leaf(5), Leaf(2))),
                Inner(
                    Inner(Leaf(7), Leaf(3)),
                    Inner(Leaf(6), Leaf(4))));

            Assert.AreEqual(1, Solve(tree));
        }

        //
        //        root (max)
        //       /    \
        //     A(min)  B(min)
        //    / \      / \
        //  [3] [1]  [GOAL] [2]
        //
        // A = min(3, 1) = 1
        // B: step 0 is a goal (opponent won) → value = -(sbyte.MaxValue + depth)
        //    B = min(very_negative, 2) = very_negative
        // root = max(1, very_negative) = 1 → step 0 (A avoids defeat)
        //
        [TestMethod]
        public void Solver_avoids_opponents_goal()
        {
            var tree = Inner(
                Inner(Leaf(3), Leaf(1)),
                Inner(Goal(), Leaf(2)));

            Assert.AreEqual(0, Solve(tree));
        }

        //
        //        root (max)
        //       /    \  
        //   [GOAL]   A(min)
        //            / \
        //          [1] [-5]
        //
        // step 0 leads to goal (root won) → very high value
        // A = min(1, -5) = -5
        // root = max(very_high, -5) → step 0
        //
        [TestMethod]
        public void Solver_finds_immediate_goal()
        {
            var tree = Inner(
                Goal(),
                Inner(Leaf(1), Leaf(-5)));

            Assert.AreEqual(0, Solve(tree));
        }

        //
        //        root (max)
        //       /    \
        //     A(min)  B(min)
        //    / \      / \
        //  [-2] [10] [7] [-3]
        //
        // A = min(-2, 10) = -2
        // B = min(7, -3)  = -3
        // root = max(-2, -3) = -2 → step 0 (A), best of two bad options
        //
        [TestMethod]
        public void All_negative_minimax_picks_least_bad()
        {
            var tree = Inner(
                Inner(Leaf(-2), Leaf(10)),
                Inner(Leaf(7), Leaf(-3)));

            Assert.AreEqual(0, Solve(tree));
        }

        //
        //        root (max)
        //           |
        //         A (min)
        //         / \
        //       [3] [7]
        //
        // Only one move from root → must pick step 0.
        //
        [TestMethod]
        public void Single_child_forced_move()
        {
            var tree = Inner(
                Inner(Leaf(3), Leaf(7)));

            Assert.AreEqual(0, Solve(tree));
        }
    }
}