using System;
using System.IO;
using System.Xml.Serialization;
using MolecularLib.Helpers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MolecularLib.PolymorphismSupport
{
    [Serializable]
    public class SerializedPolymorphicField : ISerializationCallbackReceiver
    {
        private const string UnityObject = "--UNITYOBJECT--";
        
        public string fieldName;
        public TypeVariable fieldType;
        [TextArea] public string serializedValue; 
        public Object unityObjectValue;

        public object DeserializedValue { get; set; }

        private string SerializeData(object value)
        {
            using var writer = new StringWriter();
            if (value is IPolymorphicSerializationOverride inter)
            {
                inter.Serialize(writer);
                return writer.ToString();
            }
            
            if (value is Object unityObject)
            {
                unityObjectValue = unityObject;
                return UnityObject;
            }
            
            var serializer = new XmlSerializer(fieldType);
            serializer.Serialize(writer, value!);

            return writer.ToString();
        }
        
        private object DeserializeData()
        {
            var inter = fieldType.Type.GetInterface("IPolymorphicSerializationOverride");
            if (inter != null)
            {
                var deserialized = Activator.CreateInstance(fieldType.Type);
                (deserialized as IPolymorphicSerializationOverride)?.Deserialize(serializedValue);
                return deserialized;
            }

            if (serializedValue == UnityObject && typeof(Object).IsAssignableFrom(fieldType.Type))
            {
                return unityObjectValue;
            }
            
            using var reader = new StringReader(serializedValue);
            var serializer = new XmlSerializer(fieldType.Type);
            try
            {
                return serializer.Deserialize(reader);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Could not deserialize PolymorphicVariable, reason: {e}\n\n\n XML:\n{serializedValue}\n\n Deleting XML data");
                serializedValue = "";
                return null;
            }
        }
        
        public void OnBeforeSerialize()
        {
            if (DeserializedValue is null) return;

            var valueToSerialize = DeserializedValue;
            
            fieldType.Type = valueToSerialize.GetType();

            serializedValue = SerializeData(valueToSerialize!);
        }

        public void OnAfterDeserialize()
        {
            try
            {
                if (string.IsNullOrEmpty(serializedValue))
                {
                    DeserializedValue = null;
                    return;
                }
                
                DeserializedValue = DeserializeData();
            }
            catch (ArgumentNullException)
            {
                DeserializedValue = null;
            }
        }
    }
}
