using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DontLaugh
{
    public class StaticActionInvoker<TParam0> : BaseInvoker
    {
        private Action<TParam0> _invoke;

        public StaticActionInvoker(MethodInfo methodInfo) : base(methodInfo)
        {
            if (GraceReflection.doCheck)
            {
                Ensure.That(nameof(methodInfo)).IsTrue(methodInfo.IsStatic);

                if (methodInfo.GetParameters().Length > 1)
                {
                    throw new ArgumentException("Method can only have one paramter.");
                }
            }
        }

        protected override void CompileEmit()
        {
            string methodName = $"{_methodInfo.ReflectedType.FullName}.call_{_methodInfo.Name}";
            var method = new DynamicMethod(methodName, typeof(void), new[] { typeof(TParam0) }, true);

            ILGenerator gen = method.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Callvirt, _methodInfo);
            gen.Emit(OpCodes.Ret);

            _invoke = (Action<TParam0>) method.CreateDelegate(typeof(Action<TParam0>));
        }

        protected override void CreateDelegate()
        {
            _invoke = (Action<TParam0>) _methodInfo.CreateDelegate(typeof(Action<TParam0>));
        }

        public override object Invoke(object arg0)
        {
            _invoke((TParam0) arg0);
            return null;
        }
    }
}
