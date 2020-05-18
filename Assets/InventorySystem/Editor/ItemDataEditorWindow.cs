using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemDataEditorWindow : ExtendEditorWindow
{
    public static void Open(ItemData dataObject)
    {
        ItemDataEditorWindow window = GetWindow<ItemDataEditorWindow>("Item Data Editor");
        window.serializedObject = new SerializedObject(dataObject);
    }

    private void OnGUI()
    {
        currentProperty = serializedObject.FindProperty("itemsList");
        
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true));

        DrawSidebar(currentProperty);

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
        if (selectedProperty != null)
        {

            EditorGUILayout.PropertyField(selectedProperty, true);
            //DrawProperties(selectedProperty, true);
        }
        else
        {
            EditorGUILayout.LabelField("Select an item from the list");
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        
    }
}
