using System.Linq;
using System.Reflection;
using MolecularLib.Helpers;
using UnityEditor;
using UnityEngine;

namespace MolecularEditor
{
    [CustomPropertyDrawer(typeof(Range<>), true)]
    public class RangeEditorDrawer : PropertyDrawer
    {
        internal const BindingFlags BindingFlags = System.Reflection.BindingFlags.Instance |
                                                  System.Reflection.BindingFlags.Public |
                                                  System.Reflection.BindingFlags.NonPublic |
                                                  System.Reflection.BindingFlags.DeclaredOnly;

        private IRange _range;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + 8f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _range = GetRange(property);
            
            // Draw background box
            var prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100f;

            GUI.Box(position, GUIContent.none, EditorStyles.helpBox);

            position.x += 4;
            position.width -= 4;
            position.y += 4;
            position.height += 4;

            property.serializedObject.Update();

            position = EditorGUI.PrefixLabel(position, EditorGUI.BeginProperty(position, label, property));
            EditorGUI.BeginChangeCheck();

            ShowNormalMinMax(position, property);
            _range.ValidateMinMaxValues();

            EditorGUIUtility.labelWidth = prevLabelWidth;
            
            EditorGUI.EndProperty();
            
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
                
                Undo.RecordObject(property.serializedObject.targetObject, "Range Changed");
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
        }
        
        internal static void ShowNormalMinMax(Rect position, SerializedProperty property)
        {
            var fieldPos = new Rect(position.x, position.y, (position.width / 2f) - 5f, EditorGUIUtility.singleLineHeight + 2f);

            var minProp = property.FindPropertyRelative("min");
            var maxProp = property.FindPropertyRelative("max");
                
            var prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 30f;
                
            EditorGUI.PropertyField(fieldPos, minProp, new GUIContent("Min"));
            fieldPos.x += fieldPos.width + 5f;
            EditorGUI.PropertyField(fieldPos, maxProp, new GUIContent("Max"));
                
            EditorGUIUtility.labelWidth = prevLabelWidth;
        }

