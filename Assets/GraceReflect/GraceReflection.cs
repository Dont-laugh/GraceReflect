using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DontLaugh
{
    public static class GraceReflection
    {
        private static readonly Dictionary<FieldInfo, IOptimizedAccessor> _fieldAccessors;
        private static readonly Dictionary<PropertyInfo, IOptimizedAccessor> _propertyAccessors;
        private static readonly Dictionary<MethodInfo, IOptimizedInvoker> _methodInvokers;

        static GraceReflection()
        {
            _fieldAccessors = new Dictionary<FieldInfo, IOptimizedAccessor>(16);
            _propertyAccessors = new Dictionary<PropertyInfo, IOptimizedAccessor>(16);
            _methodInvokers = new Dictionary<MethodInfo, IOptimizedInvoker>(16);
        }

        public static bool doCheck { get; set; }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
#endif
        private static void OnRuntimeSceneLoad()
        {
            doCheck = Application.isEditor || Debug.isDebugBuild;
        }

        public static bool canEmit
        {
            get
            {
#if (UNITY_EDITOR || UNITY_STANDALONE) && !ENABLE_IL2CPP
                return true;
#else
                return false;
#endif
            }
        }

        #region Support

        public static bool IsStatic(this PropertyInfo propertyInfo)
        {
            return (propertyInfo.GetGetMethod(true)?.IsStatic ?? false) || (propertyInfo.GetSetMethod(true)?.IsStatic ?? false);
        }

        // Support methods which have up to 5 parameters.
        public static bool SupportOptimization(MethodInfo methodInfo)
        {
            if (!canEmit)
            {
                return false;
            }

            if (methodInfo.IsGenericMethod)
            {
                return false;
            }

            ParameterInfo[] parameters = methodInfo.GetParameters();

            if (parameters.Length > 5)
            {
                return false;
            }

            if (parameters.Any(parameter => parameter.ParameterType.IsByRef))
            {
                return false;
            }

            return true;
        }

        #endregion Support

        #region Create

        /// <summary>
        /// Create from FieldInfo, return optimized field accessor.
        /// </summary>
        /// <param name="target">The object that field belongs to, set null if the field is static.</param>
        /// <param name="fieldInfo">Field info you want to get/set.</param>
        /// <typeparam name="TField">Field type.</typeparam>
        /// <returns>Generic-type optimized accessor.</returns>
        public static IOptimizedAccessor<TField> CreateFromField<TField>(object target, FieldInfo fieldInfo)
        {
            if (doCheck)
            {
                Ensure.That(nameof(fieldInfo.FieldType)).IsOfType<TField>(fieldInfo.FieldType);
            }
            return (IOptimizedAccessor<TField>) CreateFromField(target, fieldInfo);
        }

        /// <summary>
        /// Create from FieldInfo, return optimized field accessor.
        /// </summary>
        /// <param name="target">The object that field belongs to, set null if the field is static.</param>
        /// <param name="fieldInfo">Field info you want to get/set.</param>
        /// <returns>Weak-type optimized accessor.</returns>
        public static IOptimizedAccessor CreateFromField(object target, FieldInfo fieldInfo)
        {
            if (doCheck)
            {
                Ensure.That(nameof(fieldInfo)).IsNotNull(fieldInfo);
            }

            lock (_fieldAccessors)
            {
                if (_fieldAccessors.TryGetValue(fieldInfo, out IOptimizedAccessor accessor))
                {
                    return accessor;
                }

                Type accessorType;

                if (fieldInfo.IsStatic)
                {
                    accessorType = typeof(StaticFieldAccessor<>).MakeGenericType(fieldInfo.FieldType);
                    accessor = (IOptimizedAccessor) Activator.CreateInstance(accessorType, fieldInfo);
                }
                else
                {
                    accessorType = typeof(InstanceFieldAccessor<,>).MakeGenericType(fieldInfo.DeclaringType, fieldInfo.FieldType);
                    accessor = (IOptimizedAccessor) Activator.CreateInstance(accessorType, target, fieldInfo);
                }

                accessor.Compile();
                _fieldAccessors.Add(fieldInfo, accessor);

                return accessor;
            }
        }

        /// <summary>
        /// Create from PropertyInfo, return optimized property accessor.
        /// </summary>
        /// <param name="target">The object that property belongs to, set null if the property is static.</param>
        /// <param name="propertyInfo">Property info you want to get/set.</param>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns>Generic-type optimized accessor.</returns>
        public static IOptimizedAccessor<TProperty> CreateFromProperty<TProperty>(object target, PropertyInfo propertyInfo)
        {
            if (doCheck)
            {
                Ensure.That(nameof(propertyInfo.PropertyType)).IsOfType<TProperty>(propertyInfo.PropertyType);
            }
            return (IOptimizedAccessor<TProperty>) CreateFromProperty(target, propertyInfo);
        }

        /// <summary>
        /// Create from PropertyInfo, return optimized property accessor.
        /// </summary>
        /// <param name="target">The object that property belongs to, set null if the property is static.</param>
        /// <param name="propertyInfo">Property info you want to get/set.</param>
        /// <returns>Weak-type optimized accessor.</returns>
        public static IOptimizedAccessor CreateFromProperty(object target, PropertyInfo propertyInfo)
        {
            if (doCheck)
            {
                Ensure.That(nameof(propertyInfo)).IsNotNull(propertyInfo);
            }

            lock (_propertyAccessors)
            {
                if (_propertyAccessors.TryGetValue(propertyInfo, out IOptimizedAccessor accessor))
                {
                    return accessor;
                }

                Type accessorType;

                if (propertyInfo.IsStatic())
                {
                    accessorType = typeof(StaticPropertyAccessor<>).MakeGenericType(propertyInfo.PropertyType);
                    accessor = (IOptimizedAccessor) Activator.CreateInstance(accessorType, propertyInfo);
                }
                else
                {
                    accessorType = typeof(InstancePropertyAccessor<,>).MakeGenericType(
                        propertyInfo.DeclaringType, propertyInfo.PropertyType);
                    accessor = (IOptimizedAccessor) Activator.CreateInstance(accessorType, target, propertyInfo);
                }

                accessor.Compile();
                _propertyAccessors.Add(propertyInfo, accessor);

                return accessor;
            }
        }

        /// <summary>
        /// Create from MethodInfo, return optimized function invoker.
        /// </summary>
        /// <param name="target">The object that method belongs to, set null if the method is static.</param>
        /// <param name="methodInfo">Method info you want to call.</param>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <returns>Generic-type optimized invoker.</returns>
        public static IOptimizedInvoker<T> CreateFromMethod<T>(object target, MethodInfo methodInfo)
        {
            return (IOptimizedInvoker<T>) CreateFromMethod(target, methodInfo);
        }

        /// <summary>
        /// Create from MethodInfo, return optimized function invoker.
        /// </summary>
        /// <param name="target">The object that method belongs to, set null if the method is static.</param>
        /// <param name="methodInfo">Method info you want to call.</param>
        /// <returns>Weak-type optimized invoker.</returns>
        public static IOptimizedInvoker CreateFromMethod(object target, MethodInfo methodInfo)
        {
            if (doCheck)
            {
                Ensure.That(nameof(methodInfo)).IsNotNull(methodInfo);
            }

            lock (_methodInvokers)
            {
                if (_methodInvokers.TryGetValue(methodInfo, out IOptimizedInvoker invoker))
                {
                    return invoker;
                }

                if (SupportOptimization(methodInfo))
                {
                    if (methodInfo.ReturnType == typeof(void))
                    {
                        invoker = CreateActionInvoker(target, methodInfo);
                    }
                    else
                    {
                        invoker = CreateFunctionInvoker(target, methodInfo);
                    }
                }
                else
                {
                    invoker = new ReflectionInvoker(target, methodInfo);
                }

                invoker.Compile();
                _methodInvokers.Add(methodInfo, invoker);

                return invoker;
            }
        }

        private static IOptimizedInvoker CreateActionInvoker(object target, MethodInfo methodInfo)
        {
            ParameterInfo[] parameters = methodInfo.GetParameters();
            Type invokerType = null;

            // Static Action
            if (methodInfo.IsStatic)
            {
                if (parameters.Length == 0)
                {
                    invokerType = typeof(StaticActionInvoker);
                }
                else if (parameters.Length == 1)
                {
                    invokerType = typeof(StaticActionInvoker<>).MakeGenericType(parameters[0].ParameterType);
                }
                else if (parameters.Length == 2)
                {
                    invokerType = typeof(StaticActionInvoker<,>).MakeGenericType(
                        parameters[0].ParameterType, parameters[1].ParameterType
                    );
                }
                else if (parameters.Length == 3)
                {
                    invokerType = typeof(StaticActionInvoker<,,>).MakeGenericType(
                        parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType
                    );
                }
                else if (parameters.Length == 4)
                {
                    invokerType = typeof(StaticActionInvoker<,,,>).MakeGenericType(
                        parameters[0].ParameterType, parameters[1].ParameterType,
                        parameters[2].ParameterType, parameters[3].ParameterType
                    );
                }
                else if (parameters.Length == 5)
                {
                    invokerType = typeof(StaticActionInvoker<,,,,>).MakeGenericType(
                        parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType,
                        parameters[3].ParameterType, parameters[4].ParameterType
                    );
                }
                else
                {
                    throw new NotSupportedException();
                }

                return (IOptimizedInvoker) Activator.CreateInstance(invokerType, methodInfo);
            }

            // Instance Action
            if (parameters.Length == 0)
            {
                invokerType = typeof(InstanceActionInvoker<>).MakeGenericType(methodInfo.DeclaringType);
            }
            else if (parameters.Length == 1)
            {
                invokerType = typeof(InstanceActionInvoker<,>).MakeGenericType(
                    methodInfo.ReflectedType, parameters[0].ParameterType
                );
            }
            else if (parameters.Length == 2)
            {
                invokerType = typeof(InstanceActionInvoker<,,>).MakeGenericType(
                    methodInfo.ReflectedType, parameters[0].ParameterType, parameters[1].ParameterType
                );
            }
            else if (parameters.Length == 3)
            {
                invokerType = typeof(InstanceActionInvoker<,,>).MakeGenericType(
                    methodInfo.ReflectedType, parameters[0].ParameterType, parameters[1].ParameterType,
                    parameters[2].ParameterType
                );
            }
            else if (parameters.Length == 4)
            {
                invokerType = typeof(InstanceActionInvoker<,,,>).MakeGenericType(
                    methodInfo.ReflectedType, parameters[0].ParameterType, parameters[1].ParameterType,
                    parameters[2].ParameterType, parameters[3].ParameterType
                );
            }
            else if (parameters.Length == 5)
            {
                invokerType = typeof(InstanceActionInvoker<,,,,>).MakeGenericType(
                    methodInfo.ReflectedType, parameters[0].ParameterType, parameters[1].ParameterType,
                    parameters[2].ParameterType, parameters[3].ParameterType, parameters[4].ParameterType
                );
            }
            else
            {
                throw new NotSupportedException();
            }

            return (IOptimizedInvoker) Activator.CreateInstance(invokerType, target, methodInfo);
        }

        private static IOptimizedInvoker CreateFunctionInvoker(object target, MethodInfo methodInfo)
        {
            ParameterInfo[] parameters = methodInfo.GetParameters();
            Type invokerType = null;

            // Static Function
            if (methodInfo.IsStatic)
            {
                if (parameters.Length == 0)
                {
                    invokerType = typeof(StaticFunctionInvoker<>).MakeGenericType(methodInfo.ReturnType);
                }
                else if (parameters.Length == 1)
                {
                    invokerType = typeof(StaticFunctionInvoker<,>).MakeGenericType(
                        parameters[0].ParameterType, methodInfo.ReturnType
                    );
                }
                else if (parameters.Length == 2)
                {
                    invokerType = typeof(StaticFunctionInvoker<,,>).MakeGenericType(
                        parameters[0].ParameterType, parameters[1].ParameterType, methodInfo.ReturnType
                    );
                }
                else if (parameters.Length == 3)
                {
                    invokerType = typeof(StaticFunctionInvoker<,,,>).MakeGenericType(
                        parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType,
                        methodInfo.ReturnType
                    );
                }
                else if (parameters.Length == 4)
                {
                    invokerType = typeof(StaticFunctionInvoker<,,,,>).MakeGenericType(
                        parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType,
                        parameters[3].ParameterType, methodInfo.ReturnType
                    );
                }
                else if (parameters.Length == 5)
                {
                    invokerType = typeof(StaticFunctionInvoker<,,,,,>).MakeGenericType(
                        parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType,
                        parameters[3].ParameterType, parameters[4].ParameterType, methodInfo.ReturnType
                    );
                }
                else
                {
                    throw new NotSupportedException();
                }

                return (IOptimizedInvoker) Activator.CreateInstance(invokerType, methodInfo);
            }

            // Instance Function
            if (parameters.Length == 0)
            {
                invokerType = typeof(InstanceFunctionInvoker<,>).MakeGenericType(
                    methodInfo.DeclaringType, methodInfo.ReturnType
                );
            }
            else if (parameters.Length == 1)
            {
                invokerType = typeof(InstanceFunctionInvoker<,,>).MakeGenericType(
                    methodInfo.DeclaringType, parameters[0].ParameterType, methodInfo.ReturnType
                );
            }
            else if (parameters.Length == 2)
            {
                invokerType = typeof(InstanceFunctionInvoker<,,,>).MakeGenericType(
                    methodInfo.DeclaringType, parameters[0].ParameterType,
                    parameters[1].ParameterType, methodInfo.ReturnType
                );
            }
            else if (parameters.Length == 3)
            {
                invokerType = typeof(InstanceFunctionInvoker<,,,,>).MakeGenericType(
                    methodInfo.DeclaringType, parameters[0].ParameterType, parameters[1].ParameterType,
                    parameters[2].ParameterType, methodInfo.ReturnType
                );
            }
            else if (parameters.Length == 4)
            {
                invokerType = typeof(InstanceFunctionInvoker<,,,,,>).MakeGenericType(
                    methodInfo.DeclaringType, parameters[0].ParameterType, parameters[1].ParameterType,
                    parameters[2].ParameterType, parameters[3].ParameterType, methodInfo.ReturnType
                );
            }
            else if (parameters.Length == 5)
            {
                invokerType = typeof(InstanceFunctionInvoker<,,,,,,>).MakeGenericType(
                    methodInfo.DeclaringType, parameters[0].ParameterType, parameters[1].ParameterType,
                    parameters[2].ParameterType, parameters[3].ParameterType, parameters[4].ParameterType,
                    methodInfo.ReturnType
                );
            }
            else
            {
                throw new NotSupportedException();
            }

            return (IOptimizedInvoker) Activator.CreateInstance(invokerType, target, methodInfo);
        }

        #endregion Create
    }
}
