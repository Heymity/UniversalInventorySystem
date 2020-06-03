using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UniversalInventorySystem;

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
        EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Inventory (id: " + id.intValue.ToString() + ")"));
        position.y += 18;
        var areItemsUsable = property.FindPropertyRelative("areItemsUsable");
        var areItemsDroppable = property.FindPropertyRelative("areItemsDroppable");
        var slotAmounts = property.FindPropertyRelative("slotAmounts");
        var slots = property.FindPropertyRelative("slots");
        var inte = property.FindPropertyRelative("interactiable");

        EditorGUIUtility.wideMode = true;
        EditorGUIUtility.labelWidth = 100;
        position.height /= amountOfFilds;

        //position.y += position.height;

        var usableRect = new Rect(position.x, position.y, 120, position.height);
        areItemsUsable.boolValue = EditorGUI.Toggle(usableRect, "Can use items", areItemsUsable.boolValue);
        var dropItemRect = new Rect(position.x + 150, position.y, 120, position.height);
        areItemsDroppable.boolValue = EditorGUI.Toggle(dropItemRect, "Can drop items", areItemsDroppable.boolValue);

        //position.y += position.height;
        var inteRect = new Rect(position.x + 300, position.y, position.width - 300, position.height);
        EditorGUI.PropertyField(inteRect, inte);

        position.y += position.height;

        var foldRect = new Rect(position.x, position.y, 50, position.height);
        unfold = EditorGUI.Foldout(foldRect, unfold, new GUIContent("Slots"), true);

        var slotAmountsRect = new Rect(position.x + 50, position.y, position.width - 150, position.height);
        slotAmounts.intValue = EditorGUI.IntField(slotAmountsRect, new GUIContent("Amount of slots"), slotAmounts.intValue);

        position.y += position.height;

        if (unfold)
        {
            EditorGUI.indentLevel++;
            slots.arraySize = slotAmounts.intValue;
            //amountOfFilds = baseAmount + 1;
            //position.y += position.height;
            if (slots != null)
            {
                if (slots.arraySize > 0)
                {
                    float childHeight = 0;
                    for (int i = 0; i < slots.arraySize; i++)
                    {
                        childHeight += EditorGUI.GetPropertyHeight(slots.GetArrayElementAtIndex(i)) / 18;
                        EditorGUI.PropertyField(position, slots.GetArrayElementAtIndex(i));
                        position.y += EditorGUI.GetPropertyHeight(slots.GetArrayElementAtIndex(i));
                    }
                    amountOfFilds = baseAmount + childHeight;
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
