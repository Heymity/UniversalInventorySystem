using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public static class InventoryController
{
    public static List<InventoryUI> inventoriesUI = new List<InventoryUI>();

    public static List<Inventory> inventories = new List<Inventory>();

    //public static List<Item> items = new List<Item>();

    public static readonly Slot nullSlot = new Slot(null, 0, false);

    public static List<Inventory> GetInventories() => inventories;

    public static Inventory GetInventoryById(int id)
    {
        foreach (Inventory inv in inventories)
        {
            if (inv.id != id) continue;
            return inv;
        }
        return null;
    }

    public static Inventory GetInventory(int index)
    {
        return inventories[index];
    }

    //public static List<Item> GetItems() 
    //{ 
    //   return items;
    //}

    //public static void SetItems(List<Item> _items) => items = _items; 

    /// <summary>
    /// Adds a certain amount of an item to the first empty slot even if there are slots of the same item that can still hold more items. If the specified amount is grater than the maxAmount for that item it will fill the next slot
    /// </summary>
    /// <param name="inv">The inventory in witch the item will be placed</param>
    /// <param name="item">The item that will be stored in the inventory</param>
    /// <param name="amount">The amount of items to be stored</param>
    /// <returns>If the inventory gets full and there are still items to store it will return the number of items remaining</returns>
    public static int AddItemToNewSlot(this Inventory inv, Item item, int amount, BroadcastEventType e = BroadcastEventType.AddItem)
    {
        
        if (!item.stackable)
        {
            for (int i = 0; i < inv.slots.Count; i++)
            {
                if (inv.slots[i].hasItem && i < inv.slots.Count - 1) continue;
                else if (i < inv.slots.Count - 1)
                {
                    inv.slots[i] = new Slot(item, 1, true);
                    amount--;
                    Debug.Log(i);
                    Debug.Log(inv.slots[i].amount);
                    if (amount <= 0) break;
                    continue;
                }
                else if (!inv.slots[i].hasItem)
                {
                    inv.slots[i] = new Slot(item, 1, true);
                    if (amount <= 0) break;
                    if (amount > 0)
                    {
                        Debug.Log($"Not enougth room for {amount} items");
                        InventoryEventsItemsHandler.AddItemEventArgs aea1 = new InventoryEventsItemsHandler.AddItemEventArgs(inv, true, false, item, amount, null);
                        InventoryEventsItemsHandler.current.Broadcast(e, aea1);
                        return amount;
                    }
                }
                else
                {
                    Debug.Log("Not Enought Room");
                    return amount;
                }
            }
            InventoryEventsItemsHandler.AddItemEventArgs aea2 = new InventoryEventsItemsHandler.AddItemEventArgs(inv, true, false, item, amount, null);
            InventoryEventsItemsHandler.current.Broadcast(e, aea2);
            return 0;
        }
        for (int i = 0; i < inv.slots.Count; i++)
        {
            if (inv.slots[i].hasItem) continue;
            else if (i < inv.slots.Count - 1)
            {
                var maxAmount = item.maxAmount;
                Slot newSlot = new Slot(item, amount, true);

                if (amount <= maxAmount)
                    inv.slots[i] = newSlot;
                else
                {
                    inv.slots[i] = new Slot(item, maxAmount, true);
                    amount -= maxAmount;
                    if (amount > 0) continue;
                    else break;
                }


                Debug.Log(i);
                Debug.Log(inv.slots[i].amount);
                break;
            }
            else if (!inv.slots[i].hasItem)
            {
                InventoryEventsItemsHandler.AddItemEventArgs aea2 = new InventoryEventsItemsHandler.AddItemEventArgs(inv, true, false, item, amount, null);
                var newSlot = inv.slots[i].amount;
                amount -= item.maxAmount - newSlot;
                newSlot = item.maxAmount;
                inv.slots[i] = new Slot(item, newSlot, true);
                if (amount > 0)
                {
                    InventoryEventsItemsHandler.current.Broadcast(e, aea2);
                    Debug.Log("Not Enought Room");
                    return amount;
                }
            }
            else
            {
                Debug.Log("Not Enought Room");
                return amount;
            }
        }
        InventoryEventsItemsHandler.AddItemEventArgs aea = new InventoryEventsItemsHandler.AddItemEventArgs(inv, true, false, item, amount, null);
        InventoryEventsItemsHandler.current.Broadcast(e, aea);
        return 0;
    }

    /// <summary>
    /// Adds a certain amount of an item to the first empty slot with the same item that isnt full yet. If it fills a entire slot it will go to the next one. If the specified amount is grater than the maxAmount for that item it will fill the next slot. This is what you should use for pick-up an item.
    /// </summary>
    /// <param name="inv">The inventory in witch the item will be placed</param>
    /// <param name="item">The item that will be stored in the inventory</param>
    /// <param name="amount">The amount of items to be stored</param>
    /// <returns>If the inventory gets full and there are still items to store it will return the number of items remaining</returns>
    public static int AddItem(this Inventory inv, Item item, int amount, BroadcastEventType e = BroadcastEventType.AddItem)
    {
        if (!item.stackable) return AddItemToNewSlot(inv, item, amount, e);
        for (int i = 0; i < inv.slots.Count; i++)
        {
            if (inv.slots[i].item != item) continue;
            if (inv.slots[i].amount == inv.slots[i].item.maxAmount) continue;
            var newSlot = inv.slots[i];

            if (newSlot.amount + amount <= item.maxAmount)
            {
                newSlot.amount += amount;
                amount = 0;
                inv.slots[i] = newSlot;
                break;
            }
            else if (newSlot.amount + amount > item.maxAmount)
            {
                amount -= item.maxAmount - newSlot.amount;
                newSlot.amount = item.maxAmount;
                inv.slots[i] = newSlot;
                if (amount > 0) continue;
            }       
        }
        if (amount > 0) return AddItemToNewSlot(inv, item, amount, e);
        InventoryEventsItemsHandler.AddItemEventArgs aea = new InventoryEventsItemsHandler.AddItemEventArgs(inv, false, false, item, amount, null);
        InventoryEventsItemsHandler.current.Broadcast(e, aea);
        return 0;
    }

    /// <summary>
    /// Adds a certain amount of an item to an specific slot. If it fills slot entirily it will return the remaning items to be stored.
    /// </summary>
    /// <param name="inv">The inventory in witch the item will be placed</param>
    /// <param name="item">The item that will be stored in the inventory</param>
    /// <param name="amount">The amount of items to be stored</param>
    /// <param name="slotNumber">The index of the slot to store the items</param>
    /// <returns>If the slot gets full and there are still items to store it will return the number of items remaining</returns>
    public static int AddItemToSlot(this Inventory inv, Item item, int amount, int slotNumber, BroadcastEventType e = BroadcastEventType.AddItem)
    {
        if (!inv.slots[slotNumber].hasItem)
        {
            if (amount < item.maxAmount)
                inv.slots[slotNumber] = new Slot(item, amount, true);
            else
            {
                inv.slots[slotNumber] = new Slot(item, item.maxAmount, true);
                return amount - item.maxAmount;
            }
            InventoryEventsItemsHandler.AddItemEventArgs aea = new InventoryEventsItemsHandler.AddItemEventArgs(inv, false, true, item, amount, slotNumber);
            InventoryEventsItemsHandler.current.Broadcast(e, aea);
            return 0;
        }
        else if (inv.slots[slotNumber].item == item)
        {
            if (inv.slots[slotNumber].amount + amount < item.maxAmount)
                inv.slots[slotNumber] = new Slot(item, amount + inv.slots[slotNumber].amount, true);
            else
            {
                int valueToReeturn = amount + inv.slots[slotNumber].amount - item.maxAmount;
                inv.slots[slotNumber] = new Slot(item, item.maxAmount, true);
                return valueToReeturn;
            }
            InventoryEventsItemsHandler.AddItemEventArgs aea = new InventoryEventsItemsHandler.AddItemEventArgs(inv, false, true, item, amount, slotNumber);
            InventoryEventsItemsHandler.current.Broadcast(e, aea);
            return 0;
        }
        else Debug.Log($"Slot {slotNumber} is already occupied with a different item");
        InventoryEventsItemsHandler.AddItemEventArgs aeaNull = new InventoryEventsItemsHandler.AddItemEventArgs(inv, false, false, null, 0, slotNumber);
        InventoryEventsItemsHandler.current.Broadcast(e, aeaNull);
        return -1;
    }

    /// <summary>
    /// Drops a item. Its a cover function that calls either the RemoveItem or the RemoveItemInSlot
    /// </summary>
    /// <param name="inv">The inventory in witch the item will be removed</param>
    /// <param name="amount">The amount of items to be removed</param>
    /// <param name="item">The item that will be removed in the inventory</param>
    /// <param name="slot">The slot that will have item removed</param>
    /// <returns>The RemoveItem or RemoveItemInSlot function return value</returns>
    public static bool DropItem(this Inventory inv, int amount, Vector3 dropPosition, Item item = null, int? slot = null, BroadcastEventType e = BroadcastEventType.DropItem)
    {
        if(slot != null)
        {
            return RemoveItemInSlot(inv, slot.GetValueOrDefault(), amount, e);
        } else if(item != null)
        {
            return RemoveItem(inv, item, amount, e);
        } else
        {
            Debug.LogError($"No slot number or item provided; item: {item}, slot number: {slot}");
            return false;
        }
    }

    /// <summary>
    /// Removes a certain item in a certain amount from the first apearence in certain inventory. When a slot runs out of items it goes to the next one with that item
    /// </summary>
    /// <param name="inv">The inventory in witch the item will be removed</param>
    /// <param name="item">The item that will be removed in the inventory</param>
    /// <param name="amount">The amount of items to be removed</param>
    /// <returns>True if it was able to remove the items False if it wasnt</returns>
    public static bool RemoveItem(this Inventory inv, Item item, int amount, BroadcastEventType e = BroadcastEventType.RemoveItem, Vector3? dropPosition = null)
    {
        int total = 0;
        for(int i = 0; i < inv.slots.Count; i++)
        {
            if(inv.slots[i].item == item)
            {
                total += inv.slots[i].amount;
            }
        }
        if(total >= amount)
        {
            for (int i = 0; i < inv.slots.Count; i++)
            {
                if (inv.slots[i].item == item)
                {
                    int prevAmount = inv.slots[i].amount;
                    Slot slot = inv.slots[i];
                    slot.amount -= amount;
                    inv.slots[i] = slot;
                    if (slot.amount <= 0)
                        inv.slots[i] = nullSlot;
                    else break;
                    amount -= prevAmount;
                }
            }
        }
        else
        {
            Debug.Log("There arent enought items to take out!");
            return false;
        }
        dropPosition = (dropPosition ?? new Vector3(0, 0, 0));
        InventoryEventsItemsHandler.RemoveItemEventArgs rea = new InventoryEventsItemsHandler.RemoveItemEventArgs(inv, false, amount, item, null);

        if(e == BroadcastEventType.DropItem)
            item.OnDrop(inv, false, null, amount, false, dropPosition);
        else InventoryEventsItemsHandler.current.Broadcast(e, rea: rea);
        return true;
    }

    /// <summary>
    /// Removes a item in a certain slot and amount from the first apearence in certain inventory. When a slot runs out of items it goes to the next one with that item
    /// </summary>
    /// <param name="inv">The inventory in witch the item will be removed</param>
    /// <param name="slot">The slot that will have item removed</param>
    /// <param name="amount">The amount of items to be removed</param>
    /// <returns>True if it was able to remove the items False if it wasnt</returns>
    public static bool RemoveItemInSlot(this Inventory inv, int slot, int amount, BroadcastEventType e = BroadcastEventType.RemoveItem, Vector3? dropPosition = null)
    {
        dropPosition = (dropPosition ?? new Vector3(0, 0, 0));
        InventoryEventsItemsHandler.RemoveItemEventArgs rea = new InventoryEventsItemsHandler.RemoveItemEventArgs(inv, false, amount, inv.slots[slot].item, slot);

        if (inv.slots[slot].amount == amount)
        {
            Item tmp = inv.slots[slot].item;
            inv.slots[slot] = nullSlot;
            if (e == BroadcastEventType.DropItem)
                tmp.OnDrop(inv, true, slot, amount, false, dropPosition);
            else InventoryEventsItemsHandler.current.Broadcast(e, rea: rea);
            return true;
        }
        else if (inv.slots[slot].amount > amount)
        {
            Item tmp = inv.slots[slot].item;
            inv.slots[slot] = new Slot(inv.slots[slot].item, inv.slots[slot].amount - amount, true);
            if (e == BroadcastEventType.DropItem)
                tmp.OnDrop(inv, true, slot, amount, false, dropPosition);
            else InventoryEventsItemsHandler.current.Broadcast(e, rea: rea);
            return true;
        }
        else
        {
            Debug.Log("There arent enought items to take out!");
            return false;
        }
    }

    /// <summary>
    /// To be Implemented by the proper way
    /// </summary>
    /// <param name="inv">The inventory in witch the item will be used</param>
    /// <param name="slot">The slot that will have item used</param>
    public static void UseItemInSlot(this Inventory inv, int slot, BroadcastEventType e = BroadcastEventType.UseItem)
    {
        if (inv.slots[slot].hasItem && inv.areItemsUsable)
        {
            if (inv.slots[slot].item.destroyOnUse)
            {
                Item it = inv.slots[slot].item;
                if (RemoveItemInSlot(inv, slot, inv.slots[slot].item.useHowManyWhenUsed))
                {
                    it.OnUse(inv, slot);
                    InventoryEventsItemsHandler.UseItemEventArgs uea = new InventoryEventsItemsHandler.UseItemEventArgs(inv, it, slot);
                    InventoryEventsItemsHandler.current.Broadcast(e, uea: uea);
                }
                return;
            }
            else if (!inv.slots[slot].item.destroyOnUse) inv.slots[slot].item.OnUse(inv, slot);
        }
    }

    /// <summary>
    /// Swap the items in two slots. this function will NOT stack the items for that use SwapItemsInCertainAmountInSlots with amount = null
    /// </summary>
    /// <param name="inv">The inventary to have items swapped</param>
    /// <param name="nativeSlot">The slot to lose items</param>
    /// <param name="targetSlot">The slot to gain items</param>
    public static void SwapItemsInSlots(this Inventory inv, int nativeSlot, int targetSlot, BroadcastEventType e = BroadcastEventType.SwapItem)
    {
        Slot tmpSlot = inv.slots[targetSlot];
        inv.slots[targetSlot] = inv.slots[nativeSlot];
        inv.slots[nativeSlot] = tmpSlot;
        InventoryEventsItemsHandler.SwapItemsEventArgs sea = new InventoryEventsItemsHandler.SwapItemsEventArgs(inv, nativeSlot, targetSlot, inv.slots[targetSlot].item, tmpSlot.item, null);
        InventoryEventsItemsHandler.current.Broadcast(e, sea: sea);
    }

    /// <summary>
    ///Swap a certain amount of items in two slots. This function will stack items.
    /// </summary>
    /// <param name="inv">The inventary to have items swapped</param>
    /// <param name="nativeSlot">The slot to lose items</param>
    /// <param name="targetSlot">The slot to gain items</param>
    /// <param name="amount">The amount of items to be swaped</param>
    /// <returns>Returns the number of items that dind fit in the other slot</returns>
    public static int SwapItemsInCertainAmountInSlots(this Inventory inv, int nativeSlot, int targetSlot, int? _amount, BroadcastEventType e = BroadcastEventType.SwapItem)
    {
        int amount = (_amount ?? inv.slots[nativeSlot].amount);
        if (amount <= 0) return amount;
        InventoryEventsItemsHandler.SwapItemsEventArgs sea;
        if (amount > inv.slots[nativeSlot].amount) return amount;
        else if (inv.slots[targetSlot].item == null)
        {
            inv.slots[targetSlot] = new Slot(inv.slots[nativeSlot].item, amount, true);
            inv.slots[nativeSlot] = new Slot(inv.slots[nativeSlot].item, inv.slots[nativeSlot].amount - amount, true);
            if (inv.slots[nativeSlot].amount <= 0) inv.slots[nativeSlot] = nullSlot;
        }
        else if (inv.slots[nativeSlot].item == inv.slots[targetSlot].item)
        {
            int remaning = AddItemToSlot(inv, inv.slots[nativeSlot].item, amount, targetSlot);
            inv.slots[nativeSlot] = new Slot(inv.slots[nativeSlot].item, inv.slots[nativeSlot].amount - amount + remaning, true);
            if (inv.slots[nativeSlot].amount <= 0) 
                inv.slots[nativeSlot] = nullSlot;
            sea = new InventoryEventsItemsHandler.SwapItemsEventArgs(inv, nativeSlot, targetSlot, inv.slots[targetSlot].item, inv.slots[nativeSlot].item, amount - remaning);
            InventoryEventsItemsHandler.current.Broadcast(e, sea: sea);
            return remaning;
        }
        else SwapItemsInSlots(inv, nativeSlot, targetSlot);

        sea = new InventoryEventsItemsHandler.SwapItemsEventArgs(inv, nativeSlot, targetSlot, inv.slots[targetSlot].item, inv.slots[nativeSlot].item, amount);
        InventoryEventsItemsHandler.current.Broadcast(e, sea: sea);
        return 0;
    }

    /// <summary>
    /// This function takes out items from a slot A from the inventory A and places it in the slot B of the inventory B. If the slot B gets full and slot A has still items to be swaped, it will only return the remaing items to be swaped
    /// </summary>
    /// <param name="nativeInv">The inventory in witch the items are</param>
    /// <param name="targetInv">The inventory in witch the items will go</param>
    /// <param name="nativeSlotNumber">The slot index witch will give items</param>
    /// <param name="targetSlotNumber">The slot index witch will receive items </param>
    /// <param name="amount">The amount of items to be swaped</param>
    /// <returns>Returns the number of items that worent transfered</returns>
    public static int SwapItemThruInventoriesSlotToSlot(this Inventory nativeInv, Inventory targetInv, int nativeSlotNumber, int targetSlotNumber, int amount, BroadcastEventType e = BroadcastEventType.SwapTrhuInventory)
    {
        InventoryEventsItemsHandler.SwapItemsTrhuInvEventArgs siea;
        if (amount > nativeInv.slots[nativeSlotNumber].amount) return amount;
        else if (targetInv.slots[targetSlotNumber].item == null)
        {
            targetInv.slots[targetSlotNumber] = new Slot(nativeInv.slots[nativeSlotNumber].item, amount, true);
            nativeInv.slots[nativeSlotNumber] = new Slot(nativeInv.slots[nativeSlotNumber].item, nativeInv.slots[nativeSlotNumber].amount - amount, true);
            if (nativeInv.slots[nativeSlotNumber].amount <= 0) nativeInv.slots[nativeSlotNumber] = nullSlot;
        }
        else if (nativeInv.slots[nativeSlotNumber].item == targetInv.slots[targetSlotNumber].item)
        {
            int remaning = AddItemToSlot(targetInv, nativeInv.slots[nativeSlotNumber].item, amount, targetSlotNumber);
            nativeInv.slots[nativeSlotNumber] = new Slot(nativeInv.slots[nativeSlotNumber].item, nativeInv.slots[nativeSlotNumber].amount - amount + remaning, true);
            if (nativeInv.slots[nativeSlotNumber].amount <= 0)
                nativeInv.slots[nativeSlotNumber] = nullSlot;
            siea = new InventoryEventsItemsHandler.SwapItemsTrhuInvEventArgs(nativeInv, targetInv, nativeSlotNumber, targetSlotNumber, targetInv.slots[targetSlotNumber].item, nativeInv.slots[nativeSlotNumber].item, amount - remaning);
            InventoryEventsItemsHandler.current.Broadcast(e, siea: siea);
            return remaning;
        }
        else
        {
            Slot tmpSlot = targetInv.slots[targetSlotNumber];
            targetInv.slots[targetSlotNumber] = nativeInv.slots[nativeSlotNumber];
            nativeInv.slots[nativeSlotNumber] = tmpSlot;
        }

        siea = new InventoryEventsItemsHandler.SwapItemsTrhuInvEventArgs(nativeInv, targetInv, nativeSlotNumber, targetSlotNumber, targetInv.slots[targetSlotNumber].item, nativeInv.slots[nativeSlotNumber].item, amount);
        InventoryEventsItemsHandler.current.Broadcast(e, siea: siea);
        return 0;
    }

    /// <summary>
    /// This function takes the item I from inventory A and puts into inventory B. If B get full and there are still items to be transfered, the remaing items go back to inventory A
    /// </summary>
    /// <param name="nativeInv">The inventory in witch the items are</param>
    /// <param name="targetInv">The inventory in witch the items will go</param>
    /// <param name="item">The item that will be swaped</param>
    /// <param name="amount">The amount of items to be swaped</param>
    /// <returns>
    /// True => Basicly, every time it is not false
    /// False => When it was not able to remove the item from the nativeInv, that means the RemoveItem function returned false, that usually means there were not enought items in the inventory to take out (it is also false when it is not true btw)
    /// </returns>
    public static bool SwapItemThruInventories(this Inventory nativeInv, Inventory targetInv, Item item, int amount, BroadcastEventType e = BroadcastEventType.SwapTrhuInventory)
    {
        if (RemoveItem(nativeInv, item, amount))
        {
            int remaning = AddItem(targetInv, item, amount);
            if (remaning > 0) AddItem(nativeInv, item, remaning);
        }
        else return false;

        InventoryEventsItemsHandler.SwapItemsTrhuInvEventArgs siea = new InventoryEventsItemsHandler.SwapItemsTrhuInvEventArgs(nativeInv, targetInv, null, null, item, null, amount);
        InventoryEventsItemsHandler.current.Broadcast(e, siea: siea);
        return true;
    }

    /// <summary>
    /// This function must be called when a inventory is being created. It fills the inventory if null Slots, give an id to the inventory and add it to the list of inventories in the InventoryController. This function dont need to be called if you are using an loading system;
    /// </summary>
    /// <param name="inv">The inventory to be initialized</param>
    /// <returns>The list of slots of the inventory</returns>
    public static List<Slot> InitializeInventory(this Inventory inv, BroadcastEventType e = BroadcastEventType.InitializeInventory)
    {
        inv.slots = new List<Slot>();
        for (int i = 0; i < inv.slotAmounts; i++)
        {
            //Debug.Log(inv.slots.Count);
            inv.slots.Add(new Slot(null, 0, false));
        }
        //Debug.Log(inv.slots.Count);
        inv.id = inventories.Count;
        inventories.Add(inv);
        inv.hasInitializated = true;
        InventoryEventsItemsHandler.InitializeInventoryEventArgs iea = new InventoryEventsItemsHandler.InitializeInventoryEventArgs(inv);
        InventoryEventsItemsHandler.current.Broadcast(e, iea: iea);
        return inv.slots;
    }

    /// <summary>
    /// This function is an alternative for the InitializeInventory function. It initializes a Inventory using another inventory as a Model;
    /// </summary>
    /// <param name="inv">The inventory to be initialized</param>
    /// <returns>The list of slots of the inventory</returns>
    public static List<Slot> InitializeInventoryFromAnotherInventory(this Inventory inv, Inventory modelInv, BroadcastEventType e = BroadcastEventType.InitializeInventory)
    {
        inv = modelInv;
        Debug.Log(inv.slots.Count);
        inv.id = inventories.Count;
        inventories.Add(inv);
        inv.hasInitializated = true;
        InventoryEventsItemsHandler.InitializeInventoryEventArgs iea = new InventoryEventsItemsHandler.InitializeInventoryEventArgs(inv);
        InventoryEventsItemsHandler.current.Broadcast(e, iea: iea);
        return inv.slots;
    }

}

