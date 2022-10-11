using System;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DontLaugh.Test
{
    [ExecuteAlways]
    public class GraceReflectionSmokeTester : MonoBehaviour
    {
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GraceReflectionSmokeTester))]
    public class GraceReflectionTesterEditor : Editor
    {
        private GUIStyle _titleStyle;
        private Type _innerClassType;
        private SampleInnerClass _innerObject;
        private Type _outerClassType;
        private object _outerObject;
        private Type _outerOptionType;

        private IOptimizedAccessor<string> _i_field1;
        private IOptimizedAccessor<Vector3> _i_field2;
        private IOptimizedAccessor<SampleEnum> _i_sfield1;
        private IOptimizedAccessor<string> _i_sfield2;
        private IOptimizedAccessor<int> _i_prop1;
        private IOptimizedAccessor<char> _i_prop2;
        private IOptimizedAccessor<SampleEnum> _i_prop3;
        private IOptimizedInvoker _i_method1;
        private IOptimizedInvoker<int> _i_method2;
        private IOptimizedInvoker<SampleEnum> _i_method3;

        private IOptimizedAccessor<string> _o_field1;
        private IOptimizedAccessor<Vector3> _o_field2;
        private IOptimizedAccessor _o_sfield1;
        private IOptimizedAccessor<string> _o_sfield2;
        private IOptimizedAccessor<int> _o_prop1;
        private IOptimizedAccessor<char> _o_prop2;
        private IOptimizedAccessor _o_prop3;
        private IOptimizedInvoker _o_method1;
        private IOptimizedInvoker<int> _o_method2;
        private IOptimizedInvoker _o_method3;

        private void OnEnable()
        {
            _innerClassType = typeof(SampleInnerClass);
            _innerObject = new SampleInnerClass();

            _outerClassType = Type.GetType("DontLaugh.Test.SampleOutterClass,Assembly-CSharp");
            _outerObject = Activator.CreateInstance(_outerClassType);

            _outerOptionType = Type.GetType("DontLaugh.Test.Options,Assembly-CSharp");

            DebugToggle.enablePrint = true;

            InitAccessors();
            InitInvokers();
        }

        private void InitAccessors()
        {
            _i_field1 = GraceReflection.CreateFromField<string>(
                _innerObject, _innerClassType.GetField("m_Field1", BindingFlags.Instance | BindingFlags.NonPublic));
            _i_field2 = GraceReflection.CreateFromField<Vector3>(
                _innerObject, _innerClassType.GetField("m_Field2", BindingFlags.Instance | BindingFlags.Public));
            _i_sfield1 = GraceReflection.CreateFromField<SampleEnum>(
                null, _innerClassType.GetField("s_Field1", BindingFlags.Static | BindingFlags.NonPublic));
            _i_sfield2 = GraceReflection.CreateFromField<string>(
                null, _innerClassType.GetField("s_Field2", BindingFlags.Static | BindingFlags.Public));
            _i_prop1 = GraceReflection.CreateFromProperty<int>(
                _innerObject, _innerClassType.GetProperty("property1", BindingFlags.Instance | BindingFlags.NonPublic));
            _i_prop2 = GraceReflection.CreateFromProperty<char>(
                _innerObject, _innerClassType.GetProperty("property2", BindingFlags.Instance | BindingFlags.Public));
            _i_prop3 = GraceReflection.CreateFromProperty<SampleEnum>(
                null, _innerClassType.GetProperty("property3", BindingFlags.Static | BindingFlags.Public));

            _o_field1 = GraceReflection.CreateFromField<string>(
                _outerObject, _outerClassType.GetField("m_Field1", BindingFlags.Instance | BindingFlags.NonPublic));
            _o_field2 = GraceReflection.CreateFromField<Vector3>(
                _outerObject, _outerClassType.GetField("m_Field2", BindingFlags.Instance | BindingFlags.Public));
            _o_sfield1 = GraceReflection.CreateFromField(
                null, _outerClassType.GetField("s_Field1", BindingFlags.Static | BindingFlags.NonPublic));
            _o_sfield2 = GraceReflection.CreateFromField<string>(
                null, _outerClassType.GetField("s_Field2", BindingFlags.Static | BindingFlags.Public));
            _o_prop1 = GraceReflection.CreateFromProperty<int>(
                _outerObject, _outerClassType.GetProperty("property1", BindingFlags.Instance | BindingFlags.NonPublic));
            _o_prop2 = GraceReflection.CreateFromProperty<char>(
                _outerObject, _outerClassType.GetProperty("property2", BindingFlags.Instance | BindingFlags.Public));
            _o_prop3 = GraceReflection.CreateFromProperty(
                null, _outerClassType.GetProperty("property3", BindingFlags.Static | BindingFlags.Public));
        }

        private void InitInvokers()
        {
            _i_method1 = GraceReflection.CreateFromMethod(
                _innerObject, _innerClassType.GetMethod("Method1", BindingFlags.Instance | BindingFlags.NonPublic));
            _i_method2 = GraceReflection.CreateFromMethod<int>(
                _innerObject, _innerClassType.GetMethod("Method2", BindingFlags.Instance | BindingFlags.NonPublic));
            _i_method3 = GraceReflection.CreateFromMethod<SampleEnum>(
                null, _innerClassType.GetMethod("Method3", BindingFlags.Static | BindingFlags.Public));

            _o_method1 = GraceReflection.CreateFromMethod(
                _outerObject, _outerClassType.GetMethod("Method1", BindingFlags.Instance | BindingFlags.NonPublic));
            _o_method2 = GraceReflection.CreateFromMethod<int>(
                _outerObject, _outerClassType.GetMethod("Method2", BindingFlags.Instance | BindingFlags.NonPublic));
            _o_method3 = GraceReflection.CreateFromMethod(
                null, _outerClassType.GetMethod("Method3", BindingFlags.Static | BindingFlags.Public));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (_titleStyle == null)
            {
                _titleStyle = new GUIStyle(EditorStyles.label)
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 15,
                };
            }

            TestInnerClass();

            EditorGUILayout.Space();

            TestOuterClass();
        }

        private void TestInnerClass()
        {
            EditorGUILayout.LabelField("Test InnerClass", _titleStyle);

            if (GUILayout.Button("GET instance field 1"))
            {
                Debug.Log(_i_field1.GetValue());
            }
            if (GUILayout.Button("GET instance field 2"))
            {
                Debug.Log(_i_field2.GetValue());
            }
            if (GUILayout.Button("GET static field 1"))
            {
                Debug.Log(_i_sfield1.GetValue());
            }
            if (GUILayout.Button("GET static field 2"))
            {
                Debug.Log(_i_sfield2.GetValue());
            }
            if (GUILayout.Button("GET instance property 1"))
            {
                Debug.Log(_i_prop1.GetValue());
            }
            if (GUILayout.Button("SET instance property 2"))
            {
                _i_prop2.SetValue('i');
            }
            if (GUILayout.Button("GET & SET static property 3"))
            {
                _i_prop3.SetValue(SampleEnum.Third);
                Debug.Log(_i_prop3.GetValue());
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("TEST Method1"))
            {
                _i_method1.Invoke(233);
            }
            if (GUILayout.Button("TEST Method2"))
            {
                int layer = _i_method2.Invoke(233, SampleEnum.Third);
                Debug.Log("Return -> layer = " + layer);
            }
            if (GUILayout.Button("TEST Method3"))
            {
                SampleEnum ret = _i_method3.Invoke("Title", "This is tooltip.", 1, SampleEnum.First, Vector2.one);
                Debug.Log("Return -> @enum = " + ret);
            }
        }

        private void TestOuterClass()
        {
            EditorGUILayout.LabelField("Test OuterClass", _titleStyle);

            if (GUILayout.Button("GET instance field 1"))
            {
                Debug.Log(_o_field1.GetValue());
            }
            if (GUILayout.Button("GET instance field 2"))
            {
                Debug.Log(_o_field2.GetValue());
            }
            if (GUILayout.Button("GET static field 1"))
            {
                Debug.Log(_o_sfield1.GetValue());
            }
            if (GUILayout.Button("GET static field 2"))
            {
                Debug.Log(_o_sfield2.GetValue());
            }
            if (GUILayout.Button("GET instance property 1"))
            {
                Debug.Log(_o_prop1.GetValue());
            }
            if (GUILayout.Button("SET instance property 2"))
            {
                _o_prop2.SetValue('i');
            }
            if (GUILayout.Button("GET & SET static property 3"))
            {
                _o_prop3.SetValue(Enum.ToObject(_outerOptionType, 1));
                Debug.Log(_o_prop3.GetValue().ToString());
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("TEST Method1"))
            {
                _o_method1.Invoke(233);
            }
            if (GUILayout.Button("TEST Method2"))
            {
                int layer = _o_method2.Invoke(233, Enum.ToObject(_outerOptionType, 2));
                Debug.Log("Return -> layer = " + layer);
            }
            if (GUILayout.Button("TEST Method3"))
            {
                object ret = _o_method3.Invoke("Title", "This is tooltip.", 1, Enum.ToObject(_outerOptionType, 0), Vector2.one);
                Debug.Log("Return -> @enum = " + ret);
            }
        }
    }
#endif
}
