using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PatternRecipe))]
public class PatternRecipeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open Editor"))
        {
            PatternRecipeEditorWindow.Open((PatternRecipe)target);
        }

        var amount = (serializedObject.FindProperty("gridSize").vector2IntValue.y * 2) + 1;
        SerializedProperty pattern = serializedObject.FindProperty("pattern");
        Vector2Int gridSize = serializedObject.FindProperty("gridSize").vector2IntValue;
        
        GUIContent content = new GUIContent();

        EditorGUILayout.BeginVertical("box");
        
        for (int i = 0; i < gridSize.y; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < gridSize.x; j++)
            {
                //(pattern.GetArrayElementAtIndex(i * gridSize.y + j).objectReferenceValue as Item).sprite.texture
                var item = pattern.GetArrayElementAtIndex(i * gridSize.x + j).objectReferenceValue as Item;
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

                EditorGUILayout.LabelField(content, GUILayout.Width(36),GUILayout.Height(36));
               
            }
             EditorGUILayout.EndHorizontal();

        }


        SerializedProperty products = serializedObject.FindProperty("products");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("=", GUILayout.Width(10), GUILayout.Height(36));
       
        for (int i = 0; i < products.arraySize; i++)
        {
            var productItem = products.GetArrayElementAtIndex(i).objectReferenceValue as Item;
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
            EditorGUILayout.LabelField(content, GUILayout.Width(36), GUILayout.Height(36));

        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.SelectableLabel($"id: {serializedObject.FindProperty("id").intValue}, key: {serializedObject.FindProperty("key").stringValue}");
        EditorGUILayout.EndVertical();
    }
}
