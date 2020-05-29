using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UniversalInventorySystem;

[CustomPropertyDrawer(typeof(Item))]
public class ItemDrawer : PropertyDrawer
{
    bool itemFoldout;
    bool storageFoldout;
    bool usingFoldout;
    bool behaviourFoldout;

    float baseAmount = 6f;
    float amountOfFilds = 6f;
    float total;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 18f * amountOfFilds;
    }
    bool useObjValues;
    bool unfold = true;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        amountOfFilds = baseAmount + total;
        total = 0;
        SerializedObject serializedObject = null;
        if (property.objectReferenceValue != null) serializedObject = new SerializedObject(property.objectReferenceValue);
        if(serializedObject != null)
        {
            serializedObject.Update();
            var itemNameProp = serializedObject.FindProperty("itemName");
            var idProp = serializedObject.FindProperty("id");
            var spriteProp = serializedObject.FindProperty("sprite");
            var maxAmountProp = serializedObject.FindProperty("maxAmount");
            var destroyOnUseProp = serializedObject.FindProperty("destroyOnUse");
            var useHowManyWhenUsedProp = serializedObject.FindProperty("useHowManyWhenUsed");
            var stackableProp = serializedObject.FindProperty("stackable");
            var onUseFuncProp = serializedObject.FindProperty("onUseFunc");
            var optionalOnDropBehaviour = serializedObject.FindProperty("optionalOnDropBehaviour");

            EditorGUIUtility.wideMode = true;
            EditorGUIUtility.labelWidth = 240;
            position.height /= amountOfFilds;

            unfold = EditorGUI.Foldout(position, unfold, label);
            position.y += position.height;
            position.x += 20;

            if (unfold)
            {

                useObjValues = EditorGUI.Toggle(position, new GUIContent("Use Object Field"), useObjValues);

                position.y += position.height;

                if (!useObjValues)
                {
                    baseAmount = 6;
                    itemFoldout = EditorGUI.Foldout(position, itemFoldout, new GUIContent("Item Configuration"), true);
                    position.y += position.height;
                    if (itemFoldout)
                    {
                        position.x += 20;
                        position.width -= 40;
                        EditorGUIUtility.labelWidth -= 20;
                        total += 3;
                        EditorGUI.indentLevel++;

                        itemNameProp.stringValue = EditorGUI.TextField(position, new GUIContent("Item name"), itemNameProp.stringValue);
                        position.y += position.height;

                        idProp.intValue = EditorGUI.IntField(position, new GUIContent("Id"), idProp.intValue);
                        position.y += position.height;

                        EditorGUI.ObjectField(position, spriteProp, new GUIContent("Item sprite"));
                        position.y += position.height;

                        EditorGUI.indentLevel--;
                        position.width += 40;
                        EditorGUIUtility.labelWidth += 20;
                        position.x -= 20;
                    }

                    storageFoldout = EditorGUI.Foldout(position, storageFoldout, new GUIContent("Storage Configuration"), true);
                    position.y += position.height;
                    if (storageFoldout)
                    {
                        position.x += 20;
                        position.width -= 40;
                        EditorGUIUtility.labelWidth -= 20;
                        total += 2;
                        EditorGUI.indentLevel++;

                        maxAmountProp.intValue = EditorGUI.IntField(position, new GUIContent("Max amount per slot"), maxAmountProp.intValue);
                        position.y += position.height;

                        stackableProp.boolValue = EditorGUI.Toggle(position, new GUIContent("Stackable"), stackableProp.boolValue);
                        position.y += position.height;

                        EditorGUI.indentLevel--;
                        position.width += 40;
                        EditorGUIUtility.labelWidth += 20;
                        position.x -= 20;
                    }

                    usingFoldout = EditorGUI.Foldout(position, usingFoldout, new GUIContent("Using items Configuration"), true);
                    position.y += position.height;
                    if (usingFoldout)
                    {
                        position.x += 20;
                        position.width -= 40;
                        EditorGUIUtility.labelWidth -= 20;
                        total += 2;
                        EditorGUI.indentLevel++;

                        destroyOnUseProp.boolValue = EditorGUI.Toggle(position, new GUIContent("Remove item when used"), destroyOnUseProp.boolValue);
                        position.y += position.height;

                        useHowManyWhenUsedProp.intValue = EditorGUI.IntField(position, new GUIContent("The amount of item to remove"), useHowManyWhenUsedProp.intValue);
                        position.y += position.height;

                        EditorGUI.indentLevel--;
                        position.width += 40;
                        EditorGUIUtility.labelWidth += 20;
                        position.x -= 20;
                    }

                    behaviourFoldout = EditorGUI.Foldout(position, behaviourFoldout, new GUIContent("Behaviour Configuration"), true);
                    position.y += position.height;
                    if (behaviourFoldout)
                    {
                        position.x += 20;
                        position.width -= 40;
                        EditorGUIUtility.labelWidth -= 20;
                        total += 5;
                        EditorGUI.indentLevel++;
                        EditorGUILayout.Separator();
                        EditorGUI.HelpBox(position, "The field below accepts any script, but it will only work if the provided script has the OnUse function", MessageType.None);
                        position.y += position.height;
                        EditorGUI.ObjectField(position, onUseFuncProp, new GUIContent("On use item Behaviour"));
                        position.y += 2 * (position.height);


                        EditorGUI.HelpBox(position, "The field below accepts any script, but it will only work if the provided script has the OnDropItem function", MessageType.None);
                        position.y += position.height;
                        EditorGUI.ObjectField(position, optionalOnDropBehaviour, new GUIContent("On drop item optional Behaviour"));
                        position.y += position.height;
                        EditorGUI.indentLevel--;
                        position.width += 40;
                        EditorGUIUtility.labelWidth += 20;
                        position.x -= 20;
                    }
                }
                else
                {
                    baseAmount = 3f;
                    EditorGUI.ObjectField(position, property);
                }
            }
            else
            {
                amountOfFilds = 1;
                baseAmount = 1;
            }
            position.x -= 20;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
