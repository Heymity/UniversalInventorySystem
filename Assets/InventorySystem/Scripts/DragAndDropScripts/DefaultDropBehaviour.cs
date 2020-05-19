using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultDropBehaviour : DropBehaviour
{
    public GameObject droppedItemObj;

    public override void OnDropItem(object sender, InventoryHandler.DropItemEventArgs e)
    {
        e.inv.RemoveItemInSlot(e.slot.GetValueOrDefault(), e.amount);
        var b = Instantiate(droppedItemObj, new Vector3(e.positionDropped.x, e.positionDropped.y, 0), Quaternion.identity);
        DroppedItem droppedItem = b.GetComponent<DroppedItem>();
        droppedItem.SetSprite(e.item.sprite);
        droppedItem.SetAmount(e.amount);
    }

}
