using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DontLaugh
{
    public class StaticFunctionInvoker<TParam0, TResult> : BaseInvoker<TResult>
    {
        private Func<TParam0, TResult> _invoke;

        public StaticFunctionInvoker(MethodInfo methodInfo) : base(methodInfo)
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
            var method = new DynamicMethod(methodName, typeof(TResult), new[] { typeof(TParam0) }, true);

            ILGenerator gen = method.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Callvirt, _methodInfo);
            gen.Emit(OpCodes.Ret);

            _invoke = (Func<TParam0, TResult>) method.CreateDelegate(typeof(Func<TParam0, TResult>));
        }

        protected override void CreateDelegate()
        {
            _invoke = (Func<TParam0, TResult>) _methodInfo.CreateDelegate(typeof(Func<TParam0, TResult>));
        }

        public override TResult Invoke(object arg0)
        {
            return _invoke((TParam0) arg0);
        }
    }
}
