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

    SerializedProperty factorsAmountProp;
    SerializedProperty productsAmountProp;

    private void OnEnable()
    {
        numberOfFactorsProp = serializedObject.FindProperty("numberOfFactors");
        numberOfProductsProp = serializedObject.FindProperty("numberOfProducts");
        factorsProp = serializedObject.FindProperty("factors");
        factorsAmountProp = serializedObject.FindProperty("amountFactors");
        productsProp = serializedObject.FindProperty("products");
        productsAmountProp = serializedObject.FindProperty("amountProducts");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(numberOfFactorsProp, new GUIContent("Number of input items"));

        factorsProp.arraySize = numberOfFactorsProp.intValue;
        factorsAmountProp.arraySize = numberOfFactorsProp.intValue;
        factorsProp.isExpanded = EditorGUILayout.Foldout(factorsProp.isExpanded, "Input items");
        if (factorsProp.isExpanded)
        {
            EditorGUI.indentLevel++;

            for(int i = 0;i < factorsProp.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(factorsProp.GetArrayElementAtIndex(i), new GUIContent($"Input Item {i}"));
                factorsAmountProp.GetArrayElementAtIndex(i).intValue = EditorGUILayout.IntField(factorsAmountProp.GetArrayElementAtIndex(i).intValue);
                EditorGUILayout.EndHorizontal();
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
        productsAmountProp.arraySize = numberOfProductsProp.intValue;
        productsProp.isExpanded = EditorGUILayout.Foldout(productsProp.isExpanded, "Output items");
        if (productsProp.isExpanded)
        {
            EditorGUI.indentLevel++;

            for (int i = 0; i < productsProp.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(productsProp.GetArrayElementAtIndex(i), new GUIContent($"Output Item {i}"));
                productsAmountProp.GetArrayElementAtIndex(i).intValue = EditorGUILayout.IntField(productsAmountProp.GetArrayElementAtIndex(i).intValue);
                EditorGUILayout.EndHorizontal();
            }

            if (productsProp.arraySize == 0)
            {
                EditorGUILayout.LabelField("Array lenght is 0");
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Separator();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("id"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("key"));

        SerializedProperty pattern = serializedObject.FindProperty("factors");
        SerializedProperty product = serializedObject.FindProperty("products");

        GUIContent content = new GUIContent();

        EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));

        for (int i = 0; i < pattern.arraySize; i++)
        {
            //patternPos.x += 36;
            var item = pattern.GetArrayElementAtIndex(i).objectReferenceValue as Item;
            if (item == null)
            {
                content.text = "null";
                content.image = null;
            }
            else
            {
                content.text = "";
                content.image = item.sprite.texture;
            }
            EditorGUILayout.LabelField(content, GUILayout.Height(36), GUILayout.Width(36));
            if (i < pattern.arraySize - 1)
                EditorGUILayout.LabelField("+", GUILayout.Height(36), GUILayout.Width(10));
            else
            {
                EditorGUILayout.LabelField("=", GUILayout.Height(36), GUILayout.Width(10));
                for (int j = 0; j < product.arraySize; j++)
                {
                    var productItem = product.GetArrayElementAtIndex(j).objectReferenceValue as Item;
                    if (productItem == null)
                    {
                        content.text = "null";
                        content.image = null;
                    }
                    else
                    {
                        content.text = "";
                        content.image = productItem.sprite.texture;
                    }
                    EditorGUILayout.LabelField(content, GUILayout.Height(36), GUILayout.Width(36));

                }

                EditorGUILayout.SelectableLabel($"id: {serializedObject.FindProperty("id").intValue}, key: {serializedObject.FindProperty("key").stringValue}", GUILayout.Height(36));
            }
        }

        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
