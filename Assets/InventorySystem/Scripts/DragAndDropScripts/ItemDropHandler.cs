using System.Collections;
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