using Alligator.Solver;

namespace Alligator.ConnectFour
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Hello Connect Four!");
            Console.WriteLine();

            IRules<Position, Drop> rules = new Rules();
            IConfiguration solverConfiguration = new Configuration();

            SolverProvider<Position, Drop> solverFactory = new SolverProvider<Position, Drop>(rules, solverConfiguration, SolverLog);
            ISolver<Drop> solver = solverFactory.Create();

            Position position = new Position();
            IList<Drop> history = new List<Drop>();
            bool aiStep = true;

            while (rules.LegalStepsAt(position).Any())
            {
                PrintPosition(position);
                Drop next;

                if (aiStep)
                {
                    while (true)
                    {
                        try
                        {
                            next = AiStep(history, solver);
                            position.Take(next);
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
                            next = HumanStep(position);
                            position.Take(next);
                            break;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                history.Add(next);
                aiStep = !aiStep;
            }

            PrintPosition(position);

            if (!rules.IsGoal(position))
            {
                Console.WriteLine("Game over, DRAW!");
            }
            else
            {
                Console.WriteLine(string.Format("Game over, {0} WON!", aiStep ? "human" : "ai"));
            }

            Console.ReadKey();
        }

        private static Drop HumanStep(Position position)
        {
            Console.Write("Your turn! Enter column (0-6): ");
            while (true)
            {
                try
                {
                    string input = Console.ReadLine() ?? string.Empty;
                    int col = int.Parse(input.Trim());

                    if (col < 0 || col >= Position.Columns)
                    {
                        throw new ArgumentOutOfRangeException($"Column must be between 0 and {Position.Columns - 1}.");
                    }
                    if (position.HeightAt(col) >= Position.Rows)
                    {
                        throw new InvalidOperationException($"Column {col} is full.");
                    }

                    return Drop.At(col);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.Write("Try again (0-6): ");
                }
            }
        }

        private static Drop AiStep(IList<Drop> history, ISolver<Drop> solver)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("AI is thinking...");

            var next = solver.OptimizeNextStep(history);

            Console.WriteLine(string.Format("AI drops into column {0}", next.Column));
            Console.ForegroundColor = ConsoleColor.White;

            return next;
        }

        private static void PrintPosition(Position position)
        {
            Console.WriteLine();

            // Column headers
            Console.Write("  ");
            for (int c = 0; c < Position.Columns; c++)
            {
                Console.Write(string.Format(" {0}", c));
            }
            Console.WriteLine();

            // Board — row 5 (top) to row 0 (bottom)
            for (int r = Position.Rows - 1; r >= 0; r--)
            {
                Console.Write("  ");
                for (int c = 0; c < Position.Columns; c++)
                {
                    Disk disk = position.DiskAt(r, c);
                    switch (disk)
                    {
                        case Disk.None:
                            Console.Write(" .");
                            break;
                        case Disk.Red:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(" O");
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case Disk.Yellow:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write(" O");
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
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
