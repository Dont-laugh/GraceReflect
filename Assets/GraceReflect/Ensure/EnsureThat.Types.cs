using System;

namespace DontLaugh
{
    public partial class EnsureThat
    {
        public void IsOfType<T>(T param, Type expectedType)
        {
            if (!Ensure.isActive)
            {
                return;
            }

            if (!IsInstanceOfType(param, expectedType))
            {
                string typeStr = param?.GetType().ToString() ?? "null";
                string failedMsg = string.Format(ExceptionMessages.Types_IsOfType_Failed, expectedType, typeStr);
                throw new ArgumentException(failedMsg, paramName);
            }
        }

        public void IsOfType(Type param, Type expectedType)
        {
            if (!Ensure.isActive)
            {
                return;
            }

            if (!expectedType.IsAssignableFrom(param))
            {
                string failedMsg = string.Format(ExceptionMessages.Types_IsOfType_Failed, expectedType, param);
                throw new ArgumentException(failedMsg, paramName);
            }
        }

        public void IsOfType<T>(object param) => IsOfType(param, typeof(T));

        public void IsOfType<T>(Type param) => IsOfType(param, typeof(T));

        public static bool IsInstanceOfType(object value, Type type)
        {
            if (value == null)
            {
                return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
            }
            return type.IsInstanceOfType(value);
        }
    }
}
