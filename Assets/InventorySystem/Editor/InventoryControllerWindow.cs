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
using System.Diagnostics;
using System.Reflection;
using System;
using System.Collections.Generic;

namespace UniversalInventorySystem.Editors
{
    public class InventoryControllerWindow : EditorWindow
    {
        [MenuItem("InventorySystem/InventoryController")]
        public static void Init()
        {
            InventoryControllerWindow window = GetWindow<InventoryControllerWindow>("Controller");
            window.Show();
        }

        int selected = 0;
        Vector2 scrollPos;

        void OnGUI()
        {
            EditorGUILayout.BeginVertical("Toolbar", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(500));
            GUILayout.BeginArea(new Rect((Screen.width / 2) - 250, 1, 500, 18));

            selected = GUILayout.Toolbar(selected, new string[3] { "Inventory", "InventoryUI", "Debug" });

            GUILayout.EndArea();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            if (selected == 0)
            {
                EditorGUILayout.LabelField("Inventories");
                for (int i = 0; i < InventoryController.inventories.Count; i++)
                {
                    var a = InventoryController.inventories[i];
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"{a.key} (id: {a.id})");
                    EditorGUILayout.EnumFlagsField(a.interactiable);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.LabelField($"Size: {a.SlotAmount}");

                    MakeHeader();

                    for (int j = 0; j < a.slots.Count; j++)
                    {
                        Slot s = a.slots[j];

                        EditorGUILayout.BeginHorizontal();
                        var rect = GUILayoutUtility.GetRect(100, 18);
                        rect.x += 10;
                        rect.width -= 20;
                        HR(rect);

                        if (s.HasItem) EditorGUI.LabelField(rect, new GUIContent(" Has Item   ", EditorGUIUtility.IconContent("TestPassed").image));
                        else EditorGUI.LabelField(rect, new GUIContent(" Hasn't Item ", EditorGUIUtility.IconContent("TestFailed").image));

                        rect.x += 100;

                        DrawLine(rect, 18);

                        if (s.item != null)
                        {
                            var soRect = rect;
                            soRect.height = 16;
                            EditorGUI.LabelField(soRect, new GUIContent(s.item.itemName, EditorGUIUtility.IconContent("ScriptableObject Icon").image));
                            rect.x += 100;
                            DrawLine(rect, 18);
                            EditorGUI.LabelField(rect, s.amount.ToString());
                            rect.x += 80;
                            DrawLine(rect, 18);
                            EditorGUI.LabelField(rect, $"{s.durability} | {s.item.hasDurability}");
                            rect.x += 110;
                            DrawLine(rect, 18);
                        }
                        else
                        {
                            EditorGUI.LabelField(rect, "None");
                            rect.x += 100;
                            DrawLine(rect, 18);
                            rect.x += 80;
                            DrawLine(rect, 18);
                            rect.x += 110;
                            DrawLine(rect, 18);
                        }
                        EditorGUI.LabelField(rect, s.isProductSlot.ToString());
                        rect.x += 100;
                        DrawLine(rect, 18);

                        var auxRect = rect;
                        auxRect.height = 16;
                        auxRect.width = 130;
                        GUIContent content = new GUIContent(s.whitelist == null ? "" : s.whitelist.strId, EditorGUIUtility.IconContent("ScriptableObject Icon").image);
                        if (GUI.Button(auxRect, s.whitelist == null ? new GUIContent("None") : content))
                        {
                            ItemDataEditorWindow.Open(s.whitelist);
                        }

                        rect.x += 140;
                        DrawLine(rect, 18);

                        auxRect = rect;
                        auxRect.width = 130;
                        auxRect.height = 16;
                        content = new GUIContent(s.itemInstance == null ? "" : s.itemInstance.itemName, EditorGUIUtility.IconContent("ScriptableObject Icon").image);
                        if (GUI.Button(auxRect, s.itemInstance == null ? new GUIContent("None") : content) && s.itemInstance != null)
                        {
                            ItemInstanceInspector window = GetWindow<ItemInstanceInspector>("Item Inspector");
                            window.Show();
                            window.itemTarget = s.itemInstance;
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndVertical();
                }
            }
            else if (selected == 1)
            {
                EditorGUILayout.LabelField("InventoriesUI");
                if(!Application.isPlaying)
                    EditorGUILayout.LabelField("This only works in play mode");
                for (int i = 0; i < InventoryController.inventoriesUI.Count; i++)
                {
                    InventoryUI ui = InventoryController.inventoriesUI[i];
                    var obj = new SerializedObject(ui);
                    obj.Update();
                    var prop = obj.GetIterator().Copy();
                    EditorGUILayout.Separator();
                    HR(GUILayoutUtility.GetRect(100, 18));
                    if (prop.NextVisible(true))
                    {
                        do
                        {
                            EditorGUI.BeginDisabledGroup(true);
                            EditorGUILayout.PropertyField(prop);
                            EditorGUI.EndDisabledGroup();
                        } while (prop.NextVisible(false));
                    }
                }
            }
            else if (selected == 2)
            {
                EditorGUILayout.LabelField("Debugging");
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        Stopwatch stop;
        public void Update()
        {
            if (stop == null)
            {
                stop = new Stopwatch();
                stop.Start();
            }
            if (stop.ElapsedMilliseconds >= 1000)
            {
                stop.Restart();
                Repaint();
            }
        }

        public static void DrawLine(Rect rect, float height)
        {
            Handles.color = Color.gray;
            Handles.BeginGUI();
            Handles.DrawLine(
            new Vector3(rect.x - 5, rect.y),
            new Vector3(rect.x - 5, rect.y + height));
            Handles.EndGUI();
        }

        public static void HR(Rect rect)
        {
            Handles.color = Color.gray;
            Handles.BeginGUI();
            Handles.DrawLine(
            new Vector3(rect.x, rect.y),
            new Vector3(rect.x + rect.width, rect.y));
            Handles.EndGUI();
        }

        public void MakeHeader()
        {
            var header = GUILayoutUtility.GetRect(100, 18);
            header.x += 10;
            header.width -= 20;
            HR(header);
            EditorGUI.LabelField(header, "Has Item:");
            header.x += 100;
            DrawLine(header, 18);
            EditorGUI.LabelField(header, "Item:");
            header.x += 100;
            DrawLine(header, 18);
            EditorGUI.LabelField(header, "Amount:");
            header.x += 80;
            DrawLine(header, 18);
            EditorGUI.LabelField(header, "Durability | Has");
            header.x += 110;
            DrawLine(header, 18);
            EditorGUI.LabelField(header, "Is Product Slot:");
            header.x += 100;
            DrawLine(header, 18);
            EditorGUI.LabelField(header, "Whitelist:");
            header.x += 140;
            DrawLine(header, 18);
            EditorGUI.LabelField(header, "Item Instance:");
        }

    }
    public class ItemInstanceInspector : EditorWindow
    {
        public Item itemTarget;
        Vector2 scrollPosItem;

        public void OnGUI()
        {
            if (itemTarget == null) return;
            EditorGUILayout.LabelField(itemTarget.itemName);

            MemberTypes mt = MemberTypes.Field | MemberTypes.Property | MemberTypes.Method;
            BindingFlags bf = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic;
            MemberInfo[] members = itemTarget.GetType().FindMembers(mt, bf, (MemberInfo mi, object search) => true, null);

            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            scrollPosItem = EditorGUILayout.BeginScrollView(scrollPosItem);
            foreach (MemberInfo mi in members)
            {
                var rect = GUILayoutUtility.GetRect(100, 20);
                rect.x += 10;
                rect.width -= 20;
                InventoryControllerWindow.HR(rect);
                EditorGUI.LabelField(rect, mi.Name);
                rect.x += 200;
                InventoryControllerWindow.DrawLine(rect, 20);
                rect.width -= 205;
                rect.height = 18;
                try
                {
                    switch (mi.MemberType)
                    {
                        case MemberTypes.Field:
                            HandleField(mi as FieldInfo, rect);
                            break;
                        case MemberTypes.Property:
                            var prop = (mi as PropertyInfo).GetValue(itemTarget);
                            if (prop == null) break;
                            EditorGUI.LabelField(rect, (mi as PropertyInfo).GetValue(itemTarget).ToString());
                            break;
                        case MemberTypes.Method:
                            var method = (mi as MethodInfo);
                            if (method == null) break;
                            if (GUI.Button(rect, (mi as MethodInfo).Name))
                            {
                                MethodExecutor window = GetWindow<MethodExecutor>("Invoker");
                                window.Show();
                                window.mi = method;
                                window.param = new List<object>();
                                window.obj = itemTarget;
                                window.returnValue = null;
                            }
                            break;
                    }
                }
                catch (Exception) { }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        public void HandleField(FieldInfo fi, Rect rect)
        {
            var field = fi.GetValue(itemTarget);
            //if (field == null) return;
            if (fi.FieldType == typeof(int) || fi.FieldType == typeof(int?))
            {
                fi.SetValue(itemTarget, EditorGUI.IntField(rect, "", (int)(field ?? 0)));
            }
            else if (fi.FieldType == typeof(float) || fi.FieldType == typeof(float?))
            {
                fi.SetValue(itemTarget, EditorGUI.FloatField(rect, "", (float)(field ?? 0)));
            }
            else if (fi.FieldType == typeof(bool) || fi.FieldType == typeof(bool?))
            {
                fi.SetValue(itemTarget, EditorGUI.Toggle(rect, "", (bool)(field ?? false)));
            }
            else if (fi.FieldType == typeof(string))
            {
                fi.SetValue(itemTarget, EditorGUI.TextField(rect, "", (string)(field ?? "")));
            }
            else if (fi.FieldType.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                fi.SetValue(itemTarget, EditorGUI.ObjectField(rect, "", field as UnityEngine.Object, fi.FieldType, true));
            }
            else if (fi.FieldType == typeof(Vector3) || fi.FieldType == typeof(Vector3?))
            {
                fi.SetValue(itemTarget, EditorGUI.Vector3Field(rect, "", (Vector3)(field ?? Vector3.zero)));
            }
            else if (fi.FieldType == typeof(Vector3Int) || fi.FieldType == typeof(Vector3Int?))
            {
                fi.SetValue(itemTarget, EditorGUI.Vector3IntField(rect, "", (Vector3Int)(field ?? Vector3Int.zero)));
            }
            else if (fi.FieldType == typeof(Vector2) || fi.FieldType == typeof(Vector2?))
            {
                fi.SetValue(itemTarget, EditorGUI.Vector2Field(rect, "", (Vector2)(field ?? Vector2.zero)));
            }
            else if (fi.FieldType == typeof(Vector2Int) || fi.FieldType == typeof(Vector2Int?))
            {
                fi.SetValue(itemTarget, EditorGUI.Vector2IntField(rect, "", (Vector2Int)(field ?? Vector2Int.zero)));
            }
            else
            {
                EditorGUI.LabelField(rect, (fi as FieldInfo).GetValue(itemTarget).ToString());
            }
        }

        Stopwatch stop;
        public void Update()
        {
            if (stop == null)
            {
                stop = new Stopwatch();
                stop.Start();
            }
            if (stop.ElapsedMilliseconds >= 500)
            {
                stop.Restart();
                Repaint();
            }
        }
    }

    public class MethodExecutor : EditorWindow
    {
        public MethodInfo mi;
        public List<object> param = new List<object>();
        public object obj;
        public object returnValue;

        private void OnGUI()
        {
            bool hasAllParams = true;
            if (mi == null) return;
            EditorGUILayout.LabelField(mi.Name);
            ParameterInfo[] pis = mi.GetParameters();
            for (int i = 0; i < pis.Length; i++)
            {
                if (param.Count <= i) param.Add(null);
                HandleParam(pis[i], i);
            }

            if (hasAllParams)
            {
                if (GUILayout.Button("Invoke"))
                {
                    returnValue = mi.Invoke(obj, param.ToArray());
                }
                EditorGUILayout.LabelField($"Return Value: {returnValue}");
                EditorGUILayout.LabelField($"({mi.ReturnType})");
            }

            void HandleParam(ParameterInfo p, int index)
            {
                if (p.ParameterType == typeof(int) || p.ParameterType == typeof(int?))
                {
                    param[index] = EditorGUILayout.IntField(p.Name, (int)(param[index] ?? 0));
                }
                else if (p.ParameterType == typeof(float) || p.ParameterType == typeof(float?))
                {
                    param[index] = EditorGUILayout.FloatField(p.Name, (float)(param[index] ?? 0));
                }
                else if (p.ParameterType == typeof(bool) || p.ParameterType == typeof(bool?))
                {
                    param[index] = EditorGUILayout.Toggle(p.Name, (bool)(param[index] ?? false));
                }
                else if (p.ParameterType == typeof(string))
                {
                    param[index] = EditorGUILayout.TextField(p.Name, (string)(param[index] ?? ""));
                }
                else if (p.ParameterType.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    param[index] = EditorGUILayout.ObjectField(new GUIContent(p.Name), param[index] as UnityEngine.Object, p.ParameterType, true);
                }
                else if (p.ParameterType == typeof(Vector3) || p.ParameterType == typeof(Vector3?))
                {
                    param[index] = EditorGUILayout.Vector3Field(p.Name, (Vector3)(param[index] ?? Vector3.zero));
                }
                else if (p.ParameterType == typeof(Vector3Int) || p.ParameterType == typeof(Vector3Int?))
                {
                    param[index] = EditorGUILayout.Vector3IntField(p.Name, (Vector3Int)(param[index] ?? Vector3Int.zero));
                }
                else if (p.ParameterType == typeof(Vector2) || p.ParameterType == typeof(Vector2?))
                {
                    param[index] = EditorGUILayout.Vector2Field(p.Name, (Vector2)(param[index] ?? Vector2.zero));
                }
                else if (p.ParameterType == typeof(Vector2Int) || p.ParameterType == typeof(Vector2Int?))
                {
                    param[index] = EditorGUILayout.Vector2IntField(p.Name, (Vector2Int)(param[index] ?? Vector2Int.zero));
                }
                else
                {
                    EditorGUILayout.LabelField($"The param {p.Name} is of type {p.ParameterType}, which has not been implemented yet");
                    hasAllParams = false;
                }
            }
        }
    }
}