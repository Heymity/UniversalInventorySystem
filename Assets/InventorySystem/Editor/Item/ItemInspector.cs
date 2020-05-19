using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

    //Behaviours
    SerializedProperty onUseFuncProp;
    SerializedProperty optionalOnDropBehaviour;

    bool itemFoldout;
    bool storageFoldout;
    bool usingFoldout;
    bool behaviourFoldout;

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
    }

    public override void OnInspectorGUI()
    {
       
        itemFoldout = EditorGUILayout.Foldout(itemFoldout, new GUIContent("Item Configuration"), true, EditorStyles.foldoutHeader);
        if(itemFoldout)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.TextField(new GUIContent("Item name"), itemNameProp.stringValue);
            EditorGUILayout.IntField(new GUIContent("Id"), idProp.intValue);
            EditorGUILayout.ObjectField(spriteProp, new GUIContent("Ïtem sprite"));
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Separator();
        storageFoldout = EditorGUILayout.Foldout(storageFoldout, new GUIContent("Storage Configuration"), true, EditorStyles.foldoutHeader);
        if(storageFoldout)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.IntField(new GUIContent("Max amount per slot"), maxAmountProp.intValue);
            EditorGUILayout.Toggle(new GUIContent("Stackable"), stackableProp.boolValue);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Separator();
        usingFoldout = EditorGUILayout.Foldout(usingFoldout, new GUIContent("Using items Configuration"), true, EditorStyles.foldoutHeader);
        if(usingFoldout)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.Toggle(new GUIContent("Remove item when used"), destroyOnUseProp.boolValue);
            EditorGUILayout.IntField(new GUIContent("The amount of item to remove"), useHowManyWhenUsedProp.intValue);
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
    }
}
