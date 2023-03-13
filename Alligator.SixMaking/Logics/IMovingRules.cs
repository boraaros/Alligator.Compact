using Alligator.SixMaking.Model;

namespace Alligator.SixMaking.Logics
{
    public interface IMovingRules
    {
        IEnumerable<int> LegalMovementsAt(IPosition position, int from);
        bool AreInverses(Step one, Step other);
    }
}