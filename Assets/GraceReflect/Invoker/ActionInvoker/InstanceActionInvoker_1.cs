using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DontLaugh
{
    public class InstanceActionInvoker<TTarget, TParam0> : BaseInstanceActionInvoker<TTarget>
    {
        private Action<TTarget, TParam0> _invoke;

        protected override int _parameterCount => 1;

        public InstanceActionInvoker(object target, MethodInfo methodInfo) : base(target, methodInfo) { }

        protected override void CompileEmit()
        {
            string methodName = $"{_methodInfo.ReflectedType.FullName}.call_{_methodInfo.Name}";
            var method = new DynamicMethod(methodName, typeof(void), new[] { _targetType, typeof(TParam0) }, _targetType, true);

            ILGenerator gen = method.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Callvirt, _methodInfo);
            gen.Emit(OpCodes.Ret);

            _invoke = (Action<TTarget, TParam0>) method.CreateDelegate(typeof(Action<TTarget, TParam0>));
        }

        protected override void CreateDelegate()
        {
            _invoke = (Action<TTarget, TParam0>) _methodInfo.CreateDelegate(typeof(Action<TTarget, TParam0>));
        }

        public override object Invoke(object arg0)
        {
            _invoke(_target, (TParam0) arg0);
            return null;
        }
    }
}
