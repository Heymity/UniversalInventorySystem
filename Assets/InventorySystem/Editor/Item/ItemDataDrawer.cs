using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UniversalInventorySystem;

[CustomPropertyDrawer(typeof(ItemGroup))]
public class ItemDataDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 18f * 1;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property);

        var buttonRect = new Rect(position.x, position.y, 170, position.height);
        if(property.objectReferenceValue != null)
        {
            if (GUI.Button(buttonRect, new GUIContent("Open Editor of " + label.text)))
            {
                ItemDataEditorWindow.Open(property.objectReferenceValue as ItemGroup);
            }
        }
        
        //base.OnGUI(position, property, label);
    }
}
