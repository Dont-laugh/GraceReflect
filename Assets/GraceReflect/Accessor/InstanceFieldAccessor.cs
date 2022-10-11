using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DontLaugh
{
    public class InstanceFieldAccessor<TInstance, TField> : IOptimizedAccessor<TField>
    {
        private readonly TInstance _instance;
        private readonly FieldInfo _fieldInfo;
        private Func<TInstance, TField> _getter;
        private Action<TInstance, TField> _setter;
        private Type _instanceType;
        private Type _fieldType;

        public InstanceFieldAccessor(TInstance instance, FieldInfo fieldInfo)
        {
            if (GraceReflection.doCheck)
            {
                Ensure.That(nameof(instance)).IsNotNull(instance);
                Ensure.That(nameof(fieldInfo)).IsNotNull(fieldInfo);

                if (fieldInfo.DeclaringType != typeof(TInstance))
                {
                    throw new ArgumentException("Declaring type of field info doesn't match generic type.", nameof(fieldInfo));
                }

                if (fieldInfo.FieldType != typeof(TField))
                {
                    throw new ArgumentException("Field type of field info doesn't match generic type.", nameof(fieldInfo));
                }

                if (fieldInfo.IsStatic)
                {
                    throw new ArgumentException("The field is static.", nameof(fieldInfo));
                }
            }

            
            _instance = instance;
            _fieldInfo = fieldInfo;
            _instanceType = typeof(TInstance);
            _fieldType = typeof(TField);
        }

        public void Compile()
        {
            if (!GraceReflection.canEmit)
            {
                _getter = instance => (TField) _fieldInfo.GetValue(instance);
                _setter = (instance, field) => _fieldInfo.SetValue(instance, field);
                return;
            }

            string getterName = $"{_fieldInfo.ReflectedType.FullName}.get_{_fieldInfo.Name}";

            var getter = new DynamicMethod(getterName, _fieldType, new[] { _instanceType }, _instanceType, true);
            ILGenerator gen = getter.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, _fieldInfo);
            gen.Emit(OpCodes.Ret);

            _getter = (Func<TInstance, TField>) getter.CreateDelegate(typeof(Func<TInstance, TField>));

            string setterName = $"{_fieldInfo.ReflectedType.FullName}.set_{_fieldInfo.Name}";

            var setter = new DynamicMethod(setterName, typeof(void), new[] { _instanceType, _fieldType }, _instanceType, true);
            gen = setter.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Stfld, _fieldInfo);
            gen.Emit(OpCodes.Ret);

            _setter = (Action<TInstance, TField>) setter.CreateDelegate(typeof(Action<TInstance, TField>));
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
            return _getter(_instance);
        }

        public void SetValue(TField value)
        {
            _setter(_instance, value);
        }
    }
}
