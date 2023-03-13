namespace Alligator.SixMaking
{
    public interface IHashing
    {
        ulong HashCode { get; }
        void Modify(params int[] indices);
    }
}