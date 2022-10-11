using System;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace DontLaugh.Test
{
    public class GraceReflectionPerformanceTester : MonoBehaviour
    {
        #region Property

        public int testCount = 10;
        public InputField countInput;
        public Text toggleText;
        public Text resTimer1_1;
        public Text resTimer1_2;
        public Text resTimer2_1;
        public Text resTimer2_2;
        public Text resTimer3_1;
        public Text resTimer3_2;
        public Text resTimer4_1;
        public Text resTimer4_2;
        public Text resTimer5_1;
        public Text resTimer5_2;
        public Text resTimer6_1;
        public Text resTimer6_2;
        public Text resTimer7_1;
        public Text resTimer7_2;
        public Text resTimer8_1;
        public Text resTimer8_2;
        public Text resTimer9_1;
        public Text resTimer9_2;
        public Text resTimer10_1;
        public Text resTimer10_2;

        private Stopwatch _watch;
        private int _lastCount;
        private bool _testInnerClass = true;

        private Type _innerClassType;
        private SampleInnerClass _innerObject;
        private Type _outerClassType;
        private object _outerObject;
        private Type _outerOptionType;

        private FieldInfo _i_field1_info;
        private IOptimizedAccessor<string> _i_field1;
        private FieldInfo _i_field2_info;
        private IOptimizedAccessor<Vector3> _i_field2;
        private FieldInfo _i_sfield1_info;
        private IOptimizedAccessor<SampleEnum> _i_sfield1;
        private FieldInfo _i_sfield2_info;
        private IOptimizedAccessor<string> _i_sfield2;
        private PropertyInfo _i_prop1_info;
        private IOptimizedAccessor<int> _i_prop1;
        private PropertyInfo _i_prop2_info;
        private IOptimizedAccessor<char> _i_prop2;
        private PropertyInfo _i_prop3_info;
        private IOptimizedAccessor<SampleEnum> _i_prop3;
        private MethodInfo _i_method1_info;
        private IOptimizedInvoker _i_method1;
        private MethodInfo _i_method2_info;
        private IOptimizedInvoker<int> _i_method2;
        private MethodInfo _i_method3_info;
        private IOptimizedInvoker<SampleEnum> _i_method3;

        private FieldInfo _o_field1_info;
        private IOptimizedAccessor<string> _o_field1;
        private FieldInfo _o_field2_info;
        private IOptimizedAccessor<Vector3> _o_field2;
        private FieldInfo _o_sfield1_info;
        private IOptimizedAccessor _o_sfield1;
        private FieldInfo _o_sfield2_info;
        private IOptimizedAccessor<string> _o_sfield2;
        private PropertyInfo _o_prop1_info;
        private IOptimizedAccessor<int> _o_prop1;
        private PropertyInfo _o_prop2_info;
        private IOptimizedAccessor<char> _o_prop2;
        private PropertyInfo _o_prop3_info;
        private IOptimizedAccessor _o_prop3;
        private MethodInfo _o_method1_info;
        private IOptimizedInvoker _o_method1;
        private MethodInfo _o_method2_info;
        private IOptimizedInvoker<int> _o_method2;
        private MethodInfo _o_method3_info;
        private IOptimizedInvoker _o_method3;

        #endregion Property

        private void Awake()
        {
            _watch = new Stopwatch();
            _watch.Reset();

            _innerClassType = typeof(SampleInnerClass);
            _innerObject = new SampleInnerClass();

            _outerClassType = Type.GetType("DontLaugh.Test.SampleOutterClass,Assembly-CSharp");
            _outerObject = Activator.CreateInstance(_outerClassType);

            _outerOptionType = Type.GetType("DontLaugh.Test.Options,Assembly-CSharp");

            InitAccessors();
            InitInvokers();
        }

        private void InitAccessors()
        {
            _i_field1_info = _innerClassType.GetField("m_Field1", BindingFlags.Instance | BindingFlags.NonPublic);
            _i_field1 = GraceReflection.CreateFromField<string>(_innerObject, _i_field1_info);

            _i_field2_info = _innerClassType.GetField("m_Field2", BindingFlags.Instance | BindingFlags.Public);
            _i_field2 = GraceReflection.CreateFromField<Vector3>(_innerObject, _i_field2_info);

            _i_sfield1_info = _innerClassType.GetField("s_Field1", BindingFlags.Static | BindingFlags.NonPublic);
            _i_sfield1 = GraceReflection.CreateFromField<SampleEnum>(null, _i_sfield1_info);

            _i_sfield2_info = _innerClassType.GetField("s_Field2", BindingFlags.Static | BindingFlags.Public);
            _i_sfield2 = GraceReflection.CreateFromField<string>(null, _i_sfield2_info);

            _i_prop1_info = _innerClassType.GetProperty("property1", BindingFlags.Instance | BindingFlags.NonPublic);
            _i_prop1 = GraceReflection.CreateFromProperty<int>(_innerObject, _i_prop1_info);

            _i_prop2_info = _innerClassType.GetProperty("property2", BindingFlags.Instance | BindingFlags.Public);
            _i_prop2 = GraceReflection.CreateFromProperty<char>(_innerObject, _i_prop2_info);

            _i_prop3_info = _innerClassType.GetProperty("property3", BindingFlags.Static | BindingFlags.Public);
            _i_prop3 = GraceReflection.CreateFromProperty<SampleEnum>(null, _i_prop3_info);

            _o_field1_info = _outerClassType.GetField("m_Field1", BindingFlags.Instance | BindingFlags.NonPublic);
            _o_field1 = GraceReflection.CreateFromField<string>(_outerObject, _o_field1_info);

            _o_field2_info = _outerClassType.GetField("m_Field2", BindingFlags.Instance | BindingFlags.Public);
            _o_field2 = GraceReflection.CreateFromField<Vector3>(_outerObject, _o_field2_info);

            _o_sfield1_info = _outerClassType.GetField("s_Field1", BindingFlags.Static | BindingFlags.NonPublic);
            _o_sfield1 = GraceReflection.CreateFromField(null, _o_sfield1_info);

            _o_sfield2_info = _outerClassType.GetField("s_Field2", BindingFlags.Static | BindingFlags.Public);
            _o_sfield2 = GraceReflection.CreateFromField<string>(null, _o_sfield2_info);

            _o_prop1_info = _outerClassType.GetProperty("property1", BindingFlags.Instance | BindingFlags.NonPublic);
            _o_prop1 = GraceReflection.CreateFromProperty<int>(_outerObject, _o_prop1_info);

            _o_prop2_info = _outerClassType.GetProperty("property2", BindingFlags.Instance | BindingFlags.Public);
            _o_prop2 = GraceReflection.CreateFromProperty<char>(_outerObject, _o_prop2_info);

            _o_prop3_info = _outerClassType.GetProperty("property3", BindingFlags.Static | BindingFlags.Public);
            _o_prop3 = GraceReflection.CreateFromProperty(null, _o_prop3_info);
        }

        private void InitInvokers()
        {
            _i_method1_info = _innerClassType.GetMethod("Method1", BindingFlags.Instance | BindingFlags.NonPublic);
            _i_method1 = GraceReflection.CreateFromMethod(_innerObject, _i_method1_info);

            _i_method2_info = _innerClassType.GetMethod("Method2", BindingFlags.Instance | BindingFlags.NonPublic);
            _i_method2 = GraceReflection.CreateFromMethod<int>(_innerObject, _i_method2_info);

            _i_method3_info = _innerClassType.GetMethod("Method3", BindingFlags.Static | BindingFlags.Public);
            _i_method3 = GraceReflection.CreateFromMethod<SampleEnum>(null, _i_method3_info);

            _o_method1_info = _outerClassType.GetMethod("Method1", BindingFlags.Instance | BindingFlags.NonPublic);
            _o_method1 = GraceReflection.CreateFromMethod(_outerObject, _o_method1_info);

            _o_method2_info = _outerClassType.GetMethod("Method2", BindingFlags.Instance | BindingFlags.NonPublic);
            _o_method2 = GraceReflection.CreateFromMethod<int>(_outerObject, _o_method2_info);

            _o_method3_info = _outerClassType.GetMethod("Method3", BindingFlags.Static | BindingFlags.Public);
            _o_method3 = GraceReflection.CreateFromMethod(null, _o_method3_info);
        }

        private void Start()
        {
            _lastCount = testCount;
            countInput.text = testCount.ToString();
            countInput.onValueChanged.AddListener(OnCountInputChanged);
        }

        private void OnCountInputChanged(string value)
        {
            if (int.TryParse(value, out int intVal))
            {
                testCount = intVal;
            }
        }

        public void ToggleInnerClass()
        {
            _testInnerClass = !_testInnerClass;
            toggleText.text = _testInnerClass ? "SampleInnerClass" : "SampleOutterClass";
        }

        public void AddCount()
        {
            if (testCount < 1000000000)
            {
                testCount *= 10;
            }
        }

        public void SubCount()
        {
            if (testCount >= 10)
            {
                testCount /= 10;
            }
        }

        public void Update()
        {
            if (_lastCount != testCount)
            {
                countInput.text = testCount.ToString();
                _lastCount = testCount;
            }
        }

        public void StartTest()
        {
            DebugToggle.enablePrint = false;
            TestFields();
            TestProperties();
            TestMethods();
        }

        private void TestFields()
        {
            // Test field1
            StartTimer();
            for (int i = 0; i < testCount; i++)
            {
                _i_field1_info.GetValue(_innerObject);
            }
            StopTimer(resTimer1_1);

            StartTimer();
            for (int i = 0; i < testCount; i++)
            {
                _i_field1.GetValue();
            }
            StopTimer(resTimer1_2);

            // Test field2
            StartTimer();
            for (int i = 0; i < testCount; i++)
            {
                _i_field2_info.GetValue(_innerObject);
            }
            StopTimer(resTimer2_1);

            StartTimer();
            for (int i = 0; i < testCount; i++)
            {
                _i_field2.GetValue();
            }
            StopTimer(resTimer2_2);

            // Test s_field1
            StartTimer();
            for (int i = 0; i < testCount; i++)
            {
                _i_sfield1_info.GetValue(null);
            }
            StopTimer(resTimer3_1);

            StartTimer();
            for (int i = 0; i < testCount; i++)
            {
                _i_sfield1.GetValue();
            }
            StopTimer(resTimer3_2);

            // Test s_field2
            StartTimer();
            for (int i = 0; i < testCount; i++)
            {
                _i_sfield2_info.GetValue(null);
            }
            StopTimer(resTimer4_1);

            StartTimer();
            for (int i = 0; i < testCount; i++)
            {
                _i_sfield2.GetValue();
            }
            StopTimer(resTimer4_2);
        }

        private void TestProperties()
        {
            // Test property1
            StartTimer();
            for (int i = 0; i < testCount; i++)
            {
                _i_prop1_info.GetValue(_innerObject);
            }
            StopTimer(resTimer5_1);

            StartTimer();
            for (int i = 0; i < testCount; i++)
            {
                _i_prop1.GetValue();
            }
            StopTimer(resTimer5_2);

            // Test property2
            StartTimer();
            for (int i = 0; i < testCount; i++)
            {
                _i_prop2_info.SetValue(_innerObject, 'i');
            }
            StopTimer(resTimer6_1);

            StartTimer();
            for (int i = 0; i < testCount; i++)
            {
                _i_prop2.SetValue('i');
            }
            StopTimer(resTimer6_2);

            // Test property3
            StartTimer();
            for (int i = 0; i < testCount; i++)
            {
                _i_prop3_info.GetValue(null);
                _i_prop3_info.SetValue(null, SampleEnum.Third);
            }
            StopTimer(resTimer7_1);

            StartTimer();
            for (int i = 0; i < testCount; i++)
            {
                _i_prop3.GetValue();
                _i_prop3.SetValue(SampleEnum.Third);
            }
            StopTimer(resTimer7_2);
        }

        private void TestMethods()
        {
            // Test method1
            StartTimer();
            for (int i = 0; i < testCount; i++)
            {
                _i_method1_info.Invoke(_innerObject, new object[] { 233 });
            }
            StopTimer(resTimer8_1);

            StartTimer();
            for (int i = 0; i < testCount; i++)
            {
                _i_method1.Invoke(233);
            }
            StopTimer(resTimer8_2);

            // Test method2
            StartTimer();
            for (int i = 0; i < testCount; i++)
            {
                _i_method2_info.Invoke(_innerObject, new object[] { 233, SampleEnum.Third });
            }
            StopTimer(resTimer9_1);

            StartTimer();
            for (int i = 0; i < testCount; i++)
            {
                _i_method2.Invoke(233, SampleEnum.Third);
            }
            StopTimer(resTimer9_2);

            // Test method3
            StartTimer();
            for (int i = 0; i < testCount; i++)
            {
                _i_method3_info.Invoke(_innerObject, new object[] { "Title", "This is tooltip.", 1, SampleEnum.First, Vector2.one });
            }
            StopTimer(resTimer10_1);

            StartTimer();
            for (int i = 0; i < testCount; i++)
            {
                _i_method3.Invoke("Title", "This is tooltip.", 1, SampleEnum.First, Vector2.one);
            }
            StopTimer(resTimer10_2);
        }

        private void StartTimer()
        {
            _watch.Start();
        }

        private void StopTimer(Text text)
        {
            _watch.Stop();
            text.text = _watch.ElapsedMilliseconds.ToString();
            _watch.Reset();
        }
    }
}
