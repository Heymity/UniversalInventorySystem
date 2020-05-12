using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public Canvas canvas;
    RectTransform rectTransform;
    public InventoryUI invUI;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (invUI.inv.interactiable != IteractiableTypes.Locked)
        {
            invUI.dragObj.GetComponent<RectTransform>().anchoredPosition += eventData.delta / canvas.scaleFactor;
            //rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
            InventoryEventHandler.OnDragItemEventArgs odi = new InventoryEventHandler.OnDragItemEventArgs(invUI.inv, rectTransform.anchoredPosition, invUI.slots[int.Parse(transform.parent.name)]);
            InventoryEventHandler.current.BroadcastUIEvent(BroadcastEventType.ItemDragged, odi: odi);
            invUI.isDraging = true;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
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
            invUI.inv.SwapItemsInSlots(int.Parse(transform.parent.name), index);
        }
        invUI.dragObj.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var min = float.MaxValue;
        int index = 0;
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

        invUI.dragObj.SetActive(true);

        var o = invUI.dragObj;
        var r = o.GetComponent<RectTransform>();
        r.position = rectTransform.position;
        var sd = invUI.slots[index].GetComponent<RectTransform>().sizeDelta;
        r.sizeDelta = sd;
        for(int i = 0;i < o.transform.childCount; i++)
        {
            var c = o.transform.GetChild(i);

            Image ig;
            TextMeshProUGUI text;
            if (c.TryGetComponent(out ig))
            {
                for (int j = 0; j < invUI.slots[index].transform.childCount; j++)
                {
                    if (invUI.slots[index].transform.GetChild(i).TryGetComponent(out ig))
                    {
                        c.GetComponent<RectTransform>().sizeDelta = invUI.slots[index].transform.GetChild(i).GetComponent<RectTransform>().sizeDelta;
                    }
                    break;
                }
            } else if (c.TryGetComponent(out text)) 
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
        var image = o.GetComponentInChildren<Image>();
        image.color = new Color(1, 1, 1, 1);
        image.sprite = invUI.inv.slots[index].item.sprite;
        o.GetComponentInChildren<TextMeshProUGUI>().text = invUI.inv.slots[index].amount.ToString();
        
    }
}
