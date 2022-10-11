using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DontLaugh
{
    public class StaticActionInvoker : BaseInvoker
    {
        private Action _invoke;

        public StaticActionInvoker(MethodInfo methodInfo) : base(methodInfo)
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
            var method = new DynamicMethod(methodName, typeof(void), Type.EmptyTypes, true);

            ILGenerator gen = method.GetILGenerator();
            gen.Emit(OpCodes.Callvirt, _methodInfo);
            gen.Emit(OpCodes.Ret);

            _invoke = (Action) method.CreateDelegate(typeof(Action));
        }

        protected override void CreateDelegate()
        {
            _invoke = (Action) _methodInfo.CreateDelegate(typeof(Action));
        }

        public override object Invoke()
        {
            _invoke();
            return null;
        }
    }
}
