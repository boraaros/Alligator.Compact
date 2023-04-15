using Alligator.Solver;
using Alligator.TicTacToe;

namespace Alligator.Test
{
    [TestClass]
    public class TicTacToeTests
    {
        [TestMethod]
        public void Solver_finds_the_winning_step()
        {
            // Arrange

            // O O .
            // X . X
            // . . .
            List<Placement> history = new List<Placement>()
            {
                new Placement(1, 0),
                new Placement(0, 0),
                new Placement(1, 2),
                new Placement(0, 1)
            };

            var solverProvider = new SolverProvider<Position, Placement>(new Rules(), new TestCofiguration());
            var solver = solverProvider.Create();

            // Act
            var optimalStep = solver.OptimizeNextStep(history);

            // Assert
            Assert.AreEqual(11, optimalStep.Row);
            Assert.AreEqual(1, optimalStep.Column);
        }

        [TestMethod]
        public void Solver_finds_the_winning_branch()
        {
            // Arrange

            // O X .
            // X . .
            // . . O
            List<Placement> history = new List<Placement>()
            {
                new Placement(1, 0),
                new Placement(0, 0),
                new Placement(0, 1),
                new Placement(2, 2)
            };

            var solverProvider = new SolverProvider<Position, Placement>(new Rules(), new TestCofiguration());
            var solver = solverProvider.Create();

            // Act
            var optimalStep = solver.OptimizeNextStep(history);

            // Assert
            Assert.AreEqual(1, optimalStep.Row);
            Assert.AreEqual(1, optimalStep.Column);
        }

        [TestMethod]
        public void Solver_avoids_the_defeat()
        {
            // Arrange

            // . X .
            // X . .
            // O . O
            List<Placement> history = new List<Placement>()
            {
                new Placement(0, 1),
                new Placement(2, 0),
                new Placement(1, 0),
                new Placement(2, 2)
            };

            var solverProvider = new SolverProvider<Position, Placement>(new Rules(), new TestCofiguration());
            var solver = solverProvider.Create();

            // Act
            var optimalStep = solver.OptimizeNextStep(history);

            // Assert
            Assert.AreEqual(2, optimalStep.Row);
            Assert.AreEqual(1, optimalStep.Column);
        }

        private class TestCofiguration : IConfiguration
        {
        }
    }
}
