using System;
using System.Reflection;

namespace DontLaugh
{
    public abstract class BaseInstanceFunctionInvoker<TTarget, TResult> : BaseInvoker<TResult>
    {
        protected readonly TTarget _target;
        protected readonly Type _targetType;
        protected readonly Type _resultType;

        protected abstract int _parameterCount { get; }

        protected BaseInstanceFunctionInvoker(object target, MethodInfo methodInfo) : base(methodInfo)
        {
            if (GraceReflection.doCheck)
            {
                Ensure.That(nameof(target)).IsOfType<TTarget>(target);
                Ensure.That(nameof(methodInfo)).IsFalse(methodInfo.IsStatic);
                Ensure.That(nameof(methodInfo.ReturnType)).IsOfType<TResult>(methodInfo.ReturnType);

                if (methodInfo.GetParameters().Length > _parameterCount)
                {
                    throw new ArgumentException($"Method only have {_parameterCount} paramter(s).");
                }
            }

            _target = (TTarget) target;
            _targetType = typeof(TTarget);
            _resultType = typeof(TResult);
        }
    }
}
