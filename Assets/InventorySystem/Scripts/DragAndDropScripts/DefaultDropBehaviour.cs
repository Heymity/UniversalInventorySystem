using UnityEngine;
using UniversalInventorySystem;
public class DefaultDropBehaviour : DropBehaviour
{
    public GameObject droppedItemObj;

    public override void OnDropItem(object sender, InventoryHandler.DropItemEventArgs e)
    {
        if (droppedItemObj == null) droppedItemObj = new GameObject();
        var dur = e.inv[e.slot].durability;
        e.inv.RemoveItemInSlot(e.slot, e.amount);
        var b = Instantiate(droppedItemObj, new Vector3(e.positionDropped.x, e.positionDropped.y, 0), Quaternion.identity);
        DroppedItem droppedItem = b.GetComponent<DroppedItem>();
        if(e.item.hasDurability)
            droppedItem.SetSprite(InventoryUI.GetNearestSprite(e.item, dur));
        else
            droppedItem.SetSprite(e.item.sprite);
        droppedItem.SetAmount(e.amount);
    }
}

