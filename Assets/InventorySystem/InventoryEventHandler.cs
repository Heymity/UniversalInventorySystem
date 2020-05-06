using System;
using UnityEngine;

/// <summary>
/// Put this script on a always active GameObject.
/// </summary>
public class InventoryEventHandler : MonoBehaviour
{
    public static InventoryEventHandler current;
 
    public event EventHandler<AddItemEventArgs> OnAddItem;
    public event EventHandler<RemoveItemEventArgs> OnRemoveItem;
    public event EventHandler<SwapItemsEventArgs> OnSwapItem;
    public event EventHandler<SwapItemsTrhuInvEventArgs> OnSwapTrhuInventory;
    public event EventHandler<UseItemEventArgs> OnUseItem;
    public event EventHandler<DropItemEventArgs> OnDropItem;
    public event EventHandler<PickUpItemEventArgs> OnPickUpItem;
    public event EventHandler<InitializeInventoryEventArgs> OnInitializeInventory;

    public class AddItemEventArgs : EventArgs
    {
        public bool addedToNewSlot;
        public bool addedToSlot;
        public Item itemAdded;
        public int amount;
        public int? slotNumber;

        public AddItemEventArgs (bool _addedToNewSlot, bool _addedToSlot, Item _itemAdded, int _amount, int? _slotNumber)
        {
            addedToNewSlot = _addedToNewSlot;
            addedToSlot = _addedToSlot;
            itemAdded = _itemAdded;
            amount = _amount;
            slotNumber = _slotNumber;
        }
    }
    public class RemoveItemEventArgs
    {

    }
    public class SwapItemsEventArgs
    {

    }
    public class SwapItemsTrhuInvEventArgs
    {

    }
    public class UseItemEventArgs
    {

    }
    public class DropItemEventArgs 
    { 

    }
    public class PickUpItemEventArgs 
    {

    }
    public class InitializeInventoryEventArgs 
    { 

    }

    private void Awake()
    {
        current = this;
    }

    public void Broadcast(BroadcastEventType e,
                          AddItemEventArgs aea = null,
                          RemoveItemEventArgs rea = null,
                          SwapItemsEventArgs sea = null,
                          SwapItemsTrhuInvEventArgs siea = null,
                          UseItemEventArgs uea = null,
                          DropItemEventArgs dea = null,
                          PickUpItemEventArgs pea = null,
                          InitializeInventoryEventArgs iea = null)
    {
        Debug.Log($"Broadcasting event {e}");
        switch (e)
        {
            case BroadcastEventType.AddItem:
                OnAddItem?.Invoke(this, aea);
                break;
            case BroadcastEventType.RemoveItem:
                OnRemoveItem?.Invoke(this, rea);
                break;
            case BroadcastEventType.SwapItem:
                OnSwapItem?.Invoke(this, sea);
                break;
            case BroadcastEventType.SwapTrhuInventory:
                OnSwapTrhuInventory?.Invoke(this, siea);
                break;
            case BroadcastEventType.UseItem:
                OnUseItem?.Invoke(this, uea);
                break;
            case BroadcastEventType.DropItem:
                OnDropItem?.Invoke(this, dea);
                break;
            case BroadcastEventType.PickUpItem:
                OnPickUpItem?.Invoke(this, pea);
                break;
            case BroadcastEventType.InitializeInventory:
                OnInitializeInventory?.Invoke(this, iea);
                break;
            default:
                break;
        }
    }
}