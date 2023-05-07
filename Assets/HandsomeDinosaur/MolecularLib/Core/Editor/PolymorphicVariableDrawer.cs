using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MolecularLib.Helpers;
using MolecularLib.PolymorphismSupport;
using UnityEditor;
using UnityEngine;

namespace MolecularEditor
{
    [CustomPropertyDrawer(typeof(PolymorphicVariable<>))]
    public class PolymorphicVariableDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;
            
            var targetObj = fieldInfo.GetValue(EditorHelper.GetTargetValue(property, 1));
            _typeField ??= fieldInfo.FieldType.GetField("selectedPolymorphicType", EditorHelper.UnitySerializesBindingFlags);
            if (_typeField is null)
                throw new Exception("selectedPolymorphicType field not found");
            var typeVar = _typeField.GetValue(targetObj) as TypeVariable;

            if (typeVar?.Type == null) return EditorGUIUtility.singleLineHeight;
            
            var (editProps, attrDatas) = GetEditablePolymorphicData(typeVar, targetObj);

            var height = 4f;
            for (var i = 0; i < editProps.fields.Count; i++)
            {
                var prop = editProps.fields[i];
                var attrData = attrDatas[i];

                height += EditorHelper.AutoTypeFieldGetHeight(prop.fieldType, prop.DeserializedValue, GetUniqueId(property, prop),
                    ObjectNames.NicifyVariableName(prop.fieldName), attrData);
            }
            
