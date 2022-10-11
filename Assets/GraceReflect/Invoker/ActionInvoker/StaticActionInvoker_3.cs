using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DontLaugh
{
    public class StaticActionInvoker<TParam0, TParam1, TParam2> : BaseInvoker
    {
        private Action<TParam0, TParam1, TParam2> _invoke;

        public StaticActionInvoker(MethodInfo methodInfo) : base(methodInfo)
        {
            if (GraceReflection.doCheck)
            {
                Ensure.That(nameof(methodInfo)).IsTrue(methodInfo.IsStatic);

                if (methodInfo.GetParameters().Length > 3)
                {
                    throw new ArgumentException("Method can only have three paramter.");
                }
            }
        }

        protected override void CompileEmit()
        {
            string methodName = $"{_methodInfo.ReflectedType.FullName}.call_{_methodInfo.Name}";
            var method = new DynamicMethod(methodName, typeof(void), new[] { typeof(TParam0), typeof(TParam1), typeof(TParam2) }, true);

            ILGenerator gen = method.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Ldarg_2);
            gen.Emit(OpCodes.Callvirt, _methodInfo);
            gen.Emit(OpCodes.Ret);

            _invoke = (Action<TParam0, TParam1, TParam2>) method.CreateDelegate(typeof(Action<TParam0, TParam1, TParam2>));
        }

        protected override void CreateDelegate()
        {
            _invoke = (Action<TParam0, TParam1, TParam2>) _methodInfo.CreateDelegate(typeof(Action<TParam0, TParam1, TParam2>));
        }

        public override object Invoke(object arg0, object arg1, object arg2)
        {
            _invoke((TParam0) arg0, (TParam1) arg1, (TParam2) arg2);
            return null;
        }
    }
}
