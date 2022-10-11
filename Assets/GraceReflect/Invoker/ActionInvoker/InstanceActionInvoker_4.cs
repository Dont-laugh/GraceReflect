using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DontLaugh
{
    public class InstanceActionInvoker<TTarget, TParam0, TParam1, TParam2, TParam3> : BaseInstanceActionInvoker<TTarget>
    {
        private Action<TTarget, TParam0, TParam1, TParam2, TParam3> _invoke;

        protected override int _parameterCount => 4;

        public InstanceActionInvoker(object target, MethodInfo methodInfo) : base(target, methodInfo) { }

        protected override void CompileEmit()
        {
            string methodName = $"{_methodInfo.ReflectedType.FullName}.call_{_methodInfo.Name}";
            var method = new DynamicMethod
            (
                methodName, typeof(void),
                new[] { _targetType, typeof(TParam0), typeof(TParam1), typeof(TParam2), typeof(TParam3) },
                _targetType, true
            );

            ILGenerator gen = method.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Ldarg_2);
            gen.Emit(OpCodes.Ldarg_3);
            gen.Emit(OpCodes.Ldarg, 4);
            gen.Emit(OpCodes.Callvirt, _methodInfo);
            gen.Emit(OpCodes.Ret);

            _invoke = (Action<TTarget, TParam0, TParam1, TParam2, TParam3>) method.CreateDelegate(
                typeof(Action<TTarget, TParam0, TParam1, TParam2, TParam3>));
        }

        protected override void CreateDelegate()
        {
            _invoke = (Action<TTarget, TParam0, TParam1, TParam2, TParam3>) _methodInfo.CreateDelegate(
                typeof(Action<TTarget, TParam0, TParam1, TParam2, TParam3>));
        }

        public override object Invoke(object arg0, object arg1, object arg2, object arg3)
        {
            _invoke(_target, (TParam0) arg0, (TParam1) arg1, (TParam2) arg2, (TParam3) arg3);
            return null;
        }
    }
}
