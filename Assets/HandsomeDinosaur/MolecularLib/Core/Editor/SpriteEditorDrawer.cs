using UnityEditor;
using UnityEngine;

namespace MolecularEditor
{
    [CustomPropertyDrawer(typeof(Sprite))]
    public class SpriteEditorDrawer : PropertyDrawer
    {
        private float _spriteDisplaySize = 54f;
        private const float MinSpriteDisplaySize = 54f;

        private bool _isDragging;
        private Rect _sizeEditRect = Rect.zero;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded
                ? EditorGUIUtility.singleLineHeight + _spriteDisplaySize + 8f
                : EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            
            if (!(property.objectReferenceValue is Sprite sp))
            {
                property.isExpanded = false;
                EditorGUI.ObjectField(position, property, label);
                property.serializedObject.ApplyModifiedProperties();
                EditorGUI.EndProperty();
                return;
            }
            
            var tex = sp != null ? sp.texture : null;

            var foldoutRect = DoFoldoutLabel(position, property, label);

            var objectSelectionRect = position;
            objectSelectionRect.width -= foldoutRect.width;
            objectSelectionRect.height = EditorGUIUtility.singleLineHeight;
            objectSelectionRect.x += foldoutRect.width;
            EditorGUI.ObjectField(objectSelectionRect, property, GUIContent.none);

            if (tex != null && property.isExpanded)
            {
                HandleExpandedArea(position, tex, sp, property);
            }
            
            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();

            EditorGUI.EndProperty();
        }

        private static Rect DoFoldoutLabel(Rect position, SerializedProperty property, GUIContent label)
        {
            var foldoutRect = position;
            foldoutRect.width = EditorGUIUtility.labelWidth;
            foldoutRect.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);

            return foldoutRect;
        }

        private void HandleExpandedArea(Rect position, Texture tex, Sprite sp, SerializedProperty property)
        {
            var ratio = _spriteDisplaySize / tex.height;
                
            var displayTextureRect = position;
            displayTextureRect.height = _spriteDisplaySize;
            displayTextureRect.width = tex.width * ratio;
            displayTextureRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.DrawTextureTransparent(displayTextureRect, tex);

            var labelRect = position;
            labelRect.x += displayTextureRect.width;
            labelRect.width = position.width - displayTextureRect.width;
            labelRect.height = EditorGUIUtility.singleLineHeight;
                
            labelRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(labelRect, $"Size: (x: {tex.width}, y: {tex.height})");
                
            labelRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(labelRect, $"Pixels Per Unit: {sp.pixelsPerUnit}");

            if (_sizeEditRect == Rect.zero)
            {
                _sizeEditRect = displayTextureRect;
            }

            _sizeEditRect.height = 20;
            _sizeEditRect.width = 20;
            _sizeEditRect.x = displayTextureRect.x + displayTextureRect.width;
            _sizeEditRect.y = displayTextureRect.y + displayTextureRect.height - _sizeEditRect.height;
            
            GUI.Box(_sizeEditRect, EditorGUIUtility.FindTexture("d_ViewToolZoom On@2x"), GUIStyle.none);

            _isDragging = Event.current.type switch
            {
                EventType.MouseDown when Event.current.button == 0 && _sizeEditRect.Contains(Event.current.mousePosition) => true,
                EventType.MouseUp when _isDragging => false,
                _ => _isDragging
            };

            if (!_isDragging) return;

            var mousePos = Event.current.mousePosition;

            _spriteDisplaySize = (mousePos.y + _sizeEditRect.height / 2) - displayTextureRect.y;

            if (_spriteDisplaySize < MinSpriteDisplaySize) _spriteDisplaySize = MinSpriteDisplaySize;

            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }
    }
}
