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

        bool inventory = true;
        bool inventoryUI = false;
        bool debug = false;
        Vector2 scrollPos;

        void OnGUI()
        {
            EditorGUILayout.BeginVertical("Toolbar", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Inventories", EditorStyles.miniButtonLeft))
            {
                inventory = true;
                inventoryUI = false;
                debug = false;
            }

            if (GUILayout.Button("InventoriesUI", EditorStyles.miniButtonMid))
            {
                inventory = false;
                inventoryUI = true;
                debug = false;
            }

            if (GUILayout.Button("Debug", EditorStyles.miniButtonRight))
            {
                inventory = false;
                inventoryUI = false;
                debug = true;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            if (inventory)
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
                        HR(rect);

                        if (s.HasItem) EditorGUI.LabelField(rect, new GUIContent(" Has Item   ", EditorGUIUtility.IconContent("TestPassed").image));
                        else EditorGUI.LabelField(rect, new GUIContent(" Hasn't Item ", EditorGUIUtility.IconContent("TestFailed").image));

                        rect.x += 100;

                        DrawLine(rect);

                        if (s.item != null)
                        {
                            var soRect = rect;
                            soRect.height = 16;
                            EditorGUI.LabelField(soRect, new GUIContent(s.item.itemName, EditorGUIUtility.IconContent("ScriptableObject Icon").image));
                            rect.x += 100;
                            DrawLine(rect);
                            EditorGUI.LabelField(rect, s.amount.ToString());
                            rect.x += 80;
                            DrawLine(rect);
                            EditorGUI.LabelField(rect, $"{s.durability} | {s.item.hasDurability}");
                            rect.x += 110;
                            DrawLine(rect);
                        }
                        else
                        {
                            EditorGUI.LabelField(rect, "None");
                            rect.x += 100;
                            DrawLine(rect);
                            rect.x += 80;
                            DrawLine(rect);
                            rect.x += 110;
                            DrawLine(rect);
                        }
                        EditorGUI.LabelField(rect, s.isProductSlot.ToString());
                        rect.x += 100;
                        DrawLine(rect);

                        var auxRect = rect;
                        auxRect.height = 16;
                        GUIContent content = new GUIContent(s.whitelist == null ? "" : s.whitelist.strId, EditorGUIUtility.IconContent("ScriptableObject Icon").image);
                        EditorGUI.LabelField(auxRect, s.whitelist == null ? new GUIContent("None") : content);

                        rect.x += 120;
                        DrawLine(rect);

                        auxRect = rect;
                        auxRect.width = 130;
                        auxRect.height = 16;
                        content = new GUIContent(s.itemInstance == null ? "" : s.itemInstance.itemName, EditorGUIUtility.IconContent("ScriptableObject Icon").image);
                        if (GUI.Button(auxRect, s.itemInstance == null ? new GUIContent("None") : content) && s.itemInstance != null)
                        {
                            ItemInstanceInspector window = GetWindow<ItemInstanceInspector>(s.itemInstance.itemName);
                            window.Show();
                            window.itemTarget = s.itemInstance;
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndVertical();
                }
            }
            else if (inventoryUI)
            {
                EditorGUILayout.LabelField("InventoriesUI");
            }
            else if (debug)
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

        public void DrawLine(Rect rect)
        {
            Handles.color = Color.gray;
            Handles.BeginGUI();
            Handles.DrawLine(
            new Vector3(rect.x - 5, rect.y),
            new Vector3(rect.x - 5, rect.y + 18));
            Handles.EndGUI();
        }

        public void HR(Rect rect)
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
            HR(header);
            EditorGUI.LabelField(header, "Has Item:");
            header.x += 100;
            DrawLine(header);
            EditorGUI.LabelField(header, "Item:");
            header.x += 100;
            DrawLine(header);
            EditorGUI.LabelField(header, "Amount:");
            header.x += 80;
            DrawLine(header);
            EditorGUI.LabelField(header, "Durability | Has");
            header.x += 110;
            DrawLine(header);
            EditorGUI.LabelField(header, "Is Product Slot:");
            header.x += 100;
            DrawLine(header);
            EditorGUI.LabelField(header, "Whitelist:");
            header.x += 120;
            DrawLine(header);
            EditorGUI.LabelField(header, "Item Instance:");
        }

        public class ItemInstanceInspector : EditorWindow
        {
            public Item itemTarget;

            public void OnGUI()
            {
                if (itemTarget == null) return;
                EditorGUILayout.LabelField(itemTarget.itemName);
            }
        }
    }
}