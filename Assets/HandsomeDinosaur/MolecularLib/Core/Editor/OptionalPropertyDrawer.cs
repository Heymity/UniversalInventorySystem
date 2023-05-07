using MolecularLib.Helpers;
using UnityEditor;
using UnityEngine;

namespace MolecularEditor
{
    [CustomPropertyDrawer(typeof(Optional<>))]
    public class OptionalPropertyDrawer : PropertyDrawer
    {
        private const float ToggleSize = 18f;

  public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var valueHeight = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("value"));
            var useValueHeight = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("useValue"));

            return Mathf.Max(valueHeight, useValueHeight);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueLabel = label.text;
            
            EditorGUI.BeginProperty(position, label, property);
            property.serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            
            var valueProperty = property.FindPropertyRelative("value");
            var useValueProperty = property.FindPropertyRelative("useValue");
            
            EditorGUI.BeginDisabledGroup(!useValueProperty.boolValue);
            var valueRect = new Rect(position.x, position.y, position.width - ToggleSize - 3f, 0f)
            {
                height = EditorGUI.GetPropertyHeight(valueProperty)
            };

            EditorGUI.PropertyField(valueRect, valueProperty, new GUIContent(valueLabel));
            EditorGUI.EndDisabledGroup();

            var useValueRect = new Rect(valueRect.xMax + 3f, position.y, ToggleSize, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(useValueRect, useValueProperty, GUIContent.none);

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
                
                Undo.RecordObject(property.serializedObject.targetObject, "Optional Variable Changed");
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }

            EditorGUI.EndProperty();
        }
    }
}
