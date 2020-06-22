using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UniversalInventorySystem;

[CustomPropertyDrawer(typeof(Slot))]
public class SlotDrawer : PropertyDrawer
{
    public Dictionary<string, SlotInfo> unfold = new Dictionary<string, SlotInfo>();

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (unfold.ContainsKey(property.propertyPath))
             return unfold[property.propertyPath].boolValue ? 18 * unfold[property.propertyPath].fieldAmount : 18;
         else
            return 18;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var t = label.text.Split(' ');
        label.text = "Slot " + t[t.Length - 1];

        if (!unfold.ContainsKey(property.propertyPath))
             unfold.Add(property.propertyPath, new SlotInfo(false, 1, false, 0, false, false));

        var originalPosition = position;
        position = EditorGUI.PrefixLabel(position, label);

        originalPosition.width = position.x - 20;
        unfold[property.propertyPath].boolValue = EditorGUI.Foldout(originalPosition, unfold[property.propertyPath].boolValue, label, true);

        if (unfold[property.propertyPath].boolValue)
        {
            Rect ampos = new Rect(originalPosition.x, originalPosition.y + 18f, 100, 18);
            unfold[property.propertyPath].fieldAmount = 2.5f;

            var durRect = ampos;
            durRect.width = position.width;
            var durability = property.FindPropertyRelative("durability");
            var hasItem = property.FindPropertyRelative("hasItem");
            var slotItem = property.FindPropertyRelative("item");
            if (hasItem.boolValue && (slotItem.objectReferenceValue as Item) != null)
            {
                unfold[property.propertyPath].fieldAmount = 3.5f;
                EditorGUI.IntSlider(durRect, durability, 0, (int)(slotItem.objectReferenceValue as Item).maxDurability, "Durability");
                ampos.y += ampos.height;
            }

            bool amBool = unfold[property.propertyPath].multipleAssign;

            bool _amTmp = GUI.Button(ampos, amBool ? "Save variable" : "Assign multiple");

            bool amTmp = _amTmp ? !amBool : amBool;

            if (amTmp != amBool)
            {
                unfold[property.propertyPath].multipleAssign = amTmp;
                unfold[property.propertyPath].executeOnce = false;
            }

            var whitelistProp = property.FindPropertyRelative("whitelist");
            if (amBool)
            {
                var foldPos = new Rect(position.x, position.y + 18f, position.width, position.height);
                foldPos.y = ampos.y;
                bool useItemAsset = EditorPrefs.GetBool(property.propertyPath, true);

                Rect uia = new Rect(foldPos.x, foldPos.y, 120, foldPos.height);
                bool tmp = EditorGUI.Toggle(uia, "Use ItemGroup", useItemAsset);
                foldPos.x += 120;

                if ((tmp != useItemAsset || !unfold[property.propertyPath].executeOnce) && amTmp == amBool) 
                {
                    unfold[property.propertyPath].executeOnce = true;
                    EditorPrefs.SetBool(property.propertyPath, tmp);
                    unfold[property.propertyPath].objs = new List<Object>();

                    if (unfold[property.propertyPath].editorAssignSize < 0) unfold[property.propertyPath].editorAssignSize = 0;

                    unfold[property.propertyPath].objs = new List<Object>(unfold[property.propertyPath].editorAssignSize);
                    for (int i = 0; i < unfold[property.propertyPath].objs.Capacity; i++)
                    {
                        if (i >= unfold[property.propertyPath].objs.Count) unfold[property.propertyPath].objs.Add(null);
                    }
                    
                    if (tmp)
                    {
                        if (whitelistProp.objectReferenceValue != null)
                        {
                            if (unfold[property.propertyPath].editorAssignSize < 1) unfold[property.propertyPath].editorAssignSize = 1;
                            if (unfold[property.propertyPath].objs.Count >= 1) unfold[property.propertyPath].objs[0] = whitelistProp.objectReferenceValue;
                        }
                    } else
                    {
                        if (whitelistProp.objectReferenceValue != null)
                        {
                            var ia = whitelistProp.objectReferenceValue as ItemGroup;

                            if (unfold[property.propertyPath].editorAssignSize < ia.itemsList.Count) unfold[property.propertyPath].editorAssignSize = ia.itemsList.Count;
                            for (int i = 0; i < ia.itemsList.Count; i++)
                            {
                                if (i < unfold[property.propertyPath].objs.Count)
                                {
                                    unfold[property.propertyPath].objs[i] = ia.itemsList[i];
                                }else
                                {
                                    unfold[property.propertyPath].objs.Add(ia.itemsList[i]);
                                }
                            }
                        }
                    }
                }

                if (tmp)
                {
                    Rect ias = new Rect(foldPos.x, foldPos.y, 80, foldPos.height);
                    unfold[property.propertyPath].iasExpand = EditorGUI.Foldout(ias, unfold[property.propertyPath].iasExpand, "Item Group", true);
                    ias.x += 80;
                    ias.width = 160;

                    unfold[property.propertyPath].editorAssignSize = EditorGUI.IntField(ias, "Size for assign", unfold[property.propertyPath].editorAssignSize);

                    ias.x += 150;

                    unfold[property.propertyPath].addNullMatch = EditorGUI.ToggleLeft(ias, "Add null match", unfold[property.propertyPath].addNullMatch);

                    if (unfold[property.propertyPath].editorAssignSize < 0) unfold[property.propertyPath].editorAssignSize = 0;

                    if (unfold[property.propertyPath].objs == null) unfold[property.propertyPath].objs = new List<Object>(unfold[property.propertyPath].editorAssignSize);
                    if (unfold[property.propertyPath].objs.Count != unfold[property.propertyPath].editorAssignSize)
                    {
                        unfold[property.propertyPath].objs = new List<Object>(unfold[property.propertyPath].editorAssignSize);
                        for(int i = 0; i < unfold[property.propertyPath].objs.Capacity; i++)
                        {
                            if (i >= unfold[property.propertyPath].objs.Count) unfold[property.propertyPath].objs.Add(null);
                        }
                    }



                    if (unfold[property.propertyPath].iasExpand)
                    {
                        unfold[property.propertyPath].fieldAmount = 3.5f + unfold[property.propertyPath].editorAssignSize;

                        for (int i = 0; i < unfold[property.propertyPath].editorAssignSize; i++)
                        {
                            ias.y += 18f;
                            var objRect = new Rect(position.x + 120, ias.y, position.width - 140, ias.height);
                            Object obj = null;
                            if (i < unfold[property.propertyPath].objs.Count)
                            {
                                unfold[property.propertyPath].objs[i] = EditorGUI.ObjectField(objRect, new GUIContent($"Item Asset {i}"), unfold[property.propertyPath].objs[i], typeof(ItemGroup), false);
                            } else
                            {
                                unfold[property.propertyPath].objs.Add(EditorGUI.ObjectField(objRect, new GUIContent($"Item Asset {i}"), obj, typeof(ItemGroup), false));
                            }
                        }
                    }

                    if (amBool != amTmp)
                    {
                        ItemGroup newAsset = ScriptableObject.CreateInstance<ItemGroup>();

                        foreach (Object iaobj in unfold[property.propertyPath].objs)
                        {
                            ItemGroup ia = iaobj as ItemGroup;
                            if (ia == null) continue;
                            newAsset.name += ia.name + " ";
                            foreach (Item item in ia.itemsList)
                            {
                                if (!newAsset.itemsList.Contains(item)) newAsset.itemsList.Add(item);
                            }
                        }

                        if (!Enumerable.SequenceEqual((whitelistProp.objectReferenceValue as ItemGroup).itemsList, newAsset.itemsList) && newAsset.itemsList.Count > 0)
                        {
                            newAsset.strId = newAsset.name;
                            newAsset.id = Random.Range(10000, int.MaxValue);
                            whitelistProp.objectReferenceValue = newAsset;
                        }
                    }
                }
                else
                {
                    Rect ias = new Rect(foldPos.x, foldPos.y, 80, foldPos.height);
                    unfold[property.propertyPath].iasExpand = EditorGUI.Foldout(ias, unfold[property.propertyPath].iasExpand, "Items", true);
                    ias.x += 80;
                    ias.width = 160;


                    unfold[property.propertyPath].editorAssignSize = EditorGUI.IntField(ias, "Size for assign", unfold[property.propertyPath].editorAssignSize);

                    ias.x += 150;

                    unfold[property.propertyPath].addNullMatch = EditorGUI.ToggleLeft(ias, "Add null match", unfold[property.propertyPath].addNullMatch);

                    if (unfold[property.propertyPath].editorAssignSize < 0) unfold[property.propertyPath].editorAssignSize = 0;

                    if (unfold[property.propertyPath].objs == null) unfold[property.propertyPath].objs = new List<Object>(unfold[property.propertyPath].editorAssignSize);
                    if (unfold[property.propertyPath].objs.Count != unfold[property.propertyPath].editorAssignSize)
                    {
                        unfold[property.propertyPath].objs = new List<Object>(unfold[property.propertyPath].editorAssignSize);
                        for (int i = 0; i < unfold[property.propertyPath].objs.Capacity; i++)
                        {
                            if (i >= unfold[property.propertyPath].objs.Count) unfold[property.propertyPath].objs.Add(null);
                        }
                    }


                    if (unfold[property.propertyPath].iasExpand)
                    {
                        unfold[property.propertyPath].fieldAmount = 3.5f + unfold[property.propertyPath].editorAssignSize;

                        for (int i = 0; i < unfold[property.propertyPath].editorAssignSize; i++)
                        {
                            ias.y += 18f;
                            var objRect = new Rect(position.x + 120, ias.y, position.width - 140, ias.height);
                            Object obj = null;
                            if (i < unfold[property.propertyPath].objs.Count)
                            {
                                unfold[property.propertyPath].objs[i] = EditorGUI.ObjectField(objRect, new GUIContent($"Item {i}"), unfold[property.propertyPath].objs[i], typeof(Item), false);
                            }
                            else
                            {
                                unfold[property.propertyPath].objs.Add(EditorGUI.ObjectField(objRect, new GUIContent($"Item {i}"), obj, typeof(Item), false));
                            }
                        }
                    }

                    if (amBool != amTmp)
                    {
                        ItemGroup newAsset = ScriptableObject.CreateInstance<ItemGroup>();

                        newAsset.name += "Custom ItemGroup";
                        foreach (Object itemobj in unfold[property.propertyPath].objs)
                        {
                            Item item = itemobj as Item;
                            if (item == null) continue;

                            if (!newAsset.itemsList.Contains(item))
                            {
                                newAsset.itemsList.Add(item);
                                newAsset.strId += item.itemName;
                            }
                            
                        }

                        if(unfold[property.propertyPath].addNullMatch && !newAsset.itemsList.Contains(null))
                            newAsset.itemsList.Add(null);

                        if (whitelistProp.objectReferenceValue != null && newAsset.itemsList.Count > 0)
                        {
                            if (!Enumerable.SequenceEqual((whitelistProp.objectReferenceValue as ItemGroup).itemsList, newAsset.itemsList) && newAsset.itemsList.Count > 0)
                            {
                                newAsset.id = Random.Range(10000, int.MaxValue);
                                whitelistProp.objectReferenceValue = newAsset;
                            }
                        }
                        else if (newAsset.itemsList.Count > 0)
                        {
                            newAsset.id = Random.Range(10000, int.MaxValue);
                            whitelistProp.objectReferenceValue = newAsset;
                        }
                    }
                }
            } else
            {
                unfold[property.propertyPath].objs = new List<Object>();
                unfold[property.propertyPath].editorAssignSize = 1;
                var objRect = new Rect(ampos.x + 120, ampos.y, position.width - 140, ampos.height);
                EditorGUI.ObjectField(objRect, whitelistProp, new GUIContent("Whitelist"));
            }        
        } else
        {
            unfold[property.propertyPath].fieldAmount = 1;       
        }

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var labelHIRect = new Rect(position.x + 15, position.y, 50, position.height);
        var labelAmRect = new Rect(position.x + 75 + (position.width / 2) - 130, position.y, 70, position.height);
        var labelInteracts = new Rect(position.x + 190 + (position.width / 2) - 130, position.y, 70, position.height);

