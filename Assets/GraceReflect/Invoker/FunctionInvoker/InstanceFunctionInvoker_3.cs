using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DontLaugh
{
    public class InstanceFunctionInvoker<TTarget, TParam0, TParam1, TParam2, TResult> : BaseInstanceFunctionInvoker<TTarget, TResult>
    {
        private Func<TTarget, TParam0, TParam1, TParam2, TResult> _invoke;

        protected override int _parameterCount => 3;

        public InstanceFunctionInvoker(object target, MethodInfo methodInfo) : base(target, methodInfo) { }

        protected override void CompileEmit()
        {
            string methodName = $"{_methodInfo.ReflectedType.FullName}.call_{_methodInfo.Name}";
            var method = new DynamicMethod
            (
                methodName, _resultType,
                new[] { _targetType, typeof(TParam0), typeof(TParam1), typeof(TParam2) },
                _targetType, true
            );

            ILGenerator gen = method.GetILGenerator();
            gen.Emit(_targetType.IsValueType ? OpCodes.Ldarga : OpCodes.Ldarg, 0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Ldarg_2);
            gen.Emit(OpCodes.Ldarg_3);
            gen.Emit(OpCodes.Callvirt, _methodInfo);
            gen.Emit(OpCodes.Ret);

            _invoke = (Func<TTarget, TParam0, TParam1, TParam2, TResult>) method.CreateDelegate(
                typeof(Func<TTarget, TParam0, TParam1, TParam2, TResult>));
        }

        protected override void CreateDelegate()
        {
            _invoke = (Func<TTarget, TParam0, TParam1, TParam2, TResult>) _methodInfo.CreateDelegate(
                typeof(Func<TTarget, TParam0, TParam1, TParam2, TResult>));
        }

        public override TResult Invoke(object arg0, object arg1, object arg2)
        {
            return _invoke(_target, (TParam0) arg0, (TParam1) arg1, (TParam2) arg2);
        }
    }
}
