using System;
using System.Reflection;

namespace DontLaugh
{
    public class ReflectionInvoker : IOptimizedInvoker
    {
        private readonly object _target;
        private readonly MethodInfo _methodInfo;

        public ReflectionInvoker(object target, MethodInfo methodInfo)
        {
            if (GraceReflection.doCheck)
            {
                Ensure.That(nameof(methodInfo)).IsNotNull(methodInfo);
            }

            _target = target;
            _methodInfo = methodInfo;
        }

        public void Compile() { }

        public object Invoke()
        {
            return _methodInfo.Invoke(_target, Array.Empty<object>());
        }

        public object Invoke(object arg0)
        {
            return _methodInfo.Invoke(_target, new[] { arg0 });
        }

        public object Invoke(object arg0, object arg1)
        {
            return _methodInfo.Invoke(_target, new[] { arg0, arg1 });
        }

        public object Invoke(object arg0, object arg1, object arg2)
        {
            return _methodInfo.Invoke(_target, new[] { arg0, arg1, arg2 });
        }

        public object Invoke(object arg0, object arg1, object arg2, object arg3)
        {
            return _methodInfo.Invoke(_target, new[] { arg0, arg1, arg2, arg3 });
        }

        public object Invoke(object arg0, object arg1, object arg2, object arg3, object arg4)
        {
            return _methodInfo.Invoke(_target, new[] { arg0, arg1, arg2, arg3, arg4 });
        }
    }
}
