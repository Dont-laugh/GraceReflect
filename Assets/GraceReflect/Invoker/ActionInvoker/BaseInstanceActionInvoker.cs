using System;
using System.Reflection;

namespace DontLaugh
{
    public abstract class BaseInstanceActionInvoker<TTarget> : BaseInvoker
    {
        protected readonly TTarget _target;
        protected readonly Type _targetType;

        protected abstract int _parameterCount { get; }

        protected BaseInstanceActionInvoker(object target, MethodInfo methodInfo) : base(methodInfo)
        {
            if (GraceReflection.doCheck)
            {
                Ensure.That(nameof(target)).IsOfType<TTarget>(target);
                Ensure.That(nameof(methodInfo)).IsFalse(methodInfo.IsStatic);

                if (methodInfo.GetParameters().Length > _parameterCount)
                {
                    throw new ArgumentException($"Method only have {_parameterCount} paramter(s).");
                }
            }

            _target = (TTarget) target;
            _targetType = typeof(TTarget);
        }
    }
}
