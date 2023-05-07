// Because this is a script just demonstrating the drawers of the types, it has lots of warnings from ReSharper about unused fields or inefficient code, this line just hide those warnings so they don't clog up the screen.
// ReSharper disable NotAccessedField.Local
// ReSharper disable Unity.InefficientPropertyAccess
// ReSharper disable RedundantNameQualifier
// ReSharper disable UnusedVariable
// ReSharper disable InconsistentNaming

using System.Collections.Generic;
using MolecularLib.Helpers;
using MolecularLib.PolymorphismSupport;
using UnityEngine;
using Timer = MolecularLib.Timers.Timer;

namespace MolecularLib.Demo
{   
    public class DemoDrawersScript : MonoBehaviour
    {
        [Header("Sprite, Tag and Type variable editor drawers")] 
        [SerializeField] private Sprite sprite;
        [SerializeField] private Tag tagTest;
        [SerializeField, TypeVariableBaseType(typeof(MonoBehaviour))] private TypeVariable type;
        [Space] 
        [Header("Range variables and editor drawers")] 
        [SerializeField] private MolecularLib.Helpers.Range<double> doubleRange;
        [SerializeField] private MolecularLib.Helpers.Range<float> genericFloatRange;
        [SerializeField] private MolecularLib.Helpers.Range floatRange;
        [SerializeField] private MolecularLib.Helpers.Range intRange;
        [SerializeField, MinMaxRange(-30.6345f, 24.34634f)] private MolecularLib.Helpers.Range minMaxFloatRange;
        [SerializeField, MinMaxRange(-30, 20)] private RangeInteger minMaxIntRange;
        [SerializeField] private RangeVector2 vec2Range;
        [SerializeField] private RangeVector3 vec3Range;
        [SerializeField] private RangeVector2Int vec2IntRange;
        [SerializeField] private RangeVector3Int vec3IntRange;
        [Space] 
        [Header("Serializable Dictionary examples")] 
        [SerializeField] private SerializableDictionary<string, int> stringToInt;
        [SerializeField] private SerializableDictionary<HideFlags, Color> flagsToColor;
        [SerializeField] private SerializableDictionary<string, TestStruct> myStructs;
        [SerializeField] private SerializableDictionary<TestStruct, string> myStructsOpposite;
        [SerializeField] private SerializableDictionary<TestStruct, TestStruct> myStructsBoth;
        [SerializeField] private SerializableDictionary<string, Sprite> stringToSprite;
        [Space] 
        [Header("Polymorphic variable examples")] 
        [SerializeField] 
        private PolymorphicVariable<Base> myPolymorphicVariable;
        [Space] 
        [Header("Optional variable examples")] 
        [SerializeField] private Optional<string> myOptionalString;
        [SerializeField] private Optional<List<string>> myList;
        [SerializeField] private Optional<SerializableDictionary<string, string>> myOptionalDictionary;
        [SerializeField] private Optional<MolecularLib.Helpers.Range> myOptionalRange;
        
        public PolymorphicVariable<Base> PolymorphicVariable => myPolymorphicVariable;
        

        [ContextMenu("Demos")]
        public void Demos()
        {
            // Coroutine Timer (can also be used outside of a MonoBehaviour)
            var timer = Timer.Create(5, () => Debug.Log("Timer finished"));
            Debug.Log(timer.ElapsedSeconds);
            
            
            // Async Timers
            Timer.TimerAsync(2, () => Debug.Log("TimerAsync finished"));

            
            var timerReference = Timer.TimerAsyncReference(3, repeat: true);
            timerReference.OnFinish += () => Debug.Log("TimerAsyncReference finished");
            
            floatRange.Random();

            var min = floatRange.Min;
            var max = floatRange.Max;

            var middle = floatRange.MidPoint;

            var patrolPos = vec3Range.Random();

            if (vec2Range.IsInRange(transform.position))
            {
                // Do something...
            }
                
            
            transform.position = transform.position.WithoutX().WithZ(1);

            var myVector = transform.position;
            var myTileMapPos = myVector.ToVec2Int();

            Color titleColor = "Hello World".ToColor();
            "This green string is in bold".Color(Color.green).Bold();
            "This string will be cut to fit the container".Ellipsis(30, "Label");
            
            var backgroundColor = ColorHelper.NormalizeToColor(28, 28, 28);
            var textColor = backgroundColor.TextForegroundColor();
            textColor.WithR(1f);
            var textColorHexString = textColor.ToHexString();
            
            
            if (myOptionalString.HasValue)
                Debug.Log(myOptionalString.Value);
            
            // Or simply (Using implicit operators)
            
            if (myOptionalString)
                Debug.Log(myOptionalString);
            
            
            if (myPolymorphicVariable.As<A>(out var asA))
                Debug.Log($"As A | aClassInt: {asA.aClassInt}");
            else if (myPolymorphicVariable.As<B>(out var asB))
                Debug.Log($"As B | bClassInt: {asB.bClassInt} | bClassRange: {asB.bClassRange.Min} - {asB.bClassRange.Max}");
            else if (myPolymorphicVariable.As<C>(out var asC))
                Debug.Log($"As C | cClassFloat: {asC.cClassFloat}");
            else
                Debug.Log($"As Base | myBaseString: {myPolymorphicVariable.Value.myBaseString}");

            myPolymorphicVariable.Value.myBaseString = "Hey, I changed it!";
            
            
            stringToInt.Add("Hello", 1);
            flagsToColor.Add(HideFlags.HideAndDontSave, Color.red);
            stringToSprite.Add("Hello", sprite);
            
            stringToInt["Hello"] = 2;
            if (!stringToInt.TryGetValue("Nonexistent", out var value))
                stringToInt.Add("Nonexistent", 0);
            
            
            
            myStructs.Add("Hello", new TestStruct());
            myStructsOpposite.Add(new TestStruct(), "Hello");
            myStructsBoth.Add(new TestStruct(), new TestStruct());
        }
    }

    [System.Serializable]
    public struct TestStruct
    {
        public int MyInt;
        public bool MyBool;
        public List<string> MyStringList;
    }
        
    [System.Serializable]
    public class Base
    {
        public string myBaseString;
    }

    [System.Serializable]
    public class A : Base
    {
        public int aClassInt;
        [SerializeField] private SerializableDictionary<string, int> aPrivateDictionary; // can be public as well ;)
        [SerializeField, TextArea] protected string protectedString;
    }

    [System.Serializable]
    public class B : Base
    {
        public MolecularLib.Helpers.Range bClassRange;
        public int bClassInt;
        [SerializeField] private float bClassPrivateFloat;
        [SerializeField] protected float bClassProtectedFloat;
    }

    [System.Serializable]
    public class C : B
    {
        public float cClassFloat;
    }

    [System.Serializable]
    public class WithUnityObject : A
    {
        public Sprite sprite;
    }
}
