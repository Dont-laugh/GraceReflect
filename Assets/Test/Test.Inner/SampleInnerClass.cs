using UnityEngine;

namespace DontLaugh.Test
{
    internal enum SampleEnum
    {
        First, Second, Third
    }

    internal struct SampleInnerClass
    {
        private string m_Field1;
        public Vector3 m_Field2;

        private static SampleEnum s_Field1;
        public const string s_Field2 = "This is s_Field2";

        private int property1 => 233;

        public char property2
        {
            set
            {
                if (DebugToggle.enablePrint)
                {
                    Debug.Log($"{nameof(SampleInnerClass)}.set_property2");
                }
            }
        }

        private static SampleEnum m_Property3;

        public static SampleEnum property3
        {
            get
            {
                if (DebugToggle.enablePrint)
                {
                    Debug.Log($"{nameof(SampleInnerClass)}.get_property3");
                }
                return m_Property3;
            }
            private set
            {
                if (DebugToggle.enablePrint)
                {
                    Debug.Log($"{nameof(SampleInnerClass)}.set_property3");
                }
                m_Property3 = value;
            }
        }

        private void Method1(int layer)
        {
            if (DebugToggle.enablePrint)
            {
                Debug.Log($"{nameof(SampleInnerClass)}.Method1()  layer: {layer}");
            }
        }

        private int Method2(int layer, SampleEnum @enum)
        {
            if (DebugToggle.enablePrint)
            {
                Debug.Log($"{nameof(SampleInnerClass)}.Method2()  layer: {layer}, enum: {@enum}");
            }
            return layer;
        }

        public static SampleEnum Method3(string title, string tooltip, int layer, SampleEnum @enum, Vector2 vec)
        {
            var content = new GUIContent(title, tooltip);
            if (DebugToggle.enablePrint)
            {
                Debug.Log($"{nameof(SampleInnerClass)}.Method3()  content: {content}, layer: {layer}, enum: {@enum}, vec: {vec}");
            }
            return @enum;
        }
    }
}
