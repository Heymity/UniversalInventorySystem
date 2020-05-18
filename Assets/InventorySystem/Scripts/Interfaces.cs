using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Remember to subscribe this functioon to the InventoyrEventHandler in order to work
/// </summary>
public interface IDropBehaviour
{
    void OnDropItem(object sender, InventoryEventsItemsHandler.DropItemEventArgs e);
}

/// <summary>
/// Remember to subscribe this functioon to the InventoyrEventHandler in order to work
/// </summary>
public interface IPickUpBehaviour
{
    void OnPickUp(object sender, InventoryEventsItemsHandler.AddItemEventArgs e);
}

public interface IUsable
{
    void OnUse(object sender, InventoryEventsItemsHandler.UseItemEventArgs e);
}



public abstract class DropBehaviour : MonoBehaviour, IDropBehaviour
{
    public virtual void OnEnable()
    {
        InventoryEventsItemsHandler.current.OnDropItem += OnDropItem;
    }

    public virtual void OnDestroy()
    {
        InventoryEventsItemsHandler.current.OnDropItem -= OnDropItem;
    }

    public abstract void OnDropItem(object sender, InventoryEventsItemsHandler.DropItemEventArgs e);
}
