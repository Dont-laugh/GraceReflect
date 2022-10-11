namespace DontLaugh
{
    public interface IOptimizedAccessor
    {
        void Compile();

        object GetValue();

        void SetValue(object value);
    }

    public interface IOptimizedAccessor<T> : IOptimizedAccessor
    {
        new T GetValue();

        void SetValue(T value);
    }
}