            return EditorGUIUtility.singleLineHeight + height;
        }
        
        private FieldInfo _typeField;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            
            var typeProp = property.FindPropertyRelative("selectedPolymorphicType");

            var foldoutRect = position;
            foldoutRect.width = EditorGUIUtility.labelWidth;
            foldoutRect.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(foldoutRect, property.isExpanded, "");
            
            var typeSelectionRect = EditorGUI.PrefixLabel(position, label);
            typeSelectionRect.height = EditorGUIUtility.singleLineHeight;
            
            var originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 90;
            EditorGUI.PropertyField(typeSelectionRect, typeProp, new GUIContent("Instance Type"));
            EditorGUIUtility.labelWidth = originalLabelWidth;
            
            EditorGUI.EndFoldoutHeaderGroup();
            
            var targetObj = fieldInfo.GetValue(EditorHelper.GetTargetValue(property, 1));
            _typeField ??= fieldInfo.FieldType.GetField("selectedPolymorphicType", EditorHelper.UnitySerializesBindingFlags);
            if (_typeField is null)
                throw new Exception("selectedPolymorphicType field not found");
            var typeVar = _typeField.GetValue(targetObj) as TypeVariable;
            var (editProps, attrDatas) = GetEditablePolymorphicData(typeVar, targetObj);
     
            if (property.isExpanded)
            {
                var fieldRect = position;
                fieldRect.x += 15;
                fieldRect.width -= 15;
                fieldRect.height = EditorGUIUtility.singleLineHeight;
                for (var i = 0; i < editProps.fields.Count; i++)
                {
                    var prop = editProps.fields[i];
                    var attrData = attrDatas[i];
                    fieldRect.y += fieldRect.height + 2;
                    fieldRect.height = EditorGUIUtility.singleLineHeight;
                    
                    prop.DeserializedValue = EditorHelper.AutoTypeField(ref fieldRect, prop.fieldType,
                        prop.DeserializedValue,
                        GetUniqueId(property, prop),
                        ObjectNames.NicifyVariableName(prop.fieldName), attrData);

                    prop.OnBeforeSerialize();
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                SetSerializedPolymorphicData(editProps, targetObj);
                property.serializedObject.ApplyModifiedProperties();

                UpdateInstance(targetObj);
                
                Undo.RecordObject(property.serializedObject.targetObject, "Polymorphic Variable Changed");
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
            EditorGUI.EndProperty();
        }

        private string GetUniqueId(SerializedProperty property, SerializedPolymorphicField prop) =>
            (property.propertyPath + prop.fieldType.Type + fieldInfo.GetHashCode() + prop.fieldName).GetHashCode().ToString();
        
        private (SerializedPolymorphicData, List<IList<CustomAttributeData>>) GetEditablePolymorphicData(Type type, object targetObject)
        {
            var (idealTypePolyData, idealAttrDatas) = GetTypeIdealSerializedPolymorphicData(type);

            var polymorphicDataField = fieldInfo.FieldType.GetField("polymorphicData", EditorHelper.UnitySerializesBindingFlags);
            if (polymorphicDataField is null)
                throw new Exception("Could not find the polymorphicData field in the PolymorphicVariable class");
            
            var definedTypePolyData = polymorphicDataField.GetValue(targetObject) as SerializedPolymorphicData;
            //Debug.Log($"[GENERATION] DefinedPlyData: {definedTypePolyData.fields[0].serializedValue}");
            var editableProps = new SerializedPolymorphicData();

            if (definedTypePolyData is null || definedTypePolyData.fields.Count == 0) return (idealTypePolyData, idealAttrDatas);
            
            for (var i = 0; i < idealTypePolyData.fields.Count; i++)
            {
                SerializedPolymorphicField fieldData;

                var idealTypeCurrentField = idealTypePolyData.fields[i];

                var targetCurrentField = definedTypePolyData.fields.Find(f => f.fieldName == idealTypeCurrentField.fieldName && f.fieldType.Type == idealTypeCurrentField.fieldType.Type);

                if (targetCurrentField is null)
                {
                    fieldData = new SerializedPolymorphicField
                    {
                        fieldName = idealTypeCurrentField.fieldName,
                        fieldType = idealTypeCurrentField.fieldType,
                        DeserializedValue = null,
                        serializedValue = idealTypeCurrentField.serializedValue,
                        unityObjectValue = idealTypeCurrentField.unityObjectValue,
                    };
                }
                else
                {
                    fieldData = new SerializedPolymorphicField
                    {
                        fieldName = targetCurrentField.fieldName,
                        fieldType = targetCurrentField.fieldType,
                        DeserializedValue = targetCurrentField.DeserializedValue,
                        serializedValue = targetCurrentField.serializedValue,
                        unityObjectValue = targetCurrentField.unityObjectValue,
                    };
                }

                fieldData.OnAfterDeserialize();
                
                //Debug.Log($"[GENERATION] DeserializedValue: {fieldData.DeserializedValue} | Type: {fieldData.fieldType.Type} | FieldName: {fieldData.fieldName} | SerializedVal: {fieldData.serializedValue}");
                editableProps.fields.Add(fieldData);
            }

            return (editableProps, idealAttrDatas);
        }

        private static (SerializedPolymorphicData, List<IList<CustomAttributeData>>) GetTypeIdealSerializedPolymorphicData(Type type)
        {
            //Debug.Log($"[GENERATION] Type: {type}");
            var fields = type.GetFields(EditorHelper.UnitySerializesBindingFlags);
            var attrDatas = new List<IList<CustomAttributeData>>();
            
            var serializedData = new SerializedPolymorphicData();
            foreach (var field in fields)
            {
                var polymorphicField = new SerializedPolymorphicField
                {
                    fieldName = field.Name,
                    fieldType = field.FieldType,
                    DeserializedValue = null
                };

                serializedData.fields.Add(polymorphicField);

                attrDatas.Add(field.GetCustomAttributesData());
            }

            return (serializedData, attrDatas);
        }

        private MethodInfo _cachedOnAfterDeserializeMethod;
        private FieldInfo _cachedPolymorphicDataField;
        private Type _cachedFieldType;
        private void SetSerializedPolymorphicData(SerializedPolymorphicData newData, object targetObj)
        {
            _cachedFieldType ??= fieldInfo.FieldType;
            _cachedPolymorphicDataField ??= _cachedFieldType.GetField("polymorphicData", EditorHelper.UnitySerializesBindingFlags);
            if (_cachedPolymorphicDataField is null)
                throw new Exception("Could not find the polymorphicData field in the PolymorphicVariable class");
            
            //Debug.Log($"[REFLECTION SAVING] (Before) Current value {(polymorphicDataField.GetValue(targetObj) as SerializedPolymorphicData).fields[0].serializedValue}");
            _cachedPolymorphicDataField.SetValue(targetObj, newData);
            //Debug.Log($"[REFLECTION SAVING] (After) Current value {(polymorphicDataField.GetValue(targetObj) as SerializedPolymorphicData).fields[0].serializedValue}");
        }

        private void UpdateInstance(object targetObj)
        {
            _cachedFieldType ??= fieldInfo.FieldType;
            _cachedOnAfterDeserializeMethod ??= _cachedFieldType.GetMethod("OnAfterDeserialize", EditorHelper.UnitySerializesBindingFlags);
            
            if (_cachedOnAfterDeserializeMethod is null)
                throw new Exception("Could not find the polymorphicData method for updating the values");
            
            _cachedOnAfterDeserializeMethod?.Invoke(targetObj, null);

        }
    }
}
