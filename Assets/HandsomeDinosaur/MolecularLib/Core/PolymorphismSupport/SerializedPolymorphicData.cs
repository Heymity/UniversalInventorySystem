using System;
using System.Collections.Generic;
using System.Reflection;

namespace MolecularLib.PolymorphismSupport
{
    [Serializable]
    public class SerializedPolymorphicData
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
        
        public List<SerializedPolymorphicField> fields;
        
        public SerializedPolymorphicData()
        {
            fields = new List<SerializedPolymorphicField>();
        }

        public T SetValuesTo<T>(T target, Type targetType)
        {
            foreach (var field in fields)
            {
                var fieldInfo = targetType.GetField(field.fieldName, Flags);
                if (fieldInfo == null)
                    continue;

                try
                {
                    fieldInfo.SetValue(target, field.DeserializedValue);
                }
                catch(ArgumentException){ /*This means the type of the field was changed, should not throw*/ }
            }

            return target;
        }
    }
}
