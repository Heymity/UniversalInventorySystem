using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using UniversalInventorySystem;

[CustomPropertyDrawer(typeof(DurabilityImage))]
public class DurabilityItemDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 18f * 2;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.width /= 3;

        EditorGUIUtility.wideMode = true;
        EditorGUIUtility.labelWidth = 240;
        position.height /= 2;

        EditorGUI.LabelField(position, "Durability Trigger");
        position.y += position.height;

        var durabilityProp = property.FindPropertyRelative("durability");
        durabilityProp.intValue = EditorGUI.IntField(position, durabilityProp.intValue);
        position.y -= position.height;
        position.x += position.width;

        EditorGUI.LabelField(position, "Image Name");
        position.y += position.height;

        var nameProp = property.FindPropertyRelative("imageName");
        nameProp.stringValue = EditorGUI.TextField(position, nameProp.stringValue);
        position.y -= position.height;
        position.x += position.width;

        EditorGUI.LabelField(position, "Image");
        position.y += position.height;

        var spriteProp = property.FindPropertyRelative("sprite");
        EditorGUI.ObjectField(position, spriteProp, new GUIContent());
        position.y -= position.height;
        position.x += position.width;

    }
}
