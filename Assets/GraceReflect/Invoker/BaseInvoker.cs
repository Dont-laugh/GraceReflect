using System;
using System.Reflection;

namespace DontLaugh
{
    public abstract class BaseInvoker : IOptimizedInvoker
    {
        protected readonly MethodInfo _methodInfo;

        protected BaseInvoker(MethodInfo methodInfo)
        {
            if (GraceReflection.doCheck)
            {
                Ensure.That(nameof(methodInfo)).IsNotNull(methodInfo);
            }
            _methodInfo = methodInfo;
        }

        public void Compile()
        {
            if (GraceReflection.canEmit)
            {
                CompileEmit();
            }
            else
            {
                CreateDelegate();
            }
        }

        protected abstract void CompileEmit();

        protected abstract void CreateDelegate();

        public virtual object Invoke()
        {
            throw new NotSupportedException();
        }

        public virtual object Invoke(object arg0)
        {
            throw new NotSupportedException();
        }

        public virtual object Invoke(object arg0, object arg1)
        {
            throw new NotSupportedException();
        }

        public virtual object Invoke(object arg0, object arg1, object arg2)
        {
            throw new NotSupportedException();
        }

        public virtual object Invoke(object arg0, object arg1, object arg2, object arg3)
        {
            throw new NotSupportedException();
        }

        public virtual object Invoke(object arg0, object arg1, object arg2, object arg3, object arg4)
        {
            throw new NotSupportedException();
        }
    }

    public abstract class BaseInvoker<TResult> : IOptimizedInvoker<TResult>
    {
        protected readonly MethodInfo _methodInfo;

        protected BaseInvoker(MethodInfo methodInfo)
        {
            if (GraceReflection.doCheck)
            {
                Ensure.That(nameof(methodInfo)).IsNotNull(methodInfo);
            }
            _methodInfo = methodInfo;
        }

        public void Compile()
        {
            if (GraceReflection.canEmit)
            {
                CompileEmit();
            }
            else
            {
                CreateDelegate();
            }
        }

        protected abstract void CompileEmit();

        protected abstract void CreateDelegate();

        public virtual TResult Invoke()
        {
            throw new NotSupportedException();
        }

        public virtual TResult Invoke(object arg0)
        {
            throw new NotSupportedException();
        }

        public virtual TResult Invoke(object arg0, object arg1)
        {
            throw new NotSupportedException();
        }

        public virtual TResult Invoke(object arg0, object arg1, object arg2)
        {
            throw new NotSupportedException();
        }

        public virtual TResult Invoke(object arg0, object arg1, object arg2, object arg3)
        {
            throw new NotSupportedException();
        }

        public virtual TResult Invoke(object arg0, object arg1, object arg2, object arg3, object arg4)
        {
            throw new NotSupportedException();
        }

        object IOptimizedInvoker.Invoke()
        {
            return Invoke();
        }

        object IOptimizedInvoker.Invoke(object arg0)
        {
            return Invoke(arg0);
        }

        object IOptimizedInvoker.Invoke(object arg0, object arg1)
        {
            return Invoke(arg0, arg1);
        }

        object IOptimizedInvoker.Invoke(object arg0, object arg1, object arg2)
        {
            return Invoke(arg0, arg1, arg2);
        }

        object IOptimizedInvoker.Invoke(object arg0, object arg1, object arg2, object arg3)
        {
            return Invoke(arg0, arg1, arg2, arg3);
        }

        object IOptimizedInvoker.Invoke(object arg0, object arg1, object arg2, object arg3, object arg4)
        {
            return Invoke(arg0, arg1, arg2, arg3, arg4);
        }
    }
}
