namespace DontLaugh
{
    public interface IOptimizedInvoker
    {
        void Compile();

        object Invoke();

        object Invoke(object arg0);

        object Invoke(object arg0, object arg1);

        object Invoke(object arg0, object arg1, object arg2);

        object Invoke(object arg0, object arg1, object arg2, object arg3);

        object Invoke(object arg0, object arg1, object arg2, object arg3, object arg4);
    }

    public interface IOptimizedInvoker<out T> : IOptimizedInvoker
    {
        new T Invoke();

        new T Invoke(object arg0);

        new T Invoke(object arg0, object arg1);

        new T Invoke(object arg0, object arg1, object arg2);

        new T Invoke(object arg0, object arg1, object arg2, object arg3);

        new T Invoke(object arg0, object arg1, object arg2, object arg3, object arg4);
    }
}
