using Alligator.SixMaking.Model;

namespace Alligator.SixMaking.Logics
{
    public interface IStepPool
    {
        int Size { get; }
        Step GetPlacement(int to);
        Step GetMovement(int from, int to, int count);
    }
}