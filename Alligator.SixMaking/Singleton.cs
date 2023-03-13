namespace Alligator.SixMaking
{
    public abstract class Singleton<TClass>
        where TClass : class
    {
        private static readonly Lazy<TClass> instance =
            new Lazy<TClass>(
                () => CreateInstanceOfT());

        public static TClass Instance
        {
            get
            {
                return instance.Value;
            }
        }

        private static TClass CreateInstanceOfT()
        {
            return Activator.CreateInstance(typeof(TClass), true) as TClass;
        }
    }
}