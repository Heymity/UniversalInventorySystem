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
using UniversalInventorySystem;

[CustomPropertyDrawer(typeof(DurabilityImage))]
public class DurabilityItemDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 18f * 2;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.width /= 3;

        EditorGUIUtility.wideMode = true;
        EditorGUIUtility.labelWidth = 240;
        position.height /= 2;

        EditorGUI.LabelField(position, "Durability Trigger");
        position.y += position.height;

        var durabilityProp = property.FindPropertyRelative("durability");
        durabilityProp.intValue = EditorGUI.IntField(position, durabilityProp.intValue);
        position.y -= position.height;
        position.x += position.width;

        EditorGUI.LabelField(position, "Image Name");
        position.y += position.height;

        var nameProp = property.FindPropertyRelative("imageName");
        nameProp.stringValue = EditorGUI.TextField(position, nameProp.stringValue);
        position.y -= position.height;
        position.x += position.width;

        EditorGUI.LabelField(position, "Image");
        position.y += position.height;

        var spriteProp = property.FindPropertyRelative("sprite");
        EditorGUI.ObjectField(position, spriteProp, new GUIContent());
        position.y -= position.height;
        position.x += position.width;

    }
}
