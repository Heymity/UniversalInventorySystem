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

using UnityEngine;
using UnityEditor;

namespace UniversalInventorySystem.Editors
{
    [CustomPropertyDrawer(typeof(Item))]
    public class ItemDrawer : PropertyDrawer
    {
        bool itemFoldout;
        bool storageFoldout;
        bool usingFoldout;
        bool behaviourFoldout;
        bool tooltipFoldout;
        bool customFoldout;

        float baseAmount = 7;
        float amountOfFilds = 21f;
        float total;
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 18f * amountOfFilds;
        }
        bool useObjValues;
        bool unfold = true;
        /**public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            amountOfFilds = baseAmount + total;
            total = 0;
            SerializedObject serializedObject = null;
            if (property.objectReferenceValue != null) serializedObject = new SerializedObject(property.objectReferenceValue);
            if (serializedObject != null)
            {
                serializedObject.Update();
                var itemNameProp = serializedObject.FindProperty("name");
                var idProp = serializedObject.FindProperty("id");
                var spriteProp = serializedObject.FindProperty("sprite");
                var maxAmountProp = serializedObject.FindProperty("maxAmount");
                var destroyOnUseProp = serializedObject.FindProperty("destroyOnUse");
                var useHowManyWhenUsedProp = serializedObject.FindProperty("useHowManyWhenUsed");
                var stackableProp = serializedObject.FindProperty("stackable");
                var onUseFuncProp = serializedObject.FindProperty("onUseFunc");
                var optionalOnDropBehaviour = serializedObject.FindProperty("optionalOnDropBehaviour");
                var maxDurabilityProp = serializedObject.FindProperty("durability");
                var hasDurabilityProp = serializedObject.FindProperty("hasDurability");
                var durabilityImagesProp = serializedObject.FindProperty("_durabilityImages");
                var tooltipProp = serializedObject.FindProperty("tooltip");
                var showAmountProp = serializedObject.FindProperty("showAmount");

                EditorGUIUtility.wideMode = true;
                EditorGUIUtility.labelWidth = 240;
                EditorGUI.BeginProperty(position, null, property);
                position.height /= amountOfFilds;

                unfold = EditorGUI.Foldout(position, unfold, (property.objectReferenceValue as Item).name);
                position.y += position.height;
                position.x += 20;

                if (unfold)
                {
                    var tmp = position.width;
                    position.width = 140;
                    if (GUI.Button(position, useObjValues ? "Edit Values" : "Use Object Value")) useObjValues = !useObjValues;
                    position.width = tmp;


                    position.y += position.height;

                    if (!useObjValues)
                    {
                        baseAmount = 7;
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

                            showAmountProp.boolValue = EditorGUI.Toggle(position, new GUIContent("Show Amount"), showAmountProp.boolValue);
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
                            total += 6;
                            EditorGUI.indentLevel++;

                            if (hasDurabilityProp.boolValue)
                            {
                                EditorGUI.HelpBox(position, "You can only have durability or stackable selected, not both", MessageType.Info);
                                position.y += position.height;
                            }
                            EditorGUI.BeginDisabledGroup(hasDurabilityProp.boolValue);
                            stackableProp.boolValue = EditorGUI.Toggle(position, new GUIContent("Stackable"), stackableProp.boolValue);
                            EditorGUI.EndDisabledGroup();
                            position.y += position.height;

                            if (stackableProp.boolValue)
                            {
                                maxAmountProp.intValue = EditorGUI.IntField(position, new GUIContent("Max amount per slot"), maxAmountProp.intValue);
                                position.y += position.height;
                                hasDurabilityProp.boolValue = false;
                            }

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
                            total += 3;
                            EditorGUI.indentLevel++;

                            destroyOnUseProp.boolValue = EditorGUI.Toggle(position, new GUIContent("Remove item when finish using"), destroyOnUseProp.boolValue);
                            position.y += position.height;

                            useHowManyWhenUsedProp.intValue = EditorGUI.IntField(position, new GUIContent("The amount of item to remove"), useHowManyWhenUsedProp.intValue);
                            position.y += position.height;

                            if (stackableProp.boolValue)
                            {
                                EditorGUI.HelpBox(position, "You can only have durability or stackable selected, not both", MessageType.Info);
                                position.y += position.height;
                            }
                            EditorGUI.BeginDisabledGroup(stackableProp.boolValue);
                            hasDurabilityProp.boolValue = EditorGUI.Toggle(position, "Has durability", hasDurabilityProp.boolValue);
                            EditorGUI.EndDisabledGroup();
                            if (stackableProp.boolValue)
                            {
                                hasDurabilityProp.boolValue = false;
                            }

                            position.y += position.height;
                            if (hasDurabilityProp.boolValue)
                            {
                                total += 2;
                                EditorGUI.PropertyField(position, maxDurabilityProp, new GUIContent("Max durability"), true);
                                position.y += position.height;

                                var tmpBool = EditorGUI.Foldout(position, durabilityImagesProp.isExpanded, "Durability Images", true);
                                position.y += position.height;

                                if (tmpBool != durabilityImagesProp.isExpanded)
                                    Item.SortDurabilityImages((property.objectReferenceValue as Item).durabilityImages);
                                durabilityImagesProp.isExpanded = tmpBool;
                                if (durabilityImagesProp.isExpanded)
                                {
                                    EditorGUI.indentLevel++;
                                    durabilityImagesProp.arraySize = EditorGUI.IntField(position, "Size", durabilityImagesProp.arraySize);
                                    position.y += position.height;
                                    serializedObject.ApplyModifiedProperties();
                                    total += 2;

                                    for (int i = 0; i < durabilityImagesProp.arraySize; i++)
                                    {
                                        var old = position.height;
                                        position.height = EditorGUI.GetPropertyHeight(durabilityImagesProp.GetArrayElementAtIndex(i));
                                        EditorGUI.PropertyField(position, durabilityImagesProp.GetArrayElementAtIndex(i));
                                        position.y += EditorGUI.GetPropertyHeight(durabilityImagesProp.GetArrayElementAtIndex(i));
                                        total += (EditorGUI.GetPropertyHeight(durabilityImagesProp.GetArrayElementAtIndex(i)) / 18f) + 1;
                                        position.height = old;

                                        DurabilityImage dur = (property.objectReferenceValue as Item).durabilityImages[i];

                                        EditorGUI.ProgressBar(position, (float)dur.durability / (float)maxDurabilityProp.intValue, dur.imageName);
                                        position.y += position.height;
                                    }

                                    EditorGUI.indentLevel--;
                                    if (GUI.Button(position, "Sort"))
                                        Item.SortDurabilityImages((property.objectReferenceValue as Item).durabilityImages);
                                    position.y += position.height;
                                }
                            }

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

                        tooltipFoldout = EditorGUI.Foldout(position, tooltipFoldout, new GUIContent("Tooltip Configuration"), true);
                        position.y += position.height;
                        if (tooltipFoldout)
                        {
                            position.x += 20;
                            position.width -= 40;
                            EditorGUIUtility.labelWidth -= 20;
                            total += 5;
                            EditorGUI.indentLevel++;

                            var old = position.height;
                            position.height = EditorGUI.GetPropertyHeight(tooltipProp);
                            EditorGUI.PropertyField(position, tooltipProp, true);
                            position.y += EditorGUI.GetPropertyHeight(tooltipProp);
                            total += EditorGUI.GetPropertyHeight(tooltipProp) / 18f;

                            position.height = old;


                            EditorGUI.indentLevel--;
                            position.width += 40;
                            EditorGUIUtility.labelWidth += 20;
                            position.x -= 20;
                        }
    
                        
                        SerializedProperty last = tooltipProp.Copy();
                        if (last.Next(false))
                        {
                            customFoldout = EditorGUI.Foldout(position, customFoldout, "Custom Attributes", true);
                            position.y += position.height;
                            total++;
                            if (customFoldout)
                            {
                                do
                                {
                                    EditorGUI.PropertyField(position, last);
                                    position.y += position.height;
                                    total++;
                                }
                                while (last.Next(false));
                            }
                        }
                    }
                    else
                    {
                        baseAmount = 3f;
                        position.width -= 20;
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
                EditorGUI.EndProperty();
            }
            else
            {
                amountOfFilds = 1;
                baseAmount = 1;
                EditorGUI.ObjectField(position, property);
                if (property.objectReferenceValue != null)
                {
                    serializedObject = new SerializedObject(property.objectReferenceValue);
                    serializedObject.ApplyModifiedProperties();
                    useObjValues = true;
                }
            }
        }*/
    }
}