using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DontLaugh
{
    public class StaticActionInvoker<TParam0, TParam1> : BaseInvoker
    {
        private Action<TParam0, TParam1> _invoke;

        public StaticActionInvoker(MethodInfo methodInfo) : base(methodInfo)
        {
            if (GraceReflection.doCheck)
            {
                Ensure.That(nameof(methodInfo)).IsTrue(methodInfo.IsStatic);

                if (methodInfo.GetParameters().Length > 2)
                {
                    throw new ArgumentException("Method can only have two paramter.");
                }
            }
        }

        protected override void CompileEmit()
        {
            string methodName = $"{_methodInfo.ReflectedType.FullName}.call_{_methodInfo.Name}";
            var method = new DynamicMethod(methodName, typeof(void), new[] { typeof(TParam0), typeof(TParam1) }, true);

            ILGenerator gen = method.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Callvirt, _methodInfo);
            gen.Emit(OpCodes.Ret);

            _invoke = (Action<TParam0, TParam1>) method.CreateDelegate(typeof(Action<TParam0, TParam1>));
        }

        protected override void CreateDelegate()
        {
            _invoke = (Action<TParam0, TParam1>) _methodInfo.CreateDelegate(typeof(Action<TParam0, TParam1>));
        }

        public override object Invoke(object arg0, object arg1)
        {
            _invoke((TParam0) arg0, (TParam1) arg1);
            return null;
        }
    }
}
