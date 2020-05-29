using UnityEditor;
using UnityEngine;
using UniversalInventorySystem;

[CustomPropertyDrawer(typeof(Slot))]
public class SlotDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var t = label.text.Split(' ');
        label.text = "Slot " + t[t.Length - 1];

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var labelHIRect = new Rect(position.x + 15, position.y, 50, position.height);
        var labelAmRect = new Rect(position.x + 75 + (position.width / 2) - 130, position.y, 70, position.height);
        var labelInteracts = new Rect(position.x + 190 + (position.width / 2) - 130, position.y, 70, position.height);

        var hasItemRect = new Rect(position.x, position.y, 10, position.height);
        var nameRect = new Rect(position.x + 70, position.y, (position.width / 2) - 130, position.height);
        var amountRect = new Rect(position.x + 135 + (position.width / 2) - 130, position.y, 50, position.height);
        var interactsRect = new Rect(position.x + 245 + (position.width / 2) - 130, position.y, (position.width / 2) - 130, position.height);

        EditorGUI.PropertyField(hasItemRect, property.FindPropertyRelative("hasItem"), GUIContent.none);
        EditorGUI.LabelField(labelHIRect, new GUIContent("Has item"));
        EditorGUI.ObjectField(nameRect, property.FindPropertyRelative("item"), GUIContent.none);
        EditorGUI.LabelField(labelAmRect, new GUIContent("In Amount"));
        EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("amount"), GUIContent.none);//interative
        EditorGUI.LabelField(labelInteracts, new GUIContent("Interacts"));
        EditorGUI.PropertyField(interactsRect, property.FindPropertyRelative("interative"), GUIContent.none);

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}