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
using UniversalInventorySystem;

public class ExtendEditorWindow : EditorWindow
{
    protected SerializedObject serializedObject;
    protected SerializedProperty currentProperty;

    private string selectedPropertyPath;
    protected SerializedProperty selectedProperty;

    protected bool showItemAssets;

    protected void DrawProperties(SerializedProperty prop, bool drawChildren)
    {
        string lastPropPath = string.Empty;
        foreach (SerializedProperty p in prop)
        {
            if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
            {
                EditorGUILayout.BeginHorizontal();
                p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                EditorGUILayout.EndHorizontal();

                if (p.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    DrawProperties(p, drawChildren);
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                if(!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) { continue; }
                lastPropPath = p.propertyPath;
                EditorGUILayout.PropertyField(p, drawChildren);
            }
        }
    }

    protected void DrawSidebar(SerializedProperty prop)
    {
        if (GUILayout.Button("Item Asset"))
        {
            selectedPropertyPath = prop.propertyPath;
            showItemAssets = true;
        }

        foreach (SerializedProperty p in prop)
        {
            if (GUILayout.Button((p.objectReferenceValue as Item).name))
            {
                showItemAssets = false;
                selectedPropertyPath = p.propertyPath;
            }           
        }

        if(GUILayout.Button("Add new item"))
        {
            CreateScriptableObjectAsset window = GetWindow<CreateScriptableObjectAsset>("Create Item");
            window.property = prop;

        }

        if (!string.IsNullOrEmpty(selectedPropertyPath))
        {
            selectedProperty = serializedObject.FindProperty(selectedPropertyPath);
        }
    }
}

public class CreateScriptableObjectAsset : EditorWindow
{
    public SerializedProperty property;
    public static Item CreateItemAsset(string path, string iname, SerializedProperty prop)
    {
        Item asset = ScriptableObject.CreateInstance<Item>();

        path += $"/{iname}.asset";

        AssetDatabase.CreateAsset(asset, path);
   
        AssetDatabase.SaveAssets();

        prop.arraySize++;
        prop.GetArrayElementAtIndex(prop.arraySize - 1).objectReferenceValue = asset;

        return asset;
    }

    string itemName;
    private void OnGUI()
    {
        EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));

        string path = EditorPrefs.GetString("newItemPath", "Assets");
        path = EditorGUILayout.TextField("Path to save", path);
        EditorPrefs.SetString("newItemPath", path);

        if (string.IsNullOrEmpty(itemName)) itemName = "Untitled";
        itemName = EditorGUILayout.TextField("Name of the item", itemName);

        if(GUILayout.Button("Create Item Asset"))
        {
            CreateItemAsset(path, itemName, property);
        }

        EditorGUILayout.EndVertical();
    }
}
