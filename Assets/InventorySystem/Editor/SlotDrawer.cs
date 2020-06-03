using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEditor;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UniversalInventorySystem;

[CustomPropertyDrawer(typeof(Slot))]
public class SlotDrawer : PropertyDrawer
{
    public Dictionary<string, SlotInfo> unfold = new Dictionary<string, SlotInfo>();

    List<Object> objs;

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
             unfold.Add(property.propertyPath, new SlotInfo(false, 1, false, 0));

        var originalPosition = position;
        position = EditorGUI.PrefixLabel(position, label);

        originalPosition.width = position.x - 20;
        unfold[property.propertyPath].boolValue = EditorGUI.Foldout(originalPosition, unfold[property.propertyPath].boolValue, label, true);

        if (unfold[property.propertyPath].boolValue)
        {
            Rect ampos = new Rect(originalPosition.x, originalPosition.y + 18f, 120, 18);
            bool amBool = EditorPrefs.GetBool(property.propertyPath + "AM", false);

            bool amTmp = EditorGUI.Toggle(ampos, "Assign multiple", amBool);

            //Take out from editorPrefs
            if (amTmp != amBool) EditorPrefs.SetBool(property.propertyPath + "AM", amTmp);

            unfold[property.propertyPath].fieldAmount = 2.5f;
            var whitelistProp = property.FindPropertyRelative("whitelist");
            if (amBool)
            {
                var foldPos = new Rect(position.x, position.y + 18f, position.width, position.height);

                bool useItemAsset = EditorPrefs.GetBool(property.propertyPath, true);

                Rect uia = new Rect(foldPos.x, foldPos.y, 120, foldPos.height);
                bool tmp = EditorGUI.Toggle(uia, "Use ItemAsset", useItemAsset);
                foldPos.x += 120;

                if (tmp != useItemAsset) EditorPrefs.SetBool(property.propertyPath, tmp);

                if (tmp)
                {
                    Rect ias = new Rect(foldPos.x, foldPos.y, 80, foldPos.height);
                    unfold[property.propertyPath].iasExpand = EditorGUI.Foldout(ias, unfold[property.propertyPath].iasExpand, "Item Assets", true);
                    ias.x += 80;
                    ias.width = 160;


                    unfold[property.propertyPath].editorAssignSize = EditorGUI.IntField(ias, "Size for assign", unfold[property.propertyPath].editorAssignSize);
                    if (unfold[property.propertyPath].editorAssignSize < 0) unfold[property.propertyPath].editorAssignSize = 0;

                    if (objs == null) objs = new List<Object>(unfold[property.propertyPath].editorAssignSize);
                    if (objs.Count != unfold[property.propertyPath].editorAssignSize)
                    {
                        objs = new List<Object>(unfold[property.propertyPath].editorAssignSize);
                        for(int i = 0; i < objs.Capacity; i++)
                        {
                            if (i >= objs.Count) objs.Add(null);
                        }
                    }
                    if (whitelistProp.objectReferenceValue != null)
                    {
                        if (unfold[property.propertyPath].editorAssignSize < 1) unfold[property.propertyPath].editorAssignSize = 1;
                        if (objs.Count >= 1) if(objs[0] == null) objs[0] = whitelistProp.objectReferenceValue;
                    }

                    if (unfold[property.propertyPath].iasExpand)
                    {
                        unfold[property.propertyPath].fieldAmount = 3.5f + unfold[property.propertyPath].editorAssignSize;

                        for (int i = 0; i < unfold[property.propertyPath].editorAssignSize; i++)
                        {
                            ias.y += 18f;
                            var objRect = new Rect(position.x + 120, ias.y, position.width - 140, ias.height);
                            Object obj = null;
                            if (i < objs.Count)
                            {
                                objs[i] = EditorGUI.ObjectField(objRect, new GUIContent($"Item Asset {i}"), objs[i], typeof(ItemAsset), false);
                            } else
                            {
                                objs.Add(EditorGUI.ObjectField(objRect, new GUIContent($"Item Asset {i}"), obj, typeof(ItemAsset), false));
                            }
                        }
                    }

                    //Add merge btn
                    if (amBool != amTmp)
                    {
                        ItemAsset newAsset = ScriptableObject.CreateInstance<ItemAsset>();

                        foreach (Object iaobj in objs)
                        {
                            ItemAsset ia = iaobj as ItemAsset;
                            if (ia == null) continue;
                            foreach (Item item in ia.itemsList)
                            {
                                if (!newAsset.itemsList.Contains(item)) newAsset.itemsList.Add(item);
                            }
                        }

                        if (!Enumerable.SequenceEqual((whitelistProp.objectReferenceValue as ItemAsset).itemsList, newAsset.itemsList))
                        {
                            whitelistProp.objectReferenceValue = newAsset;
                        }
                    }
                }
                else
                {

                }
            } else
            {
                objs = new List<Object>();
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
        public float fieldAmount;
        public int editorAssignSize;

        public SlotInfo(bool _boolValues, int _fieldAmount, bool _iasExpand, int _editorAssignSize)
        {
            boolValue = _boolValues;
            fieldAmount = _fieldAmount;
            iasExpand = _iasExpand;
            editorAssignSize = _editorAssignSize;
        }
    }
}
