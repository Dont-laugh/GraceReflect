using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DontLaugh
{
    public class InstanceActionInvoker<TTarget> : BaseInstanceActionInvoker<TTarget>
    {
        private Action<TTarget> _invoke;

        protected override int _parameterCount => 0;

        public InstanceActionInvoker(object target, MethodInfo methodInfo) : base(target, methodInfo) { }

        protected override void CompileEmit()
        {
            string methodName = $"{_methodInfo.ReflectedType.FullName}.call_{_methodInfo.Name}";
            var method = new DynamicMethod(methodName, typeof(void), new[] { _targetType }, _targetType, true);

            ILGenerator gen = method.GetILGenerator();
            gen.Emit(_targetType.IsValueType ? OpCodes.Ldarga : OpCodes.Ldarg, 0);
            gen.Emit(OpCodes.Callvirt, _methodInfo);
            gen.Emit(OpCodes.Ret);

            _invoke = (Action<TTarget>) method.CreateDelegate(typeof(Action<TTarget>));
        }

        protected override void CreateDelegate()
        {
            _invoke = (Action<TTarget>) _methodInfo.CreateDelegate(typeof(Action<TTarget>));
        }

        public override object Invoke()
        {
            _invoke(_target);
            return null;
        }
    }
}
