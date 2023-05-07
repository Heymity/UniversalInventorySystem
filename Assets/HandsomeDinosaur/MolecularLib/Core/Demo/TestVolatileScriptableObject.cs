using System;
using System.Collections.Generic;
using MolecularLib.Helpers;
using UnityEngine;

namespace MolecularLib.Demo
{
    [CreateAssetMenu(fileName = "Volatile SO", menuName = "New Volatile SO", order = 0)]
    public class TestVolatileScriptableObject 
        : VolatileScriptableObject<TestVolatileScriptableObject.Data>
    {
        // Here you will put all the data you want to be volatile
        [Serializable]
        public class Data
        { 
            [TextArea] public string myString;
            public MonoBehaviour myBehaviour;
            public int myInt;
            public float myFloat;
            public List<string> myList;
            public Optional<SerializableDictionary<int, string>> myOptionalDictionary;
            public ScriptableObject myScriptableObject;
        } 
        
        // Here is a quick way of accessing the data. Can be done in other ways too.
        public Data VolatileData
        {
            get => Value;
            set => Value = value;
        }
        
        // Can be done like this too.
        public string MyString
        {
            get => Value.myString;
            set => Value.myString = value;
        }
        public int MyInt
        {
            get => Value.myInt;
            set => Value.myInt = value;
        }
        // ...etc...
        
        public static implicit operator Data(TestVolatileScriptableObject obj)
        {
            return obj.VolatileData;
        }
    }
}
