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
using UnityEditor.Callbacks;

namespace UniversalInventorySystem.Editors
{
    public class AssetHandler
    {
        [OnOpenAsset()]
        public static bool OpenEditor(int instanceId, int line)
        {
            ItemDatabase obj = EditorUtility.InstanceIDToObject(instanceId) as ItemDatabase;
            if (obj != null)
            {
                ItemDataEditorWindow.Open(obj);
                return true;
            }
            return false;

        }
    }



    [CustomEditor(typeof(ItemDatabase))]
    public class ItemDataEditor : Editor
    {
        SerializedProperty itemsListProp;
        SerializedProperty strIdProp;
        SerializedProperty idProp;

        private void OnEnable()
        {
            itemsListProp = serializedObject.FindProperty("itemsList");
            strIdProp = serializedObject.FindProperty("strId");
            idProp = serializedObject.FindProperty("id");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            itemsListProp.isExpanded = EditorGUILayout.Foldout(itemsListProp.isExpanded, new GUIContent("Items list"), true);
            if (itemsListProp.isExpanded)
            {
                EditorGUI.indentLevel++;
                itemsListProp.arraySize = EditorGUILayout.IntField(new GUIContent("Size"), itemsListProp.arraySize);
                for (int i = 0; i < itemsListProp.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(itemsListProp.GetArrayElementAtIndex(i), new GUIContent($"Item {i}"));
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(strIdProp);

            EditorGUILayout.PropertyField(idProp);

            if (GUILayout.Button("Open Editor"))
            {
                ItemDataEditorWindow.Open((ItemDatabase)target);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}