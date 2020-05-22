using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityScript.Scripting.Pipeline;

[CustomPropertyDrawer(typeof(PatternRecipe))]
public class PatternRecipeDrawer : PropertyDrawer
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
            if (GUI.Button(buttonRect, new GUIContent("Open Editor of " + label.text)))
            {
                PatternRecipeEditorWindow.Open(property.objectReferenceValue as PatternRecipe);
            }
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
                if (showRecipe)
                {
                    amount = (serializedObject.FindProperty("gridSize").vector2IntValue.y * 2) + 1;
                    SerializedProperty pattern = serializedObject.FindProperty("pattern");
                    Vector2Int gridSize = serializedObject.FindProperty("gridSize").vector2IntValue;
                    position.y += position.height;
                    Rect patternPos = new Rect(position.x, position.y, position.width, position.height + 18);
                    for (int i = 0;i < gridSize.y; i++)
                    {
                        for (int j = 0; j < gridSize.x; j++)
                        {
                            //(pattern.GetArrayElementAtIndex(i * gridSize.y + j).objectReferenceValue as Item).sprite.texture
                            GUIContent content = new GUIContent();
                            var item = pattern.GetArrayElementAtIndex(i * gridSize.y + j).objectReferenceValue as Item;
                            if (item == null) 
                            {
                                content.text = "null";
                                content.image = null;
                            } else
                            {
                                content.text = "";
                                content.image = item.sprite.texture;
                            }

                            EditorGUI.LabelField(patternPos, content);
                            patternPos.x += 36;
                        }
                        patternPos.y += patternPos.height;
                        patternPos.x -= gridSize.x * 36;
                    }
                }
                else amount = baseAmount;
            }
        }

        //base.OnGUI(position, property, label);
    }
}
