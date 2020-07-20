/*  Copyright 2020 Gabriel Pasquale Rodrigues Scavone
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 * 
 *  
 *  This is an Editor Script, it is responsible for drawing the inpector or drawer of the class in the attribute before the class
 */

using UnityEditor;
using UnityEngine;

namespace UniversalInventorySystem.Editors
{
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
        SerializedProperty showAmountProp;
        /**SerializedProperty stackAlwaysProp;
        SerializedProperty stackOnMaxDurabiliyProp;
        SerializedProperty stackOnSpecifDurabilityProp;
        SerializedProperty stackDurabilitiesProp;**/

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
            maxDurabilityProp = serializedObject.FindProperty("durability");
            hasDurabilityProp = serializedObject.FindProperty("hasDurability");
            durabilityImagesProp = serializedObject.FindProperty("_durabilityImages");
            showAmountProp = serializedObject.FindProperty("showAmount");
            /**stackAlwaysProp = serializedObject.FindProperty("stackAlways");
            stackOnMaxDurabiliyProp = serializedObject.FindProperty("stackOnMaxDurabiliy");
            stackOnSpecifDurabilityProp = serializedObject.FindProperty("stackOnSpecifDurability");
            stackDurabilitiesProp = serializedObject.FindProperty("stackDurabilities");**/
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            itemFoldout = EditorGUILayout.Foldout(itemFoldout, new GUIContent("Item Configuration"), true, EditorStyles.foldoutHeader);
            if (itemFoldout)
            {
                EditorGUI.indentLevel++;
                itemNameProp.stringValue = EditorGUILayout.TextField(new GUIContent("Item name"), itemNameProp.stringValue);
                idProp.intValue = EditorGUILayout.IntField(new GUIContent("Id"), idProp.intValue);
                showAmountProp.boolValue = EditorGUILayout.Toggle(new GUIContent("Show Amount"), showAmountProp.boolValue);
                EditorGUILayout.ObjectField(spriteProp, new GUIContent("Item sprite"));
                var item = spriteProp.objectReferenceValue as Sprite;
                if (item != null)
                    EditorGUILayout.LabelField(new GUIContent(item.texture), GUILayout.Height(54));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Separator();
            storageFoldout = EditorGUILayout.Foldout(storageFoldout, new GUIContent("Storage Configuration"), true, EditorStyles.foldoutHeader);
            if (storageFoldout)
            {
                EditorGUI.indentLevel++;
                if (hasDurabilityProp.boolValue) EditorGUILayout.HelpBox(new GUIContent("You can only have durability or stackable selected, not both"));
                EditorGUI.BeginDisabledGroup(hasDurabilityProp.boolValue);
                stackableProp.boolValue = EditorGUILayout.Toggle(new GUIContent("Stackable"), stackableProp.boolValue);
                EditorGUI.EndDisabledGroup();
                if (stackableProp.boolValue)
                {
                    maxAmountProp.intValue = EditorGUILayout.IntField(new GUIContent("Max amount per slot"), maxAmountProp.intValue);
                    hasDurabilityProp.boolValue = false;
                }
                /**if (hasDurabilityProp.boolValue && stackableProp.boolValue)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("stackOptions"), new GUIContent("On change durability action"));
                    EditorGUILayout.PropertyField(stackAlwaysProp);
                    EditorGUILayout.PropertyField(stackOnMaxDurabiliyProp);
                    EditorGUILayout.PropertyField(stackOnSpecifDurabilityProp);
                    if(stackOnSpecifDurabilityProp.boolValue)
                    {
                        //EditorGUILayout.PropertyField(stackDurabilitiesProp);
                        stackDurabilitiesProp.isExpanded = EditorGUILayout.Foldout(stackDurabilitiesProp.isExpanded, "Stack Durabilities");
                        if (stackDurabilitiesProp.isExpanded)
                        {
                            EditorGUI.indentLevel++;
                            stackDurabilitiesProp.arraySize = EditorGUILayout.IntField("Size" ,stackDurabilitiesProp.arraySize);
                            for(int i = 0;i < stackDurabilitiesProp.arraySize; i++)
                            {
                                stackDurabilitiesProp.GetArrayElementAtIndex(i).intValue = EditorGUILayout.IntField("Durability", stackDurabilitiesProp.GetArrayElementAtIndex(i).intValue);
                                if (i >= (target as Item).stackDurabilities.Count) continue;
                                uint dur = (target as Item).stackDurabilities[i];
                                var progressRect = GUILayoutUtility.GetRect(38, 18);
                                progressRect.x += 30;
                                progressRect.width -= 30;
                                EditorGUI.ProgressBar(progressRect, (float)dur / (float)maxDurabilityProp.intValue, "Durability");
                            }
                            EditorGUI.indentLevel--;
                        }
                    }
                }*/
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Separator();
            usingFoldout = EditorGUILayout.Foldout(usingFoldout, new GUIContent("Using items Configuration"), true, EditorStyles.foldoutHeader);
            if (usingFoldout)
            {
                EditorGUI.indentLevel++;
                destroyOnUseProp.boolValue = EditorGUILayout.Toggle(new GUIContent("Remove item when finish used"), destroyOnUseProp.boolValue);
                useHowManyWhenUsedProp.intValue = EditorGUILayout.IntField(new GUIContent("The amount of item to remove"), useHowManyWhenUsedProp.intValue);

                if (stackableProp.boolValue) EditorGUILayout.HelpBox(new GUIContent("You can only have durability or stackable selected, not both"));
                EditorGUI.BeginDisabledGroup(stackableProp.boolValue);
                hasDurabilityProp.boolValue = EditorGUILayout.Toggle("Has durability", hasDurabilityProp.boolValue);
                EditorGUI.EndDisabledGroup();
                if (stackableProp.boolValue)
                {
                    hasDurabilityProp.boolValue = false;
                }
                if (hasDurabilityProp.boolValue)
                {
                    EditorGUILayout.PropertyField(maxDurabilityProp, new GUIContent("Max durability"), true);

                    var tmpBool = EditorGUILayout.Foldout(durabilityImagesProp.isExpanded, "Durability Images", true);
                    if (tmpBool != durabilityImagesProp.isExpanded)
                        Item.SortDurabilityImages((target as Item).durabilityImages);
                    durabilityImagesProp.isExpanded = tmpBool;
                    if (durabilityImagesProp.isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        durabilityImagesProp.arraySize = EditorGUILayout.IntField("Size", durabilityImagesProp.arraySize);
                        serializedObject.ApplyModifiedProperties();
                        for (int i = 0; i < durabilityImagesProp.arraySize; i++)
                        {
                            EditorGUILayout.PropertyField(durabilityImagesProp.GetArrayElementAtIndex(i));
                            DurabilityImage dur = (target as Item).durabilityImages[i];
                            var progressRect = GUILayoutUtility.GetRect(38, 18);
                            progressRect.x += 30;
                            progressRect.width -= 30;
                            EditorGUI.ProgressBar(progressRect, (float)dur.durability / (float)maxDurabilityProp.intValue, dur.imageName);
                        }
                        EditorGUI.indentLevel--;
                        if (GUILayout.Button("Sort"))
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
}