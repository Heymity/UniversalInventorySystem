using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public static class InventoryController 
{
    public static List<Inventory> inventories = new List<Inventory>();

    public static List<Inventory> GetInventories() => inventories;

    public static Inventory GetInventoryById(int id)
    {
        foreach(Inventory inv in inventories)
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

    /// <summary>
    /// Adds a certain amount of an item to the first empty slot even if there are slots of the same item that can still hold more items. If the specified amount is grater than the maxAmount for that item it will fill the next slot
    /// </summary>
    /// <param name="inv">The inventory in witch the item will be placed</param>
    /// <param name="item">The item that will be stored in the inventory</param>
    /// <param name="amount">The amount of items to be stored</param>
    /// <returns>If the inventory gets full and there are still items to store it will return the number of items remaining</returns>
    public static int AddItemToNewSlot(this Inventory inv, Item item, int amount)
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
                        return amount;
                    }
                }
                else
                {
                    Debug.Log("Not Enought Room");
                    return amount;
                }
            }
            return 0;
        }
        for (int i = 0; i < inv.slots.Count; i++)
        {
            if (inv.slots[i].hasItem && i < inv.slots.Count - 1) continue;
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
            else if (!inv.slots[i].hasItem) inv.slots[i] = new Slot(item, amount, true);
            else
            {
                Debug.Log("Not Enought Room");
                return amount;
            }
        }
        return 0;
    }

    /// <summary>
    /// Adds a certain amount of an item to the first empty slot with the same item that isnt full yet. If it fills a entire slot it will go to the next one. If the specified amount is grater than the maxAmount for that item it will fill the next slot. This is what you should use for pick-up an item.
    /// </summary>
    /// <param name="inv">The inventory in witch the item will be placed</param>
    /// <param name="item">The item that will be stored in the inventory</param>
    /// <param name="amount">The amount of items to be stored</param>
    /// <returns>If the inventory gets full and there are still items to store it will return the number of items remaining</returns>
    public static int AddItem(this Inventory inv, Item item, int amount)
    {
        if (!item.stackable) return AddItemToNewSlot(inv, item, amount);
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
        if (amount > 0) return AddItemToNewSlot(inv, item, amount);
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
    public static int AddItemToSlot(this Inventory inv, Item item, int amount, int slotNumber)
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
            return 0;
        }
        else Debug.Log($"Slot {slotNumber} is already occupied with a different item");
        return -1;
    }

    /// <summary>
    /// Removes a certain item in a certain amount from the first apearence in certain inventory. When a slot runs out of items it goes to the next one with that item
    /// </summary>
    /// <param name="inv">The inventory in witch the item will be removed</param>
    /// <param name="item">The item that will be removed in the inventory</param>
    /// <param name="amount">The amount of items to be removed</param>
    /// <returns>True if it was able to remove the items False if it wasnt</returns>
    public static bool RemoveItem(this Inventory inv, Item item, int amount)
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
                    if (slot.amount <= 0)
                        inv.slots[i] = new Slot(null, 0, false);
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
        return true;
    }

    /// <summary>
    /// Removes a item in a certain slot and amount from the first apearence in certain inventory. When a slot runs out of items it goes to the next one with that item
    /// </summary>
    /// <param name="inv">The inventory in witch the item will be removed</param>
    /// <param name="slot">The slot that will have item removed</param>
    /// <param name="amount">The amount of items to be removed</param>
    /// <returns>True if it was able to remove the items False if it wasnt</returns>
    public static bool RemoveItemInSlot(this Inventory inv, int slot, int amount)
    {
        if (inv.slots[slot].amount == amount)
        {
            inv.slots[slot] = new Slot(null, 0, false);
            return true;
        }
        else if (inv.slots[slot].amount > amount)
        {
            inv.slots[slot] = new Slot(inv.slots[slot].item, inv.slots[slot].amount - amount, true);
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
    public static void UseItemInSlot(this Inventory inv, int slot)
    {
        if (inv.slots[slot].hasItem && inv.areItemsUsable)
        {
            if (inv.slots[slot].item.destroyOnUse)
            {
                Item it = inv.slots[slot].item;
                if (RemoveItemInSlot(inv, slot, inv.slots[slot].item.useHowManyWhenUsed))
                    it.OnUse();
                return;
            }
            else if (!inv.slots[slot].item.destroyOnUse) inv.slots[slot].item.OnUse();
        }
    }

    /// <summary>
    /// Swap the items in two slots
    /// </summary>
    /// <param name="inv">The inventary to have items swapped</param>
    /// <param name="nativeSlot">The slot to lose items</param>
    /// <param name="targetSlot">The slot to gain items</param>
    public static void SwapItemsInSlots(this Inventory inv, int nativeSlot, int targetSlot)
    {
        Slot tmpSlot = inv.slots[targetSlot];
        inv.slots[targetSlot] = inv.slots[nativeSlot];
        inv.slots[nativeSlot] = tmpSlot;
    }

    /// <summary>
    ///Swap a certain amount of items in two slots
    /// </summary>
    /// <param name="inv">The inventary to have items swapped</param>
    /// <param name="nativeSlot">The slot to lose items</param>
    /// <param name="targetSlot">The slot to gain items</param>
    /// <param name="amount">The amount of items to be swaped</param>
    /// <returns>Returns the number of items that dind fit in the other slot</returns>
    public static int SwapItemsInCertainAmountInSlots(this Inventory inv, int nativeSlot, int targetSlot, int amount)
    {
        if (amount > inv.slots[nativeSlot].amount) return amount;
        //if (amount == inv.slots[nativeSlot].amount && inv.slots[nativeSlot].amount != 1) SwapItemsInSlots(inv, nativeSlot, targetSlot);
        else if (inv.slots[targetSlot].item == null)
        {
            inv.slots[targetSlot] = new Slot(inv.slots[nativeSlot].item, amount, true);
            inv.slots[nativeSlot] = new Slot(inv.slots[nativeSlot].item, inv.slots[nativeSlot].amount - amount, true);
            if (inv.slots[nativeSlot].amount <= 0) inv.slots[nativeSlot] = new Slot(null, 0, false);
        }
        else if (inv.slots[nativeSlot].item == inv.slots[targetSlot].item)
        {
            int remaning = AddItemToSlot(inv, inv.slots[nativeSlot].item, amount, targetSlot);
            inv.slots[nativeSlot] = new Slot(inv.slots[nativeSlot].item, inv.slots[nativeSlot].amount - amount + remaning, true);
            if (inv.slots[nativeSlot].amount <= 0) 
                inv.slots[nativeSlot] = new Slot(null, 0, false);
            return remaning;
        }
        else SwapItemsInSlots(inv, nativeSlot, targetSlot);

        return 0;
    }

    public static int SwapItemThruInventoriesSlotToSlot(this Inventory nativeInv, Inventory targetInv, int nativeSlotNumber, int targetSlotNumber, int amount)
    {
        Item item = nativeInv.slots[nativeSlotNumber].item;
        Slot slot = targetInv.slots[targetSlotNumber];
        if (slot.item == item)
        {
            if (RemoveItemInSlot(nativeInv, nativeSlotNumber, amount))
            {
                int remaning = AddItemToSlot(targetInv, item, targetSlotNumber, amount);
                if (slot.hasItem)
                {

                }
                if (remaning > 0) AddItemToSlot(nativeInv, item, nativeSlotNumber, remaning);
            }
        }
               
        return 0;
    }

    public static int SwapItemThruInventories(this Inventory nativeInv, Inventory targetInv, Item item, int amount)
    {       
        if (RemoveItem(nativeInv, item, amount))
        {
            int remaning = AddItem(targetInv, item, amount);
            if (remaning > 0) AddItem(nativeInv, item, remaning);
        }

        return 0;
    }

    /// <summary>
    /// This function must be called when a inventory is being created. It fills the inventory if null Slots, give an id to the inventory and add it to the list of inventories in the InventoryController. This function dont need to be called if you are using an loading system;
    /// </summary>
    /// <param name="inv">The inventory to be initialized</param>
    /// <returns>The list of slots of the inventory</returns>
    public static List<Slot> InitializeInventory(this Inventory inv)
    {
        inv.slots = new List<Slot>();
        for (int i = 0; i < inv.slotAmounts; i++)
        {
            Debug.Log(inv.slots.Count);
            inv.slots.Add(new Slot(null, 0, false));
        }
        Debug.Log(inv.slots.Count);
        inv.id = inventories.Count;
        inventories.Add(inv);
        return inv.slots;
    }

    /// <summary>
    /// This function is an alternative for the InitializeInventory function. It initializes a Inventory using another inventory as a Model;
    /// </summary>
    /// <param name="inv">The inventory to be initialized</param>
    /// <returns>The list of slots of the inventory</returns>
    public static List<Slot> InitializeInventoryFromAnotherInventory(this Inventory inv, Inventory modelInv)
    {
        inv = modelInv;
        Debug.Log(inv.slots.Count);
        inv.id = inventories.Count;
        inventories.Add(inv);
        return inv.slots;
    }

}

[System.Serializable]
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

[System.Serializable]
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
    InventoryToInventory,
    SlotToSlot,
    Any,
    InvToSpecificInv,
    SpecificInvSlot,
    Locked
}

