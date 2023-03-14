using Alligator.Solver;

namespace Alligator.Demo
{
    internal class Position : IPosition<Placement>
    {
        private readonly Symbol[,] board;
        private Symbol nextSymbolType;

        public ulong Identifier { get; private set; }
        public IList<Placement> History { get; }
        public sbyte Value => 0;

        public const int BoardSize = 3;

        public Position()
        {
            Identifier = 0ul;
            board = new Symbol[BoardSize, BoardSize];
            History = new List<Placement>();
            nextSymbolType = Symbol.X;
        }

        public Position(IEnumerable<Placement> history)
            : this()
        {
            foreach (var move in history)
            {
                Take(move);
            }
        }

        public void Take(Placement move)
        {
            if (move == null)
            {
                throw new ArgumentNullException(nameof(move));
            }
            if (board[move.Row, move.Column] != Symbol.Empty)
            {
                throw new InvalidOperationException(string.Format("Invalid placement because target cell is not empty: [{0},{1}]",
                    move.Row, move.Column));
            }
            board[move.Row, move.Column] = nextSymbolType;
            History.Add(move);
            nextSymbolType = SwapSymbol(nextSymbolType);
            Identifier = ComputeIdentifier();
        }

        public void TakeBack()
        {
            if (History.Count == 0)
            {
                throw new InvalidOperationException("Cannot remove last placement from the empty board");
            }
            var lasTMove = History[History.Count - 1];
            if (board[lasTMove.Row, lasTMove.Column] == Symbol.Empty)
            {
                throw new InvalidOperationException($"Cannot remove last placement because target cell is already empty: [{lasTMove.Row},{lasTMove.Column}]");
            }
            board[lasTMove.Row, lasTMove.Column] = Symbol.Empty;
            History.RemoveAt(History.Count - 1);
            nextSymbolType = SwapSymbol(nextSymbolType);
            Identifier = ComputeIdentifier();
        }

        public Symbol GetSymbolAt(int row, int column)
        {
            return board[row, column];
        }

        private ulong ComputeIdentifier()
        {
            var hashCode = 0ul;
            var exp = 0;

            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    if (board[i, j] == Symbol.X)
                    {
                        hashCode += (ulong)Math.Pow(2, exp);
                    }
                    exp++;
                    if (board[i, j] == Symbol.O)
                    {
                        hashCode += (ulong)Math.Pow(2, exp);
                    }
                    exp++;
                }
            }
            return hashCode;
        }

        private static Symbol SwapSymbol(Symbol Mark)
        {
            if (Mark == Symbol.Empty)
            {
                throw new InvalidOperationException("Cannot change the undefined symbol");
            }
            return Mark == Symbol.X ? Symbol.O : Symbol.X;
        }
    }
}