using Alligator.SixMaking.Model;

namespace Alligator.SixMaking.Logics
{
    public interface IStepPool
    {
        Step GetPlacement(int to);
        Step GetMovement(int from, int to, int count);
    }
}