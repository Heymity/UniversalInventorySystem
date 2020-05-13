using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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


        if (!RectTransformUtility.RectangleContainsScreenPoint(invPanel, Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            nativeInvUI.shouldSwap = false;
            //Debug.Log($"Item out of inventory!{this.name}");
            foreach (InventoryUI invUI in invsUI)
            {
                if (!invUI.togglableObject.activeInHierarchy) continue;
                if (invUI.DontDropItemRect.activeInHierarchy && RectTransformUtility.RectangleContainsScreenPoint(invUI.DontDropItemRect.GetComponent<RectTransform>(), Camera.main.ScreenToWorldPoint(Input.mousePosition)))
                {
                    Debug.Log("Placinng Item into another inventory");
                    var min = float.MaxValue;
                    int index = 0;
                    for (int i = 0; i < invUI.slots.Length; i++)
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
            InventoryEventHandler.DropItemEventArgs dea = new InventoryEventHandler.DropItemEventArgs(nativeInvUI.inv, true, nativeInvUI.dragSlotNumber, s.item, s.amount, true, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            InventoryEventHandler.current.Broadcast(BroadcastEventType.DropItem, dea: dea);
            Debug.Log("Droping Item");
        }
        else nativeInvUI.shouldSwap = true;
            
    }

        //Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
}
