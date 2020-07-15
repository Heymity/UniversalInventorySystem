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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UniversalInventorySystem
{
    [CustomEditor(typeof(Inventory))]
    public class InventoryInspector : Editor
    {
        SerializedProperty slotsProp;
        SerializedProperty slotAmountProp;
        SerializedProperty idProp;
        SerializedProperty interactiableProp;

        private void OnEnable()
        {
            slotsProp = serializedObject.FindProperty("_slots");
            slotAmountProp = serializedObject.FindProperty("_slotAmounts");
            idProp = serializedObject.FindProperty("_id");
            interactiableProp = serializedObject.FindProperty("_interactiable");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUIUtility.wideMode = true;
            EditorGUIUtility.labelWidth = 100;

            idProp.intValue = EditorGUILayout.IntField("Id", idProp.intValue);

            EditorGUILayout.PropertyField(interactiableProp);

            EditorGUILayout.BeginVertical();

            slotsProp.isExpanded = EditorGUILayout.Foldout(slotsProp.isExpanded, "Slots");

            if (slotsProp.isExpanded)
            {
                EditorGUI.indentLevel++;

                var tmp = slotsProp.arraySize;
                if (slotAmountProp.intValue != slotsProp.arraySize) slotAmountProp.intValue = slotsProp.arraySize;
                slotAmountProp.intValue = EditorGUILayout.IntField("Size", slotAmountProp.intValue);
                if (slotAmountProp.intValue < 0) slotAmountProp.intValue = 0;
                slotsProp.arraySize = slotAmountProp.intValue;

                for (int i = 0;i < slotsProp.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(slotsProp.GetArrayElementAtIndex(i));
                    if (i > tmp - 1)
                    {
                        slotsProp.GetArrayElementAtIndex(i).FindPropertyRelative("interative").intValue = -1;
                    }
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
