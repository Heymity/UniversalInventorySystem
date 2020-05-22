using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityScript.Scripting.Pipeline;

[CustomPropertyDrawer(typeof(Recipe))]
public class RecipeDrawer : PropertyDrawer
{
    bool showRecipe = false;
    int baseAmount = 1;
    int amount = 1;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 18f * amount;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUIUtility.wideMode = true;

        position.height /= amount;

        EditorGUI.PropertyField(position, property);

        var buttonRect = new Rect(position.x, position.y, 165, position.height);
        if (property.objectReferenceValue != null)
        {
            buttonRect.x += 165;
            buttonRect.width = 85;
            if (GUI.Button(buttonRect, new GUIContent("Show Recipe")))
            {
                showRecipe = !showRecipe;
            }

            SerializedObject serializedObject = null;
            if (property.objectReferenceValue != null) serializedObject = new SerializedObject(property.objectReferenceValue);
            if (serializedObject != null)
            {
                position.y += position.height;
                if (showRecipe)
                {
                    amount = 3;
                    SerializedProperty pattern = serializedObject.FindProperty("factors");
                    SerializedProperty product = serializedObject.FindProperty("products");
                    Rect patternPos = new Rect(position.x, position.y, position.width, position.height + 18);

                    //(pattern.GetArrayElementAtIndex(i * gridSize.y + j).objectReferenceValue as Item).sprite.texture
                    GUIContent content = new GUIContent();
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
                        EditorGUI.LabelField(patternPos, content);
                        patternPos.x += 36;
                        if(i < pattern.arraySize - 1)
                            EditorGUI.LabelField(patternPos, "+");
                        else
                        {
                            EditorGUI.LabelField(patternPos, "=");
                            patternPos.x += 5;
                            for (int j = 0;j < product.arraySize; j++)
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
                                EditorGUI.LabelField(patternPos, content);
                                patternPos.x += 36;
                            }
                            patternPos.x += 36;
                            EditorGUI.SelectableLabel(patternPos, $"id: {serializedObject.FindProperty("id").intValue}, key: {serializedObject.FindProperty("key").stringValue}");
                        }
                    }                  
                }
                else amount = baseAmount;
            }
        }

        //base.OnGUI(position, property, label);
    }
}
