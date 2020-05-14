using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Remember to subscribe this functioon to the InventoyrEventHandler in order to work
/// </summary>
public interface IDropBehaviour
{
    void OnDropItem(object sender, InventoryEventHandler.DropItemEventArgs e);
}

/// <summary>
/// Remember to subscribe this functioon to the InventoyrEventHandler in order to work
/// </summary>
public interface IPickUpBehaviour
{
    void OnPickUp(object sender, InventoryEventHandler.AddItemEventArgs e);
}

public interface IUsable
{
    void OnUse();
}
