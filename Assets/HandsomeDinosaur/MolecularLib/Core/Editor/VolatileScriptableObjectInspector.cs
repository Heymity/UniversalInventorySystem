using System;
using System.Reflection;
using MolecularLib;
using UnityEditor;
using UnityEngine;

namespace MolecularEditor
{
    [CustomEditor(typeof(VolatileScriptableObject<>), true)]
    [CanEditMultipleObjects]
    public class VolatileScriptableObjectInspector : UnityEditor.Editor
    {
        private bool _editorValuesFoldout;
        private bool _editEditorValuesFoldout;
        
        private Type GetGenericTypeForVolatileScriptableObject()
        {
            var type = target.GetType();

            var baseType = type.BaseType;
            if (baseType is null) throw new Exception("The type passed as the generic argument for VolatileScriptableObject<> is not Serializable by Unity. Maybe you forgot to add the [System.Serializable] attribute?");

            var breakFlag = 0;
            while (!baseType.IsGenericType || baseType.GetGenericTypeDefinition() != typeof(VolatileScriptableObject<>))
            {
                if (breakFlag >= 100000) throw new Exception("Infinite loop detected");
                
                baseType = baseType.BaseType;
                
                if (baseType is null) throw new Exception("The type passed as the generic argument for VolatileScriptableObject<> is not Serializable by Unity. Maybe you forgot to add the [System.Serializable] attribute?");
                breakFlag++;
            }

            return baseType.GetGenericArguments()[0];
        }

        public override void OnInspectorGUI()
        {
            var isPlaying = Application.isPlaying;
            
            if (isPlaying)
            {
                DrawRuntimeDataProp();

                _editorValuesFoldout = EditorGUILayout.Foldout(_editorValuesFoldout, "Show Editor Saved Values", true);
            }
            
            if (!isPlaying || _editorValuesFoldout)
            {
                if (isPlaying)
                    _editEditorValuesFoldout = EditorGUILayout.Toggle("Edit Editor Saved Values", _editEditorValuesFoldout);
                
                EditorGUI.BeginDisabledGroup(isPlaying && !_editEditorValuesFoldout);

                if (isPlaying && GUILayout.Button("Copy Runtime values INTO Editor values (NON-REVERSIBLE)"))
                {
                    var method = target.GetType().GetMethod("CopyRuntimeValuesToEditorSaved",
                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                    if (method is null)
                    {
                        Debug.LogError("An unexpected error occurred. Could not set the values.");
                        return;
                    }

                    method.Invoke(target, null);
                }
                
                DrawEditorDataProp();
                
                EditorGUI.EndDisabledGroup();
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawEditorDataProp()
        {
            var editorDataProp = serializedObject.FindProperty("editorSavedValue");
                
            if (editorDataProp is null)
            {
                Debug.LogError(
                    $"The provided data type {GetGenericTypeForVolatileScriptableObject()} is not serializable by Unity. By chance, haven't you forgotten to add the [System.Serializable] to the type?");
                return;
            }
            
            editorDataProp.NextVisible(true);
            do
            {
                DrawProperty(editorDataProp);
            } while (editorDataProp.NextVisible(false));
        }

        private void DrawRuntimeDataProp()
        {
            var runtimeDataProp = serializedObject.FindProperty("runtimeValue");
                
            if (runtimeDataProp is null)
            {
                Debug.LogError(
                    $"The provided data type {GetGenericTypeForVolatileScriptableObject()} is not serializable by Unity. By chance, haven't you forgotten to add the [System.Serializable] to the type?");
                return;
            }
            
            runtimeDataProp.NextVisible(true);
            do
            {
                DrawProperty(runtimeDataProp);
            } while (runtimeDataProp.NextVisible(false) && runtimeDataProp.propertyPath != "editorSavedValue");
        }

        private static void DrawProperty(SerializedProperty property)
        {
            EditorGUI.BeginChangeCheck();
                
            EditorGUILayout.PropertyField(property, true);
                
            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();
        }
    }
}
