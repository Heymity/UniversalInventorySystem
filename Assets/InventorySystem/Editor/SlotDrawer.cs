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
 
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UniversalInventorySystem.Editors
{
    [CustomPropertyDrawer(typeof(Slot))]
    public class SlotDrawer : PropertyDrawer
    {
        public Dictionary<string, SlotInfo> unfold = new Dictionary<string, SlotInfo>();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (unfold.ContainsKey(property.propertyPath))
                return unfold[property.propertyPath].boolValue ? 18 * unfold[property.propertyPath].fieldAmount : 18;
            else
                return 18;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position.height = 18f;
            var t = label.text.Split(' ');
            label.text = "Slot " + t[t.Length - 1];

            if (!unfold.ContainsKey(property.propertyPath))
                unfold.Add(property.propertyPath, new SlotInfo(false, 1, false, 0, false, false));

            var originalPosition = position;
            position = EditorGUI.PrefixLabel(position, label);

            originalPosition.width = position.x - 20;
            unfold[property.propertyPath].boolValue = EditorGUI.Foldout(originalPosition, unfold[property.propertyPath].boolValue, label, true);

            if (unfold[property.propertyPath].boolValue)
            {
                Rect ampos = new Rect(originalPosition.x, originalPosition.y + 18f, 100, 18);
                unfold[property.propertyPath].fieldAmount = 2.5f;

                var durRect = ampos;
                durRect.width = position.width;
                var durability = property.FindPropertyRelative("_durability");
                var hasItem = property.FindPropertyRelative("hasItem");
                var slotItem = property.FindPropertyRelative("item");
                if (hasItem.boolValue && slotItem.FindPropertyRelative("hasDurability").boolValue)
                {
                    unfold[property.propertyPath].fieldAmount = 3.5f;
                    EditorGUI.IntSlider(durRect, durability, 0, slotItem.FindPropertyRelative("durability").intValue, "Durability");
                    ampos.y += ampos.height;
                }

                var whitelistProp = property.FindPropertyRelative("whitelist");

                unfold[property.propertyPath].objs = new List<Object>();
                unfold[property.propertyPath].editorAssignSize = 1;
                var objRect = new Rect(ampos.x + 120, ampos.y, position.width - 140, ampos.height);
                EditorGUI.ObjectField(objRect, whitelistProp, new GUIContent("Whitelist"));    
            }
            else
            {
                unfold[property.propertyPath].fieldAmount = 1;
            }

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var labelHIRect = new Rect(position.x + 15, position.y, 55, position.height);
            var labelAmRect = new Rect(position.x + 80 + (position.width / 2) - 135, position.y, 70, position.height);
            var labelInteracts = new Rect(position.x + 190 + (position.width / 2) - 130, position.y, 70, position.height);

            var hasItemRect = new Rect(position.x, position.y, 10, position.height);
            var nameRect = new Rect(position.x + 75, position.y, (position.width / 2) - 130, position.height);
            var amountRect = new Rect(position.x + 135 + (position.width / 2) - 130, position.y, 50, position.height);
            var interactsRect = new Rect(position.x + 245 + (position.width / 2) - 130, position.y, (position.width / 2) - 130, position.height);

            var hasItemProp = property.FindPropertyRelative("hasItem");
            var itemProp = property.FindPropertyRelative("itemValue");
            var variable = itemProp.FindPropertyRelative("variable");
            var constantValue = itemProp.FindPropertyRelative("constantValue");

            //hasItemProp.boolValue = variable.objectReferenceValue == null && constantValue == null;
            hasItemRect.height = 17;
            hasItemRect.width = 17;
            if (hasItemProp.boolValue) EditorGUI.LabelField(hasItemRect, EditorGUIUtility.IconContent("TestPassed"));
            else EditorGUI.LabelField(hasItemRect, EditorGUIUtility.IconContent("TestFailed"));

            EditorGUI.LabelField(labelHIRect, new GUIContent(" Has item"));
            EditorGUI.PropertyField(nameRect, itemProp, GUIContent.none);
            EditorGUI.LabelField(labelAmRect, new GUIContent("In Amount"));
            EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("amount"), GUIContent.none);//interative
            EditorGUI.LabelField(labelInteracts, new GUIContent("Interacts"));
            EditorGUI.PropertyField(interactsRect, property.FindPropertyRelative("interative"), GUIContent.none);

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        public class SlotInfo
        {
            public bool boolValue;
            public bool iasExpand;
            public bool multipleAssign;
            public bool addNullMatch;
            public float fieldAmount;
            public int editorAssignSize;
            public List<Object> objs;
            public bool executeOnce;

            public SlotInfo(bool _boolValues, int _fieldAmount, bool _iasExpand, int _editorAssignSize, bool _multipleAssign, bool _addNullMatch)
            {
                boolValue = _boolValues;
                fieldAmount = _fieldAmount;
                iasExpand = _iasExpand;
                editorAssignSize = _editorAssignSize;
                multipleAssign = _multipleAssign;
                addNullMatch = _addNullMatch;
            }
        }
    }
}