        var hasItemRect = new Rect(position.x, position.y, 10, position.height);
        var nameRect = new Rect(position.x + 70, position.y, (position.width / 2) - 130, position.height);
        var amountRect = new Rect(position.x + 135 + (position.width / 2) - 130, position.y, 50, position.height);
        var interactsRect = new Rect(position.x + 245 + (position.width / 2) - 130, position.y, (position.width / 2) - 130, position.height);

        EditorGUI.PropertyField(hasItemRect, property.FindPropertyRelative("hasItem"), GUIContent.none);
        EditorGUI.LabelField(labelHIRect, new GUIContent("Has item"));
        EditorGUI.ObjectField(nameRect, property.FindPropertyRelative("item"), GUIContent.none);
        EditorGUI.LabelField(labelAmRect, new GUIContent("In Amount"));
        EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("amount"), GUIContent.none);//interative
        EditorGUI.LabelField(labelInteracts, new GUIContent("Interacts"));
        EditorGUI.PropertyField(interactsRect, property.FindPropertyRelative("interative"), GUIContent.none);

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public class SlotInfo
    {
        public bool boolValue;
        public bool iasExpand;
        public bool multipleAssign;
        public bool addNullMatch;
        public float fieldAmount;
        public int editorAssignSize;
        public List<Object> objs;
        public bool executeOnce;

        public SlotInfo(bool _boolValues, int _fieldAmount, bool _iasExpand, int _editorAssignSize, bool _multipleAssign, bool _addNullMatch)
        {
            boolValue = _boolValues;
            fieldAmount = _fieldAmount;
            iasExpand = _iasExpand;
            editorAssignSize = _editorAssignSize;
            multipleAssign = _multipleAssign;
            addNullMatch = _addNullMatch;
        }
    }
}
