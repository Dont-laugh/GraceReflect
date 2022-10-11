using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DontLaugh
{
    public class StaticFunctionInvoker<TResult> : BaseInvoker<TResult>
    {
        private Func<TResult> _invoke;

        public StaticFunctionInvoker(MethodInfo methodInfo) : base(methodInfo)
        {
            if (GraceReflection.doCheck)
            {
                Ensure.That(nameof(methodInfo)).IsTrue(methodInfo.IsStatic);

                if (methodInfo.GetParameters().Length > 0)
                {
                    throw new ArgumentException("Method cannot have any paramters.");
                }
            }
        }

        protected override void CompileEmit()
        {
            string methodName = $"{_methodInfo.ReflectedType.FullName}.call_{_methodInfo.Name}";
            var method = new DynamicMethod(methodName, typeof(TResult), Type.EmptyTypes, true);

            ILGenerator gen = method.GetILGenerator();
            gen.Emit(OpCodes.Callvirt, _methodInfo);
            gen.Emit(OpCodes.Ret);

            _invoke = (Func<TResult>) method.CreateDelegate(typeof(Func<TResult>));
        }

        protected override void CreateDelegate()
        {
            _invoke = (Func<TResult>) _methodInfo.CreateDelegate(typeof(Func<TResult>));
        }

        public override TResult Invoke()
        {
            return _invoke();
        }
    }
}
