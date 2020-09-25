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

using UnityEditor;
using UnityEngine;

namespace UniversalInventorySystem.Editors
{
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
            /*EditorGUIUtility.wideMode = true;

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
                        SerializedProperty factorsAmount = serializedObject.FindProperty("amountFactors");
                        SerializedProperty productsAmount = serializedObject.FindProperty("amountProducts");
                        Rect patternPos = new Rect(position.x, position.y, position.width, position.height + 18);

                        //(pattern.GetArrayElementAtIndex(i * gridSize.y + j).objectReferenceValue as Item).sprite.texture
                        GUIContent content = new GUIContent();
                        for (int i = 0; i < pattern.arraySize; i++)
                        {
                            //patternPos.x += 36;
                            var item = pattern.GetArrayElementAtIndex(i).objectReferenceValue as Item;
                            var fAmount = factorsAmount.GetArrayElementAtIndex(i).intValue;
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
                            GUIContent secondContent = new GUIContent();
                            secondContent.text = fAmount == 0 ? "" : fAmount.ToString();
                            EditorGUI.LabelField(patternPos, secondContent);
                            patternPos.x += 36;
                            if (i < pattern.arraySize - 1)
                            {
                                EditorGUI.LabelField(patternPos, "+");
                                patternPos.x += 16;
                            }
                            else
                            {
                                EditorGUI.LabelField(patternPos, "=");
                                patternPos.x += 15;
                                for (int j = 0; j < product.arraySize; j++)
                                {
                                    var productItem = product.GetArrayElementAtIndex(j).objectReferenceValue as Item;
                                    var pAmount = productsAmount.GetArrayElementAtIndex(j).intValue;
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
                                    patternPos.x -= 5;
                                    GUIContent prodContent = new GUIContent();
                                    prodContent.text = pAmount == 0 ? "" : pAmount.ToString();
                                    EditorGUI.LabelField(patternPos, prodContent);
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
            */
            base.OnGUI(position, property, label);
        }
    }
}