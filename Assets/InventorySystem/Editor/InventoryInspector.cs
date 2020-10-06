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

namespace UniversalInventorySystem.Editors
{
    [CustomEditor(typeof(InventoryVarialbe)), CanEditMultipleObjects]
    public class InventoryInspector : Editor
    {
        SerializedProperty inventoryProp;
        SerializedProperty _inventoryProp;

        bool edit = false;
        AnimBool showSlots;
        AnimBool showSeeds;
        AnimBool showRuntimeSlots;

        private void OnEnable()
        {
            inventoryProp = serializedObject.FindProperty("value");
            _inventoryProp = serializedObject.FindProperty("_value");

            showSlots = new AnimBool(true);
            showRuntimeSlots = new AnimBool(true);
            showSeeds = new AnimBool(true);
            showSlots.valueChanged.AddListener(Repaint);
            showRuntimeSlots.valueChanged.AddListener(Repaint);
            showSeeds.valueChanged.AddListener(Repaint);
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

                var runtimeSlotsProp = inventoryProp.FindPropertyRelative("slots");
                var runtimeIdProp = inventoryProp.FindPropertyRelative("id");
                var runtimeInteractiableProp = inventoryProp.FindPropertyRelative("interactiable");
                var runtimeKeyProp = inventoryProp.FindPropertyRelative("key");

                runtimeIdProp.intValue = EditorGUILayout.IntField("Id", runtimeIdProp.intValue);
                runtimeKeyProp.stringValue = EditorGUILayout.TextField("Key", runtimeKeyProp.stringValue);

                EditorGUILayout.PropertyField(runtimeInteractiableProp);

                EditorGUILayout.BeginVertical();

                runtimeSlotsProp.isExpanded = EditorGUILayout.Foldout(runtimeSlotsProp.isExpanded, "Slots");
                showRuntimeSlots.target = runtimeSlotsProp.isExpanded;

                if (EditorGUILayout.BeginFadeGroup(showRuntimeSlots.faded))
                {
                    EditorGUI.indentLevel++;

                    var tmp = EditorGUILayout.IntField("Size", runtimeSlotsProp.arraySize);
                    if (tmp >= 0) runtimeSlotsProp.arraySize = tmp;

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

            var idProp = _inventoryProp.FindPropertyRelative("id");
            var keyProp = _inventoryProp.FindPropertyRelative("key");
            var interactiableProp = _inventoryProp.FindPropertyRelative("interactiable");
            var slotsProp = _inventoryProp.FindPropertyRelative("slots");
            var seedsProp = _inventoryProp.FindPropertyRelative("seeds");

            idProp.intValue = EditorGUILayout.IntField("Id", idProp.intValue);
            keyProp.stringValue = EditorGUILayout.TextField("Key", keyProp.stringValue);

            EditorGUILayout.PropertyField(interactiableProp);

            EditorGUILayout.BeginVertical();

            slotsProp.isExpanded = EditorGUILayout.Foldout(slotsProp.isExpanded, "Slots");
            showSlots.target = slotsProp.isExpanded;
            
            if (EditorGUILayout.BeginFadeGroup(showSlots.faded))
            {
                EditorGUI.indentLevel++;

                var tmpsize = EditorGUILayout.IntField("Size", slotsProp.arraySize);
                if (tmpsize >= 0) slotsProp.arraySize = tmpsize;

                for (int i = 0;i < slotsProp.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(slotsProp.GetArrayElementAtIndex(i));
                    if (i > tmpsize - 1)
                    {
                        slotsProp.GetArrayElementAtIndex(i).FindPropertyRelative("interative").intValue = -1;
                    }
                }

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFadeGroup();

            seedsProp.isExpanded = EditorGUILayout.Foldout(seedsProp.isExpanded, "Seeds");
            showSeeds.target = seedsProp.isExpanded;


            if (EditorGUILayout.BeginFadeGroup(showSeeds.faded))
            {
                EditorGUI.indentLevel++;
                var tmpseed = EditorGUILayout.IntField("Size", seedsProp.arraySize);
                if (tmpseed >= 0) seedsProp.arraySize = tmpseed;

                for (int i = 0; i < seedsProp.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(seedsProp.GetArrayElementAtIndex(i));
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
