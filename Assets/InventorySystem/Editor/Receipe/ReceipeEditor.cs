using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Recipe))]
public class ReceipeEditor : Editor
{
    SerializedProperty numberOfFactorsProp;
    SerializedProperty numberOfProductsProp;

    SerializedProperty factorsProp;
    SerializedProperty productsProp;

    private void OnEnable()
    {
        numberOfFactorsProp = serializedObject.FindProperty("numberOfFactors");
        numberOfProductsProp = serializedObject.FindProperty("numberOfProducts");
        factorsProp = serializedObject.FindProperty("factors");
        productsProp = serializedObject.FindProperty("products");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(numberOfFactorsProp, new GUIContent("Number of input items"));

        factorsProp.arraySize = numberOfFactorsProp.intValue;
        factorsProp.isExpanded = EditorGUILayout.Foldout(factorsProp.isExpanded, "Input items");
        if (factorsProp.isExpanded)
        {
            EditorGUI.indentLevel++;

            for(int i = 0;i < factorsProp.arraySize; i++)
            {
                EditorGUILayout.ObjectField(factorsProp.GetArrayElementAtIndex(i), new GUIContent($"Input Item {i}"));
            }

            if(factorsProp.arraySize == 0)
            {
                EditorGUILayout.LabelField("Array lenght is 0");
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Separator();

        EditorGUILayout.PropertyField(numberOfProductsProp, new GUIContent("Number of output items"));

        productsProp.arraySize = numberOfProductsProp.intValue;
        productsProp.isExpanded = EditorGUILayout.Foldout(productsProp.isExpanded, "Output items");
        if (productsProp.isExpanded)
        {
            EditorGUI.indentLevel++;

            for (int i = 0; i < productsProp.arraySize; i++)
            {
                EditorGUILayout.ObjectField(productsProp.GetArrayElementAtIndex(i), new GUIContent($"Output Item {i}"));
            }

            if (productsProp.arraySize == 0)
            {
                EditorGUILayout.LabelField("Array lenght is 0");
            }

            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
