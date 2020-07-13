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
using UniversalInventorySystem;

[CustomEditor(typeof(Recipe))]
public class ReceipeEditor : Editor
{
    SerializedProperty numberOfFactorsProp;
    SerializedProperty numberOfProductsProp;

    SerializedProperty factorsProp;
    SerializedProperty productsProp;

    SerializedProperty factorsAmountProp;
    SerializedProperty productsAmountProp;

    private void OnEnable()
    {
        numberOfFactorsProp = serializedObject.FindProperty("numberOfFactors");
        numberOfProductsProp = serializedObject.FindProperty("numberOfProducts");
        factorsProp = serializedObject.FindProperty("factors");
        factorsAmountProp = serializedObject.FindProperty("amountFactors");
        productsProp = serializedObject.FindProperty("products");
        productsAmountProp = serializedObject.FindProperty("amountProducts");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(numberOfFactorsProp, new GUIContent("Number of input items"));

        factorsProp.arraySize = numberOfFactorsProp.intValue;
        factorsAmountProp.arraySize = numberOfFactorsProp.intValue;
        factorsProp.isExpanded = EditorGUILayout.Foldout(factorsProp.isExpanded, "Input items");
        if (factorsProp.isExpanded)
        {
            EditorGUI.indentLevel++;

            for(int i = 0;i < factorsProp.arraySize; i++)
            {

                EditorGUILayout.ObjectField(factorsProp.GetArrayElementAtIndex(i), new GUIContent($"Input Item {i}"));
                factorsAmountProp.GetArrayElementAtIndex(i).intValue = EditorGUILayout.IntField(new GUIContent($"Item amount {i}"),factorsAmountProp.GetArrayElementAtIndex(i).intValue);
                EditorGUILayout.Separator();
            }

            if(factorsProp.arraySize == 0)
            {
                EditorGUILayout.LabelField("Array lenght is 0");
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Separator();

        EditorGUILayout.PropertyField(numberOfProductsProp, new GUIContent("Number of output items"));

        productsProp.arraySize = numberOfProductsProp.intValue;
        productsAmountProp.arraySize = numberOfProductsProp.intValue;
        productsProp.isExpanded = EditorGUILayout.Foldout(productsProp.isExpanded, "Output items");
        if (productsProp.isExpanded)
        {
            EditorGUI.indentLevel++;

            for (int i = 0; i < productsProp.arraySize; i++)
            {

                EditorGUILayout.ObjectField(productsProp.GetArrayElementAtIndex(i), new GUIContent($"Output Item {i}"));
                productsAmountProp.GetArrayElementAtIndex(i).intValue = EditorGUILayout.IntField(new GUIContent($"Item amount {i}"), productsAmountProp.GetArrayElementAtIndex(i).intValue);
                EditorGUILayout.Separator();
            }

            if (productsProp.arraySize == 0)
            {
                EditorGUILayout.LabelField("Array lenght is 0");
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Separator();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("id"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("key"));

        SerializedProperty pattern = serializedObject.FindProperty("factors");
        SerializedProperty product = serializedObject.FindProperty("products");

        GUIContent content = new GUIContent();

        EditorGUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true));

        for (int i = 0; i < pattern.arraySize; i++)
        {
            //patternPos.x += 36;
            var item = pattern.GetArrayElementAtIndex(i).objectReferenceValue as Item;
            if (item == null)
            {
                content.text = "null";
                content.image = null;
            }
            else
            {
                content.text = "";
                content.image = item.sprite.texture;
            }
            EditorGUILayout.LabelField(content, GUILayout.Height(36), GUILayout.Width(36));
            if (i < pattern.arraySize - 1)
                EditorGUILayout.LabelField("+", GUILayout.Height(36), GUILayout.Width(10));
            else
            {
                EditorGUILayout.LabelField("=", GUILayout.Height(36), GUILayout.Width(10));
                for (int j = 0; j < product.arraySize; j++)
                {
                    var productItem = product.GetArrayElementAtIndex(j).objectReferenceValue as Item;
                    if (productItem == null)
                    {
                        content.text = "null";
                        content.image = null;
                    }
                    else
                    {
                        content.text = "";
                        content.image = productItem.sprite.texture;
                    }
                    EditorGUILayout.LabelField(content, GUILayout.Height(36), GUILayout.Width(36));

                }

                EditorGUILayout.SelectableLabel($"id: {serializedObject.FindProperty("id").intValue}, key: {serializedObject.FindProperty("key").stringValue}", GUILayout.Height(36));
            }
        }

        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
