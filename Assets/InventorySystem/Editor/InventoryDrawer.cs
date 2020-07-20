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
    [CustomPropertyDrawer(typeof(Inventory))]
    public class InventoryDrawer : PropertyDrawer
    {
        float baseAmount = 3f;
        float amountOfFilds = 3f;
        bool useObjValues;
        float total = 0;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 18f * amountOfFilds;
        }
        bool unfold = false;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            amountOfFilds = baseAmount + total;
            total = 0;
            SerializedObject serializedObject = null;
            if (property.objectReferenceValue != null) serializedObject = new SerializedObject(property.objectReferenceValue);
            if (serializedObject != null)
            {
                serializedObject.Update();
                var id = serializedObject.FindProperty("_id");
                var key = serializedObject.FindProperty("_key");
                var slots = serializedObject.FindProperty("inventorySlots");
                var inte = serializedObject.FindProperty("_interactiable");

                EditorGUIUtility.wideMode = true;
                EditorGUIUtility.labelWidth = 100;
                position.height /= amountOfFilds;

                //position.y += position.height;

                EditorGUI.BeginProperty(position, label, property);

                EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent($"{key.stringValue} (id: {id.intValue})"));
                var tmpRect = position;
                position.x += 110;
                position.width = 140;
                if (GUI.Button(position, useObjValues ? "Edit Values" : "Use Object Value")) useObjValues = !useObjValues;
                position = tmpRect;

                if (!useObjValues)
                {
                    position.y += 18;
                    baseAmount = 3f;

                    EditorGUI.PropertyField(position, inte);

                    position.y += position.height;

                    var foldRect = new Rect(position.x, position.y, 50, position.height);
                    unfold = EditorGUI.Foldout(foldRect, unfold, new GUIContent("Slots"), true);

                    var slotAmountsRect = new Rect(position.x + 50, position.y, position.width - 150, position.height);
                    var tmpSize = EditorGUI.IntField(slotAmountsRect, new GUIContent("Amount of slots"), slots.arraySize);
                    if (tmpSize < 0) tmpSize = 0;

                    position.y += position.height;

                    var tmp = slots.arraySize;
                    slots.arraySize = tmpSize >= 0 ? tmpSize : slots.arraySize;
                    if (unfold)
                    {
                        EditorGUI.indentLevel++;
                        //amountOfFilds = baseAmount + 1;
                        //position.y += position.height;
                        if (slots != null)
                        {
                            if (slots.arraySize > 0)
                            {
                                float childHeight = 0;
                                for (int i = 0; i < slots.arraySize; i++)
                                {
                                    childHeight += EditorGUI.GetPropertyHeight(slots.GetArrayElementAtIndex(i)) / 18;
                                    EditorGUI.PropertyField(position, slots.GetArrayElementAtIndex(i));
                                    if (i > tmp - 1)
                                    {
                                        slots.GetArrayElementAtIndex(i).FindPropertyRelative("interative").intValue = -1;
                                    }
                                    position.y += EditorGUI.GetPropertyHeight(slots.GetArrayElementAtIndex(i));
                                }
                                total += childHeight;
                            }
                        }
                        EditorGUI.indentLevel--;
                    }
                    else
                    {
                        if (slots != null)
                        {
                            if (slots.arraySize > 0)
                            {
                                for (int i = tmp - 1; i < slots.arraySize; i++)
                                {
                                    if (i > tmp - 1)
                                    {
                                        slots.GetArrayElementAtIndex(i).FindPropertyRelative("interative").intValue = -1;
                                    }
                                }
                            }
                        }
                        amountOfFilds = baseAmount;
                    }
                }
                else
                {
                    baseAmount = 1f;
                    position.width -= 260;
                    position.x += 260;
                    EditorGUI.ObjectField(position, property, new GUIContent(""));
                }
                serializedObject.ApplyModifiedProperties();
                EditorGUI.EndProperty();
            }
            else
            {
                amountOfFilds = 1;
                baseAmount = 1;
                EditorGUI.ObjectField(position, property);
                if (property.objectReferenceValue != null)
                {
                    serializedObject = new SerializedObject(property.objectReferenceValue);
                    serializedObject.ApplyModifiedProperties();
                    useObjValues = true;
                }
            }
        }
    }
}