using Alligator.SixMaking.Logics;
using Alligator.SixMaking.Model;
using Alligator.Solver;

namespace Alligator.SixMaking
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Hello Six Making!");

            IRules<IPosition, Step> rules = new Rules(StepPool.Instance, new MovingRules());
            IConfiguration solverConfiguration = new Configuration();

            SolverProvider<IPosition, Step> solverFactory = new SolverProvider<IPosition, Step>(rules, solverConfiguration, SolverLog);
            ISolver<Step> solver = solverFactory.Create();

            IPosition position = new Position();
            IList<Step> history = new List<Step>();
            bool aiStep = true;

            while (rules.LegalStepsAt(position).Any())
            {
                PrintPosition(position);
                Step next;
                Position copy = new Position(position.History);

                if (aiStep)
                {
                    while (true)
                    {
                        try
                        {
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
                aiStep = !aiStep;
            }
            if (!rules.IsGoal(position))
            {
                Console.WriteLine("Game over, DRAW!");
            }
            else
            {
                Console.WriteLine(string.Format("Game over, {0} WON!", aiStep ? "human" : "ai"));
            }

            PrintPosition(position);

            Console.ReadKey();
        }

        private static Step HumanStep()
        {
            Console.Write("Next step [from:to:count]: ");
            while (true)
            {
                try
                {
                    string[] msg = Console.ReadLine().Split(':');
                    int from = int.Parse(msg[0]);
                    int to = int.Parse(msg[1]);
                    int count = int.Parse(msg[2]);
                    if (from == -1)
                    {
                        return StepPool.Instance.GetPlacement(to);
                    }
                    else
                    {
                        return StepPool.Instance.GetMovement(from, to, count);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static Step AiStep(IList<Step> history, ISolver<Step> solver)
        {
            var next = solver.OptimizeNextStep(history);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("AI is thinking...");
            Console.WriteLine(string.Format("Optimal next step: {0}", next));
            Console.ForegroundColor = ConsoleColor.White;

            return next;
        }

        private static void PrintPosition(IPosition position)
        {
            Console.WriteLine();

            for (int i = 0; i < Constants.BoardSize; i++)
            {
                for (int j = 0; j < Constants.BoardSize; j++)
                {
                    for (int k = 0; k < 10; k++)
                    {
                        switch (position.DiskAt(Constants.BoardSize * i + j, k))
                        {
                            case Disk.None:
                                Console.Write(string.Format(" {0}", "."));
                                break;
                            case Disk.Red:
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(string.Format(" {0}", "|"));
                                break;
                            case Disk.Yellow:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write(string.Format(" {0}", "|"));
                                break;
                            default:
                                throw new ArgumentOutOfRangeException($"Unknown disk type: {position.DiskAt(Constants.BoardSize * i + j, k)}");
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.Write(" ");
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