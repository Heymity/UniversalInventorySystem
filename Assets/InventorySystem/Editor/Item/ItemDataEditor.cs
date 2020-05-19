using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class AssetHandler
{
    [OnOpenAsset()]
    public static bool OpenEditor(int instanceId, int line)
    {
        ItemAsset obj = EditorUtility.InstanceIDToObject(instanceId) as ItemAsset;
        if(obj != null)
        {
            ItemDataEditorWindow.Open(obj);
            return true;
        }
        return false;

    }
}


[CustomEditor(typeof(ItemAsset))]
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
            for (int i = 0; i < itemsListProp.arraySize;i++)
            {
                EditorGUILayout.ObjectField(itemsListProp.GetArrayElementAtIndex(i), new GUIContent($"Item {i}"));
            }
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Separator();

        EditorGUILayout.PropertyField(strIdProp);

        EditorGUILayout.PropertyField(idProp);

        if(GUILayout.Button("Open Editor"))
        {
            ItemDataEditorWindow.Open((ItemAsset)target);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
