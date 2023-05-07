using System.Collections.Generic;
using MolecularLib;
using UnityEditor;
using UnityEngine;

namespace MolecularEditor
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>), true)]
    public class SerializableDictionaryEditorDrawer : PropertyDrawer
    {
        private const float Padding = 6f;
        private const float HeaderHeight = 25f;
        private const float FooterHeight = 20f;
        private const float NoElementHeight = 25f;
        private const float ElementHeight = 22f;
        
        private int _selectedIndex = -1;
        private bool _dontCheckForNewSelection;
        
        private List<object> _usedKeys;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return HeaderHeight;

            var keysProp = property.FindPropertyRelative("keys");
            var valuesProp = property.FindPropertyRelative("values");
            
            var height = HeaderHeight + FooterHeight;

            if (keysProp.arraySize == 0)
            {
                height += NoElementHeight;
                return height;
            }
            
            for (var i = 0; i < keysProp.arraySize; i++)
            {
                var keyProp = keysProp.GetArrayElementAtIndex(i);
                
                if (i >= valuesProp.arraySize) 
                {
                    Debug.LogError("The number of keys is greater than the number of values, aborting");
                    return height;
                }
                var valueProp = valuesProp.GetArrayElementAtIndex(i);
                
                var keyHeight = EditorGUI.GetPropertyHeight(keyProp);
                var valueHeight = EditorGUI.GetPropertyHeight(valueProp);
                var maxHeight = Mathf.Max(keyHeight, valueHeight);
                
                var elementHeight = maxHeight <= ElementHeight ? ElementHeight : maxHeight;

                height += elementHeight + 2;
            }
          
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.BeginChangeCheck();

            _dontCheckForNewSelection = false;
        
            var keysProp = property.FindPropertyRelative("keys");
            var valuesProp = property.FindPropertyRelative("values");

            if (keysProp is null || valuesProp is null)
            {
                EditorGUI.LabelField(position, $"{label.text} Error: Key or DeserializedValue type isn't serializable; Use [HideInInspector] to hide this field");
                EditorGUI.EndProperty();
                return;
            }

            var boxRect = DrawBox(position, label, keysProp.arraySize > 0, property);
            
            if (property.isExpanded)
                DrawDictionary(boxRect, keysProp, valuesProp);

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
                
                Undo.RecordObject(property.serializedObject.targetObject, "Dictionary Changed");
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }

            EditorGUI.EndProperty();
        }

        private static Rect DrawBox(Rect position, GUIContent label, bool hasElements, SerializedProperty property)
        {
            // Draw header
            var headerStyle = (GUIStyle)"RL Header";
            headerStyle.fixedHeight = 0;

            var rectBox = new Rect(position.x, position.y, position.width, HeaderHeight);
            GUI.Box(rectBox, GUIContent.none, headerStyle);

            var headerTextRect = rectBox;
            headerTextRect.x += 18;
            headerTextRect.width -= 18;
            property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(headerTextRect, property.isExpanded, label, EditorStyles.foldout);
            EditorGUI.EndFoldoutHeaderGroup();
            
            if (!property.isExpanded) return rectBox;
            
            // Draw background
            var bgStyle = (GUIStyle)"RL Background";
  
            rectBox.y += rectBox.height;
            rectBox.height = position.height - rectBox.height - FooterHeight;
            if (!hasElements) rectBox.height = NoElementHeight;
            
            GUI.Box(rectBox, GUIContent.none, bgStyle);
            if (!hasElements)
            {
                var noElementRect = rectBox;
                noElementRect.x += Padding;
                EditorGUI.LabelField(noElementRect, "No elements");
            }

            return rectBox;
        }
        
        private void DrawDictionary(Rect boxRect, SerializedProperty keysProp, SerializedProperty valuesProp)
        {
            // Draw elements
            _usedKeys ??= new List<object>();
            _usedKeys.Clear();
            
            var cumulativeHeight = 0f;
            const float handleWidth = 0; //= 10f;
            for (var i = 0; i < keysProp.arraySize; i++)
            {
                var keyProp = keysProp.GetArrayElementAtIndex(i);
               
                var valueProp = valuesProp.GetArrayElementAtIndex(i);
                
                var keyHeight = EditorGUI.GetPropertyHeight(keyProp);
                var valueHeight = EditorGUI.GetPropertyHeight(valueProp);
                var maxHeight = Mathf.Max(keyHeight, valueHeight);

                var elementHeight = maxHeight <= ElementHeight ? ElementHeight : maxHeight;
                var elementAreaRect = new Rect(boxRect.x + 1, boxRect.y + cumulativeHeight, boxRect.width - 2, elementHeight);
                var elementRect = new Rect(
                    elementAreaRect.x + 2 * Padding + handleWidth, 
                    elementAreaRect.y, 
                    elementAreaRect.width - (3 * Padding) - handleWidth, 
                    elementAreaRect.height);

                cumulativeHeight += elementHeight;
                
                HandleSelection(elementAreaRect, i, keyProp);
                
                var isDuplicate = false;

                var keyObj = GetKeyValue(keyProp);
                if (_usedKeys.Contains(keyObj)) isDuplicate = true;
                else _usedKeys.Add(keyObj);
                
                var style = _selectedIndex == i ? "selectionRect" : i % 2 == 0 ? "CN EntryBackEven" : "CN EntryBackOdd"; 
                if (Event.current.type == EventType.Repaint) ((GUIStyle)style).Draw(elementAreaRect, false, false, false, false);

                //var dragHandleStyle = (GUIStyle)"RL DragHandle";
                //var dragHandleRect = new Rect(elementAreaRect.x + Padding, elementAreaRect.y + elementAreaRect.height / 2 - dragHandleStyle.fixedHeight / 2.5f, handleWidth, elementAreaRect.height);
                //if (Event.current.type == EventType.Repaint) dragHandleStyle.Draw(dragHandleRect, false, false, false, false);
               
                DrawDictionaryElement(elementRect, keyProp, valueProp, isDuplicate, i);
            }

            DrawFooter(boxRect, keysProp, valuesProp);
        }

        private static void DrawDictionaryElement(Rect position, SerializedProperty keyProp, SerializedProperty valueProp, bool duplicatedKey, int index)
        {
            var keyRect = new Rect(position.x, position.y + 2, position.width * 0.5f - Padding, ElementHeight - 4);
            var valueRect = new Rect(position.x + position.width * 0.5f + Padding,
                position.y + 2, position.width * 0.5f - Padding, ElementHeight - 4);
            
            if (duplicatedKey)
            {
                var duplicatedKeyRect = keyRect;
                duplicatedKeyRect.width = 18;
                
                keyRect.x += 18;
                keyRect.width -= 18;
                EditorGUI.LabelField(duplicatedKeyRect, EditorGUIUtility.TrIconContent("d_Invalid@2x", "Duplicated key"));
            }
            else EditorGUI.LabelField(Rect.zero, "");

            var originalLabelWidth = EditorGUIUtility.labelWidth;
            
            EditorGUIUtility.labelWidth = 50;
            if (keyProp.isExpanded) EditorGUIUtility.labelWidth = originalLabelWidth / 2;
            EditorGUI.PropertyField(keyRect, keyProp, new GUIContent($"Key {index}"), true);
            
            // Even though this line seems arbitrary and useless, it somehow is needed to fix a problem of int and string keys not updating properly, the problem is probably caused by something else in the code, but I couldn't find what.
            keyProp.serializedObject.ApplyModifiedProperties();
            
            if (!valueProp.isExpanded) EditorGUIUtility.labelWidth = 55;
            else EditorGUIUtility.labelWidth = originalLabelWidth / 2;
            EditorGUI.PropertyField(valueRect, valueProp, new GUIContent($"Value {index}"), true);

            // Even though this line seems arbitrary and useless, it somehow is needed to fix a problem of int and string values not updating properly, the problem is probably caused by something else in the code, but I couldn't find what.
            valueProp.serializedObject.ApplyModifiedProperties();
            
            EditorGUIUtility.labelWidth = originalLabelWidth;
          }

        private void DrawFooter(Rect position, SerializedProperty keysProp, SerializedProperty valuesProp)
        {
            const int footerSize = 60;
            var footerStyle = (GUIStyle) "RL Footer";
            
            var footerRect = new Rect(position.width - footerSize + Padding, position.y + position.height, footerSize, FooterHeight);
            GUI.Box(footerRect, GUIContent.none, footerStyle);

            var btnsRect = new Rect(footerRect.x + 5, footerRect.y, footerRect.width - 10, footerRect.height);
            var addBtnRect = new Rect(btnsRect.x, btnsRect.y, btnsRect.width / 2, btnsRect.height);
            var removeBtnRect = new Rect(btnsRect.x + btnsRect.width / 2, btnsRect.y, btnsRect.width / 2, btnsRect.height);
            if (GUI.Button(addBtnRect, EditorGUIUtility.TrIconContent("Toolbar Plus", "Add to the list"), "RL FooterButton"))
            {
                keysProp.arraySize++;
                valuesProp.arraySize++;
            }
            if (GUI.Button(removeBtnRect, EditorGUIUtility.TrIconContent("Toolbar Minus", "Remove selection from the list"), "RL FooterButton") && _selectedIndex >= 0)
            {
                keysProp.DeleteArrayElementAtIndex(_selectedIndex);
                valuesProp.DeleteArrayElementAtIndex(_selectedIndex);
                _selectedIndex = -1;
            }
        }
        
        private void HandleSelection(Rect area, int currentIndex, SerializedProperty property)
        {
            if (_dontCheckForNewSelection || Event.current.type != EventType.MouseDown) return;
            if (!area.Contains(Event.current.mousePosition))
                return;
            
            _selectedIndex = currentIndex;
            _dontCheckForNewSelection = true;
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }
        
        private static object GetKeyValue(SerializedProperty property)
        {
            return EditorHelper.GetTargetValue<object>(property);
        }
    }
}
