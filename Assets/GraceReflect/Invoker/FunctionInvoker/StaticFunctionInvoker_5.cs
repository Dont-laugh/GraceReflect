using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DontLaugh
{
    public class StaticFunctionInvoker<TParam0, TParam1, TParam2, TParam3, TParam4, TResult> : BaseInvoker<TResult>
    {
        private Func<TParam0, TParam1, TParam2, TParam3, TParam4, TResult> _invoke;

        public StaticFunctionInvoker(MethodInfo methodInfo) : base(methodInfo)
        {
            if (GraceReflection.doCheck)
            {
                Ensure.That(nameof(methodInfo)).IsTrue(methodInfo.IsStatic);

                if (methodInfo.GetParameters().Length > 5)
                {
                    throw new ArgumentException("Method can only have five paramters.");
                }
            }
        }

        protected override void CompileEmit()
        {
            string methodName = $"{_methodInfo.ReflectedType.FullName}.call_{_methodInfo.Name}";
            var method = new DynamicMethod
            (
                methodName, typeof(TResult),
                new[] { typeof(TParam0), typeof(TParam1), typeof(TParam2), typeof(TParam3), typeof(TParam4) }, true
            );

            ILGenerator gen = method.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Ldarg_2);
            gen.Emit(OpCodes.Ldarg_3);
            gen.Emit(OpCodes.Ldarg, 4);
            gen.Emit(OpCodes.Callvirt, _methodInfo);
            gen.Emit(OpCodes.Ret);

            _invoke = (Func<TParam0, TParam1, TParam2, TParam3, TParam4, TResult>) method.CreateDelegate(
                typeof(Func<TParam0, TParam1, TParam2, TParam3, TParam4, TResult>));
        }

        protected override void CreateDelegate()
        {
            _invoke = (Func<TParam0, TParam1, TParam2, TParam3, TParam4, TResult>) _methodInfo.CreateDelegate(
                typeof(Func<TParam0, TParam1, TParam2, TParam3, TParam4, TResult>));
        }

        public override TResult Invoke(object arg0, object arg1, object arg2, object arg3, object arg4)
        {
            return _invoke((TParam0) arg0, (TParam1) arg1, (TParam2) arg2, (TParam3) arg3, (TParam4) arg4);
        }
    }
}
