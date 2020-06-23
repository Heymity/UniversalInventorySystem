using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UniversalInventorySystem;

[CustomEditor(typeof(Item))]
public class ItemInspector : Editor
{
    //Item props
    SerializedProperty itemNameProp;
    SerializedProperty idProp;
    SerializedProperty spriteProp;

    //Storage Props
    SerializedProperty maxAmountProp;
    SerializedProperty stackableProp;

    //Using Props
    SerializedProperty destroyOnUseProp;
    SerializedProperty useHowManyWhenUsedProp;
    SerializedProperty maxDurabilityProp;
    SerializedProperty hasDurabilityProp;
    SerializedProperty durabilityImagesProp;

    //Behaviours
    SerializedProperty onUseFuncProp;
    SerializedProperty optionalOnDropBehaviour;

    //Tooltip
    SerializedProperty tooltipProp;

    bool itemFoldout;
    bool storageFoldout;
    bool usingFoldout;
    bool behaviourFoldout;
    bool tooltipFoldout;

    private void OnEnable()
    {
        itemNameProp = serializedObject.FindProperty("itemName");
        idProp = serializedObject.FindProperty("id");
        spriteProp = serializedObject.FindProperty("sprite");
        maxAmountProp = serializedObject.FindProperty("maxAmount");
        destroyOnUseProp = serializedObject.FindProperty("destroyOnUse");
        useHowManyWhenUsedProp = serializedObject.FindProperty("useHowManyWhenUsed");
        stackableProp = serializedObject.FindProperty("stackable");
        onUseFuncProp = serializedObject.FindProperty("onUseFunc");
        optionalOnDropBehaviour = serializedObject.FindProperty("optionalOnDropBehaviour");
        tooltipProp = serializedObject.FindProperty("tooltip");
        maxDurabilityProp = serializedObject.FindProperty("maxDurability");
        hasDurabilityProp = serializedObject.FindProperty("hasDurability");
        durabilityImagesProp = serializedObject.FindProperty("_durabilityImages");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        itemFoldout = EditorGUILayout.Foldout(itemFoldout, new GUIContent("Item Configuration"), true, EditorStyles.foldoutHeader);
        if(itemFoldout)
        {
            EditorGUI.indentLevel++;
            itemNameProp.stringValue = EditorGUILayout.TextField(new GUIContent("Item name"), itemNameProp.stringValue);
            idProp.intValue = EditorGUILayout.IntField(new GUIContent("Id"), idProp.intValue);
            EditorGUILayout.ObjectField(spriteProp, new GUIContent("Item sprite"));
            var item = spriteProp.objectReferenceValue as Sprite;
            if (item != null)
                EditorGUILayout.LabelField(new GUIContent(item.texture), GUILayout.Height(54));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Separator();
        storageFoldout = EditorGUILayout.Foldout(storageFoldout, new GUIContent("Storage Configuration"), true, EditorStyles.foldoutHeader);
        if(storageFoldout)
        {
            EditorGUI.indentLevel++;
            maxAmountProp.intValue = EditorGUILayout.IntField(new GUIContent("Max amount per slot"), maxAmountProp.intValue);
            stackableProp.boolValue = EditorGUILayout.Toggle(new GUIContent("Stackable"), stackableProp.boolValue);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Separator();
        usingFoldout = EditorGUILayout.Foldout(usingFoldout, new GUIContent("Using items Configuration"), true, EditorStyles.foldoutHeader);
        if(usingFoldout)
        {
            EditorGUI.indentLevel++;
            destroyOnUseProp.boolValue = EditorGUILayout.Toggle(new GUIContent("Remove item when used"), destroyOnUseProp.boolValue);
            useHowManyWhenUsedProp.intValue = EditorGUILayout.IntField(new GUIContent("The amount of item to remove"), useHowManyWhenUsedProp.intValue);
            hasDurabilityProp.boolValue = EditorGUILayout.Toggle("Has durability", hasDurabilityProp.boolValue);
            if (hasDurabilityProp.boolValue)
            {
                EditorGUILayout.PropertyField(maxDurabilityProp, new GUIContent("Max durability"), true);
                //EditorGUILayout.PropertyField(durabilityImagesProp, new GUIContent("Durability Images"), true);
                //Debug.Log(durabilityImagesProp);
                var tmpBool = EditorGUILayout.Foldout(durabilityImagesProp.isExpanded, "Durability Images", true);
                if(tmpBool != durabilityImagesProp.isExpanded)
                    Item.SortDurabilityImages((target as Item).durabilityImages);
                durabilityImagesProp.isExpanded = tmpBool;
                if (durabilityImagesProp.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    durabilityImagesProp.arraySize = EditorGUILayout.IntField("Size", durabilityImagesProp.arraySize);
                    for(int i = 0; i < durabilityImagesProp.arraySize; i++)
                    {
                        EditorGUILayout.PropertyField(durabilityImagesProp.GetArrayElementAtIndex(i));
                        DurabilityImage dur = (target as Item).durabilityImages[i];
                        EditorGUI.ProgressBar(GUILayoutUtility.GetRect(38, 18), (float)dur.durability / (float)maxDurabilityProp.intValue, "Durability");
                    }
                    EditorGUI.indentLevel--;
                    if(GUILayout.Button("Sort"))
                        Item.SortDurabilityImages((target as Item).durabilityImages);
                }
            }
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Separator();
        behaviourFoldout = EditorGUILayout.Foldout(behaviourFoldout, new GUIContent("Behaviour Configuration"), true, EditorStyles.foldoutHeader);
        if (behaviourFoldout)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.Separator();
            EditorGUILayout.HelpBox(new GUIContent("The field below accepts any script, but it will only work if the provided script has the OnUse function"));
            EditorGUILayout.ObjectField(onUseFuncProp, new GUIContent("On use item Behaviour"));
            EditorGUILayout.Separator();

            EditorGUILayout.HelpBox(new GUIContent("The field below accepts any script, but it will only work if the provided script has the OnDropItem function"));
            EditorGUILayout.ObjectField(optionalOnDropBehaviour, new GUIContent("On drop item optional Behaviour"));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Separator();
        tooltipFoldout = EditorGUILayout.Foldout(tooltipFoldout, "Tooltip Configuration", true, EditorStyles.foldoutHeader);
        if (tooltipFoldout)
        {
            EditorGUILayout.PropertyField(tooltipProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
