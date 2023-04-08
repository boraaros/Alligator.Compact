using Alligator.Solver;
using Alligator.TicTacToe;

namespace Alligator.Demo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Hello tic-tac-toe demo!");

            var rules = new Rules();
            var solverConfiguration = new Configuration();
            var solverFactory = new SolverProvider<Position, Placement>(rules, solverConfiguration, SolverLog);
            ISolver<Placement> solver = solverFactory.Create();

            Position position = new Position();
            IList<Placement> history = new List<Placement>();
            bool aiTurn = true;

            while (rules.LegalStepsAt(position).Any())
            {
                PrintPosition(position);
                Placement next;
                Position copy = new Position(position.History);

                if (aiTurn)
                {
                    while (true)
                    {
                        try
                        {
                            solver = solverFactory.Create();
                            next = AiStep(history, solver);
                            copy.Take(next);
                            break;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                else
                {
                    while (true)
                    {
                        try
                        {
                            next = HumanStep();
                            copy.Take(next);
                            break;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                position.Take(next);
                history.Add(next);
                aiTurn = !aiTurn;
            }
            if (!rules.IsGoal(position))
            {
                Console.WriteLine("Game over, DRAW!");
            }
            else
            {
                Console.WriteLine(string.Format("Game over, {0} WON!", aiTurn ? "human" : "ai"));
            }

            PrintPosition(position);

            Console.ReadKey();
        }

        private static Placement HumanStep()
        {
            Console.Write("Next step [row:column]: ");
            while (true)
            {
                try
                {
                    string[] msg = Console.ReadLine().Split(':');
                    return new Placement(int.Parse(msg[0]), int.Parse(msg[1]));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static Placement AiStep(IList<Placement> history, ISolver<Placement> solver)
        {
            var next = solver.OptimizeNextStep(history);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("AI is thinking...");
            Console.WriteLine(string.Format("Optimal next step: {0}", next));
            Console.ForegroundColor = ConsoleColor.White;

            return next;
        }

        private static void PrintPosition(Position position)
        {
            Console.WriteLine(string.Join("-", Enumerable.Range(0, Position.BoardSize + 1).Select(t => "-")));

            for (int i = 0; i < Position.BoardSize; i++)
            {
                for (int j = 0; j < Position.BoardSize; j++)
                {
                    switch (position.GetSymbolAt(i, j))
                    {
                        case Symbol.Empty:
                            Console.Write(string.Format(" {0}", "."));
                            break;
                        case Symbol.X:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(string.Format(" {0}", Symbol.X));
                            break;
                        case Symbol.O:
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write(string.Format(" {0}", Symbol.O));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException($"Unknown tic-tac-toe symbol: {position.GetSymbolAt(i, j)}");
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine();
            }
            Console.WriteLine(string.Join("-", Enumerable.Range(0, Position.BoardSize + 1).Select(t => "-")));
        }

        private static void SolverLog(string message)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(string.Format("[SolverLog] {0}", message));
            Console.ForegroundColor = prevColor;
        }
    }
}