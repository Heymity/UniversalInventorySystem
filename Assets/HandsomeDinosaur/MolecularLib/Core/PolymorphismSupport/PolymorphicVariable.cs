using System;
using MolecularLib.Helpers;
using UnityEngine;

namespace MolecularLib.PolymorphismSupport
{
    [Serializable]
    public class PolymorphicVariable<TBase> : ISerializationCallbackReceiver where TBase : class
    {
        [SerializeField] private TypeVariable<TBase> selectedPolymorphicType;
        [SerializeField] private SerializedPolymorphicData polymorphicData;

        public TBase Value { get; private set; }
        public Type SelectedPolymorphicType => selectedPolymorphicType.Type;
        public Type ValueType => Value?.GetType();
        
        public bool As<TDerived>(out TDerived value, bool onlyPerfectTypeMatch = true) where TDerived : class, TBase
        {
            switch (onlyPerfectTypeMatch)
            {
                case true when Value.GetType() == typeof(TDerived):
                    value = Value as TDerived;
                    return true;
                case false when Value is TDerived derivedValue:
                    value = derivedValue;
                    return true;
                default:
                    value = null;
                    return false;
            }
        }

        public void OnBeforeSerialize()
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(TBase)))
                throw new Exception("PolymorphicVariable<TBase> cannot be used with UnityEngine.Object types");
        }

        public void OnAfterDeserialize()
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(TBase)))
                throw new Exception("PolymorphicVariable<TBase> cannot be used with UnityEngine.Object types");
            
            Value = Activator.CreateInstance(selectedPolymorphicType.Type) as TBase;
            
            Value = polymorphicData.SetValuesTo(Value, selectedPolymorphicType.Type);
        }
        
        public static implicit operator TBase(PolymorphicVariable<TBase> variable)
        {
            return variable.Value;
        }
    }
}
