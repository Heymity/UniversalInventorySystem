using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultDropBehaviour : MonoBehaviour, IDropBehaviour
{
    public GameObject droppedItemObj;

    void Start()
    {
        InventoryEventHandler.current.OnDropItem += OnDropItem;   
    }

    private void OnDestroy()
    {
        InventoryEventHandler.current.OnDropItem -= OnDropItem;
    }

    public void OnDropItem(object sender, InventoryEventHandler.DropItemEventArgs e)
    {
        Debug.Log("Drop");
        e.inv.RemoveItemInSlot(e.slot.GetValueOrDefault(), e.amount);
        var b = Instantiate(droppedItemObj, new Vector3(e.positionDropped.x, e.positionDropped.y, 0), Quaternion.identity);
        DroppedItem droppedItem = b.GetComponent<DroppedItem>();
        droppedItem.SetSprite(e.item.sprite);
        droppedItem.SetAmount(e.amount);
    }

}
