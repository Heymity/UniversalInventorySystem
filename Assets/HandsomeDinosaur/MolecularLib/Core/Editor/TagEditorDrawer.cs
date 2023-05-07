using System.Linq;
using MolecularLib.Helpers;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MolecularEditor
{
    [CustomPropertyDrawer(typeof(Tag))]
    public class TagEditorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();

            var tag = property.FindPropertyRelative("tag");

            var pos = EditorGUI.PrefixLabel(position, label);

            var index = InternalEditorUtility.tags.ToList().IndexOf(tag.stringValue);
            tag.stringValue = InternalEditorUtility.tags[EditorGUI.Popup(pos, index, InternalEditorUtility.tags)];

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
