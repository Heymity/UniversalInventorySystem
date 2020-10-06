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
    public class ItemDataEditorWindow : ExtendEditorWindow
    {
        Vector2 sideBar;
        Vector2 content;

        public static void Open(ItemDatabase dataObject)
        {
            ItemDataEditorWindow window = GetWindow<ItemDataEditorWindow>("Item Group Editor");
            window.serializedObject = new SerializedObject(dataObject);
        }

        //bool confirmButton;
        private void OnGUI()
        {
            if (serializedObject == null) return;
            currentProperty = serializedObject.FindProperty("itemsList");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true));

            sideBar = EditorGUILayout.BeginScrollView(sideBar);
            DrawSidebar(currentProperty);
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
            if (selectedProperty != null)
            {
                content = EditorGUILayout.BeginScrollView(content);
                if (!showItemAssets) EditorGUILayout.PropertyField(selectedProperty, true);
                else
                {
                    serializedObject.FindProperty("id").intValue = EditorGUILayout.IntField(new GUIContent("Item Asset id"), serializedObject.FindProperty("id").intValue);
                    serializedObject.FindProperty("strId").stringValue = EditorGUILayout.TextField(new GUIContent("Item Asset key"), serializedObject.FindProperty("strId").stringValue);
                }
                /*if (GUILayout.Button("Remove Item", GUILayout.Width(150), GUILayout.Height(20)))
                {
                    confirmButton = !confirmButton;
                }
                if(confirmButton)
                {
                    if (GUILayout.Button("Confirm", GUILayout.Width(150), GUILayout.Height(20)))
                    {
                        for(int i = 0;i < currentProperty.arraySize;i++)
                        {
                            if(currentProperty.GetArrayElementAtIndex(i) == selectedProperty)
                            {
                                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(selectedProperty.objectReferenceInstanceIDValue));
                                List<SerializedProperty> props = new List<SerializedProperty>();

                                currentProperty.DeleteArrayElementAtIndex(i);

                                for(int j = 0;j < currentProperty.arraySize;j++) 
                                {
                                    if(currentProperty.GetArrayElementAtIndex(j) != null)
                                    {
                                        props.Add(currentProperty.GetArrayElementAtIndex(j));
                                    }
                                }

                            }
                        }
                    }
                    if (GUILayout.Button("Cancel", GUILayout.Width(150), GUILayout.Height(20)))
                    {
                        confirmButton = false;
                    }
                }
                //DrawProperties(selectedProperty, true);*/
                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.LabelField("Select an item from the list");
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();

        }
    }
}