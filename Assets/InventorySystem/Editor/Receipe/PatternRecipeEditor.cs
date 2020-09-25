/*  Copyright 2020 Gabriel Pasquale Rodrigues Scavone
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 * 
 *  
 *  This is an Editor Script, it is responsible for drawing the inpector or drawer of the class in the attribute before the class
 */

using UnityEngine;
using UnityEditor;

namespace UniversalInventorySystem.Editors
{
    [CustomEditor(typeof(PatternRecipe))]
    public class PatternRecipeEditor : Editor
    {
        /**public override void OnInspectorGUI()
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

                    EditorGUILayout.LabelField(content, GUILayout.Width(36), GUILayout.Height(36));

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
        }*/
    }
}