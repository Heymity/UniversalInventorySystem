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
using UnityEditor.AnimatedValues;

namespace UniversalInventorySystem
{
    [CustomEditor(typeof(Inventory))]
    public class InventoryInspector : Editor
    {
        SerializedProperty slotsProp;
        SerializedProperty runtimeSlotsProp;
        SerializedProperty slotAmountProp;
        SerializedProperty runtimeSlotAmountProp;
        SerializedProperty idProp;
        SerializedProperty runtimeIdProp;
        SerializedProperty interactiableProp;
        SerializedProperty runtimeInteractiableProp;

        bool edit = false;
        AnimBool showSlots;
        AnimBool showRuntimeSlots;

        private void OnEnable()
        {
            slotsProp = serializedObject.FindProperty("inventorySlots");
            slotAmountProp = serializedObject.FindProperty("_slotAmounts");
            idProp = serializedObject.FindProperty("_id");
            interactiableProp = serializedObject.FindProperty("_interactiable");
            showSlots = new AnimBool(true);
            showRuntimeSlots = new AnimBool(true);
            showSlots.valueChanged.AddListener(Repaint);
            showRuntimeSlots.valueChanged.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUIUtility.wideMode = true;
            EditorGUIUtility.labelWidth = 100;

            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Here is where you change runtime values", MessageType.None);
                EditorGUILayout.LabelField("Runtime Values: ");

                runtimeSlotsProp = serializedObject.FindProperty("slots");
                runtimeSlotAmountProp = serializedObject.FindProperty("slotAmounts");
                runtimeIdProp = serializedObject.FindProperty("id");
                runtimeInteractiableProp = serializedObject.FindProperty("interactiable");

                runtimeIdProp.intValue = EditorGUILayout.IntField("Id", runtimeIdProp.intValue);

                EditorGUILayout.PropertyField(runtimeInteractiableProp);

                EditorGUILayout.BeginVertical();

                runtimeSlotsProp.isExpanded = EditorGUILayout.Foldout(runtimeSlotsProp.isExpanded, "Slots");
                showRuntimeSlots.target = runtimeSlotsProp.isExpanded;

                if (EditorGUILayout.BeginFadeGroup(showRuntimeSlots.faded))
                {
                    EditorGUI.indentLevel++;

                    var tmp = runtimeSlotsProp.arraySize;
                    if (runtimeSlotAmountProp.intValue != runtimeSlotsProp.arraySize) runtimeSlotAmountProp.intValue = runtimeSlotsProp.arraySize;
                    slotAmountProp.intValue = EditorGUILayout.IntField("Size", runtimeSlotAmountProp.intValue);
                    if (runtimeSlotAmountProp.intValue < 0) slotAmountProp.intValue = 0;
                    runtimeSlotsProp.arraySize = runtimeSlotAmountProp.intValue;

                    for (int i = 0; i < runtimeSlotsProp.arraySize; i++)
                    {
                        EditorGUILayout.PropertyField(runtimeSlotsProp.GetArrayElementAtIndex(i));
                        if (i > tmp - 1)
                        {
                            runtimeSlotsProp.GetArrayElementAtIndex(i).FindPropertyRelative("interative").intValue = -1;
                        }
                    }

                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndFadeGroup();

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();

                EditorGUILayout.HelpBox("The values below if changed will override the instance ones and WILL be saved after exiting play mode. If you want change only runtime values, change the ones Before this", MessageType.Info);
                edit = EditorGUILayout.Toggle("Edit Editor Values", edit);
            }

            EditorGUI.BeginDisabledGroup(!edit && Application.isPlaying);

            idProp.intValue = EditorGUILayout.IntField("Id", idProp.intValue);

            EditorGUILayout.PropertyField(interactiableProp);

            EditorGUILayout.BeginVertical();

            slotsProp.isExpanded = EditorGUILayout.Foldout(slotsProp.isExpanded, "Slots");
            showSlots.target = slotsProp.isExpanded;
            
            if (EditorGUILayout.BeginFadeGroup(showSlots.faded))
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
            EditorGUILayout.EndFadeGroup();

            EditorGUILayout.EndVertical();

            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
