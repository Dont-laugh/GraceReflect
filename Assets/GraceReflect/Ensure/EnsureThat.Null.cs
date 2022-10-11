using System;
using JetBrains.Annotations;

namespace DontLaugh
{
    public partial class EnsureThat
    {
        public void IsNull<T>(T value)
        {
            if (!Ensure.isActive)
            {
                return;
            }

            if (value != null)
            {
                throw new ArgumentNullException(paramName, ExceptionMessages.Common_IsNull_Failed);
            }
        }

        public void IsNotNull<T>(T value)
        {
            if (!Ensure.isActive)
            {
                return;
            }

            if (value == null)
            {
                throw new ArgumentNullException(paramName, ExceptionMessages.Common_IsNotNull_Failed);
            }
        }
    }
}
