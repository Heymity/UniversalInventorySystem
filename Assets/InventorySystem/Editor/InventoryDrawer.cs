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

            var id = property.FindPropertyRelative("id");
            var key = property.FindPropertyRelative("key");
            var slots = property.FindPropertyRelative("slots");
            var inte = property.FindPropertyRelative("interactiable");

            EditorGUIUtility.wideMode = true;
            EditorGUIUtility.labelWidth = 100;
            position.height = 18;

            EditorGUI.BeginProperty(position, label, property);

            position.width /= 2;
            key.stringValue = EditorGUI.TextField(position, "Key", key.stringValue);
            position.x += position.width;
            id.intValue = EditorGUI.IntField(position, "Id", id.intValue);
            position.x -= position.width;
            position.width *= 2;

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

            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(InventoryReference))]
    public class InventoryReferenceDrawer : PropertyDrawer
    {
        public string[] options = new string[] { "Use Value", "Use Reference" };
        public int index = 0;
        private GUIStyle popupStyle;
        private float fields = 0;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 18f + fields;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            if (popupStyle == null)
            {
                var menu = (Texture2D)Resources.Load("Menu");
                popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
                popupStyle.imagePosition = ImagePosition.ImageOnly;
                popupStyle.normal.background = menu;
            }

            var useC = property.FindPropertyRelative("useConstant");

            if (useC.boolValue) index = 0;
            else index = 1;

            var tmp = position.width;
            position.width = 20;
            var tmph = position.height;
            position.height = 18;
            index = EditorGUI.Popup(position, index, options, popupStyle);
            position.x += 20;
            position.width = tmp - 20;
            position.height = tmph;

            var tmpr = position;
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            if (index == 0)
            {
                position = tmpr;
                position.y += 18;
                useC.boolValue = true;
                var prop = property.FindPropertyRelative("constantValue");
                EditorGUI.PropertyField(position, prop);
                fields = EditorGUI.GetPropertyHeight(prop);
            }
            else
            {
                useC.boolValue = false;
                EditorGUI.ObjectField(position, property.FindPropertyRelative("variable"), new GUIContent(""));
                fields = 0;
            }
        }
    }
}