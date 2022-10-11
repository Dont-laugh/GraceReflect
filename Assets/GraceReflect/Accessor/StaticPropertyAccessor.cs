using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DontLaugh
{
    public class StaticPropertyAccessor<TProperty> : IOptimizedAccessor<TProperty>
    {
        private readonly PropertyInfo _propertyInfo;
        private Func<TProperty> _getter;
        private Action<TProperty> _setter;
        private Type _propertyType;

        public StaticPropertyAccessor(PropertyInfo propertyInfo)
        {
            if (GraceReflection.doCheck)
            {
                Ensure.That(nameof(propertyInfo)).IsNotNull(propertyInfo);

                if (propertyInfo.PropertyType != typeof(TProperty))
                {
                    throw new ArgumentException("Property type of property info doesn't match generic type.", nameof(propertyInfo));
                }

                if (!propertyInfo.IsStatic())
                {
                    throw new ArgumentException("The property isn't static.", nameof(propertyInfo));
                }
            }

            _propertyInfo = propertyInfo;
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
                    _getter = _getter = () => (TProperty) _propertyInfo.GetValue(null, null);
                }
                if (setMethod != null)
                {
                    _setter = _setter = property => _propertyInfo.SetValue(null, property, null);
                }
                return;
            }

            if (getMethod != null)
            {
                string getterName = $"{_propertyInfo.ReflectedType.FullName}.get_{_propertyInfo.Name}";

                var getter = new DynamicMethod(getterName, typeof(TProperty), Type.EmptyTypes, true);
                ILGenerator gen = getter.GetILGenerator();

                gen.Emit(OpCodes.Call, getMethod);
                gen.Emit(OpCodes.Ret);

                _getter = (Func<TProperty>) getter.CreateDelegate(typeof(Func<TProperty>));
            }

            if (setMethod != null)
            {
                string setterName = $"{_propertyInfo.ReflectedType.FullName}.set_{_propertyInfo.Name}";

                var setter = new DynamicMethod(setterName, typeof(void), new[] { typeof(TProperty) }, true);
                ILGenerator gen = setter.GetILGenerator();

                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Call, setMethod);
                gen.Emit(OpCodes.Ret);

                _setter = (Action<TProperty>) setter.CreateDelegate(typeof(Action<TProperty>));
            }
        }

        object IOptimizedAccessor.GetValue()
        {
            return GetValue();
        }

        void IOptimizedAccessor.SetValue(object value)
        {
            SetValue((TProperty) value);
        }

        public TProperty GetValue()
        {
            if (_getter == null)
            {
                throw new ArgumentException("Property must have a get method.");
            }
            return _getter();
        }

        public void SetValue(TProperty value)
        {
            if (_setter == null)
            {
                throw new ArgumentException("Property must have a set method.");
            }
            _setter(value);
        }
    }
}
