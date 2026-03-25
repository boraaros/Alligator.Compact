using Alligator.ConnectFour;
using Alligator.Solver;

namespace Alligator.Test
{
    [TestClass]
    public class ConnectFourTests
    {
        private Rules rules = null!;
        private SolverProvider<Position, Drop> solverFactory = null!;

        [TestInitialize]
        public void Setup()
        {
            rules = new Rules();
            solverFactory = new SolverProvider<Position, Drop>(rules, new ConnectFourConfiguration());
        }

        [TestMethod]
        public void InitialPosition_has_7_legal_moves()
        {
            var position = rules.InitialPosition();
            var moves = rules.LegalStepsAt(position).ToList();

            Assert.AreEqual(Position.Columns, moves.Count);
        }

        [TestMethod]
        public void HasWinner_detects_vertical_win()
        {
            var position = new Position();
            // Red drops in column 0 four times, Yellow drops in column 1
            position.Take(Drop.At(0)); // Red
            position.Take(Drop.At(1)); // Yellow
            position.Take(Drop.At(0)); // Red
            position.Take(Drop.At(1)); // Yellow
            position.Take(Drop.At(0)); // Red
            position.Take(Drop.At(1)); // Yellow
            position.Take(Drop.At(0)); // Red — 4 in column 0

            Assert.IsTrue(position.HasWinner());
        }

        [TestMethod]
        public void HasWinner_detects_horizontal_win()
        {
            var position = new Position();
            // Red: cols 0,1,2,3. Yellow: cols 0,1,2 (row above)
            position.Take(Drop.At(0)); // Red r0c0
            position.Take(Drop.At(0)); // Yellow r1c0
            position.Take(Drop.At(1)); // Red r0c1
            position.Take(Drop.At(1)); // Yellow r1c1
            position.Take(Drop.At(2)); // Red r0c2
            position.Take(Drop.At(2)); // Yellow r1c2
            position.Take(Drop.At(3)); // Red r0c3 — 4 horizontal

            Assert.IsTrue(position.HasWinner());
        }

        [TestMethod]
        public void HasWinner_detects_diagonal_win()
        {
            var position = new Position();
            // Build a diagonal for Red: (0,0), (1,1), (2,2), (3,3)
            position.Take(Drop.At(0)); // Red r0c0
            position.Take(Drop.At(1)); // Yellow r0c1
            position.Take(Drop.At(1)); // Red r1c1
            position.Take(Drop.At(2)); // Yellow r0c2
            position.Take(Drop.At(2)); // Red r1c2
            position.Take(Drop.At(3)); // Yellow r0c3
            position.Take(Drop.At(2)); // Red r2c2
            position.Take(Drop.At(3)); // Yellow r1c3
            position.Take(Drop.At(3)); // Red r2c3
            position.Take(Drop.At(4)); // Yellow r0c4
            position.Take(Drop.At(3)); // Red r3c3 — diagonal win

            Assert.IsTrue(position.HasWinner());
        }

        [TestMethod]
        public void No_legal_moves_after_win()
        {
            var position = new Position();
            position.Take(Drop.At(0));
            position.Take(Drop.At(1));
            position.Take(Drop.At(0));
            position.Take(Drop.At(1));
            position.Take(Drop.At(0));
            position.Take(Drop.At(1));
            position.Take(Drop.At(0)); // Red wins vertically

            var moves = rules.LegalStepsAt(position).ToList();
            Assert.AreEqual(0, moves.Count);
        }

        [TestMethod]
        public void TakeBack_restores_position()
        {
            var position = new Position();
            var idBefore = position.Identifier;

            position.Take(Drop.At(3));
            Assert.AreNotEqual(idBefore, position.Identifier);

            position.TakeBack();
            Assert.AreEqual(idBefore, position.Identifier);
            Assert.AreEqual(0, position.HeightAt(3));
        }

        [TestMethod]
        public void Solver_finds_winning_move_in_connect_four()
        {
            // Red has 3 in a row at bottom (cols 0,1,2), column 3 is open
            // Solver should find the winning drop
            var position = new Position();
            position.Take(Drop.At(0)); // Red
            position.Take(Drop.At(0)); // Yellow
            position.Take(Drop.At(1)); // Red
            position.Take(Drop.At(1)); // Yellow
            position.Take(Drop.At(2)); // Red
            position.Take(Drop.At(2)); // Yellow
            // Red's turn — dropping in col 3 wins

            var solver = solverFactory.Create();
            var history = new List<Drop>();
            // Replay moves as history
            history.Add(Drop.At(0)); history.Add(Drop.At(0));
            history.Add(Drop.At(1)); history.Add(Drop.At(1));
            history.Add(Drop.At(2)); history.Add(Drop.At(2));

            var bestMove = solver.OptimizeNextStep(history);
            Assert.AreEqual(3, bestMove.Column);
        }

        [TestMethod]
        public void Solver_blocks_opponent_winning_move()
        {
            // Yellow has 3 in a row at bottom (cols 0,1,2), Red must block col 3
            var history = new List<Drop>
            {
                Drop.At(6), Drop.At(0),  // Red col6, Yellow col0
                Drop.At(6), Drop.At(1),  // Red col6, Yellow col1
                Drop.At(5), Drop.At(2),  // Red col5, Yellow col2
                // Red's turn — must block Yellow at col 3
            };

            var solver = solverFactory.Create();
            var bestMove = solver.OptimizeNextStep(history);
            Assert.AreEqual(3, bestMove.Column);
        }

        private class ConnectFourConfiguration : IConfiguration
        {
        }
    }
}
