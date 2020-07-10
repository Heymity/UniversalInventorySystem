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
 *  
 *  This handles the actual dropping of an item
 */
 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniversalInventorySystem
{
    public class ItemDropHandler : MonoBehaviour, IDropHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            List<InventoryUI> invsUI = InventoryController.inventoriesUI;
            RectTransform invPanel = this.GetComponent<RectTransform>();
            InventoryUI nativeInvUI = null;
            foreach (InventoryUI UI in invsUI)
            {
                if (!UI.isDraging) continue;
                UI.isDraging = false;
                invPanel = UI.DontDropItemRect.GetComponent<RectTransform>();
                nativeInvUI = UI;
                break;
            }

            if (nativeInvUI != null)
            {
                if (!RectTransformUtility.RectangleContainsScreenPoint(invPanel, Camera.main.ScreenToWorldPoint(Input.mousePosition)))
                {
                    nativeInvUI.shouldSwap = false;

                    foreach (InventoryUI invUI in invsUI)
                    {
                        if (!invUI.togglableObject.activeInHierarchy) continue;
                        if (invUI.DontDropItemRect.activeInHierarchy && RectTransformUtility.RectangleContainsScreenPoint(invUI.DontDropItemRect.GetComponent<RectTransform>(), Camera.main.ScreenToWorldPoint(Input.mousePosition)))
                        {
                            var min = float.MaxValue;
                            int index = 0;
                            for (int i = 0; i < invUI.slots.Count; i++)
                            {
                                var tmp = Vector3.Distance(Camera.main.ScreenToWorldPoint(Input.mousePosition), invUI.slots[i].GetComponent<RectTransform>().position);
                                if (tmp <= min)
                                {
                                    min = tmp;
                                    index = i;
                                }
                            }
                            InventoryController.SwapItemThruInventoriesSlotToSlot(nativeInvUI.inv, invUI.inv, nativeInvUI.dragSlotNumber ?? -1, index, nativeInvUI.dragObj.GetComponent<DragSlot>().GetAmount());
                            return;
                        }
                    }
                    Slot s = nativeInvUI.inv.slots[nativeInvUI.dragSlotNumber.GetValueOrDefault()];

                    s.item.OnDrop(nativeInvUI.inv, true, nativeInvUI.dragSlotNumber.GetValueOrDefault(), nativeInvUI.dragObj.GetComponent<DragSlot>().amount, true, Camera.main.ScreenToWorldPoint(Input.mousePosition));

                }
                else nativeInvUI.shouldSwap = true;
            }
        }
    }
}