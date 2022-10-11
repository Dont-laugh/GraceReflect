using UnityEngine;

namespace DontLaugh
{
    public static class Ensure
    {
        public static bool isActive => Application.isEditor || Debug.isDebugBuild;

        private static EnsureThat _instance;

        public static EnsureThat That(string param)
        {
            if (_instance == null)
            {
                _instance = new EnsureThat();
            }
            _instance.paramName = param;
            return _instance;
        }
    }
}
