using Alligator.Solver.Algorithms;

namespace Alligator.Test
{
    [TestClass]
    public class SolverTests
    {
        [TestMethod]
        public void FirstSolverTest()
        {
            // Arrange
            var valuesById = new Dictionary<ulong, sbyte>
            {
                { 0, 0 },
                { 1, 0 },
                { 2, 0 },
                { 3, 3 },
                { 4, 1 },
                { 5, 2 },
                { 6, 0 },
                { 7, 0 },
                { 8, 0 },
                { 9, 0 },
                { 10, 2 },
                { 11, 1 },
                { 12, 3 },
                { 13, 0 },
                { 14, 3 },
                { 15, 0 }
            };
            uint degree = 2;
            int depth = 2;

            var rules = new UniformTreeRules(valuesById, degree);
            var cacheTables = new CacheTables<UniformTreePosition, int>();
            var heuristicTables = new HeuristicTables<int>();
            var searchManager = new SearchManager(depth - 1);
            var alphabeta = new AlphaBetaPruning<UniformTreePosition, int>(rules, cacheTables, heuristicTables, searchManager);
            var solver = new AlphaBetaSolver<UniformTreePosition, int>(alphabeta, rules);

            // Act
            var bestStep = solver.OptimizeNextStep(new List<int>());

            // Assert
            Assert.AreEqual(0, bestStep);
        }
    }
}