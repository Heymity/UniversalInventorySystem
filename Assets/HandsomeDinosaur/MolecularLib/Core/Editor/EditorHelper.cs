using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using MolecularLib;
using MolecularLib.Helpers;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace MolecularEditor
{
    public static class EditorHelper
    {
        #region IMGUI

        private static readonly GUIStyle HeaderBackground = "RL Header";
        private static readonly GUIStyle BoxBackground = "RL Background";

        // TODO Make a foldout option with this
        public static Rect DrawBoxWithTitle(Rect totalPos, GUIContent tittle)
        {
            if (Event.current.type == EventType.Repaint)
                HeaderBackground.Draw(totalPos, false, false, false, false);
   
            var fieldRect = totalPos;
            fieldRect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(fieldRect, tittle);
  
            var boxRect = fieldRect;
            boxRect.y += fieldRect.height;
            boxRect.height = totalPos.height - fieldRect.height;
            if (Event.current.type == EventType.Repaint)
                BoxBackground.Draw(boxRect, false, false, false, false);

            var boxAreaRect = boxRect;
            boxAreaRect.height -= 6;
            boxAreaRect.y += 3;

            return boxAreaRect;
        }

        public static Rect BeginBoxWithTittle(GUIContent tittle, params GUILayoutOption[] options)
        {
            var tittleRect = EditorGUILayout.GetControlRect();
            
            if (Event.current.type == EventType.Repaint)
                HeaderBackground.Draw(tittleRect, false, false, false, false);

            tittleRect.x += 2;
            tittleRect.height = EditorGUIUtility.singleLineHeight + 2;
            EditorGUI.LabelField(tittleRect, tittle);

            var paddingStyle = new GUIStyle
            {
                padding = new RectOffset(8, 8, 8, 8)
            };
            var verticalRect = EditorGUILayout.BeginVertical(paddingStyle, options);
            
            if (Event.current.type == EventType.Repaint)
                BoxBackground.Draw(verticalRect, false, false, false, false);

            return verticalRect;
        }

        public static void EndBoxWithTittle()
        {
            EditorGUILayout.EndVertical();
        }
        
        public static void AutoTypeFieldInfo(ref Rect rect, FieldInfo fi, string uniqueIdentifier, object targetObj, string label = null)
        {
            var field = fi.GetValue(targetObj);
            label ??= fi.Name;

            fi.SetValue(targetObj, AutoTypeField(ref rect, fi.FieldType, field, uniqueIdentifier, label, fi.GetCustomAttributesData()));
        }

        public static void AutoTypePropertyInfo(ref Rect rect, PropertyInfo pi, string uniqueIdentifier, object targetObj, string label = null)
        {
            var field = pi.GetValue(targetObj);
            label ??= pi.Name;

            // some properties dont have the set method, to be refactored
            pi.SetValue(targetObj, AutoTypeField(ref rect, pi.PropertyType, field, uniqueIdentifier, label, pi.GetCustomAttributesData()));
        }
        
        private static readonly Dictionary<string, Type> CachedRuntimeTypesForAutoTypeField =
            new SerializableDictionary<string, Type>();
        private static readonly Dictionary<string, ScriptableObject> CachedRuntimeScriptableObjectsForAutoTypeField =
            new SerializableDictionary<string, ScriptableObject>();
        public static object AutoTypeField(ref Rect rect, Type valueType, object value, string uniqueIdentifier, string labelStr = null, IList<CustomAttributeData> attributes = null)
        {
            var label = GUIContent.none;
            if (!string.IsNullOrEmpty(labelStr)) label = new GUIContent(labelStr);

            if (attributes != null && attributes.Any(a => a.AttributeType != typeof(SerializeField))) goto DrawAnyType;
            if (GetPropertyDrawer(valueType) != null) goto DrawAnyType;
            
            if (valueType == typeof(int)) return EditorGUI.IntField(rect, label, value is int i ? i : 0);
            if (valueType == typeof(float)) return EditorGUI.FloatField(rect, label, value is float i ? i : 0);
            if (valueType == typeof(double)) return EditorGUI.DoubleField(rect, label, value is double i ? i : 0);
            if (valueType == typeof(bool)) return EditorGUI.Toggle(rect, label, value is bool i && i);
            if (valueType == typeof(string)) return EditorGUI.TextField(rect, label, value is string i ? i : "");
            if (valueType == typeof(Vector4)) return EditorGUI.Vector4Field(rect, label, value is Vector4 vector4 ? vector4 : Vector4.zero);
            if (valueType == typeof(Vector3)) return EditorGUI.Vector3Field(rect, label, value is Vector3 vec3 ? vec3 : Vector3.zero);
            if (valueType == typeof(Vector3Int)) return EditorGUI.Vector3IntField(rect, label, value is Vector3Int vec3Int ? vec3Int : Vector3Int.zero);
            if (valueType == typeof(Vector2)) return EditorGUI.Vector2Field(rect, label, value is Vector2 vec2 ? vec2 : Vector2.zero);
            if (valueType == typeof(Vector2Int)) return EditorGUI.Vector2IntField(rect, label, value is Vector2Int vec2Int ? vec2Int : Vector2Int.zero);
            if (valueType == typeof(Rect)) return EditorGUI.RectField(rect, label, value is Rect rect2 ? rect2 : Rect.zero);
            if (valueType == typeof(RectInt)) return EditorGUI.RectIntField(rect, label, value is RectInt rectInt ? rectInt : new RectInt());
            if (valueType == typeof(Bounds)) return EditorGUI.BoundsField(rect, label, value is Bounds bounds ? bounds : new Bounds(Vector3.zero, Vector3.one));
            if (valueType == typeof(BoundsInt)) return EditorGUI.BoundsIntField(rect, label, value is BoundsInt boundsInt ? boundsInt : new BoundsInt(Vector3Int.zero, Vector3Int.one));
            if (valueType == typeof(Color)) return EditorGUI.ColorField(rect, label, value is Color color ? color : Color.white);
            if (valueType == typeof(Color32)) return EditorGUI.ColorField(rect, label, value is Color32 color32 ? color32 : Color.white.ToColor32());
            if (valueType == typeof(LayerMask)) return EditorGUI.LayerField(rect, label, value is LayerMask layerMask ? layerMask : (LayerMask)0);
            if (valueType == typeof(AnimationCurve)) return EditorGUI.CurveField(rect, label, value is AnimationCurve curve ? curve : AnimationCurve.Linear(0, 0, 1, 1));
            if (valueType == typeof(Gradient)) return EditorGUI.GradientField(rect, label, value is Gradient gradient ? gradient : new Gradient());
            
            if (valueType == typeof(Quaternion))
            {
                var quaternion = value is Quaternion qua ? qua : Quaternion.identity;
                var quaternionAsVec4 = new Vector4(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
                
                var result = EditorGUI.Vector4Field(rect, label, quaternionAsVec4);
                
                return new Quaternion(result.x, result.y, result.z, result.w);
            }

            if (valueType.IsEnum)
                return EditorGUI.EnumPopup(rect, label, (Enum) Enum.Parse(valueType, value?.ToString() ?? ""));

            if (valueType.IsSubclassOf(typeof(Object)))
                return EditorGUI.ObjectField(rect, label, value as Object, valueType, true);
            
            DrawAnyType:
            if (valueType.GetCustomAttribute<SerializableAttribute>() != null || typeof(Object).IsAssignableFrom(valueType))
            {
                /*
                To support user defined types we need this weird code below
                Basically, we could just use normal reflections to get the fields and than draw them, but that would 
                ignore custom property drawers.
                To use them, we need a serializedProperty, which we dont have, that's the whole purpose of this function.
                So what this code does is that it creates a runtime type deriving from ScriptableObject with only one field,
                the value. Than a instance of this scriptable object is created and the value is assigned to it.
                Now, all we do is create a new SerializedObject from this scriptable object, and call PropertyField on its property.
                This works for all types marked with the Serializable attribute. The reason this is not the whole method is that it
                is considerably slower than just converting the value and calling the EditorGUI.TYPEField(...) like it is done above.
                */

                var runtimeTypeName = $"RuntimeSerializableObjectFor{valueType.FullName?.Replace('.', '_') ?? valueType.Name.Replace('.', '_')}";

                SerializedObject serializedObject;
                ScriptableObject so = null;
                if (!CachedRuntimeTypesForAutoTypeField.TryGetValue(runtimeTypeName + uniqueIdentifier, out var dynamicType))
                {
                    var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                        new AssemblyName("RuntimeSerializableObjectAssembly"), AssemblyBuilderAccess.Run);
                    var moduleBuilder = assemblyBuilder.DefineDynamicModule("RuntimeSerializableObjectModule");
                    var typeBuilder = moduleBuilder.DefineType(runtimeTypeName + uniqueIdentifier, TypeAttributes.Public,
                        typeof(ScriptableObject));

                    var fieldBuilder = typeBuilder.DefineField("value", valueType, FieldAttributes.Public);
                    if (attributes != null)
                    {
                        foreach (var attr in attributes)
                        {
                            if (attr.AttributeType == typeof(SerializeField)) continue;
                            var attrBuilder = new CustomAttributeBuilder(attr.Constructor,
                                attr.ConstructorArguments.Select(a => a.Value).ToArray());
                            fieldBuilder.SetCustomAttribute(attrBuilder);
                        }
                    }

                    dynamicType = typeBuilder.CreateType();
                    
                    CachedRuntimeTypesForAutoTypeField.Add(runtimeTypeName + uniqueIdentifier, dynamicType);
                }
                
                (serializedObject, so) = GetSerializedObjectAndScriptableObject(runtimeTypeName + uniqueIdentifier, dynamicType, value);
                
                if (so == null) so = GetScriptableObject(runtimeTypeName + uniqueIdentifier, dynamicType);
                var property = serializedObject.GetIterator();
                
                property.NextVisible(true); // Move to first property
                property.NextVisible(true); // Skip m_Script
                
                rect.height = EditorGUI.GetPropertyHeight(property);
                EditorGUI.PropertyField(rect, property, label,true);
                
                var valueField = dynamicType.GetField("value");
                return valueField.GetValue(so);
            }

            EditorGUI.LabelField(rect, label, new GUIContent(value?.ToString() ?? $"The provided value of type {valueType.Name} is null and is not supported"));

            return value;
        }
        
        private static (SerializedObject serializedObject, ScriptableObject so) GetSerializedObjectAndScriptableObject(string runtimeTypeName, Type dynamicType, object value)
        {
            var so = GetScriptableObject(runtimeTypeName, dynamicType);
                    
            var valueField = dynamicType.GetField("value");
            valueField.SetValue(so, value);

            return (new SerializedObject(so), so);
        }

        private static ScriptableObject GetScriptableObject(string runtimeTypeName, Type dynamicType)
        {
            if (CachedRuntimeScriptableObjectsForAutoTypeField.TryGetValue(runtimeTypeName,
                    out var scriptableObject))
                return scriptableObject;
                
            scriptableObject = ScriptableObject.CreateInstance(dynamicType);
            scriptableObject.hideFlags = HideFlags.DontSave;
                
            CachedRuntimeScriptableObjectsForAutoTypeField.Add(runtimeTypeName, scriptableObject);

            return scriptableObject;
        }
        
        public static float AutoTypeFieldGetHeight(Type valueType, object value, string uniqueIdentifier, string labelStr = null, IList<CustomAttributeData> attributes = null)
        {
            var label = GUIContent.none;
            if (!string.IsNullOrEmpty(labelStr)) label = new GUIContent(labelStr);
            
            if (attributes != null && attributes.Any(a => a.AttributeType != typeof(SerializeField))) goto DrawAnyType;
            if (GetPropertyDrawer(valueType) != null) goto DrawAnyType;
            
            if (valueType == typeof(int)) return EditorGUI.GetPropertyHeight(SerializedPropertyType.Integer, label);
            if (valueType == typeof(float)) return EditorGUI.GetPropertyHeight(SerializedPropertyType.Float, label);
            if (valueType == typeof(double)) return EditorGUI.GetPropertyHeight(SerializedPropertyType.Float, label);
            if (valueType == typeof(bool)) return EditorGUI.GetPropertyHeight(SerializedPropertyType.Boolean, label);
            if (valueType == typeof(string)) return EditorGUI.GetPropertyHeight(SerializedPropertyType.String, label);
            if (valueType == typeof(Vector2)) return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector2, label);
            if (valueType == typeof(Vector3)) return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3, label);
            if (valueType == typeof(Vector4)) return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector4, label);
            if (valueType == typeof(Vector2Int)) return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector2Int, label);
            if (valueType == typeof(Vector3Int)) return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3Int, label);
            if (valueType == typeof(Rect)) return EditorGUI.GetPropertyHeight(SerializedPropertyType.Rect, label);
            if (valueType == typeof(RectInt)) return EditorGUI.GetPropertyHeight(SerializedPropertyType.RectInt, label);
            if (valueType == typeof(Bounds)) return EditorGUI.GetPropertyHeight(SerializedPropertyType.Bounds, label);
            if (valueType == typeof(BoundsInt)) return EditorGUI.GetPropertyHeight(SerializedPropertyType.BoundsInt, label);
            if (valueType == typeof(Color)) return EditorGUI.GetPropertyHeight(SerializedPropertyType.Color, label);
            if (valueType == typeof(Color32)) return EditorGUI.GetPropertyHeight(SerializedPropertyType.Color, label);
            if (valueType == typeof(LayerMask)) return EditorGUI.GetPropertyHeight(SerializedPropertyType.LayerMask, label);
            if (valueType == typeof(AnimationCurve)) return EditorGUI.GetPropertyHeight(SerializedPropertyType.AnimationCurve, label);
            if (valueType == typeof(Gradient)) return EditorGUI.GetPropertyHeight(SerializedPropertyType.Gradient, label);
            if (valueType == typeof(Quaternion)) return EditorGUI.GetPropertyHeight(SerializedPropertyType.Quaternion, label);
            
            if (valueType.IsEnum) return EditorGUI.GetPropertyHeight(SerializedPropertyType.Enum, label);
            
            if (valueType.IsSubclassOf(typeof(Object))) return EditorGUI.GetPropertyHeight(SerializedPropertyType.ObjectReference, label);

            DrawAnyType:
            if (valueType.GetCustomAttribute<SerializableAttribute>() != null || typeof(Object).IsAssignableFrom(valueType))
            {
                var runtimeTypeName = $"RuntimeSerializableObjectFor{valueType.FullName?.Replace('.', '_') ?? valueType.Name.Replace('.', '_')}";

                SerializedObject serializedObject;
                ScriptableObject so = null;
                if (!CachedRuntimeTypesForAutoTypeField.TryGetValue(runtimeTypeName + uniqueIdentifier, out var dynamicType))
                {
                    var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                        new AssemblyName("RuntimeSerializableObjectAssembly"), AssemblyBuilderAccess.Run);
                    var moduleBuilder = assemblyBuilder.DefineDynamicModule("RuntimeSerializableObjectModule");
                    var typeBuilder = moduleBuilder.DefineType(runtimeTypeName + uniqueIdentifier, TypeAttributes.Public,
                        typeof(ScriptableObject));

                    var fieldBuilder = typeBuilder.DefineField("value", valueType, FieldAttributes.Public);
                    if (attributes != null)
                    {
                        foreach (var attr in attributes)
                        {
                            if (attr.AttributeType == typeof(SerializeField)) continue;
                            var attrBuilder = new CustomAttributeBuilder(attr.Constructor,
                                attr.ConstructorArguments.Select(a => a.Value).ToArray());
                            fieldBuilder.SetCustomAttribute(attrBuilder);
                        }
                    }
                    
                    dynamicType = typeBuilder.CreateType();
                    
                    CachedRuntimeTypesForAutoTypeField.Add(runtimeTypeName + uniqueIdentifier, dynamicType);
                }
                
                (serializedObject, so) = GetSerializedObjectAndScriptableObject(runtimeTypeName + uniqueIdentifier, dynamicType, value);

                if (so == null) so = GetScriptableObject(runtimeTypeName + uniqueIdentifier, dynamicType);
                
                var property = serializedObject.GetIterator();
                
                property.NextVisible(true); // Move to first property
                property.NextVisible(true); // Skip m_Script
                
                return EditorGUI.GetPropertyHeight(property);
            }
            
            return EditorGUIUtility.singleLineHeight;
        }

        public static Type TypeField<TBaseClass>(Rect rect, string label, Type currentValue, bool showBaseType)
        {
            return TypeField(rect, label, currentValue, typeof(TBaseClass), showBaseType);
        }

        public static Type TypeField(Rect rect, string label, Type currentValue, Type baseType, bool showBaseType)
        {
            var types = GetTypesForPopup(baseType, showBaseType);

            var r = DrawTypeField(rect, label, types, currentValue);
            if (r != currentValue) GUI.changed = true;
            
            return r;
        }

        public static Type DrawTypeField(Rect rect, string label, List<Type> types, Type current)
        {
            var selected = types.FindIndex(t => t == current);
            if (selected <= 0) selected = 0;

            var popupNames = types.Select(t => t.FullName?.Replace('.', '/')).ToArray();

            selected = EditorGUI.Popup(
                rect,
                label,
                selected,
                popupNames);

            return types.Count <= 0 ? null : types[selected];
        }

        #endregion

        #region UIElements

        public static void LinkNodes(this GraphView graphView, Port outputPort, Port inputPort)
        {
            var tmpEdge = new Edge
            {
                output = outputPort,
                input = inputPort
            };

            tmpEdge.input.Connect(tmpEdge);
            tmpEdge.output.Connect(tmpEdge);

            graphView.Add(tmpEdge);
        }

        public static Button ButtonWithText(Action clickEvent, string text)
        {
            var btn = new Button(clickEvent)
            {
                text = text
            };

            return btn;
        }

        public static ToolbarButton ToolbarButtonWithText(Action clickEvent, string text)
        {
            var btn = new ToolbarButton(clickEvent)
            {
                text = text
            };

            return btn;
        }

        public static void MakeBorder(this VisualElement container, float borderWidth, Color borderColor, float borderCornerRadius = 0)
        {
            container.MakeTopBorder(borderWidth, borderColor, borderCornerRadius);
            container.MakeBottomBorder(borderWidth, borderColor, borderCornerRadius);
            container.MakeLeftBorder(borderWidth, borderColor, borderCornerRadius);
            container.MakeRightBorder(borderWidth, borderColor, borderCornerRadius);
        }

        public static void MakeTopBorder(this VisualElement container, float borderWidth, Color borderColor, float borderCornerRadius = 0)
        {
            container.style.borderTopWidth = borderWidth;
            container.style.borderTopColor = borderColor;
            container.style.borderTopLeftRadius = borderCornerRadius;
            container.style.borderTopLeftRadius = borderCornerRadius;
        }
        public static void MakeBottomBorder(this VisualElement container, float borderWidth, Color borderColor, float borderCornerRadius = 0)
        {
            container.style.borderBottomWidth = borderWidth;
            container.style.borderBottomColor = borderColor;
            container.style.borderBottomLeftRadius = borderCornerRadius;
            container.style.borderBottomRightRadius = borderCornerRadius;
        }
        public static void MakeLeftBorder(this VisualElement container, float borderWidth, Color borderColor, float borderCornerRadius = 0)
        {
            container.style.borderLeftWidth = borderWidth;
            container.style.borderLeftColor = borderColor;
            container.style.borderTopLeftRadius = borderCornerRadius;
            container.style.borderBottomLeftRadius = borderCornerRadius;
        }
        public static void MakeRightBorder(this VisualElement container, float borderWidth, Color borderColor, float borderCornerRadius = 0)
        {
            container.style.borderRightWidth = borderWidth;
            container.style.borderRightColor = borderColor;
            container.style.borderTopRightRadius = borderCornerRadius;
            container.style.borderBottomRightRadius = borderCornerRadius;
        }

        #endregion

        #region General Utilities

        public const BindingFlags UnitySerializesBindingFlags = BindingFlags.Instance |
                                                                BindingFlags.Public |
                                                                BindingFlags.NonPublic |
                                                                BindingFlags.FlattenHierarchy;

        private static List<Type> _cachedTypes;
        public static IReadOnlyList<Type> AllTypes
        {
            get
            {
                if (_cachedTypes != null && _cachedTypes.Count > 0) return _cachedTypes;
                
                if (TypeLibrary.AllAssemblies == null || TypeLibrary.AllAssemblies.Count == 0)
                {
                    TypeLibrary.BootstrapEditor();
                }
                
                _cachedTypes = TypeLibrary.AllAssemblies!.SelectMany(a => a.Value.GetTypes()).ToList();
                return _cachedTypes;
            }
        }

        private static readonly Dictionary<string, List<Type>> cachedDerivedTypes = new Dictionary<string, List<Type>>();

        public static List<Type> GetTypesForPopup<TBaseClass>(bool showBaseType) => GetTypesForPopup(typeof(TBaseClass), showBaseType);
        public static List<Type> GetTypesForPopup(Type baseType, bool showBaseType)
        {
            if (cachedDerivedTypes.TryGetValue(baseType.ToString(), out var derivedTypes))
                return derivedTypes;
            
            var types = AllTypes.Where(baseType.IsAssignableFrom).Where(t => showBaseType || t != baseType).ToList();
            cachedDerivedTypes.Add(baseType.ToString(), types);

            return types;
        }

        public static Texture2D Tex2DOfColorScreenSize(Color32 color) => Tex2DOfColorAndSize(color, Screen.width, Screen.height);

        public static Texture2D Tex2DOfColorAndSize(Color32 color, int width, int height)
        {
            var texture = new Texture2D(width, height);
            var pixels = Enumerable.Repeat(color, width * height).ToArray();
            texture.SetPixels32(pixels);
            texture.Apply();
            return texture;
        }
        
        public static Texture2D Tex2DOfColorScreenSize(Color color) => Tex2DOfColorScreenSize(color.ToColor32());

        public static Texture2D Tex2DOfColorAndSize(Color color, int width, int height) => Tex2DOfColorAndSize(color.ToColor32(), width, height);

        public static Texture2D Tex2DOfColor(Color color)
        {
            var newTex = new Texture2D(1, 1);
            newTex.SetPixel(0, 0, color);
            newTex.Apply();
            return newTex;
        }
 
        public static T GetTargetValue<T>(SerializedProperty property)
        {
            return (T)GetTargetValue(property);
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        public static object GetTargetValue(SerializedProperty property, int getParentOfIndex = 0)
        {
            var propertyPath = property.propertyPath;
            var pathSegments = propertyPath.Split('.');
            
            object currentSearchObj = property.serializedObject.targetObject;
            var currentSearchObjType = currentSearchObj.GetType();

            if (getParentOfIndex >= pathSegments.Length)
            {
                if (getParentOfIndex == pathSegments.Length) return property.serializedObject.targetObject;
                Debug.LogError($"Cannot retrieve nonexistent object: Requested to get father of index {getParentOfIndex} in property {propertyPath}, which is outside bounds");
            }
            
            for (var i = 0; i < pathSegments.Length - getParentOfIndex; i++)
            {
                var pathSegment = pathSegments[i];
                
                if (pathSegment == "Array" && i < pathSegments.Length - 1 && pathSegments[i + 1].Split('[')[0] == "data")
                {
                    var arrayIndex = int.Parse(pathSegments[i + 1].Split('[', ']')[1]);
                    
                    currentSearchObj = ((IList)currentSearchObj)[arrayIndex];
                    currentSearchObjType = currentSearchObj.GetType();
                    
                    // Skip the data[] part of the path
                    i++;
                    continue;
                }

                var fieldInfo = currentSearchObjType.GetField(pathSegment, UnitySerializesBindingFlags);

                if (fieldInfo == null)
                {
                    Debug.LogError($"Field {pathSegment} not found on {currentSearchObjType}");
                    return default;
                }

                currentSearchObj = fieldInfo.GetValue(currentSearchObj);
                currentSearchObjType = currentSearchObj.GetType();
            }

            return currentSearchObj;
        }

        internal static bool IsGenericList(this object o)
        {
            var oType = o.GetType();
            return oType.IsGenericType && oType.GetGenericTypeDefinition() == typeof(List<>);
        }

        private static MethodInfo _getDrawerTypeForTypeFuncCached;
        private static object _scriptAttributeUtilityCached;
        public static Type GetPropertyDrawer(Type classType)
        {
            if (_getDrawerTypeForTypeFuncCached == null || _scriptAttributeUtilityCached == null)
            {
                var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
                var scriptAttributeUtility = assembly.CreateInstance("UnityEditor.ScriptAttributeUtility");
                var scriptAttributeUtilityType = scriptAttributeUtility?.GetType();

                _scriptAttributeUtilityCached = scriptAttributeUtility;
                
                const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Static;
                _getDrawerTypeForTypeFuncCached = scriptAttributeUtilityType?.GetMethod("GetDrawerTypeForType", bindingFlags);
            }

            return (Type)_getDrawerTypeForTypeFuncCached?.Invoke(_scriptAttributeUtilityCached, new object[] { classType });
        }
        
        #endregion
    }
}
