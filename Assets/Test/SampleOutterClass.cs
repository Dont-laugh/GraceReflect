using UnityEngine;

namespace DontLaugh.Test
{
    internal enum Options
    {
        Sample1, Sample2, Sample3
    }

    internal class SampleOutterClass
    {
        private string m_Field1 = "This is m_Field1";
        public Vector3 m_Field2;

        private static Options s_Field1;
        public const string s_Field2 = "This is s_Field2";

        private int property1 => 233;

        public char property2
        {
            set
            {
                if (DebugToggle.enablePrint)
                {
                    Debug.Log($"{nameof(SampleOutterClass)}.set_property2");
                }
            }
        }

        private static Options m_Property3;

        public static Options property3
        {
            get
            {
                if (DebugToggle.enablePrint)
                {
                    Debug.Log($"{nameof(SampleOutterClass)}.get_property3");
                }
                return m_Property3;
            }
            private set
            {
                if (DebugToggle.enablePrint)
                {
                    Debug.Log($"{nameof(SampleOutterClass)}.set_property3");
                }
                m_Property3 = value;
            }
        }

        private void Method1(int layer)
        {
            if (DebugToggle.enablePrint)
            {
                Debug.Log($"{nameof(SampleOutterClass)}.Method1()  layer: {layer}");
            }
        }

        private int Method2(int layer, Options options)
        {
            if (DebugToggle.enablePrint)
            {
                Debug.Log($"{nameof(SampleOutterClass)}.Method2()  layer: {layer}, enum: {options}");
            }
            return layer;
        }

        public static Options Method3(string title, string tooltip, int layer, Options options, Vector2 vec)
        {
            var content = new GUIContent(title, tooltip);
            if (DebugToggle.enablePrint)
            {
                Debug.Log($"{nameof(SampleOutterClass)}.Method3()  content: {content}, layer: {layer}, enum: {options}, vec: {vec}");
            }
            return options;
        }
    }
}
