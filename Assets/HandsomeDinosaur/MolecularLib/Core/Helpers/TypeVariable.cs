using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using UnityEngine;

namespace MolecularLib.Helpers
{
    [Serializable, DataContract]
    public class TypeVariable : ISerializationCallbackReceiver
    {
        [SerializeField, DataMember] private string typeName;
        [SerializeField, DataMember] private string assemblyName;

        [XmlIgnore] private Type _type;

        [XmlIgnore] public Type Type 
        {
            get 
            { 
                if (_type is null) OnAfterDeserialize();
                    return _type;
            }
            set => _type = value; 
        }

        public TypeVariable()
        {
            _type = null;
        }
        
        public void OnAfterDeserialize()
        {
            if (string.IsNullOrEmpty(assemblyName) || string.IsNullOrEmpty(typeName)) return;
#if UNITY_EDITOR
            if (TypeLibrary.AllAssemblies == null || TypeLibrary.AllAssemblies.Count == 0)
            {
                TypeLibrary.BootstrapEditor();
            }
#endif
            if (TypeLibrary.AllAssemblies != null && TypeLibrary.AllAssemblies.TryGetValue(assemblyName, out var assembly))
            {
                Type = assembly.GetType(typeName);
                return;
            }

            assembly = TypeLibrary.AllAssemblies?.First().Value;
            Type = assembly?.GetType(typeName);
        }

        public void OnBeforeSerialize()
        {
            typeName = Type?.FullName;
            assemblyName = Type?.Assembly.GetName().Name;
        }

        public static implicit operator Type(TypeVariable typeVariable) => typeVariable.Type;
        public static implicit operator TypeVariable(Type type) => new TypeVariable { Type = type };
    }

    [Serializable]
    public class TypeVariable<TBase> : TypeVariable
    {
        
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class TypeVariableBaseTypeAttribute : PropertyAttribute
    {
        public TypeVariableBaseTypeAttribute(Type type)
        {
            Type = type;
        }

        public Type Type { get; }
    }

    [Serializable, DataContract]
    public class MyClass
    {
        public int myPublicField;
        
        [SerializeField, DataMember] private int myPrivateField;
    }
}