[Serializable]
public class Inventory
{
    public List<Slot> slots;
    public int slotAmounts;
    public int id;
    public bool areItemsUsable;
    public bool areItemsDroppable;
    /// <summary>
    /// Defines the type of interaractions you can have with the inventory
    /// </summary>
    public IteractiableTypes interactiable;

    public bool hasInitializated;

    public Inventory(List<Slot> _slots, int _slotAmounts, IteractiableTypes _interactiable, bool _areItemsUsable = true, bool _areItemsDroppable = true)
    {
        slots = _slots;
        slotAmounts = _slotAmounts;
        areItemsUsable = _areItemsUsable;
        interactiable = _interactiable;
        areItemsDroppable = _areItemsDroppable; 
    }   

    public Inventory(List<Slot> _slots, int _slotAmounts, bool _areItemsUsable)
    {
        slots = _slots;
        slotAmounts = _slotAmounts;
        areItemsUsable = _areItemsUsable;
    }

    public Inventory(List<Slot> _slots, int _slotAmounts)
    {
        slots = _slots;
        slotAmounts = _slotAmounts;
    }

    public Inventory(int _slotAmounts, bool _areItemsUsable, IteractiableTypes _interactiable = IteractiableTypes.Any, bool _areItemsDroppable = true)
    {
        slots = new List<Slot>();
        slotAmounts = _slotAmounts;
        areItemsUsable = _areItemsUsable;
        interactiable = _interactiable;
        areItemsDroppable = _areItemsDroppable;
    }

