using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DontLaugh
{
    public class InstanceFunctionInvoker<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TResult>
        : BaseInstanceFunctionInvoker<TTarget, TResult>
    {
        private Func<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TResult> _invoke;

        protected override int _parameterCount => 5;

        public InstanceFunctionInvoker(object target, MethodInfo methodInfo) : base(target, methodInfo) { }

        protected override void CompileEmit()
        {
            string methodName = $"{_methodInfo.ReflectedType.FullName}.call_{_methodInfo.Name}";
            var method = new DynamicMethod
            (
                methodName, _resultType,
                new[] { _targetType, typeof(TParam0), typeof(TParam1), typeof(TParam2), typeof(TParam3), typeof(TParam4) },
                _targetType, true
            );

            ILGenerator gen = method.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            if (_targetType.IsClass)
            {
                gen.Emit(OpCodes.Castclass, _targetType);
            }
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Ldarg_2);
            gen.Emit(OpCodes.Ldarg_3);
            gen.Emit(OpCodes.Ldarg, 4);
            gen.Emit(OpCodes.Ldarg, 5);
            gen.Emit(OpCodes.Callvirt, _methodInfo);
            gen.Emit(OpCodes.Ret);

            _invoke = (Func<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TResult>) method.CreateDelegate(
                typeof(Func<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TResult>));
        }

        protected override void CreateDelegate()
        {
            _invoke = (Func<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TResult>) _methodInfo.CreateDelegate(
                typeof(Func<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TResult>));
        }

        public override TResult Invoke(object arg0, object arg1, object arg2, object arg3, object arg4)
        {
            return _invoke(_target, (TParam0) arg0, (TParam1) arg1, (TParam2) arg2, (TParam3) arg3, (TParam4) arg4);
        }
    }
}
