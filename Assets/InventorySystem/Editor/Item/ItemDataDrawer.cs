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

[CustomPropertyDrawer(typeof(ItemGroup))]
public class ItemDataDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 18f * 1;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property);

        var buttonRect = new Rect(position.x, position.y, 170, position.height);
        if(property.objectReferenceValue != null)
        {
            if (GUI.Button(buttonRect, new GUIContent("Open Editor of " + label.text)))
            {
                ItemDataEditorWindow.Open(property.objectReferenceValue as ItemGroup);
            }
        }
        
        //base.OnGUI(position, property, label);
    }
}