    public Inventory(int _slotAmounts, bool _areItemsUsable, IteractiableTypes _interactiable = IteractiableTypes.Any)
    {
        slots = new List<Slot>();
        slotAmounts = _slotAmounts;
        areItemsUsable = _areItemsUsable;
        interactiable = _interactiable;
    }

    public Inventory(int _slotAmounts, bool _areItemsUsable = true)
    {
        slots = new List<Slot>();
        slotAmounts = _slotAmounts;
        areItemsUsable = _areItemsUsable;
    }

    public Inventory(int _slotAmounts)
    {
        slots = new List<Slot>();
        slotAmounts = _slotAmounts;
    }

}

[Serializable]
public struct Slot
{
    public int amount;
    public Item item;
    public bool hasItem;

    public Slot(Item _item)
    {
        item = _item;
        amount = 1;
        hasItem = item != null ? false : true;

    }

    public Slot(Item _item, int _amount)
    {
        item = _item;
        amount = _amount;
        hasItem = amount == 0 ? false : true;

    }

    public Slot(Item _item, int _amount, bool _hasItem)
    {
        item = _item;
        amount = _amount;
        hasItem = _hasItem;
    }
}

public enum IteractiableTypes
{
    InventoryToInventory = 0,
    SlotToSlot = 1,
    InvToSpecificInv = 2,
    Any = 3,
    Locked = 4
}



