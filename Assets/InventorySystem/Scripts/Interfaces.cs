using UnityEngine;
using UnityEngine.EventSystems;

public interface IDropBehaviour
{
    void OnDropItem(object sender, InventoryEventHandler.DropItemEventArgs e);
}

public interface IPickUppable
{
    void OnPickUp();
}
