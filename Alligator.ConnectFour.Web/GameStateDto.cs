namespace Alligator.ConnectFour.Web;

public class GameStateDto
{
    public string[][] Board { get; init; } = [];
    public bool GameOver { get; init; }
    public string? Winner { get; init; }
    public int MoveCount { get; init; }
    public int? LastCpuColumn { get; init; }
}
