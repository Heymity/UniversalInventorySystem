using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [HideInInspector]
    public Canvas canvas;
    RectTransform rectTransform;
    [HideInInspector]
    public InventoryUI invUI;
    int index;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (invUI.togglableObject.activeInHierarchy)
        { 
            if (invUI.inv.interactiable != IteractiableTypes.Locked && invUI.GetInventory().slots[index].hasItem && invUI.GetInventory().slots[index].amount > 0 && !(Mathf.RoundToInt(invUI.GetInventory().slots[index].amount / 2) <= 0 && eventData.button == PointerEventData.InputButton.Right))
            {
                invUI.dragObj.GetComponent<RectTransform>().anchoredPosition += eventData.delta / canvas.scaleFactor;
                InventoryEventsItemsHandler.OnDragItemEventArgs odi = new InventoryEventsItemsHandler.OnDragItemEventArgs(invUI.inv, rectTransform.anchoredPosition, invUI.slots[int.Parse(transform.parent.name)]);
                InventoryEventsItemsHandler.current.BroadcastUIEvent(BroadcastEventType.ItemDragged, odi: odi);
                invUI.isDraging = true;
            }
        } else
        {
            invUI.dragObj.SetActive(false);
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        invUI.canvas.GetComponent<ItemDropHandler>().OnDrop(eventData);
        if (invUI.shouldSwap)
        {
            var min = float.MaxValue;
            int index = 0;
            for (int i = 0; i < invUI.slots.Length; i++)
            {
                var tmp = Vector3.Distance(invUI.dragObj.transform.position, invUI.slots[i].GetComponent<RectTransform>().position);
                if (tmp <= min)
                {
                    min = tmp;
                    index = i;
                }
            }
            if(invUI.dragObj.GetComponent<DragSlot>().GetAmount() >= 0)
                invUI.inv.SwapItemsInCertainAmountInSlots(int.Parse(transform.parent.name), index, invUI.dragObj.GetComponent<DragSlot>().GetAmount());
        }
        invUI.dragObj.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var min = float.MaxValue;
        index = 0;
        for (int i = 0; i < invUI.slots.Length; i++)
        {
            var tmp = Vector3.Distance(rectTransform.position, invUI.slots[i].GetComponent<RectTransform>().position);
            if (tmp <= min)
            {
                min = tmp;
                index = i;
            }
        }
        invUI.dragSlotNumber = index;
        if (invUI.GetInventory().slots[index].hasItem && !(Mathf.RoundToInt(invUI.GetInventory().slots[index].amount / 2) <= 0 && eventData.button == PointerEventData.InputButton.Right))
        {


            invUI.dragObj.SetActive(true);

            var o = invUI.dragObj;
            var r = o.GetComponent<RectTransform>();
            r.position = rectTransform.position;
            var sd = invUI.slots[index].GetComponent<RectTransform>().sizeDelta;
            r.sizeDelta = sd;
            for (int i = 0; i < o.transform.childCount; i++)
            {
                var c = o.transform.GetChild(i);

                Image igUI;
                Image ig;
                TextMeshProUGUI text;
                if (c.TryGetComponent(out igUI))
                {
                    for (int j = 0; j < invUI.slots[index].transform.childCount; j++)
                    {
                        if (invUI.slots[index].transform.GetChild(i).TryGetComponent(out ig))
                        {
                            igUI.material.SetFloat("_Size", invUI.outlineSize);
                            igUI.material.SetColor("_Color", invUI.outlineColor);
                            c.GetComponent<RectTransform>().sizeDelta = invUI.slots[index].transform.GetChild(i).GetComponent<RectTransform>().sizeDelta;
                        }
                        break;
                    }
                }
                else if (c.TryGetComponent(out text))
                {
                    for (int j = 0; j < invUI.slots[index].transform.childCount; j++)
                    {
                        if (invUI.slots[index].transform.GetChild(i).TryGetComponent(out text))
                        {
                            c.GetComponent<RectTransform>().sizeDelta = invUI.slots[index].transform.GetChild(i).GetComponent<RectTransform>().sizeDelta;
                            var t = c.GetComponent<TextMeshProUGUI>();
                            t.fontSize = text.fontSize;
                            t.color = text.color;
                            t.alignment = text.alignment;
                        }
                        break;
                    }
                }
            }

            var dragSlot = o.GetComponent<DragSlot>();
            int amountToTransfer;
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                amountToTransfer = invUI.inv.slots[index].amount;
                dragSlot.SetAmount(amountToTransfer);
            }
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                amountToTransfer = Mathf.RoundToInt(invUI.inv.slots[index].amount / 2f);
                dragSlot.SetAmount(amountToTransfer);
            }
            else
            {
                amountToTransfer = invUI.inv.slots[index].amount;
                dragSlot.SetAmount(amountToTransfer);
            }
            dragSlot.SetInventory(invUI.GetInventory());
            dragSlot.SetInventoryUI(invUI);
            dragSlot.SetItem(null);
            dragSlot.SetSlotNumber(index);
            var image = o.GetComponentInChildren<Image>();
            image.color = new Color(1, 1, 1, 1);
            image.sprite = invUI.inv.slots[index].item.sprite;
            o.GetComponentInChildren<TextMeshProUGUI>().text = amountToTransfer.ToString();

        }
    }
}
