using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler
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
        if(invUI.inv.interactiable != IteractiableTypes.Locked)
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        var min = float.MaxValue;
        int index = 0;
        for(int i = 0;i < invUI.slots.Length;i++)
        {
            var tmp = Vector3.Distance(rectTransform.position, invUI.slots[i].GetComponent<RectTransform>().position);
            if (tmp <= min)
            {
                min = tmp;
                index = i;
            }
        }
        invUI.inv.SwapItemsInSlots(int.Parse(transform.parent.name), index);
        transform.localPosition = Vector3.zero;
    }

}
