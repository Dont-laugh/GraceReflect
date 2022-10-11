using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DontLaugh
{
    public class StaticFieldAccessor<TField> : IOptimizedAccessor<TField>
    {
        private readonly FieldInfo _fieldInfo;
        private Func<TField> _getter;
        private Action<TField> _setter;
        private Type _fieldType;

        public StaticFieldAccessor(FieldInfo fieldInfo)
        {
            if (GraceReflection.doCheck)
            {
                Ensure.That(nameof(fieldInfo)).IsNotNull(fieldInfo);

                if (fieldInfo.FieldType != typeof(TField))
                {
                    throw new ArgumentException("Field type of field info doesn't match generic type.", nameof(fieldInfo));
                }

                if (!fieldInfo.IsStatic)
                {
                    throw new ArgumentException("The field isn't static.", nameof(fieldInfo));
                }
            }

            _fieldInfo = fieldInfo;
            _fieldType = typeof(TField);
        }

        public void Compile()
        {
            if (!GraceReflection.canEmit)
            {
                _getter = () => (TField) _fieldInfo.GetValue(null);
                _setter = field => _fieldInfo.SetValue(null, field);
                return;
            }

            if (_fieldInfo.IsLiteral)
            {
                var value = (TField) _fieldInfo.GetValue(null);
                _getter = () => value;
                _setter = null;
                return;
            }

            string getterName = $"{_fieldInfo.ReflectedType.FullName}.get_{_fieldInfo.Name}";

            var getter = new DynamicMethod(getterName, _fieldType, Type.EmptyTypes, true);
            ILGenerator gen = getter.GetILGenerator();

            gen.Emit(OpCodes.Ldsfld, _fieldInfo);
            gen.Emit(OpCodes.Ret);

            _getter = (Func<TField>) getter.CreateDelegate(typeof(Func<TField>));

            string setterName = $"{_fieldInfo.ReflectedType.FullName}.set_{_fieldInfo.Name}";

            var setter = new DynamicMethod(setterName, typeof(void), new[] { _fieldType }, true);
            gen = setter.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Stsfld, _fieldInfo);
            gen.Emit(OpCodes.Ret);

            _setter = (Action<TField>) setter.CreateDelegate(typeof(Action<TField>));
        }

        object IOptimizedAccessor.GetValue()
        {
            return GetValue();
        }

        void IOptimizedAccessor.SetValue(object value)
        {
            SetValue((TField) value);
        }

        public TField GetValue()
        {
            return _getter();
        }

        public void SetValue(TField value)
        {
            _setter(value);
        }
    }
}
