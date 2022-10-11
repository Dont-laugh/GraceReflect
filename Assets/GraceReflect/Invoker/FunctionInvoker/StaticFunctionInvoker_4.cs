using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DontLaugh
{
    public class StaticFunctionInvoker<TParam0, TParam1, TParam2, TParam3, TResult> : BaseInvoker<TResult>
    {
        private Func<TParam0, TParam1, TParam2, TParam3, TResult> _invoke;

        public StaticFunctionInvoker(MethodInfo methodInfo) : base(methodInfo)
        {
            if (GraceReflection.doCheck)
            {
                Ensure.That(nameof(methodInfo)).IsTrue(methodInfo.IsStatic);

                if (methodInfo.GetParameters().Length > 4)
                {
                    throw new ArgumentException("Method can only have four paramters.");
                }
            }
        }

        protected override void CompileEmit()
        {
            string methodName = $"{_methodInfo.ReflectedType.FullName}.call_{_methodInfo.Name}";
            var method = new DynamicMethod
            (
                methodName, typeof(TResult),
                new[] { typeof(TParam0), typeof(TParam1), typeof(TParam2), typeof(TParam3) }, true
            );

            ILGenerator gen = method.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Ldarg_2);
            gen.Emit(OpCodes.Ldarg_3);
            gen.Emit(OpCodes.Callvirt, _methodInfo);
            gen.Emit(OpCodes.Ret);

            _invoke = (Func<TParam0, TParam1, TParam2, TParam3, TResult>) method.CreateDelegate(
                typeof(Func<TParam0, TParam1, TParam2, TParam3, TResult>));
        }

        protected override void CreateDelegate()
        {
            _invoke = (Func<TParam0, TParam1, TParam2, TParam3, TResult>) _methodInfo.CreateDelegate(
                typeof(Func<TParam0, TParam1, TParam2, TParam3, TResult>));
        }

        public override TResult Invoke(object arg0, object arg1, object arg2, object arg3)
        {
            return _invoke((TParam0) arg0, (TParam1) arg1, (TParam2) arg2, (TParam3) arg3);
        }
    }
}
