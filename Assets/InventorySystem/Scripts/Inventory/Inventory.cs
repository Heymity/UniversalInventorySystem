using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniversalInventorySystem
{
    [Serializable]
    public static class InventoryController
    {
        public static List<InventoryUI> inventoriesUI = new List<InventoryUI>();

        public static List<Inventory> inventories = new List<Inventory>();

        public static readonly Slot nullSlot = new Slot(null, 0, false);

        #region Protection readonly and methods

        public static readonly InventoryProtection[] allInventoryProtections = new InventoryProtection[6]
        {
        InventoryProtection.Any,
        InventoryProtection.InventoryToInventory,
        InventoryProtection.Locked,
        InventoryProtection.LockSlots,
        InventoryProtection.LockThruInventory,
        InventoryProtection.SlotToSlot
        };

        public static InventoryProtection[] newInvProtectionArray(params InventoryProtection[] protections) { return protections; }

        public static readonly SlotProtection[] allSlotProtections = new SlotProtection[4]
        {
        SlotProtection.Any,
        SlotProtection.Locked,
        SlotProtection.OnlyAdd,
        SlotProtection.OnlyRemove
        };

        public static SlotProtection[] newSlotProtectionArray(params SlotProtection[] protections) { return protections; }

        #endregion

        public static InventoryData inventoryData = new InventoryData();

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

        public static Inventory GetInventory(int index) => inventories[index];

        public static InventoryData SaveInventoryData()
        {
            inventoryData.inventories = inventories.ToArray();
            return inventoryData;
        }

        public static InventoryData LoadInventoryData(InventoryData loadData)
        {
            inventories = loadData.inventories.ToList();
            return SaveInventoryData();
        }

        #region Add

        /// <summary>
        /// Adds a certain amount of an item to the first empty slot even if there are slots of the same item that can still hold more items. If the specified amount is grater than the maxAmount for that item it will fill the next slot
        /// </summary>
        /// <param name="inv">The inventory in witch the item will be placed</param>
        /// <param name="item">The item that will be stored in the inventory</param>
        /// <param name="amount">The amount of items to be stored</param>
        /// <returns>If the inventory gets full and there are still items to store it will return the number of items remaining</returns>
        public static int AddItemToNewSlot(this Inventory inv, Item item, int amount, BroadcastEventType e = BroadcastEventType.AddItem, bool overrideSlotProtection = false, uint? durability = null, Action callback = null)
        {
            if (inv.interactiable == InventoryProtection.Locked) return amount;

            if (inv == null)
            {
                Debug.LogError("Null inventory provided for AddItemToNewSlot");
                return -1;
            }
            if (item == null)
            {
                Debug.LogError("Null item provided for AddItemToNewSlot");
                return -1;
            }

            if(durability == null) durability = item.maxDurability; 

            if (!item.stackable)
            {
                for (int i = 0; i < inv.slots.Count; i++)
                {
                    if (inv.slots[i].hasItem && i < inv.slots.Count - 1) continue;
                    else if (i < inv.slots.Count - 1)
                    {
                        if ((inv.slots[i].interative == SlotProtection.Locked || inv.slots[i].interative == SlotProtection.OnlyRemove) && !overrideSlotProtection) continue;
                        inv.slots[i] = new Slot(item, 1, true, inv.slots[i].isProductSlot, inv.slots[i].interative, inv.slots[i].whitelist, durability.GetValueOrDefault());
                        amount--;

                        if (amount <= 0) break;
                        continue;
                    }
                    else if (!inv.slots[i].hasItem)
                    {
                        inv.slots[i] = new Slot(item, 1, true, inv.slots[i].isProductSlot, inv.slots[i].interative, inv.slots[i].whitelist, durability.GetValueOrDefault());
                        if (amount <= 0) break;
                        if (amount > 0)
                        {
                            Debug.Log($"Not enougth room for {amount} items");
                            InventoryHandler.AddItemEventArgs aea1 = new InventoryHandler.AddItemEventArgs(inv, true, false, item, amount, null);
                            InventoryHandler.current.Broadcast(e, aea1);
                            return amount;
                        }
                    }
                    else
                    {
                        Debug.Log("Not Enought Room");
                        return amount;
                    }
                }
                callback?.Invoke();
                InventoryHandler.AddItemEventArgs aea2 = new InventoryHandler.AddItemEventArgs(inv, true, false, item, amount, null);
                InventoryHandler.current.Broadcast(e, aea2);
                return 0;
            }
            for (int i = 0; i < inv.slots.Count; i++)
            {
                if (inv.slots[i].hasItem) continue;
                if ((inv.slots[i].interative == SlotProtection.Locked || inv.slots[i].interative == SlotProtection.OnlyRemove) && !overrideSlotProtection) continue;
                if (!inv.slots[i].whitelist?.itemsList.Contains(item) ?? false) continue;

                else if (i < inv.slots.Count - 1)
                {
                    var maxAmount = item.maxAmount;
                    Slot newSlot = new Slot(item, amount, true, inv.slots[i].isProductSlot, inv.slots[i].interative, inv.slots[i].whitelist, durability.GetValueOrDefault());

                    if (amount <= maxAmount)
                        inv.slots[i] = newSlot;
                    else
                    {
                        inv.slots[i] = new Slot(item, maxAmount, true, inv.slots[i].isProductSlot, inv.slots[i].interative, inv.slots[i].whitelist, durability.GetValueOrDefault());
                        amount -= maxAmount;
                        if (amount > 0) continue;
                        else break;
                    }

                    break;
                }
                else if (!inv.slots[i].hasItem)
                {
                    InventoryHandler.AddItemEventArgs aea2 = new InventoryHandler.AddItemEventArgs(inv, true, false, item, amount, null);
                    var newSlot = inv.slots[i].amount;
                    amount -= item.maxAmount - newSlot;
                    newSlot = item.maxAmount;
                    inv.slots[i] = new Slot(item, newSlot, true, inv.slots[i].isProductSlot, inv.slots[i].interative, inv.slots[i].whitelist, durability.GetValueOrDefault());
                    if (amount > 0)
                    {
                        InventoryHandler.current.Broadcast(e, aea2);
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
            callback?.Invoke();
            InventoryHandler.AddItemEventArgs aea = new InventoryHandler.AddItemEventArgs(inv, true, false, item, amount, null);
            InventoryHandler.current.Broadcast(e, aea);
            return 0;
        }

        /// <summary>
        /// Adds a certain amount of an item to the first empty slot with the same item that isnt full yet. If it fills a entire slot it will go to the next one. If the specified amount is grater than the maxAmount for that item it will fill the next slot. This is what you should use for pick-up an item.
        /// </summary>
        /// <param name="inv">The inventory in witch the item will be placed</param>
        /// <param name="item">The item that will be stored in the inventory</param>
        /// <param name="amount">The amount of items to be stored</param>
        /// <returns>If the inventory gets full and there are still items to store it will return the number of items remaining</returns>
        public static int AddItem(this Inventory inv, Item item, int amount, BroadcastEventType e = BroadcastEventType.AddItem, bool overrideSlotProtection = false, uint? durability = null, Action callback = null)
        {
            if (inv.interactiable == InventoryProtection.Locked) return amount;

            if (inv == null)
            {
                Debug.LogError("Null inventory provided for AddItem");
                return -1;
            }
            if (item == null)
            {
                Debug.LogError("Null item provided for AddItem");
                return -1;
            }

            if (durability == null) durability = item.maxDurability;

            if (!item.stackable) return AddItemToNewSlot(inv, item, amount, e);
            for (int i = 0; i < inv.slots.Count; i++)
            {
                if (inv.slots[i].item != item) continue;
                if (inv.slots[i].amount == inv.slots[i].item.maxAmount) continue;
                if ((inv.slots[i].interative == SlotProtection.Locked || inv.slots[i].interative == SlotProtection.OnlyRemove) && !overrideSlotProtection) continue;
                if (!inv.slots[i].whitelist?.itemsList.Contains(item) ?? false) continue;
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
            callback?.Invoke();
            InventoryHandler.AddItemEventArgs aea = new InventoryHandler.AddItemEventArgs(inv, false, false, item, amount, null);
            InventoryHandler.current.Broadcast(e, aea);
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
        public static int AddItemToSlot(this Inventory inv, Item item, int amount, int slotNumber, BroadcastEventType e = BroadcastEventType.AddItem, bool overrideSlotProtection = false, uint? durability = null, Action callback = null)
        {
            if (inv.interactiable == InventoryProtection.Locked) return amount;

            if ((inv.slots[slotNumber].interative == SlotProtection.Locked || inv.slots[slotNumber].interative == SlotProtection.OnlyRemove) && !overrideSlotProtection) return amount;
            if (!inv.slots[slotNumber].whitelist?.itemsList.Contains(item) ?? false) return amount;

            if (durability == null) durability = item.maxDurability;

            if (inv == null)
            {
                Debug.LogError("Null inventory provided for AddItemToSlot");
                return -1;
            }
            if (item == null)
            {
                Debug.LogError("Null item provided for AddItemToSlot");
                return -1;
            }

            if (!inv.slots[slotNumber].hasItem)
            {
                if (amount < item.maxAmount)
                    inv.slots[slotNumber] = new Slot(item, amount, true, inv.slots[slotNumber].isProductSlot, inv.slots[slotNumber].interative, inv.slots[slotNumber].whitelist);
                else
                {
                    inv.slots[slotNumber] = new Slot(item, item.maxAmount, true, inv.slots[slotNumber].isProductSlot, inv.slots[slotNumber].interative, inv.slots[slotNumber].whitelist);
                    return amount - item.maxAmount;
                }
                callback?.Invoke();
                InventoryHandler.AddItemEventArgs aea = new InventoryHandler.AddItemEventArgs(inv, false, true, item, amount, slotNumber);
                InventoryHandler.current.Broadcast(e, aea);
                return 0;
            }
            else if (inv.slots[slotNumber].item == item)
            {
                if (inv.slots[slotNumber].amount + amount < item.maxAmount)
                    inv.slots[slotNumber] = new Slot(item, amount + inv.slots[slotNumber].amount, true, inv.slots[slotNumber].isProductSlot, inv.slots[slotNumber].interative, inv.slots[slotNumber].whitelist);
                else
                {
                    int valueToReeturn = amount + inv.slots[slotNumber].amount - item.maxAmount;
                    inv.slots[slotNumber] = new Slot(item, item.maxAmount, true, inv.slots[slotNumber].isProductSlot, inv.slots[slotNumber].interative, inv.slots[slotNumber].whitelist);
                    return valueToReeturn;
                }
                callback?.Invoke();
                InventoryHandler.AddItemEventArgs aea = new InventoryHandler.AddItemEventArgs(inv, false, true, item, amount, slotNumber);
                InventoryHandler.current.Broadcast(e, aea);
                return 0;
            }
            else Debug.Log($"Slot {slotNumber} is already occupied with a different item");
            InventoryHandler.AddItemEventArgs aeaNull = new InventoryHandler.AddItemEventArgs(inv, false, false, null, 0, slotNumber);
            InventoryHandler.current.Broadcast(e, aeaNull);
            return -1;
        }
     
        #endregion

        #region Remove

        /// <summary>
        /// Drops a item. Its a cover function that calls either the RemoveItem or the RemoveItemInSlot
        /// </summary>
        /// <param name="inv">The inventory in witch the item will be removed</param>
        /// <param name="amount">The amount of items to be removed</param>
        /// <param name="item">The item that will be removed in the inventory</param>
        /// <param name="slot">The slot that will have item removed</param>
        /// <returns>The RemoveItem or RemoveItemInSlot function return value</returns>
        public static bool DropItem(this Inventory inv, int amount, Vector3 dropPosition, Item item, BroadcastEventType e = BroadcastEventType.DropItem, bool overrideSlotProtecion = true)
        {
            if (inv.interactiable == InventoryProtection.Locked) return false;

            if (inv == null)
            {
                Debug.LogError("Null inventory provided for DropItem");
                return false;
            }

            else if (item != null)
            {
                return RemoveItem(inv, item, amount, e, dropPosition, overrideSlotProtecion);
            }
            else
            {
                Debug.LogError($"Null item provided for DropItem");
                return false;
            }
        }

        public static bool DropItem(this Inventory inv, int amount, Vector3 dropPosition, int slot, BroadcastEventType e = BroadcastEventType.DropItem, bool overrideSlotProtecion = true)
        {
            if (inv.interactiable == InventoryProtection.Locked) return false;

            if (inv == null)
            {
                Debug.LogError("Null inventory provided for DropItem");
                return false;
            }

            if (slot >= 0 && slot < inv.slots.Count)
            {
                return RemoveItemInSlot(inv, slot, amount, e, dropPosition, overrideSlotProtecion);
            }
            else
            {
                Debug.LogError($"Invalid slot number provided for DropItem; slot number: {slot}");
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
        public static bool RemoveItem(this Inventory inv, Item item, int amount, BroadcastEventType e = BroadcastEventType.RemoveItem, Vector3? dropPosition = null, bool overrideSlotProtecion = false)
        {
            if (inv.interactiable == InventoryProtection.Locked) return false;

            if (inv == null)
            {
                Debug.LogError("Null inventory provided for RemoveItem");
                return false;
            }
            if (item == null)
            {
                Debug.LogError("Null item provided for RemoveItem");
                return false;
            }

            int total = 0;
            for (int i = 0; i < inv.slots.Count; i++)
            {
                if (inv.slots[i].item == item && (inv.slots[i].interative == SlotProtection.OnlyRemove || inv.slots[i].interative == SlotProtection.Any || overrideSlotProtecion))
                {
                    total += inv.slots[i].amount;
                }
            }
            if (total >= amount)
            {
                for (int i = 0; i < inv.slots.Count; i++)
                {
                    if (inv.slots[i].item == item && (inv.slots[i].interative == SlotProtection.OnlyRemove || inv.slots[i].interative == SlotProtection.Any || overrideSlotProtecion))
                    {
                        int prevAmount = inv.slots[i].amount;
                        Slot slot = inv.slots[i];
                        slot.amount -= amount;
                        inv.slots[i] = slot;
                        if (slot.amount <= 0)
                            inv.slots[i] = new Slot(nullSlot, inv.slots[i].isProductSlot, inv.slots[i].interative, inv.slots[i].whitelist);
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
            InventoryHandler.RemoveItemEventArgs rea = new InventoryHandler.RemoveItemEventArgs(inv, false, amount, item, null);

            if (e == BroadcastEventType.DropItem)
                item.OnDrop(inv, false, null, amount, false, dropPosition);
            else InventoryHandler.current.Broadcast(e, rea: rea);
            return true;
        }

        /// <summary>
        /// Removes a item in a certain slot and amount from the first apearence in certain inventory. When a slot runs out of items it goes to the next one with that item
        /// </summary>
        /// <param name="inv">The inventory in witch the item will be removed</param>
        /// <param name="slot">The slot that will have item removed</param>
        /// <param name="amount">The amount of items to be removed</param>
        /// <returns>True if it was able to remove the items False if it wasnt</returns>
        public static bool RemoveItemInSlot(this Inventory inv, int slot, int amount, BroadcastEventType e = BroadcastEventType.RemoveItem, Vector3? dropPosition = null, bool overrideSlotProtecion = false)
        {
            if (inv.interactiable == InventoryProtection.Locked) return false;

            if (inv == null)
            {
                Debug.LogError("Null inventory provided for RemoveItemInSlot");
                return false;
            }

            if (!(inv.slots[slot].interative == SlotProtection.OnlyRemove || inv.slots[slot].interative == SlotProtection.Any) && !overrideSlotProtecion) return false;

            dropPosition = (dropPosition ?? new Vector3(0, 0, 0));
            InventoryHandler.RemoveItemEventArgs rea = new InventoryHandler.RemoveItemEventArgs(inv, false, amount, inv.slots[slot].item, slot);

            if (inv.slots[slot].amount == amount)
            {
                Item tmp = inv.slots[slot].item;
                inv.slots[slot] = new Slot(
                    nullSlot,
                    inv.slots[slot].isProductSlot,
                    inv.slots[slot].interative,
                    inv.slots[slot].whitelist
                );

                if (e == BroadcastEventType.DropItem)
                    tmp?.OnDrop(inv, true, slot, amount, false, dropPosition);
                else InventoryHandler.current.Broadcast(e, rea: rea);
                return true;
            }
            else if (inv.slots[slot].amount > amount)
            {
                Item tmp = inv.slots[slot].item;
                inv.slots[slot] = new Slot(
                    inv.slots[slot].item, inv.slots[slot].amount - amount, 
                    true, 
                    inv.slots[slot].isProductSlot, 
                    inv.slots[slot].interative,
                    inv.slots[slot].whitelist
                );

                if (e == BroadcastEventType.DropItem)
                    tmp?.OnDrop(inv, true, slot, amount, false, dropPosition);
                else InventoryHandler.current.Broadcast(e, rea: rea);
                return true;
            }
            else
            {
                Debug.Log("There arent enought items to take out!");
                return false;
            }
        }

        #endregion

        #region Using

        /// <summary>
        /// Calls the OnUse function on the item. This function calls the OnUse on the provided useBehavoiur script
        /// </summary>
        /// <param name="inv">The inventory in witch the item will be used</param>
        /// <param name="slot">The slot that will have item used</param>
        public static void UseItemInSlot(this Inventory inv, int slot, BroadcastEventType e = BroadcastEventType.UseItem)
        {
            if (inv.interactiable == InventoryProtection.Locked) return;

            if (inv == null)
            {
                Debug.LogError("Null inventory provided for UseItemInSlot");
                return;
            }

            if (inv.slots[slot].hasItem && inv.areItemsUsable)
            {
                if (inv.slots[slot].item.destroyOnUse)
                {
                    Item it = inv.slots[slot].item;
                    if (it.hasDurability)
                    {
                        if(inv.slots[slot].durability > 0)
                        {
                            var tmp = inv.slots[slot];
                            Slot.SetDurability(ref tmp, inv.slots[slot].durability - 1u);
                            inv.slots[slot] = tmp;
                            InventoryHandler.UseItemEventArgs uea = new InventoryHandler.UseItemEventArgs(inv, it, slot);
                            if (inv.slots[slot].durability == 0)
                            {
                                if (RemoveItemInSlot(inv, slot, inv.slots[slot].item.useHowManyWhenUsed))
                                {
                                    it.OnUse(inv, slot);
                                    InventoryHandler.current.Broadcast(e, uea: uea);
                                }
                            }
                            else
                            {
                                it.OnUse(inv, slot);
                                InventoryHandler.current.Broadcast(e, uea: uea);
                            }
                        }
                    }
                    else
                    {
                        if (RemoveItemInSlot(inv, slot, inv.slots[slot].item.useHowManyWhenUsed))
                        {
                            it.OnUse(inv, slot);
                            InventoryHandler.UseItemEventArgs uea = new InventoryHandler.UseItemEventArgs(inv, it, slot);
                            InventoryHandler.current.Broadcast(e, uea: uea);
                        }
                    }
                    return;
                }
                else if (!inv.slots[slot].item.destroyOnUse)
                {
                    if (inv.slots[slot].item.hasDurability)
                    {
                        if (inv.slots[slot].durability > 0)
                        {
                            var tmp = inv.slots[slot];
                            Slot.SetDurability(ref tmp, inv.slots[slot].durability - 1u);
                            inv.slots[slot] = tmp;
                            InventoryHandler.UseItemEventArgs uea = new InventoryHandler.UseItemEventArgs(inv, inv.slots[slot].item, slot);
                            inv.slots[slot].item.OnUse(inv, slot);
                            InventoryHandler.current.Broadcast(e, uea: uea);

                        }
                    }
                    else
                    {
                        inv.slots[slot].item.OnUse(inv, slot);
                        InventoryHandler.UseItemEventArgs uea = new InventoryHandler.UseItemEventArgs(inv, inv.slots[slot].item, slot);
                        InventoryHandler.current.Broadcast(e, uea: uea);
                    }
                }
            }
        }

        /// <summary>
        /// Calls the OnUse function on the item. This function calls the OnUse on the provided useBehavoiur script
        /// </summary>
        /// <param name="inv">The inventory in witch the item will be used</param>
        /// <param name="item">The item that will be used</param>
        public static void UseItem(this Inventory inv, Item item, BroadcastEventType e = BroadcastEventType.UseItem)
        {
            if (inv.interactiable == InventoryProtection.Locked) return;
            if (!inv.areItemsUsable) return;

            if (inv == null)
            {
                Debug.LogError("Null inventory provided for UseItemInSlot");
                return;
            }

            for(int i = 0; i < inv.slots.Count; i++)
            {
                if (!inv.slots[i].hasItem) continue;
                if (inv.slots[i].item != item) continue;
                if (inv.slots[i].item.destroyOnUse)
                {
                    Item it = inv.slots[i].item;
                    if (it.hasDurability)
                    {
                        if (inv.slots[i].durability > 0)
                        {
                            var tmp = inv.slots[i];
                            Slot.SetDurability(ref tmp, inv.slots[i].durability - 1u);
                            inv.slots[i] = tmp;
                            InventoryHandler.UseItemEventArgs uea = new InventoryHandler.UseItemEventArgs(inv, it, i);
                            if (inv.slots[i].durability == 0)
                            {
                                if (RemoveItemInSlot(inv, i, inv.slots[i].item.useHowManyWhenUsed))
                                {
                                    it.OnUse(inv, i);
                                    InventoryHandler.current.Broadcast(e, uea: uea);
                                }
                            }
                            else
                            {
                                it.OnUse(inv, i);
                                InventoryHandler.current.Broadcast(e, uea: uea);
                            }
                        }
                    }
                    else
                    {
                        if (RemoveItemInSlot(inv, i, inv.slots[i].item.useHowManyWhenUsed))
                        {
                            it.OnUse(inv, i);
                            InventoryHandler.UseItemEventArgs uea = new InventoryHandler.UseItemEventArgs(inv, it, i);
                            InventoryHandler.current.Broadcast(e, uea: uea);
                        }
                    }
                    return;
                }
                else if (!inv.slots[i].item.destroyOnUse)
                {
                    if (inv.slots[i].item.hasDurability)
                    {
                        if (inv.slots[i].durability > 0)
                        {
                            var tmp = inv.slots[i];
                            Slot.SetDurability(ref tmp, inv.slots[i].durability - 1u);
                            inv.slots[i] = tmp;
                            InventoryHandler.UseItemEventArgs uea = new InventoryHandler.UseItemEventArgs(inv, inv.slots[i].item, i);
                            inv.slots[i].item.OnUse(inv, i);
                            InventoryHandler.current.Broadcast(e, uea: uea);

                        }
                    }
                    else
                    {
                        inv.slots[i].item.OnUse(inv, i);
                        InventoryHandler.UseItemEventArgs uea = new InventoryHandler.UseItemEventArgs(inv, inv.slots[i].item, i);
                        InventoryHandler.current.Broadcast(e, uea: uea);
                    }
                }
            }
        }

        #endregion

        #region Swap

        /// <summary>
        /// Swap the items in two slots. this function will NOT stack the items for that use SwapItemsInCertainAmountInSlots with amount = null
        /// </summary>
        /// <param name="inv">The inventary to have items swapped</param>
        /// <param name="nativeSlot">The slot to lose items</param>
        /// <param name="targetSlot">The slot to gain items</param>
        public static void SwapItemsInSlots(this Inventory inv, int nativeSlot, int targetSlot, BroadcastEventType e = BroadcastEventType.SwapItem)
        {
            if (inv.interactiable == InventoryProtection.Locked || inv.interactiable == InventoryProtection.LockSlots) return;

            if (inv == null)
            {
                Debug.LogError("Null inventory provided for SwapItemsInSlots");
                return;
            }

            if (inv.slots[targetSlot].interative == SlotProtection.Locked || inv.slots[nativeSlot].interative == SlotProtection.Locked || inv.slots[targetSlot].isProductSlot) return;


            //Verifys if the items to be swaped are in the whitelists
            bool whitelist =
            (inv.slots[nativeSlot].whitelist?.itemsList.ContainsWNull(inv.slots[targetSlot].item)
            ??
            (inv.slots[targetSlot].whitelist == null ? true : inv.slots[targetSlot].whitelist.itemsList.ContainsWNull(inv.slots[nativeSlot].item)))
            &&
            (inv.slots[targetSlot].whitelist?.itemsList.ContainsWNull(inv.slots[nativeSlot].item)
            ??
            (inv.slots[nativeSlot].whitelist == null ? true : inv.slots[nativeSlot].whitelist.itemsList.ContainsWNull(inv.slots[targetSlot].item)));

            if (!whitelist) return;

            if (inv.interactiable == InventoryProtection.SlotToSlot || inv.interactiable == InventoryProtection.Any)
            {
                Slot tmpSlot = inv.slots[targetSlot];

                if (inv.slots[nativeSlot].isProductSlot || inv.slots[targetSlot].isProductSlot)
                {
                    if (tmpSlot.item == null)
                    {
                        inv.slots[targetSlot] = new Slot(
                            inv.slots[nativeSlot], inv.slots[targetSlot].isProductSlot, 
                            inv.slots[targetSlot].interative,
                            inv.slots[targetSlot].whitelist
                        );
                        inv.slots[nativeSlot] = new Slot(
                            tmpSlot, inv.slots[nativeSlot].isProductSlot, 
                            inv.slots[nativeSlot].interative,
                            inv.slots[nativeSlot].whitelist
                        );

                        InventoryHandler.SwapItemsEventArgs sea2 = new InventoryHandler.SwapItemsEventArgs(inv, nativeSlot, targetSlot, inv.slots[targetSlot].item, tmpSlot.item, null);
                        InventoryHandler.current.Broadcast(e, sea: sea2);
                        return;
                    }
                    return;
                }

                inv.slots[targetSlot] = new Slot(
                    inv.slots[nativeSlot],
                    inv.slots[targetSlot].isProductSlot, 
                    inv.slots[targetSlot].interative, 
                    inv.slots[targetSlot].whitelist
                );

                inv.slots[nativeSlot] = new Slot(
                    tmpSlot, 
                    inv.slots[nativeSlot].isProductSlot, 
                    inv.slots[nativeSlot].interative,
                    inv.slots[nativeSlot].whitelist
                );

                InventoryHandler.SwapItemsEventArgs sea = new InventoryHandler.SwapItemsEventArgs(inv, nativeSlot, targetSlot, inv.slots[targetSlot].item, tmpSlot.item, null);
                InventoryHandler.current.Broadcast(e, sea: sea);
            }
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
            if (inv.interactiable == InventoryProtection.Locked || inv.interactiable == InventoryProtection.LockSlots) return (_amount ?? inv.slots[nativeSlot].amount);

            if (inv.slots[targetSlot].interative == SlotProtection.Locked || inv.slots[nativeSlot].interative == SlotProtection.Locked || inv.slots[targetSlot].isProductSlot) return (_amount ?? inv.slots[nativeSlot].amount);

            //Verifys if the items to be swaped are in the whitelists
            bool whitelist = 
            (inv.slots[nativeSlot].whitelist?.itemsList.ContainsWNull(inv.slots[targetSlot].item) 
            ?? 
            (inv.slots[targetSlot].whitelist == null ? true : inv.slots[targetSlot].whitelist.itemsList.ContainsWNull(inv.slots[nativeSlot].item)))
            &&
            (inv.slots[targetSlot].whitelist?.itemsList.ContainsWNull(inv.slots[nativeSlot].item) 
            ?? 
            (inv.slots[nativeSlot].whitelist == null ? true : inv.slots[nativeSlot].whitelist.itemsList.ContainsWNull(inv.slots[targetSlot].item)));

            if (!whitelist) return (_amount ?? inv.slots[nativeSlot].amount);

            if (inv == null)
            {
                Debug.LogError("Null inventory provided for SwapItemsInCertainAmountInSlots");
                return -1;
            }

            int amount = (_amount ?? inv.slots[nativeSlot].amount);
            if (inv.interactiable == InventoryProtection.SlotToSlot || inv.interactiable == InventoryProtection.Any)
            {
                if (amount <= 0) return amount;
                InventoryHandler.SwapItemsEventArgs sea;
                if (amount > inv.slots[nativeSlot].amount) return amount;
                else if (inv.slots[targetSlot].item == null)
                {
                    inv.slots[targetSlot] = new Slot(
                        inv.slots[nativeSlot].item, 
                        amount, 
                        true, 
                        inv.slots[targetSlot].isProductSlot, 
                        inv.slots[targetSlot].interative,
                        inv.slots[targetSlot].whitelist
                    );

                    inv.slots[nativeSlot] = new Slot(
                        inv.slots[nativeSlot].item, 
                        inv.slots[nativeSlot].amount - amount, 
                        true, inv.slots[nativeSlot].isProductSlot,
                        inv.slots[nativeSlot].interative, 
                        inv.slots[nativeSlot].whitelist
                    );

                    if (inv.slots[nativeSlot].amount <= 0) inv.slots[nativeSlot] = new Slot(
                        nullSlot, inv.slots[nativeSlot].isProductSlot, 
                        inv.slots[nativeSlot].interative,
                        inv.slots[nativeSlot].whitelist
                    );
                }
                else if (inv.slots[nativeSlot].item == inv.slots[targetSlot].item)
                {
                    int remaning = AddItemToSlot(
                        inv, 
                        inv.slots[nativeSlot].item,
                        amount, 
                        targetSlot
                    );

                    inv.slots[nativeSlot] = new Slot(
                        inv.slots[nativeSlot].item,
                        inv.slots[nativeSlot].amount - amount + remaning, 
                        true, inv.slots[nativeSlot].isProductSlot, 
                        inv.slots[nativeSlot].interative,
                        inv.slots[nativeSlot].whitelist
                    );

                    if (inv.slots[nativeSlot].amount <= 0)
                        inv.slots[nativeSlot] = new Slot(
                            nullSlot, 
                            inv.slots[nativeSlot].isProductSlot, 
                            inv.slots[nativeSlot].interative,
                            inv.slots[nativeSlot].whitelist
                        );
                    sea = new InventoryHandler.SwapItemsEventArgs(inv, nativeSlot, targetSlot, inv.slots[targetSlot].item, inv.slots[nativeSlot].item, amount - remaning);
                    InventoryHandler.current.Broadcast(e, sea: sea);
                    return remaning;
                }
                else SwapItemsInSlots(inv, nativeSlot, targetSlot);

                sea = new InventoryHandler.SwapItemsEventArgs(inv, nativeSlot, targetSlot, inv.slots[targetSlot].item, inv.slots[nativeSlot].item, amount);
                InventoryHandler.current.Broadcast(e, sea: sea);
                return 0;
            }
            return amount;
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
            if (nativeInv == null)
            {
                Debug.LogError("Null native inventory provided for SwapItemThruInventoriesSlotToSlot");
                return -1;
            }
            if (targetInv == null)
            {
                Debug.LogError("Null target inventory provided for SwapItemThruInventoriesSlotToSlot");
                return -1;
            }

            if (nativeInv.slots[nativeSlotNumber].interative == SlotProtection.Locked) return amount;

            if (targetInv.slots[targetSlotNumber].interative == SlotProtection.Locked || targetInv.slots[targetSlotNumber].isProductSlot) return amount;

            //Verifys if the items to be swaped are in the whitelists
            bool whitelist =
            (nativeInv.slots[nativeSlotNumber].whitelist?.itemsList.ContainsWNull(targetInv.slots[targetSlotNumber].item)
            ??
            (targetInv.slots[targetSlotNumber].whitelist == null ? true : targetInv.slots[targetSlotNumber].whitelist.itemsList.ContainsWNull(nativeInv.slots[nativeSlotNumber].item)))
            &&
            (targetInv.slots[targetSlotNumber].whitelist?.itemsList.ContainsWNull(nativeInv.slots[nativeSlotNumber].item)
            ??
            (nativeInv.slots[nativeSlotNumber].whitelist == null ? true : nativeInv.slots[nativeSlotNumber].whitelist.itemsList.ContainsWNull(targetInv.slots[targetSlotNumber].item)));

            if (!whitelist) return amount;

            if (nativeInv.interactiable == InventoryProtection.Locked || targetInv.interactiable == InventoryProtection.Locked || nativeInv.interactiable == InventoryProtection.LockThruInventory || targetInv.interactiable == InventoryProtection.LockThruInventory) return amount;
            if ((nativeInv.interactiable == InventoryProtection.InventoryToInventory || nativeInv.interactiable == InventoryProtection.Any) && (targetInv.interactiable == InventoryProtection.InventoryToInventory || targetInv.interactiable == InventoryProtection.Any))
            {
                InventoryHandler.SwapItemsTrhuInvEventArgs siea;
                if (amount > nativeInv.slots[nativeSlotNumber].amount) return amount;
                else if (targetInv.slots[targetSlotNumber].item == null)
                {
                    targetInv.slots[targetSlotNumber] = new Slot(
                        nativeInv.slots[nativeSlotNumber].item, 
                        amount, 
                        true, 
                        targetInv.slots[targetSlotNumber].isProductSlot, 
                        targetInv.slots[targetSlotNumber].interative,
                        targetInv.slots[targetSlotNumber].whitelist
                    );

                    nativeInv.slots[nativeSlotNumber] = new Slot(
                        nativeInv.slots[nativeSlotNumber].item, 
                        nativeInv.slots[nativeSlotNumber].amount - amount, 
                        true, nativeInv.slots[nativeSlotNumber].isProductSlot, 
                        nativeInv.slots[nativeSlotNumber].interative,
                        nativeInv.slots[nativeSlotNumber].whitelist
                    );

                    if (nativeInv.slots[nativeSlotNumber].amount <= 0) nativeInv.slots[nativeSlotNumber] = new Slot(
                        nullSlot, 
                        nativeInv.slots[nativeSlotNumber].isProductSlot, 
                        nativeInv.slots[nativeSlotNumber].interative,
                        nativeInv.slots[nativeSlotNumber].whitelist
                    );
                }
                else if (nativeInv.slots[nativeSlotNumber].item == targetInv.slots[targetSlotNumber].item)
                {
                    int remaning = AddItemToSlot(targetInv, nativeInv.slots[nativeSlotNumber].item, amount, targetSlotNumber);

                    nativeInv.slots[nativeSlotNumber] = new Slot(
                        nativeInv.slots[nativeSlotNumber].item,
                        nativeInv.slots[nativeSlotNumber].amount - amount + remaning,
                        true, nativeInv.slots[nativeSlotNumber].isProductSlot, 
                        nativeInv.slots[nativeSlotNumber].interative,
                        nativeInv.slots[nativeSlotNumber].whitelist
                    );

                    if (nativeInv.slots[nativeSlotNumber].amount <= 0)
                        nativeInv.slots[nativeSlotNumber] = new Slot(
                            nullSlot, 
                            nativeInv.slots[nativeSlotNumber].isProductSlot,
                            nativeInv.slots[nativeSlotNumber].interative,
                            nativeInv.slots[nativeSlotNumber].whitelist
                        );

                    siea = new InventoryHandler.SwapItemsTrhuInvEventArgs(nativeInv, targetInv, nativeSlotNumber, targetSlotNumber, targetInv.slots[targetSlotNumber].item, nativeInv.slots[nativeSlotNumber].item, amount - remaning);
                    InventoryHandler.current.Broadcast(e, siea: siea);

                    return remaning;
                }
                else
                {
                    Slot tmpSlot = targetInv.slots[targetSlotNumber];

                    targetInv.slots[targetSlotNumber] = new Slot(
                        nativeInv.slots[nativeSlotNumber], 
                        targetInv.slots[targetSlotNumber].isProductSlot, 
                        targetInv.slots[targetSlotNumber].interative,
                        targetInv.slots[targetSlotNumber].whitelist
                    );

                    nativeInv.slots[nativeSlotNumber] = new Slot(tmpSlot,
                        nativeInv.slots[nativeSlotNumber].isProductSlot, 
                        nativeInv.slots[nativeSlotNumber].interative,
                        targetInv.slots[nativeSlotNumber].whitelist
                    );
                }

                siea = new InventoryHandler.SwapItemsTrhuInvEventArgs(nativeInv, targetInv, nativeSlotNumber, targetSlotNumber, targetInv.slots[targetSlotNumber].item, nativeInv.slots[nativeSlotNumber].item, amount);
                InventoryHandler.current.Broadcast(e, siea: siea);
                return 0;
            }
            return amount;
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
            if (nativeInv == null)
            {
                Debug.LogError("Null native inventory provided for SwapItemThruInventories");
                return false;
            }
            if (targetInv == null)
            {
                Debug.LogError("Null target inventory provided for SwapItemThruInventories");
                return false;
            }

            if (nativeInv.interactiable == InventoryProtection.Locked || targetInv.interactiable == InventoryProtection.Locked || nativeInv.interactiable == InventoryProtection.LockThruInventory || targetInv.interactiable == InventoryProtection.LockThruInventory) return false;
            if ((nativeInv.interactiable == InventoryProtection.InventoryToInventory || nativeInv.interactiable == InventoryProtection.Any) && (targetInv.interactiable == InventoryProtection.InventoryToInventory || targetInv.interactiable == InventoryProtection.Any))
            {
                {
                    if (RemoveItem(nativeInv, item, amount))
                    {
                        int remaning = AddItem(targetInv, item, amount);
                        if (remaning > 0) AddItem(nativeInv, item, remaning);
                    }
                    else return false;

                    InventoryHandler.SwapItemsTrhuInvEventArgs siea = new InventoryHandler.SwapItemsTrhuInvEventArgs(nativeInv, targetInv, null, null, item, null, amount);
                    InventoryHandler.current.Broadcast(e, siea: siea);
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Initialize

        /// <summary>
        /// This function must be called when a inventory is being created. It fills the inventory if null Slots if the list of slots is null or have less elments than inv.slotAmounts, give an id to the inventory and add it to the list of inventories in the InventoryController. This function dont need to be called if you are using an loading system;
        /// </summary>
        /// <param name="inv">The inventory to be initialized</param>
        /// <returns>The initiaized inventory</returns>
        public static Inventory InitializeInventory(this Inventory inv, BroadcastEventType e = BroadcastEventType.InitializeInventory)
        {
            if (inv == null)
            {
                Debug.LogError("Null inventory provided for InitializeInventory");
                return null;
            }

            if (inv.hasInitializated) return inv;
            
            if(inv.slots.Count != inv.slotAmounts)
            {
                if (inv.slots == null) inv.slots = new List<Slot>();
                for (int i = 0; i < inv.slotAmounts; i++)
                {
                    //Debug.Log(inv.slots.Count);
                    if (i < inv.slots.Count)
                        inv.slots[i] = new Slot(
                            null, 
                            0,
                            false,
                            inv.slots[i].isProductSlot, 
                            inv.slots[i].interative,
                            inv.slots[i].whitelist
                        );
                    else
                        inv.slots.Add(new Slot(null, 0, false));
                }
            }

            //Debug.Log(inv.slots.Count);
            inv.id = inventories.Count;
            inventories.Add(inv);
            inv.hasInitializated = true;
            InventoryHandler.InitializeInventoryEventArgs iea = new InventoryHandler.InitializeInventoryEventArgs(inv);
            InventoryHandler.current.Broadcast(e, iea: iea);
            return inv;
        }

        /// <summary>
        /// This function is an alternative for the InitializeInventory function. It initializes a Inventory using another inventory as a Model;
        /// </summary>
        /// <param name="inv">The inventory to be initialized</param>
        /// <returns>The initialized inventory</returns>
        public static Inventory InitializeInventoryFromAnotherInventory(this Inventory inv, Inventory modelInv, BroadcastEventType e = BroadcastEventType.InitializeInventory)
        {
            if (inv == null)
            {
                Debug.LogError("Null inventory provided for InitializeInventory");
                return null;
            }
            if (modelInv == null)
            {
                Debug.LogError("Null model inventory provided for InitializeInventory");
                return null;
            }

            if (inv.hasInitializated) return inv;

            inv = modelInv;
            Debug.Log(inv.slots.Count);
            inv.id = inventories.Count;
            inventories.Add(inv);
            inv.hasInitializated = true;
            InventoryHandler.InitializeInventoryEventArgs iea = new InventoryHandler.InitializeInventoryEventArgs(inv);
            InventoryHandler.current.Broadcast(e, iea: iea);
            return inv;
        }

        #endregion

        #region Craft 

        /// <summary>
        /// This function checks every recipe in the InventoryManager and if it finds a match it returns the product. The pattern recipes are checked before the normal ones, and once a match is find it returns
        /// </summary>
        /// <param name="inv">The crafting inventory</param>
        /// <param name="grid">The items grid (the rest of the inventory will be ignored)</param>
        /// <param name="gridSize">The size of the crafting grid</param>
        /// <param name="craftItem">Wheter it should remove the items form the grid or not</param>
        /// <param name="allowPatternRecipe">Wheter it should check for pattern recipes or not (This is useful if you have a big game and dont want to check the patterns, since they are much more time consuming than normal recipes)</param>
        /// <param name="productSlots">The amount of slot to products</param>
        /// <returns>The products of the recipe matched</returns>
        public static CraftItemData CraftItem(this Inventory inv, (Item[], int[]) grid, Vector2Int gridSize, bool craftItem, bool allowPatternRecipe, int productSlots)
        {
            if (InventoryHandler.current == null) return CraftItemData.nullData;

            InventoryHandler handler = InventoryHandler.current;

            foreach (RecipeGroup asset in handler.recipeAssets)
            {
                if (allowPatternRecipe)
                {
                    //--PATTERN-//
                    foreach (PatternRecipe pattern in asset.receipePatternsList)
                    {
                        var result = CraftItem(inv, grid, gridSize, craftItem, pattern, productSlots);
                        if (result != CraftItemData.nullData) return result;
                    }
                }
                foreach (Recipe recipe in asset.recipesList)
                {
                    var recipeRes = CraftItem(inv, grid, gridSize, craftItem, recipe, productSlots);
                    if (recipeRes != CraftItemData.nullData) return recipeRes;
                }
            }
            return CraftItemData.nullData;
        }

        /// <summary>
        /// This function checks every recipe in the provided RecipeGroup and if it finds a match it returns the product. The pattern recipes are checked before the normal ones, and once a match is find it returns
        /// </summary>
        /// <param name="inv">The crafting inventory</param>
        /// <param name="grid">The items grid (the rest of the inventory will be ignored)</param>
        /// <param name="gridSize">The size of the crafting grid</param>
        /// <param name="craftItem">Wheter it should remove the items form the grid or not</param>
        /// <param name="asset">The RecipeGroup to be checked</param>
        /// <param name="allowPatternRecipe">Wheter it should check for pattern recipes or not (This is useful if you have a big game and dont want to check the patterns, since they are much more time consuming than normal recipes)</param>
        /// <param name="productSlots">The amount of slot to products</param>
        /// <returns>The products of the recipe matched</returns>
        public static CraftItemData CraftItem(this Inventory inv, (Item[], int[]) grid, Vector2Int gridSize, bool craftItem, RecipeGroup asset, bool allowPatternRecipe, int productSlots)
        {
            if (allowPatternRecipe)
            {
                foreach (PatternRecipe pattern in asset.receipePatternsList)
                {
                    var result = CraftItem(inv, grid, gridSize, craftItem, pattern, productSlots);
                    if (result != CraftItemData.nullData) return result;
                }
            }
            foreach (Recipe recipe in asset.recipesList)
            {
                var recipeRes = CraftItem(inv, grid, gridSize, craftItem, recipe, productSlots);
                if (recipeRes != CraftItemData.nullData) return recipeRes;
            }
            return CraftItemData.nullData;
        }

        /// <summary>
        /// This function checks a specific Pattern Recipe and if it is a match it returns the product
        /// </summary>
        /// <param name="inv">The crafting inventory</param>
        /// <param name="grid">The items grid (the rest of the inventory will be ignored)</param>
        /// <param name="gridSize">The size of the crafting grid</param>
        /// <param name="craftItem">Wheter it should remove the items form the grid or not</param>
        /// <param name="pattern">The PatternRecipe to be checked</param>
        /// <param name="productSlots">The amount of slot to products</param>
        /// <returns>The products of the recipe matched</returns>
        public static CraftItemData CraftItem(this Inventory inv, (Item[], int[]) grid, Vector2Int gridSize, bool craftItem, PatternRecipe pattern, int productSlots)
        {

            if (pattern.pattern.Length > grid.Item1.Length) return CraftItemData.nullData;
            if (pattern.products.Length > productSlots) return CraftItemData.nullData;
            else if (pattern.pattern.Length == grid.Item1.Length && pattern.amountPattern.Length == grid.Item2.Length)
            {
                if (Enumerable.SequenceEqual(pattern.pattern, grid.Item1) && SequenceEqualOrGreter(pattern.amountPattern, grid.Item2))
                {
                    if (craftItem)
                    {
                        bool canAdd = true;
                        for (int h = grid.Item1.Length; h - grid.Item1.Length < pattern.products.Length; h++)
                        {
                            //if (h - grid.Length >= pattern.products.Length) break;
                            if (!inv.slots[h].hasItem) continue;
                            if (inv.slots[h].amount >= inv.slots[h].item.maxAmount) { canAdd = false; break; }
                            if (inv.slots[h].item != pattern.products[h - grid.Item1.Length]) { canAdd = false; break; }
                        }
                        if (canAdd)
                        {
                            int i = 0;
                            for (int k = grid.Item1.Length; k < inv.slots.Count; k++)
                            {
                                if (k > grid.Item1.Length - 1)
                                {
                                    if (k - grid.Item1.Length >= pattern.products.Length) break;

                                    i = inv.AddItemToSlot(pattern.products[k - grid.Item1.Length], pattern.amountProducts[k - grid.Item1.Length], k, overrideSlotProtection: true);
                                    if (i > 0) return CraftItemData.nullData;
                                    //inv.slots[k] = new Slot(pattern.products[k - grid.Length], inv.slots[k].amount + 1, true, true);
                                }
                            }
                            if (i > 0) return CraftItemData.nullData;


                            for (int k = 0; k < grid.Item1.Length; k++)
                            {
                                if (inv.slots[k].hasItem && k <= grid.Item1.Length - 1)
                                    inv.RemoveItemInSlot(k, pattern.amountPattern[k]);

                            }
                        }
                    }
                    return new CraftItemData(pattern.products, pattern.amountProducts);
                }
            }
            else if (pattern.pattern.Length < grid.Item1.Length)
            {
                int fit = (gridSize.y - pattern.gridSize.y + 1) * (gridSize.x - pattern.gridSize.x + 1);

                List<int> indexes;
                for (int i = 0; i < fit; i++)
                {
                    var result = CraftItem(inv, GetSectionFromGrid(grid, gridSize, pattern.gridSize, i, out indexes), pattern.gridSize, false, pattern, productSlots);
                    if (result.items != null)
                    {
                        bool canReturn = true;
                        for (int j = 0; j < grid.Item1.Length; j++)
                        {
                            if (indexes.Contains(j)) continue;
                            if (grid.Item1[j] != null) canReturn = false;
                        }
                        if (canReturn)
                        {
                            if (craftItem)
                            {
                                bool canAdd = true;
                                for (int h = grid.Item1.Length; h - grid.Item1.Length < pattern.products.Length; h++)
                                {
                                    //if (h - grid.Length >= pattern.products.Length) break;
                                    if (!inv.slots[h].hasItem) continue;
                                    if (inv.slots[h].amount >= inv.slots[h].item.maxAmount) { canAdd = false; break; }
                                    if (inv.slots[h].item != pattern.products[h - grid.Item1.Length]) { canAdd = false; break; }
                                }
                                if (canAdd)
                                {
                                    int w = 0;
                                    for (int k = grid.Item1.Length; k < inv.slots.Count; k++)
                                    {
                                        if (k > grid.Item1.Length - 1)
                                        {
                                            if (k - grid.Item1.Length >= pattern.products.Length) break;

                                            w = inv.AddItemToSlot(pattern.products[k - grid.Item1.Length], pattern.amountProducts[k - grid.Item1.Length], k, overrideSlotProtection: true);
                                            if (w > 0) return CraftItemData.nullData;
                                            //inv.slots[k] = new Slot(pattern.products[k - grid.Length], inv.slots[k].amount + 1, true, true);
                                        }
                                    }
                                    if (w > 0) return CraftItemData.nullData;

                                    for (int v = 0; v < pattern.gridSize.y; v++)
                                    {
                                        for (int u = 0; u < pattern.gridSize.x; u++)
                                        {
                                            var index = (v * gridSize.x) + u;
                                            if (inv.slots[index].hasItem && index <= grid.Item1.Length - 1)
                                            {
                                                Debug.Log(index + " " + " " + pattern.amountPattern.Length);
                                                inv.RemoveItemInSlot(index, pattern.amountPattern[v * pattern.gridSize.x + u]);
                                            }
                                        }
                                    }
                                }
                            }
                            return result;
                        }
                    }
                }
            }
            return CraftItemData.nullData;
        }

        /// <summary>
        /// This function checks a specific Recipe and if it is a match it returns the product
        /// </summary>
        /// <param name="inv">The crafting inventory</param>
        /// <param name="grid">The items grid (the rest of the inventory will be ignored)</param>
        /// <param name="gridSize">The size of the crafting grid</param>
        /// <param name="craftItem">Wheter it should remove the items form the grid or not</param>
        /// <param name="recipe">The Recipe to be checked</param>
        /// <param name="productSlots">The amount of slot to products</param>
        /// <returns>The products of the recipe matched</returns>
        public static CraftItemData CraftItem(this Inventory inv, (Item[], int[]) grid, Vector2Int gridSize, bool craftItem, Recipe recipe, int productSlots)
        {
            List<int> jumpIndexes = new List<int>();
            List<int> tmpjumpIndexes = new List<int>();
            List<int> removeAmount = new List<int>();
            for (int i = 0; i < grid.Item1.Length; i++)
            {
                for (int j = 0; j < recipe.numberOfFactors; j++)
                {
                    if (grid.Item1[i] == recipe.factors[j] && !tmpjumpIndexes.Contains(j))
                    {
                        //i++;
                        tmpjumpIndexes.Add(j);
                        jumpIndexes.Add(i);
                        removeAmount.Add(recipe.amountFactors[j]);
                        break;
                    }
                }
            }
            bool canReturn = true;
            if (jumpIndexes.Count != recipe.numberOfFactors) return CraftItemData.nullData;
            for (int j = 0; j < grid.Item1.Length; j++)
            {
                if (grid.Item1[j] != null && !jumpIndexes.Contains(j))
                {
                    canReturn = false;
                }
            }

            for (int i = 0; i < jumpIndexes.Count; i++)
            {
                if (grid.Item2[jumpIndexes[i]] < recipe.amountFactors[i])
                {
                    canReturn = false;
                }
            }

            if (canReturn)
            {
                if (craftItem)
                {
                    bool canAdd = true;
                    for (int h = grid.Item1.Length; h - grid.Item1.Length < recipe.products.Length; h++)
                    {
                        //if (h - grid.Length >= pattern.products.Length) break;
                        if (!inv.slots[h].hasItem) continue;
                        if (inv.slots[h].amount >= inv.slots[h].item.maxAmount) { canAdd = false; break; }
                        if (inv.slots[h].item != recipe.products[h - grid.Item1.Length]) { canAdd = false; break; }
                    }
                    if (canAdd)
                    {
                        int i = 0;
                        for (int k = grid.Item1.Length; k < inv.slots.Count; k++)
                        {
                            if (k > grid.Item1.Length - 1)
                            {
                                if (k - grid.Item1.Length >= recipe.products.Length) break;

                                i = inv.AddItemToSlot(recipe.products[k - grid.Item1.Length], recipe.amountProducts[k - grid.Item1.Length], k, overrideSlotProtection: true);
                                if (i > 0) return CraftItemData.nullData;
                                //inv.slots[k] = new Slot(pattern.products[k - grid.Length], inv.slots[k].amount + 1, true, true);
                            }
                        }
                        if (i > 0) return CraftItemData.nullData;

                        var index = 0;
                        for (int k = 0; k < grid.Item1.Length; k++)
                        {
                            if (inv.slots[k].hasItem && k <= grid.Item1.Length - 1)
                            {
                                inv.RemoveItemInSlot(k, removeAmount[index]);
                                index++;
                            }
                        }
                    }
                }
                return new CraftItemData(recipe.products, recipe.amountProducts);
            }

            return CraftItemData.nullData;
        }

        /// <summary>
        /// This function is used for internal use, but its public in case that for some reason another script needs it. Basicaly it gets a section from a grid based on a section size and a offsetIndex
        /// </summary>
        /// <param name="originalGrid">The original grid</param>
        /// <param name="originalGridSize">The size of the original grid</param>
        /// <param name="sectionSize">The seize of the desired section</param>
        /// <param name="offsetIndex">The offset of the section</param>
        /// <param name="usedIndexes">The index of the original grid that were selected</param>
        /// <returns>a new grid that is a section from the original one</returns>
        public static (Item[], int[]) GetSectionFromGrid((Item[], int[]) originalGrid, Vector2Int originalGridSize, Vector2Int sectionSize, int offsetIndex, out List<int> usedIndexes)
        {
            Item[] returnGrid = new Item[sectionSize.x * sectionSize.y];
            int[] returnIntGrid = new int[sectionSize.x * sectionSize.y];
            usedIndexes = new List<int>();

            int fitx = originalGridSize.x - sectionSize.x + 1;
            int offsety = Mathf.FloorToInt(offsetIndex / fitx);
            int offsetx = offsetIndex - (offsety * fitx);
            for (int i = 0; i < sectionSize.y; i++)
            {
                for (int j = 0; j < sectionSize.x; j++)
                {
                    returnGrid[i * sectionSize.x + j] = originalGrid.Item1[(i + offsety) * originalGridSize.x + j + offsetx];
                    returnIntGrid[i * sectionSize.x + j] = originalGrid.Item2[(i + offsety) * originalGridSize.x + j + offsetx];
                    usedIndexes.Add((i + offsety) * originalGridSize.x + j + offsetx);
                }
            }
            return (returnGrid, returnIntGrid);
        }

        private static bool SequenceEqualOrGreter(int[] firstInt, int[] greterInt)
        {
            bool isEqualOrGreter = true;
            for (int i = 0; i < firstInt.Length; i++)
            {
                if (greterInt[i] >= firstInt[i]) continue;
                isEqualOrGreter = false;
            }
            return isEqualOrGreter;
        }

        #endregion

        #region InventoryUtility

        /// <summary>
        /// Checks an item in the inventory and return a class with all the info gathered
        /// </summary>
        /// <param name="inv">Inventory to check</param>
        /// <param name="itemToCheck">Item to ckeck</param>
        /// <param name="minAmount">Minimun amount of items to return true</param>
        /// <param name="acceptableInvProtections">Inventory protection accepted for checking</param>
        /// <param name="acceptableSlotProtections">Slot protection accepted for checking</param>
        /// <param name="mustBeOnSameSlot">If the minimun amount of items must be on the same slot</param>
        /// <returns>Returns a clas containing 7 attributes: inventory(The inventory checked), slotsCheckced(The slots that where checked), slotsWithItem(The Slots in witch there is the item, does not need to have minimun amount), amout(The total amout of that item in the inventory), hasItem(If the item was found in the provided conditions), mustBeOnSameSlot(SelfExplanatory), checkedItem(The item that was checked)</returns>
        public static CheckItemData CheckItemInInventory(this Inventory inv, Item itemToCheck, int minAmount, InventoryProtection[] acceptableInvProtections = null, SlotProtection[] acceptableSlotProtections = null, bool mustBeOnSameSlot = false)
        {
            if (inv == null)
            {
                Debug.LogError("Null inventory provided for CheckItemInInventory");
                return null;
            }
            if (itemToCheck == null)
            {
                Debug.LogError("Null item to check provided for CheckItemInInventory");
                return null;
            }

            List<int> slotsToCheck = new List<int>();

            for (int i = 0; i < inv.slots.Count; i++)
            {
                slotsToCheck.Add(i);
            }

            return CheckItemInInventory(inv, itemToCheck, minAmount, acceptableInvProtections, acceptableSlotProtections, mustBeOnSameSlot, slotsToCheck.ToArray());
        }

        /// <summary>
        /// Checks an item in the inventory in the slots provided and return a class with all the info gathered
        /// </summary>
        /// <param name="inv">Inventory to check</param>
        /// <param name="itemToCheck">Item to ckeck</param>
        /// <param name="minAmount">Minimun amount of items to return true</param>
        /// <param name="acceptableInvProtections">Inventory protection accepted for checking</param>
        /// <param name="acceptableSlotProtections">Slot protection accepted for checking</param>
        /// <param name="mustBeOnSameSlot">If the minimun amount of items must be on the same slot</param>
        /// <param name="slotsToCheck">Slots that will be checked</param>
        /// <returns>Returns a clas containing 7 attributes: inventory(The inventory checked), slotsCheckced(The slots that where checked), slotsWithItem(The Slots in witch there is the item, does not need to have minimun amount), amout(The total amout of that item in the inventory), hasItem(If the item was found in the provided conditions), mustBeOnSameSlot(SelfExplanatory), checkedItem(The item that was checked)</returns>
        public static CheckItemData CheckItemInInventory(this Inventory inv, Item itemToCheck, int minAmount, InventoryProtection[] acceptableInvProtections = null, SlotProtection[] acceptableSlotProtections = null, bool mustBeOnSameSlot = false, params int[] slotsToCheck)
        {
            if (inv == null)
            {
                Debug.LogError("Null inventory provided for CheckItemInInventory");
                return null;
            }
            if (itemToCheck == null)
            {
                Debug.LogError("Null item to check provided for CheckItemInInventory");
                return null;
            }

            if (acceptableInvProtections == null) acceptableInvProtections = allInventoryProtections;
            if (acceptableSlotProtections == null) acceptableSlotProtections = allSlotProtections;

            if (!acceptableInvProtections.Contains(inv.interactiable)) return null;

            int total = 0;
            List<int> slotsWithItem = new List<int>();
            foreach (int slot in slotsToCheck)
            {
                if (slot >= inv.slots.Count)
                {
                    Debug.LogError($"Provided slot index to check is out of array bounds (index: {slot} Array lenght: {inv.slots.Count})\n The code continued to next index");
                    continue;
                }

                if (!acceptableSlotProtections.Contains(inv.slots[slot].interative)) continue;

                if (mustBeOnSameSlot)
                {
                    if (inv.slots[slot].item == itemToCheck)
                    {
                        if (inv.slots[slot].amount >= minAmount)
                        {
                            return new CheckItemData(inv, slotsToCheck, new int[1] { slot }, inv.slots[slot].amount, true, true, itemToCheck);
                        }
                    }
                }
                else
                {
                    if (inv.slots[slot].item == itemToCheck)
                    {
                        total += inv.slots[slot].amount;
                        slotsWithItem.Add(slot);
                    }
                }
            }

            if (total >= minAmount && !mustBeOnSameSlot)
            {
                return new CheckItemData(inv, slotsToCheck, slotsWithItem.ToArray(), total, true, false, itemToCheck);
            }

            return new CheckItemData(inv, slotsToCheck, new int[0], 0, false, mustBeOnSameSlot, itemToCheck);
        }

        /// <summary>
        /// Gets the tooltip info of an item in the slot provided from the inventory provided
        /// </summary>
        /// <param name="inv">Inventory</param>
        /// <param name="slot">Slot number</param>
        /// <returns>Return the tooltip info of the current item in the slot and inventory provided</returns>
        public static ToolTipInfo GetTooltipInfoFromSlot(this Inventory inv, int slot)
        {   
            if(inv == null)
            {
                Debug.LogError("Null inventory provided for GetTooltipInfoFromSlot");
                return null;
            }
            if(slot < 0 || slot >= inv.slots.Count)
            {
                Debug.LogError("Slot number provided for GetTooltipInfoFromSlot is outside the inventory slots array bounds");
                return null;
            }

            return inv.slots[slot].item.tooltip;
        }

        /*public static bool SliptSlot(this Inventory inv, int slot)
        {
            if(inv.AddItemToNewSlot())
        }*/
        #endregion

        #region PseudoMethods

        /// <summary>
        /// Adds a certain amount of an item to the first empty slot even if there are slots of the same item that can still hold more items. If the specified amount is grater than the maxAmount for that item it will fill the next slot
        /// </summary>
        /// <param name="inv">The inventory in witch the item will be placed</param>
        /// <param name="item">The item that will be stored in the inventory</param>
        /// <param name="amount">The amount of items to be stored</param>
        /// <param name="durability">The durability of the items to be stored</param>
        /// <returns>If the inventory gets full and there are still items to store it will return the number of items remaining</returns>
        public static int AddItemToNewSlot(this Inventory inv, Item item, int amount, uint durability)
        {
            return AddItemToNewSlot(inv, item, amount, durability: durability, e: BroadcastEventType.AddItem);
        }

        /// <summary>
        /// Adds a certain amount of an item to the first empty slot even if there are slots of the same item that can still hold more items. If the specified amount is grater than the maxAmount for that item it will fill the next slot
        /// </summary>
        /// <param name="inv">The inventory in witch the item will be placed</param>
        /// <param name="item">The item that will be stored in the inventory</param>
        /// <param name="amount">The amount of items to be stored</param>
        /// <param name="durability">The durability of the items to be stored</param>
        /// <param name="callback">The calback function that will be called if everything runs smoothly</param>
        /// <returns>If the inventory gets full and there are still items to store it will return the number of items remaining</returns>
        public static int AddItemToNewSlot(this Inventory inv, Item item, int amount, uint durability, Action callback)
        {
            return AddItemToNewSlot(inv, item, amount, durability: durability, callback: callback, e: BroadcastEventType.AddItem);
        }

        /// <summary>
        /// Adds a certain amount of an item to the first empty slot even if there are slots of the same item that can still hold more items. If the specified amount is grater than the maxAmount for that item it will fill the next slot
        /// </summary>
        /// <param name="inv">The inventory in witch the item will be placed</param>
        /// <param name="item">The item that will be stored in the inventory</param>
        /// <param name="amount">The amount of items to be stored</param>
        /// <param name="callback">The calback function that will be called if everything runs smoothly</param>
        /// <returns>If the inventory gets full and there are still items to store it will return the number of items remaining</returns>
        public static int AddItemToNewSlot(this Inventory inv, Item item, int amount, Action callback)
        {
            return AddItemToNewSlot(inv, item, amount, callback: callback, e: BroadcastEventType.AddItem);
        }

        /// <summary>
        /// Adds a certain amount of an item to the first empty slot with the same item that isnt full yet. If it fills a entire slot it will go to the next one. If the specified amount is grater than the maxAmount for that item it will fill the next slot. This is what you should use for pick-up an item.
        /// </summary>
        /// <param name="inv">The inventory in witch the item will be placed</param>
        /// <param name="item">The item that will be stored in the inventory</param>
        /// <param name="amount">The amount of items to be stored</param>
        /// <returns>If the inventory gets full and there are still items to store it will return the number of items remaining</returns>
        public static int AddItem(this Inventory inv, Item item, int amount, uint durability)
        {
            return AddItem(inv, item, amount, durability: durability, e: BroadcastEventType.AddItem);
        }

        /// <summary>
        /// Adds a certain amount of an item to the first empty slot with the same item that isnt full yet. If it fills a entire slot it will go to the next one. If the specified amount is grater than the maxAmount for that item it will fill the next slot. This is what you should use for pick-up an item.
        /// </summary>
        /// <param name="inv">The inventory in witch the item will be placed</param>
        /// <param name="item">The item that will be stored in the inventory</param>
        /// <param name="amount">The amount of items to be stored</param>
        /// <returns>If the inventory gets full and there are still items to store it will return the number of items remaining</returns>
        public static int AddItem(this Inventory inv, Item item, int amount, uint durability, Action callback)
        {
            return AddItem(inv, item, amount, durability: durability, callback: callback, e: BroadcastEventType.AddItem);
        }

        /// <summary>
        /// Adds a certain amount of an item to the first empty slot with the same item that isnt full yet. If it fills a entire slot it will go to the next one. If the specified amount is grater than the maxAmount for that item it will fill the next slot. This is what you should use for pick-up an item.
        /// </summary>
        /// <param name="inv">The inventory in witch the item will be placed</param>
        /// <param name="item">The item that will be stored in the inventory</param>
        /// <param name="amount">The amount of items to be stored</param>
        /// <returns>If the inventory gets full and there are still items to store it will return the number of items remaining</returns>
        public static int AddItem(this Inventory inv, Item item, int amount, Action callback)
        {
            return AddItem(inv, item, amount, callback: callback, e: BroadcastEventType.AddItem);
        }

        /// <summary>
        /// Adds a certain amount of an item to an specific slot. If it fills slot entirily it will return the remaning items to be stored.
        /// </summary>
        /// <param name="inv">The inventory in witch the item will be placed</param>
        /// <param name="item">The item that will be stored in the inventory</param>
        /// <param name="amount">The amount of items to be stored</param>
        /// <param name="slotNumber">The index of the slot to store the items</param>
        /// <returns>If the slot gets full and there are still items to store it will return the number of items remaining</returns>
        public static int AddItem(this Inventory inv, Item item, int amount, int slotNumber)
        {
            return AddItemToSlot(inv, item, amount, slotNumber);
        }

        /// <summary>
        /// Adds a certain amount of an item to an specific slot. If it fills slot entirily it will return the remaning items to be stored.
        /// </summary>
        /// <param name="inv">The inventory in witch the item will be placed</param>
        /// <param name="item">The item that will be stored in the inventory</param>
        /// <param name="amount">The amount of items to be stored</param>
        /// <param name="slotNumber">The index of the slot to store the items</param>
        /// <returns>If the slot gets full and there are still items to store it will return the number of items remaining</returns>
        public static int AddItem(this Inventory inv, Item item, int amount, int slotNumber, uint durability)
        {
            return AddItemToSlot(inv, item, amount, slotNumber, durability: durability);
        }

        /// <summary>
        /// Adds a certain amount of an item to an specific slot. If it fills slot entirily it will return the remaning items to be stored.
        /// </summary>
        /// <param name="inv">The inventory in witch the item will be placed</param>
        /// <param name="item">The item that will be stored in the inventory</param>
        /// <param name="amount">The amount of items to be stored</param>
        /// <param name="slotNumber">The index of the slot to store the items</param>
        /// <returns>If the slot gets full and there are still items to store it will return the number of items remaining</returns>
        public static int AddItem(this Inventory inv, Item item, int amount, int slotNumber, uint durability, Action callback)
        {
            return AddItemToSlot(inv, item, amount, slotNumber, durability: durability, callback: callback);
        }

        /// <summary>
        /// Adds a certain amount of an item to an specific slot. If it fills slot entirily it will return the remaning items to be stored.
        /// </summary>
        /// <param name="inv">The inventory in witch the item will be placed</param>
        /// <param name="item">The item that will be stored in the inventory</param>
        /// <param name="amount">The amount of items to be stored</param>
        /// <param name="slotNumber">The index of the slot to store the items</param>
        /// <returns>If the slot gets full and there are still items to store it will return the number of items remaining</returns>
        public static int AddItem(this Inventory inv, Item item, int amount, int slotNumber, Action callback)
        {
            return AddItemToSlot(inv, item, amount, slotNumber, callback: callback);
        }

        #endregion
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
        /// Defines the type of interactions you can have with the inventory
        /// </summary>
        public InventoryProtection interactiable;

        public bool hasInitializated;

        public Inventory(List<Slot> _slots, int _slotAmounts, InventoryProtection _interactiable, bool _areItemsUsable = true, bool _areItemsDroppable = true)
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

        public Inventory(int _slotAmounts, bool _areItemsUsable, InventoryProtection _interactiable = InventoryProtection.Any, bool _areItemsDroppable = true)
        {
            slots = new List<Slot>();
            slotAmounts = _slotAmounts;
            areItemsUsable = _areItemsUsable;
            interactiable = _interactiable;
            areItemsDroppable = _areItemsDroppable;
        }

        public Inventory(int _slotAmounts, bool _areItemsUsable, InventoryProtection _interactiable = InventoryProtection.Any)
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
        public bool isProductSlot;
        public SlotProtection interative;
        public ItemGroup whitelist;
        public uint durability;
        public readonly static Slot nullSlot = new Slot(null, 0, false, false, SlotProtection.Any, null);

        public static uint SetDurability(ref Slot slot, uint value, bool op = false)
        {
            if (op)
            {
                slot.durability = value;
                return value;
            }
            if (slot.item == null || !slot.hasItem || value > slot.item.maxDurability || !slot.item.hasDurability)
                return 0u;
            slot.durability = value;
            return value;
        }

        public uint GetDurability() { return durability; }
        public int GetDurabiliyIntValue() { return checked((int)durability); }

        public Slot(Slot slot, bool _isProductSlot, SlotProtection _interactive, ItemGroup _whitelist)
        {
            item = slot.item;
            amount = slot.amount;
            hasItem = slot.hasItem;
            durability = slot.durability;
            isProductSlot = _isProductSlot;
            interative = _interactive;
            whitelist = _whitelist;
        }

        public Slot(Slot slot, bool _isProductSlot, SlotProtection _interactive)
        {
            item = slot.item;
            amount = slot.amount;
            hasItem = slot.hasItem;
            durability = slot.durability;
            isProductSlot = _isProductSlot;
            interative = _interactive;
            whitelist = null;
        }

        public Slot(Item _item)
        {
            item = _item;
            amount = 1;
            hasItem = item != null ? false : true;
            isProductSlot = false;
            interative = SlotProtection.Any;
            whitelist = null;
            durability = 0;
        }

        public Slot(Item _item, int _amount)
        {
            item = _item;
            amount = _amount;
            hasItem = amount == 0 ? false : true;
            isProductSlot = false;
            interative = SlotProtection.Any;
            whitelist = null;
            durability = 0;
        }

        public Slot(Item _item, int _amount, bool _hasItem)
        {
            item = _item;
            amount = _amount;
            hasItem = _hasItem;
            isProductSlot = false;
            interative = SlotProtection.Any;
            whitelist = null;
            durability = 0;
        } 
        
        public Slot(Item _item, int _amount, bool _hasItem, uint _durability)
        {
            item = _item;
            amount = _amount;
            hasItem = _hasItem;
            isProductSlot = false;
            interative = SlotProtection.Any;
            whitelist = null;
            durability = _durability;
        }

        public Slot(Item _item, int _amount, bool _hasItem, bool _isProductSlot)
        {
            item = _item;
            amount = _amount;
            hasItem = _hasItem;
            isProductSlot = _isProductSlot;
            interative = SlotProtection.Any;
            whitelist = null;
            durability = 0;
        }
        
        public Slot(Item _item, int _amount, bool _hasItem, bool _isProductSlot, uint _durability)
        {
            item = _item;
            amount = _amount;
            hasItem = _hasItem;
            isProductSlot = _isProductSlot;
            interative = SlotProtection.Any;
            whitelist = null;
            durability = _durability;
        }

        public Slot(Item _item, int _amount, bool _hasItem, bool _isProductSlot, SlotProtection _interactive)
        {
            item = _item;
            amount = _amount;
            hasItem = _hasItem;
            isProductSlot = _isProductSlot;
            interative = _interactive;
            whitelist = null;
            durability = 0;
        }

        public Slot(Item _item, int _amount, bool _hasItem, bool _isProductSlot, SlotProtection _interactive, ItemGroup _whitelist)
        {
            item = _item;
            amount = _amount;
            hasItem = _hasItem;
            isProductSlot = _isProductSlot;
            interative = _interactive;
            whitelist = _whitelist;
            durability = 0;
        }
        
        public Slot(Item _item, int _amount, bool _hasItem, bool _isProductSlot, SlotProtection _interactive, ItemGroup _whitelist, uint _durability)
        {
            item = _item;
            amount = _amount;
            hasItem = _hasItem;
            isProductSlot = _isProductSlot;
            interative = _interactive;
            whitelist = _whitelist;
            durability = _durability;
        }
    }

    [Serializable]
    public class InventoryData
    {
        public Inventory[] inventories;
    }

    [Serializable]
    public class CheckItemData
    {
        public Inventory inventory;
        public int[] slotsChecked;
        public int[] slotsWithItem;
        public int amount;
        public bool hasItem;
        public bool mustBeOnSameSlot;
        public Item checkedItem;

        public CheckItemData(Inventory _inventory, int[] _slotsChecked, int[] _slotsWithItem, int _amount, bool _hasItem, bool _mustBeOnSameSlot, Item _checkedItem)
        {
            inventory = _inventory;
            slotsChecked = _slotsChecked;
            slotsWithItem = _slotsWithItem;
            amount = _amount;
            hasItem = _hasItem;
            mustBeOnSameSlot = _mustBeOnSameSlot;
            checkedItem = _checkedItem;
        }
    }

    [Serializable]
    public class CraftItemData
    {
        public Item[] items;
        public int[] amounts;

        public CraftItemData(Item[] _items, int[] _amounts)
        {
            items = _items;
            amounts = _amounts;
        }

        public readonly static CraftItemData nullData = new CraftItemData(null, null);
    }

    [Serializable]
    public enum InventoryProtection
    {
        Any = 0,
        InventoryToInventory = 1,
        SlotToSlot = 2,
        LockSlots = 4,
        LockThruInventory = 8,
        Locked = 16
    }

    [Serializable]
    public enum SlotProtection
    {
        Any = 0,
        Locked = 1,
        OnlyAdd = 2,
        OnlyRemove = 4
    }

}