        private static IRange GetRange(SerializedProperty property)
        {
            return EditorHelper.GetTargetValue<IRange>(property);
        }
    }

    [CustomPropertyDrawer(typeof(RangeVector3), true)]
    public class RangeVector3Drawer : PropertyDrawer
    {
        private RangeVector3 _range;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + 8f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _range = GetRange(property);
            
            // Draw background box
            var prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100f;

            GUI.Box(position, GUIContent.none, EditorStyles.helpBox);

            position.x += 4;
            position.width -= 4;
            position.y += 4;
            position.height += 4;

            property.serializedObject.Update();

            position = EditorGUI.PrefixLabel(position, EditorGUI.BeginProperty(position, label, property));
            EditorGUI.BeginChangeCheck();

            ShowMinMax();
            _range.ValidateMinMaxValues();

            EditorGUI.EndProperty();
            
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
               
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }

            void ShowMinMax()
            {
                var fieldPos = new Rect(position.x, position.y, (position.width / 2f) - 5f, EditorGUIUtility.singleLineHeight + 2f);

                var minVec3 = _range.Min;
                var maxVec3 = _range.Max;
                
                EditorGUIUtility.labelWidth = 30f;

                _range.Min = EditorGUI.Vector3Field(fieldPos, new GUIContent("Min"), minVec3);
                fieldPos.x += fieldPos.width + 5f;
                _range.Max = EditorGUI.Vector3Field(fieldPos, new GUIContent("Max"), maxVec3);
                
                EditorGUIUtility.labelWidth = prevLabelWidth;
            }
        }
        
        private static RangeVector3 GetRange(SerializedProperty property)
        {
            return EditorHelper.GetTargetValue<RangeVector3>(property);
        }
    }
    
    [CustomPropertyDrawer(typeof(RangeVector2), true)]
    public class RangeVector2Drawer : PropertyDrawer
    {
        private RangeVector2 _range;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + 8f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _range = GetRange(property);
            
            // Draw background box
            var prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100f;

            GUI.Box(position, GUIContent.none, EditorStyles.helpBox);

            position.x += 4;
            position.width -= 4;
            position.y += 4;
            position.height += 4;

            property.serializedObject.Update();

            position = EditorGUI.PrefixLabel(position, EditorGUI.BeginProperty(position, label, property));
            EditorGUI.BeginChangeCheck();

            ShowMinMax();
            _range.ValidateMinMaxValues();

            EditorGUI.EndProperty();
            
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(property.serializedObject.targetObject);

                property.serializedObject.ApplyModifiedProperties();
            }

            void ShowMinMax()
            {
                var fieldPos = new Rect(position.x, position.y, (position.width / 2f) - 5f, EditorGUIUtility.singleLineHeight + 2f);

                var minVec3 = _range.Min;
                var maxVec3 = _range.Max;
                
                EditorGUIUtility.labelWidth = 30f;

                _range.Min = EditorGUI.Vector2Field(fieldPos, new GUIContent("Min"), minVec3);
                fieldPos.x += fieldPos.width + 5f;
                _range.Max = EditorGUI.Vector2Field(fieldPos, new GUIContent("Max"), maxVec3);
                
                EditorGUIUtility.labelWidth = prevLabelWidth;
            }
        }
        
        private static RangeVector2 GetRange(SerializedProperty property)
        {
            return EditorHelper.GetTargetValue<RangeVector2>(property);
        }
    }
    
    [CustomPropertyDrawer(typeof(RangeVector3Int), true)]
    public class RangeVector3IntDrawer : PropertyDrawer
    {
        private RangeVector3Int _range;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + 8f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _range = GetRange(property);
            
            // Draw background box
            var prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100f;

            GUI.Box(position, GUIContent.none, EditorStyles.helpBox);

            position.x += 4;
            position.width -= 4;
            position.y += 4;
            position.height += 4;

            property.serializedObject.Update();

            position = EditorGUI.PrefixLabel(position, EditorGUI.BeginProperty(position, label, property));
            EditorGUI.BeginChangeCheck();

            ShowMinMax();
            _range.ValidateMinMaxValues();

            EditorGUI.EndProperty();
            
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(property.serializedObject.targetObject);

                property.serializedObject.ApplyModifiedProperties();
            }

            void ShowMinMax()
            {
                var fieldPos = new Rect(position.x, position.y, (position.width / 2f) - 5f, EditorGUIUtility.singleLineHeight + 2f);

                var minVec3 = _range.Min;
                var maxVec3 = _range.Max;
                
                EditorGUIUtility.labelWidth = 30f;

                _range.Min = EditorGUI.Vector3IntField(fieldPos, new GUIContent("Min"), minVec3);
                fieldPos.x += fieldPos.width + 5f;
                _range.Max = EditorGUI.Vector3IntField(fieldPos, new GUIContent("Max"), maxVec3);
                
                EditorGUIUtility.labelWidth = prevLabelWidth;
            }
        }
        
        private static RangeVector3Int GetRange(SerializedProperty property)
        {
            return EditorHelper.GetTargetValue<RangeVector3Int>(property);
        }
    }
    
    [CustomPropertyDrawer(typeof(RangeVector2Int), true)]
    public class RangeVector2IntDrawer : PropertyDrawer
    {
        private RangeVector2Int _range;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + 8f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _range = GetRange(property);
            
            // Draw background box
            var prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100f;

            GUI.Box(position, GUIContent.none, EditorStyles.helpBox);

            position.x += 4;
            position.width -= 4;
            position.y += 4;
            position.height += 4;

            property.serializedObject.Update();

            position = EditorGUI.PrefixLabel(position, EditorGUI.BeginProperty(position, label, property));
            EditorGUI.BeginChangeCheck();

            ShowMinMax();
            _range.ValidateMinMaxValues();

            EditorGUI.EndProperty();
            
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(property.serializedObject.targetObject);

                property.serializedObject.ApplyModifiedProperties();
            }

            void ShowMinMax()
            {
                var fieldPos = new Rect(position.x, position.y, (position.width / 2f) - 5f, EditorGUIUtility.singleLineHeight + 2f);

                var minVec2 = _range.Min;
                var maxVec2 = _range.Max;
                
                EditorGUIUtility.labelWidth = 30f;

                _range.Min = EditorGUI.Vector2IntField(fieldPos, new GUIContent("Min"), minVec2);
                fieldPos.x += fieldPos.width + 5f;
                _range.Max = EditorGUI.Vector2IntField(fieldPos, new GUIContent("Max"), maxVec2);
                
                EditorGUIUtility.labelWidth = prevLabelWidth;
            }
        }
        
        private static RangeVector2Int GetRange(SerializedProperty property)
        {
            return EditorHelper.GetTargetValue<RangeVector2Int>(property);
        }
    }

    [CustomPropertyDrawer(typeof(Range), true)]
    public class FloatRangeEditorDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + 8f;
        }

        private const float LabelWidth = 100f;
        private const float ReducedControlSize = 90f;
        private const float Padding = 10f;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100f;

            GUI.Box(position, GUIContent.none, EditorStyles.helpBox);

            position.x += 4;
            position.width -= 4;
            position.y += 4;
            position.height = EditorGUIUtility.singleLineHeight + 2f;

            EditorGUI.BeginChangeCheck();

            var range = EditorHelper.GetTargetValue<Range>(property);
            
            var attrs = fieldInfo.GetCustomAttributes<MinMaxRangeAttribute>().ToList();
            var newLabel = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PrefixLabel(position, newLabel);

            var hasMinMaxRangeDefined = attrs.Any();
            var maxLimit = 1f;
            var minLimit = -1f;
            if (hasMinMaxRangeDefined)
            {
                maxLimit = attrs.FirstOrDefault()?.Max ?? 1;
                minLimit = attrs.FirstOrDefault()?.Min ?? -1;
            }
            
            position.x += LabelWidth;
            position.width -= LabelWidth + 4;
            
            if (hasMinMaxRangeDefined)
                ShowMinMaxRange();
            else
                RangeEditorDrawer.ShowNormalMinMax(position, property);

            if (hasMinMaxRangeDefined && range.Min < minLimit) range.Min = minLimit;
            if (hasMinMaxRangeDefined && range.Max > maxLimit) range.Max = maxLimit;

            EditorGUI.EndProperty();
            EditorGUIUtility.labelWidth = prevLabelWidth;

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(property.serializedObject.targetObject);
                property.serializedObject.ApplyModifiedProperties();
                range.ValidateMinMaxValues(); 
            }

            void ShowMinMaxRange()
            {
                EditorGUIUtility.labelWidth = 30f;
                
                var minValuePos = new Rect(position.x, position.y, ReducedControlSize, position.height);
                range.Min = EditorGUI.FloatField(minValuePos, "Min", range.Min);

                var minMaxSliderPos = new Rect(minValuePos.x + minValuePos.width + Padding, position.y, position.width - (2 * ReducedControlSize) - (2 * Padding), position.height);

                var maxValuePos = new Rect(minMaxSliderPos.x + minMaxSliderPos.width + Padding, position.y, ReducedControlSize, position.height);
                range.Max = EditorGUI.FloatField(maxValuePos, "Max", range.Max);

                var min = range.Min;
                var max = range.Max;

                EditorGUI.MinMaxSlider(minMaxSliderPos, ref min, ref max, minLimit, maxLimit);

                if (System.Math.Abs(range.Min - min) > 0.00000000000000001f)
                    range.Min = min;
                if (System.Math.Abs(range.Max - max) > 0.00000000000000001f)
                    range.Max = max;
            }
        }
    }

    [CustomPropertyDrawer(typeof(RangeInteger), true)]
    public class IntRangeEditorDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded ? 2 * (EditorGUIUtility.singleLineHeight + 8f) : EditorGUIUtility.singleLineHeight + 8f;
        }

        private const float LabelWidth = 100f;
        private const float ReducedControlSize = 90f;
        private const float Padding = 10f;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var prevLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100f;

            GUI.Box(position, GUIContent.none, EditorStyles.helpBox);

            position.x += 4;
            position.width -= 4;
            position.y += 4;
            position.height = EditorGUIUtility.singleLineHeight + 2f;
            
            EditorGUI.BeginChangeCheck();
            property.serializedObject.Update();

            var range = EditorHelper.GetTargetValue<RangeInteger>(property);
            
            var attrs = fieldInfo.GetCustomAttributes<MinMaxRangeAttribute>().ToList();
            var newLabel = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PrefixLabel(position, newLabel);

            var hasMinMaxRangeDefined = attrs.Any();
            var maxLimit = 1;
            var minLimit = -1;
            if (hasMinMaxRangeDefined)
            {
                maxLimit = Mathf.RoundToInt(attrs.FirstOrDefault()?.Max ?? 1);
                minLimit = Mathf.RoundToInt(attrs.FirstOrDefault()?.Min ?? -1);
            }
            
            position.x += LabelWidth;
            position.width -= LabelWidth + 4;
            
            if (hasMinMaxRangeDefined)
                ShowMinMaxRange();
            else
                RangeEditorDrawer.ShowNormalMinMax(position, property);

            if (hasMinMaxRangeDefined && range.Min < minLimit) range.Min = minLimit;
            if (hasMinMaxRangeDefined && range.Max > maxLimit) range.Max = maxLimit;

            EditorGUI.EndProperty();
            EditorGUIUtility.labelWidth = prevLabelWidth;

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(property.serializedObject.targetObject);
                range.ValidateMinMaxValues();
                property.serializedObject.ApplyModifiedProperties();
            }

            property.serializedObject.ApplyModifiedProperties();

            void ShowMinMaxRange()
            {
                EditorGUIUtility.labelWidth = 30f;
                
                var minValuePos = new Rect(position.x, position.y, ReducedControlSize, position.height);
                range.Min = EditorGUI.IntField(minValuePos, "Min", range.Min);

                var minMaxSliderPos = new Rect(minValuePos.x + minValuePos.width + Padding, position.y, position.width - (2 * ReducedControlSize) - (2 * Padding), position.height);

                var maxValuePos = new Rect(minMaxSliderPos.x + minMaxSliderPos.width + Padding, position.y, ReducedControlSize, position.height);
                range.Max = EditorGUI.IntField(maxValuePos, "Max", range.Max);

                float min = range.Min;
                float max = range.Max;

                EditorGUI.MinMaxSlider(minMaxSliderPos, ref min, ref max, minLimit, maxLimit);

                if (System.Math.Abs(range.Min - min) > 0.00000000000000001f) 
                    range.Min = Mathf.RoundToInt(min);
                if (System.Math.Abs(range.Max - max) > 0.00000000000000001f) 
                    range.Max = Mathf.RoundToInt(max);
            }
        }
    }
}
