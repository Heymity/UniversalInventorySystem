using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Inventory))]
public class InventoryDrawer : PropertyDrawer
{
    float baseAmount = 3f;
    float amountOfFilds = 3f;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 18f * amountOfFilds;
    }
    bool unfold = false;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var id = property.FindPropertyRelative("id");
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Inventory " + id.intValue.ToString()));
        var areItemsUsable = property.FindPropertyRelative("areItemsUsable");
        var areItemsDroppable = property.FindPropertyRelative("areItemsDroppable");
        var slotAmounts = property.FindPropertyRelative("slotAmounts");
        var slots = property.FindPropertyRelative("slots");
        var inte = property.FindPropertyRelative("interactiable");

        EditorGUIUtility.wideMode = true;
        EditorGUIUtility.labelWidth = 100;
        position.height /= amountOfFilds;

        //position.y += position.height;

        areItemsUsable.boolValue = EditorGUI.Toggle(position, "Can use items", areItemsUsable.boolValue);
        var dropItemRect = new Rect(position.x + 150, position.y, position.width, position.height);
        areItemsDroppable.boolValue = EditorGUI.Toggle(dropItemRect, "Can drop items", areItemsDroppable.boolValue);

        position.y += position.height;

        EditorGUI.PropertyField(position, inte);

        position.y += position.height;

        unfold = EditorGUI.Foldout(position, unfold, new GUIContent("Slots"), true);

        var slotAmountsRect = new Rect(position.x + 50, position.y, position.width - 150, position.height);
        slotAmounts.intValue = EditorGUI.IntField(slotAmountsRect, new GUIContent("Amount of slots"), slotAmounts.intValue);

        position.y += position.height;

        if (unfold)
        {
            EditorGUI.indentLevel++;
            slots.arraySize = EditorGUI.IntField(position, new GUIContent("Size"), slots.arraySize);
            amountOfFilds = baseAmount + 1;
            position.y += position.height;
            if (slots != null)
            {
                if (slots.arraySize > 0)
                {
                    amountOfFilds = baseAmount + 1 + slots.arraySize;
                    for (int i = 0; i < slots.arraySize; i++)
                    {
                        EditorGUI.PropertyField(position, slots.GetArrayElementAtIndex(i));
                        position.y += position.height;
                    }
                }
            }
            EditorGUI.indentLevel--;
        }
        else
        {
            amountOfFilds = baseAmount;
        }
    }
}
