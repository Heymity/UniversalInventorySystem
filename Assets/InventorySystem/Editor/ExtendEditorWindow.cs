using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
            if (GUILayout.Button("Item " + p.displayName.TrimStart('E', 'l', 'e', 'm', 'e', 'n', 't')))
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
