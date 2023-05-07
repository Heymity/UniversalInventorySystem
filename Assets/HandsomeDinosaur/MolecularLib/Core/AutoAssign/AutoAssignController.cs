using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MolecularLib.AutoAssign
{
    public static class AutoAssignController
    {
        private enum Mode
        {
            None,
            GetComponent,
            GetComponents,
            GetComponentInChild,
            GetComponentsInChild,
            GetComponentInParent,
            GetComponentsInParent,
            Find,
            FindWithTag,
            FindGameObjectsWithTag,
            FindObjectOfType,
            FindObjectsOfType,
            LoadResource,
        }
        
        private struct AutoAssignData
        {
            //public AutoAssignAt AssignAt;
            public Mode AssignMode;
            public MemberInfo MemberInfo;
            public Type ProvidedType;
            public string NameOrTagOrPath;
        }
        
        private static readonly Dictionary<string, List<AutoAssignData>> autoAssignData = new Dictionary<string, List<AutoAssignData>>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Start()
        {
            foreach (var t in TypeLibrary.AllNonUnityAssembliesTypes)
            {
                if (!t.IsSubclassOf(typeof(MonoBehaviour))) continue;
                var useAutoAssignAtt = t.GetCustomAttribute<UseAutoAssignAttribute>();
                if (useAutoAssignAtt is null && !t.IsSubclassOf(typeof(AutoAssignMonoBehaviour))) continue;
                //if (useAutoAssignAtt.DefaultAutoAssignMoment == AutoAssignAt.None) throw new NotSupportedException("Cannot have UseAutoAssignAttribute with DefaultAutoAssignMoment of None");

                var typeName = t.FullName;

                if (typeName is null) throw new Exception("Type name is null");
                
                var members = t.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.SetField);

                foreach (var member in members)
                {
                    Mode mode;
                    var nameOrTagOrPath = "";
                    Type providedType = null;
                    
                    var getCompAtt = member.GetCustomAttribute<GetComponentAttribute>();
                    var getAllCompAtt = member.GetCustomAttribute<GetComponentsAttribute>();
                    var getCompInChildAtt = member.GetCustomAttribute<GetComponentInChildrenAttribute>();
                    var getAllCompInChildAtt = member.GetCustomAttribute<GetComponentsInChildrenAttribute>();
                    var getCompInParentAtt = member.GetCustomAttribute<GetComponentInParentAttribute>();
                    var getAllCompInParentAtt = member.GetCustomAttribute<GetComponentsInParentAttribute>();
                    var findAtt = member.GetCustomAttribute<FindAttribute>();
                    var findTagAtt = member.GetCustomAttribute<FindWithTagAttribute>();
                    var findAllTagAtt = member.GetCustomAttribute<FindGameObjectsWithTag>();
                    var findTypeAtt = member.GetCustomAttribute<FindObjectOfTypeAttribute>();
                    var findAllTypeAtt = member.GetCustomAttribute<FindObjectsOfTypeAttribute>();
                    var loadResourceAtt = member.GetCustomAttribute<LoadResourceAttribute>();
                   
                    if (getCompAtt != null)
                    {
                        mode = Mode.GetComponent;
                        providedType = getCompAtt.ComponentType;
                    }
                    else if (getCompInChildAtt != null)
                    {
                        mode = Mode.GetComponentInChild;
                        providedType = getCompInChildAtt.ComponentType;
                    }
                    else if (getAllCompAtt != null)
                    {
                        mode = Mode.GetComponents;
                        providedType = getAllCompAtt.ComponentType;
                    }
                    else if (getAllCompInChildAtt != null)
                    {
                        mode = Mode.GetComponentsInChild;
                        providedType = getAllCompInChildAtt.ComponentType;
                    }
                    else if (getCompInParentAtt != null)
                    {
                        mode = Mode.GetComponentInParent;
                        providedType = getCompInParentAtt.ComponentType;
                    }
                    else if (getAllCompInParentAtt != null)
                    {
                        mode = Mode.GetComponentsInParent;
                        providedType = getAllCompInParentAtt.ComponentType;
                    }
                    else if (findAtt != null)
                    {
                        mode = Mode.Find;
                        nameOrTagOrPath = findAtt.Name;
                    }
                    else if (findTagAtt != null)
                    {
                        mode = Mode.FindWithTag;
                        nameOrTagOrPath = findTagAtt.Tag;
                    }
                    else if (findAllTagAtt != null)
                    {
                        mode = Mode.FindGameObjectsWithTag;
                        nameOrTagOrPath = findAllTagAtt.Tag;
                    }
                    else if (findTypeAtt != null)
                    {
                        mode = Mode.FindObjectOfType;
                        providedType = findTypeAtt.Type;
                    }
                    else if (findAllTypeAtt != null)
                    {
                        mode = Mode.FindObjectsOfType;
                        providedType = findAllTypeAtt.Type;
                    }
                    else if (loadResourceAtt != null)
                    {
                        mode = Mode.LoadResource;
                        nameOrTagOrPath = loadResourceAtt.ResourcePath;
                    }
                    else continue;
                    
                    var data = new AutoAssignData
                    {
                        //AssignAt = att.OverrideAssignMoment == AutoAssignAt.None ? useAutoAssignAtt.DefaultAutoAssignMoment : att.OverrideAssignMoment,
                        MemberInfo = member,
                        AssignMode = mode,
                        ProvidedType = providedType,
                        NameOrTagOrPath = nameOrTagOrPath,
                    };
                    
                    if (autoAssignData.TryGetValue(typeName, out var datas))
                        datas.Add(data);
                    else
                        autoAssignData.Add(typeName, new List<AutoAssignData> {data});
                }
            }
        }

        public static void AutoAssign(this MonoBehaviour targetMonoBehaviour)
        {
            var targetType = targetMonoBehaviour.GetType();
            
            if (targetType.FullName is null) throw new Exception("Type name is null");
            
            foreach (var assignData in autoAssignData[targetType.FullName])
            {
                switch (assignData.MemberInfo.MemberType)
                {
                    case MemberTypes.Field:
                        var field = (FieldInfo) assignData.MemberInfo;
                        var value = GetValue(targetMonoBehaviour, assignData, field.FieldType);

                        field.SetValue(targetMonoBehaviour, HandleTypeConversions(value, field.FieldType));
                        break;
                    case MemberTypes.Property:
                        var property = (PropertyInfo) assignData.MemberInfo;
                        var valueProp = GetValue(targetMonoBehaviour, assignData, property.PropertyType);

                        property.SetValue(targetMonoBehaviour, HandleTypeConversions(valueProp, property.PropertyType));
                        break;
                    case MemberTypes.All:
                    case MemberTypes.Constructor:
                    case MemberTypes.Custom:
                    case MemberTypes.Event:
                    case MemberTypes.Method:
                    case MemberTypes.NestedType:
                    case MemberTypes.TypeInfo:
                    default:
                        throw new NotSupportedException($"Can only use AutoAssign in fields and properties, not on {assignData.MemberInfo.MemberType}");
                }
            }
        }

        private static object HandleTypeConversions(object currentValue, Type targetType)
        {
            object GetList(Type t)
            {
                var listType = typeof(List<>).MakeGenericType(t);
                var list = Activator.CreateInstance(listType);
                var tmp = ((IEnumerable<object>) currentValue).ToList();

                var cList = (IList) list;
                foreach (var i in tmp)
                {
                    cList.Add(i);
                }

                return list;
            }
            
            if (!currentValue.GetType().IsArray) return currentValue;

            var valuePropElementType = currentValue.GetType().GetElementType();
            var propElementType = targetType.GetArrayOrListElementType();

            if (targetType.IsGenericList())
            {
                currentValue = GetList(propElementType);
            }
            
            if (!valuePropElementType!.IsAssignableFrom(propElementType))
                return currentValue;

            var list = GetList(propElementType);

            if (targetType.IsGenericList())
                currentValue = list;
            else
            {
                var arrayType = propElementType.MakeArrayType();
                var array = Activator.CreateInstance(arrayType, ((IList) list).Count);

                for (var i = 0; i < ((IList) list).Count; i++)
                    ((Array) array).SetValue(((IList) list)[i], i);

                currentValue = array;
            }
            
            return currentValue;
        }

        private static bool IsGenericList(this Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>);

        private static Type GetArrayOrListElementType(this Type t) =>
            t.IsArray ? 
                t.GetElementType()
                : t.GenericTypeArguments[0];

        private static object GetValue(Component targetMonoBehaviour, AutoAssignData data, Type memberType)
        {
            return data.AssignMode switch
            {
                Mode.GetComponent => targetMonoBehaviour.GetComponent(data.ProvidedType ?? memberType),
                Mode.GetComponents => targetMonoBehaviour.GetComponents(data.ProvidedType ?? memberType.GetArrayOrListElementType()),
                Mode.GetComponentInChild => targetMonoBehaviour.GetComponentInChildren(data.ProvidedType ?? memberType),
                Mode.GetComponentsInChild => targetMonoBehaviour.GetComponentsInChildren(data.ProvidedType ?? memberType.GetArrayOrListElementType()),
                Mode.GetComponentInParent => targetMonoBehaviour.GetComponentInParent(data.ProvidedType ?? memberType),
                Mode.GetComponentsInParent => targetMonoBehaviour.GetComponentsInParent(data.ProvidedType ?? memberType.GetArrayOrListElementType()),
                Mode.Find => GameObject.Find(data.NameOrTagOrPath),
                Mode.FindWithTag => GameObject.FindWithTag(data.NameOrTagOrPath),
                Mode.FindGameObjectsWithTag => GameObject.FindGameObjectsWithTag(data.NameOrTagOrPath),
                Mode.FindObjectOfType => Object.FindObjectOfType(data.ProvidedType ?? memberType),
                Mode.FindObjectsOfType => Object.FindObjectsOfType(data.ProvidedType ?? memberType.GetArrayOrListElementType()),
                Mode.LoadResource => Resources.Load(data.NameOrTagOrPath),
                Mode.None => throw new NotSupportedException(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}