using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DontLaugh
{
    public class InstancePropertyAccessor<TInstance, TProperty> : IOptimizedAccessor<TProperty>
    {
        private readonly TInstance _instance;
        private readonly PropertyInfo _propertyInfo;
        private Func<TInstance, TProperty> _getter;
        private Action<TInstance, TProperty> _setter;
        private Type _instanceType;
        private Type _propertyType;

        public InstancePropertyAccessor(TInstance instance, PropertyInfo propertyInfo)
        {
            if (GraceReflection.doCheck)
            {
                Ensure.That(nameof(instance)).IsNotNull(instance);
                Ensure.That(nameof(propertyInfo)).IsNotNull(propertyInfo);

                if (propertyInfo.DeclaringType != typeof(TInstance))
                {
                    throw new ArgumentException("Declaring type of property info doesn't match generic type.", nameof(propertyInfo));
                }

                if (propertyInfo.PropertyType != typeof(TProperty))
                {
                    throw new ArgumentException("Property type of property info doesn't match generic type.", nameof(propertyInfo));
                }

                if (propertyInfo.IsStatic())
                {
                    throw new ArgumentException("The property is static.", nameof(propertyInfo));
                }
            }

            _instance = instance;
            _propertyInfo = propertyInfo;
            _instanceType = typeof(TInstance);
            _propertyType = typeof(TProperty);
        }

        public void Compile()
        {
            MethodInfo getMethod = _propertyInfo.GetGetMethod(true);
            MethodInfo setMethod = _propertyInfo.GetSetMethod(true);

            if (!GraceReflection.canEmit)
            {
                if (getMethod != null)
                {
                    _getter = instance => (TProperty) _propertyInfo.GetValue(instance, null);
                }
                if (setMethod != null)
                {
                    _setter = (instance, property) => _propertyInfo.SetValue(instance, property, null);
                }
                return;
            }

            if (getMethod != null)
            {
                string getterName = $"{_propertyInfo.ReflectedType.FullName}.get_{_propertyInfo.Name}";

                var getter = new DynamicMethod(getterName, _propertyType, new[] { _instanceType }, _instanceType, true);
                ILGenerator gen = getter.GetILGenerator();

                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Callvirt, getMethod);
                gen.Emit(OpCodes.Ret);

                _getter = (Func<TInstance, TProperty>) getter.CreateDelegate(typeof(Func<TInstance, TProperty>));
            }

            if (setMethod != null)
            {
                string setterName = $"{_propertyInfo.ReflectedType.FullName}.set_{_propertyInfo.Name}";

                var setter = new DynamicMethod(setterName, typeof(void), new[] { _instanceType, _propertyType }, _instanceType, true);
                ILGenerator gen = setter.GetILGenerator();

                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Callvirt, setMethod);
                gen.Emit(OpCodes.Ret);

                _setter = (Action<TInstance, TProperty>) setter.CreateDelegate(typeof(Action<TInstance, TProperty>));
            }
        }

        object IOptimizedAccessor.GetValue()
        {
            return GetValue();
        }

        public void SetValue(object value)
        {
            SetValue((TProperty) value);
        }

        public TProperty GetValue()
        {
            if (_getter == null)
            {
                throw new ArgumentException("Property must have a get method.");
            }
            return _getter(_instance);
        }

        public void SetValue(TProperty value)
        {
            if (_setter == null)
            {
                throw new ArgumentException("Property must have a set method.");
            }
            _setter(_instance, value);
        }
    }
}
