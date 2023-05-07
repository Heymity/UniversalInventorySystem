using System;
using System.Linq;
using System.Reflection;
using MolecularLib;
using MolecularLib.Helpers;
using UnityEditor;
using UnityEngine;

namespace MolecularEditor
{
    [CustomPropertyDrawer(typeof(TypeVariable), true)]
    public class TypeVariableDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            EditorGUI.BeginChangeCheck();

            var typeNameProp = property.FindPropertyRelative("typeName");
            var assemblyNameProp = property.FindPropertyRelative("assemblyName");
            
            var type = GetType(assemblyNameProp.stringValue, typeNameProp.stringValue);
            
            Type selectedType;
            var typeAtt = fieldInfo.GetCustomAttributes(typeof(TypeVariableBaseTypeAttribute), true);
            if (typeAtt.Any())
            {
                var baseType = (typeAtt.FirstOrDefault() as TypeVariableBaseTypeAttribute)?.Type;

                selectedType = EditorHelper.TypeField(position, label.text, type, baseType, true);
            }
            else if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.BaseType == typeof(TypeVariable) && fieldInfo.FieldType.GetGenericTypeDefinition().ToString().Contains("TBase"))
            {
                var baseType = fieldInfo.FieldType.GetGenericArguments()[0];

                selectedType = EditorHelper.TypeField(position, label.text, type, baseType, true);
            }
            else
            {
                selectedType = EditorHelper.TypeField<object>(position, label.text, type, true);
            }

            assemblyNameProp.stringValue = selectedType.Assembly.GetName().Name;
            typeNameProp.stringValue = selectedType.FullName;

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(property.serializedObject.targetObject, "TypeVariable changed");

                property.serializedObject.ApplyModifiedProperties();

                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
            
            EditorGUI.EndProperty();
        }

        private Assembly _cachedAssembly;
        private Type _cachedType;
        private string _lastAssemblyName;
        private string _lastTypeName;
        private Type GetType(string assemblyName, string typeName)
        {
            if (assemblyName != "" && assemblyName == _lastAssemblyName)
            {
                //Debug.Log("Cached");
                if (_lastTypeName != "" && _lastTypeName == typeName)
                {
                    return _cachedType;
                }

                var type = _cachedAssembly.GetType(typeName);
                if (type != null)
                {
                    _cachedType = type;
                    _lastTypeName = typeName;
                    return type;
                }
            }
            //Debug.Log($"Not Cached | {_lastAssemblyName} | {_lastTypeName} | {assemblyName} | {typeName}");
            _lastAssemblyName = assemblyName;
            _lastTypeName = typeName;

            if (!TypeLibrary.AllAssemblies!.TryGetValue(assemblyName, out var assembly))
                return !string.IsNullOrEmpty(typeName) ? 
                    (TypeLibrary.AllAssemblies.FirstOrDefault().Value.GetType(typeName) ?? TypeLibrary.AllAssemblies.FirstOrDefault().Value.GetTypes().FirstOrDefault()) 
                    : TypeLibrary.AllAssemblies.FirstOrDefault().Value.GetTypes().FirstOrDefault();
            
            _cachedAssembly = assembly;
            _cachedType = assembly.GetType(typeName);
            return _cachedType;

        }
    }
}
