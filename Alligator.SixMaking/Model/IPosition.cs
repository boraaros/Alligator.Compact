using Alligator.Solver;

namespace Alligator.SixMaking.Model
{
    public interface IPosition : IPosition<Step>
    {
        Disk Next { get; }
        Step LastStep { get; }
        IList<Step> History { get; }
        int ColumnHeightAt(int cell);
        Disk DiskAt(int cell, int height);
        bool IsOver { get; }
    }
}