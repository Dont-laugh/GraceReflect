using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DontLaugh
{
    public class InstanceFunctionInvoker<TTarget, TResult> : BaseInstanceFunctionInvoker<TTarget, TResult>
    {
        private Func<TTarget, TResult> _invoke;

        protected override int _parameterCount => 0;

        public InstanceFunctionInvoker(object target, MethodInfo methodInfo) : base(target, methodInfo) { }

        protected override void CompileEmit()
        {
            string methodName = $"{_methodInfo.ReflectedType.FullName}.call_{_methodInfo.Name}";
            var method = new DynamicMethod(methodName, _resultType, new[] { _targetType }, _targetType, true);

            ILGenerator gen = method.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            if (_targetType.IsClass)
            {
                gen.Emit(OpCodes.Castclass, _targetType);
            }
            gen.Emit(OpCodes.Callvirt, _methodInfo);
            gen.Emit(OpCodes.Ret);

            _invoke = (Func<TTarget, TResult>) method.CreateDelegate(typeof(Func<TTarget, TResult>));
        }

        protected override void CreateDelegate()
        {
            _invoke = (Func<TTarget, TResult>) _methodInfo.CreateDelegate(typeof(Func<TTarget, TResult>));
        }

        public override TResult Invoke()
        {
            return _invoke(_target);
        }
    }
}
