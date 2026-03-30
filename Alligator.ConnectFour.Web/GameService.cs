using Alligator.Solver;

namespace Alligator.ConnectFour.Web;

public class GameService
{
    private readonly object sync = new();
    private readonly Rules rules = new();
    private ISolver<Drop> solver;
    private Position position;
    private List<Drop> history;
    private int? lastCpuColumn;

    public GameService()
    {
        solver = CreateSolver();
        position = new Position();
        history = [];
    }

    public GameStateDto NewGame()
    {
        lock (sync)
        {
            solver = CreateSolver();
            position = new Position();
            history = [];
            lastCpuColumn = null;

            PlayCpu();
            return BuildState();
        }
    }

    public GameStateDto GetState()
    {
        lock (sync)
        {
            return BuildState();
        }
    }

    public GameStateDto MakeMove(int column)
    {
        lock (sync)
        {
            if (IsGameOver())
                throw new InvalidOperationException("Game is already over.");
            if (column < 0 || column >= Position.Columns)
                throw new ArgumentOutOfRangeException(nameof(column), $"Column must be 0\u2013{Position.Columns - 1}.");
            if (position.HeightAt(column) >= Position.Rows)
                throw new InvalidOperationException($"Column {column} is full.");

            var drop = Drop.At(column);
            position.Take(drop);
            history.Add(drop);
            lastCpuColumn = null;

            if (!IsGameOver())
            {
                PlayCpu();
            }

            return BuildState();
        }
    }

    private ISolver<Drop> CreateSolver()
    {
        return new SolverProvider<Position, Drop>(rules, new WebConfiguration()).Create();
    }

    private void PlayCpu()
    {
        var cpuDrop = solver.OptimizeNextStep(history);
        position.Take(cpuDrop);
        history.Add(cpuDrop);
        lastCpuColumn = cpuDrop.Column;
    }

    private bool IsGameOver() => !rules.LegalStepsAt(position).Any();

    private GameStateDto BuildState()
    {
        var board = new string[Position.Rows][];
        for (int displayRow = 0; displayRow < Position.Rows; displayRow++)
        {
            int boardRow = Position.Rows - 1 - displayRow;
            board[displayRow] = new string[Position.Columns];
            for (int c = 0; c < Position.Columns; c++)
            {
                board[displayRow][c] = position.DiskAt(boardRow, c) switch
                {
                    Disk.Red => "R",
                    Disk.Yellow => "Y",
                    _ => ""
                };
            }
        }

        string? winner = null;
        if (position.HasWinner())
        {
            winner = history.Count % 2 == 1 ? "Red" : "Yellow";
        }

        return new GameStateDto
        {
            Board = board,
            GameOver = IsGameOver(),
            Winner = winner,
            MoveCount = history.Count,
            LastCpuColumn = lastCpuColumn
        };
    }

    private class WebConfiguration : Alligator.Solver.IConfiguration
    {
        public int MaxDepth => 43;
        public TimeSpan? TimeBudget => TimeSpan.FromSeconds(5);
    }
}
