using UnityEditor;
using UnityEngine;

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
        var labelAmRect = new Rect(position.x + 60 + position.width - 175, position.y, 70, position.height);
        var hasItemRect = new Rect(position.x, position.y, 10, position.height);
        var nameRect = new Rect(position.x + 70, position.y, position.width - 190, position.height);
        var amountRect = new Rect(position.x + 125 + position.width - 175, position.y, 50, position.height);

        EditorGUI.PropertyField(hasItemRect, property.FindPropertyRelative("hasItem"), GUIContent.none);
        EditorGUI.LabelField(labelHIRect, new GUIContent("Has item"));
        EditorGUI.ObjectField(nameRect, property.FindPropertyRelative("item"), GUIContent.none);
        EditorGUI.LabelField(labelAmRect, new GUIContent("In Amount"));
        EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("amount"), GUIContent.none);

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}