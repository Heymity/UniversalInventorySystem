/*  Copyright 2020 Gabriel Pasquale Rodrigues Scavone
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 * 
 * 
 *  This code is the core of the inventory system, it manipulates the inventories and contains some of the base classes of the system like Slot and Inventory
 */

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

        public static readonly Slot nullSlot = new Slot(null, 0, false, 0);

        #region Protection consts

        public const InventoryProtection AllInventoryFlags = InventoryProtection.Add
            | InventoryProtection.InventoryToInventory
            | InventoryProtection.Locked
            | InventoryProtection.Remove
            | InventoryProtection.Drop
            | InventoryProtection.SlotToSlot
            | InventoryProtection.Use;

        public const InventoryProtection AddInvFlags = InventoryProtection.Add;
        public const InventoryProtection RemoveInvFlags = InventoryProtection.Remove;
        public const InventoryProtection UseInvFlags = InventoryProtection.Use;
        public const InventoryProtection LocalSwapInvFlags = InventoryProtection.SlotToSlot;
        public const InventoryProtection SwapInvFlags = InventoryProtection.InventoryToInventory;
        public const InventoryProtection DropInvFlags = (InventoryProtection)0b_0010_1000;

        public const SlotProtection AllSlotFlags = SlotProtection.Locked
            | SlotProtection.Add
            | SlotProtection.Remove
            | SlotProtection.Use
            | SlotProtection.Swap;

        public const SlotProtection AddFlags = SlotProtection.Add;
        public const SlotProtection RemoveFlags = SlotProtection.Remove;
        public const SlotProtection SwapFlags = SlotProtection.Swap;
        public const SlotProtection UseFlags = SlotProtection.Use;

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

        public static Slot GetSlotInInventory(int invIndex, int slotIndex) => inventories[invIndex][slotIndex];

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

        ///TODO: Crafting Events
        #region Add

        /// <summary>
        /// Adds a certain amount of an item to the first empty slot even if there are slots of the same item that can still hold more items. If the specified amount is grater than the maxAmount for that item it will fill the next slot
        /// </summary>
        /// <param name="inv">The inventory in witch the item will be placed</param>
        /// <param name="item">The item that will be stored in the inventory</param>
        /// <param name="amount">The amount of items to be stored</param>
        /// <returns>If the inventory gets full and there are still items to store it will return the number of items remaining</returns>
        public static int AddItemToNewSlot(this Inventory inv, Item item, int amount, BroadcastEventType e = BroadcastEventType.AddItem, bool overrideSlotProtection = false, int? durability = null, Action callback = null)
        {
            if (inv == null)
            {
                Debug.LogError("Null inventory provided for AddItemToNewSlot");
                throw new ArgumentNullException("inv", "Null inventory provided");
            }
            if (item == null)
            {
                Debug.LogError("Null item provided for AddItemToNewSlot");
                throw new ArgumentNullException("inv", "Null item provided");
            }

            if (!AcceptsInventoryProtection(inv, MethodType.Add)) return amount;

            if (durability == null) durability = item.maxDurability;

            if (!item.stackable)
            {
                for (int i = 0; i < inv.slots.Count; i++)
                {
                    if (!AcceptsSlotProtection(inv.slots[i], MethodType.Add) && !overrideSlotProtection) continue;
                    if (!inv.slots[i].whitelist?.itemsList.Contains(item) ?? false) continue;

                    if (inv.slots[i].HasItem && i < inv.slots.Count - 1) continue;
                    else if (i < inv.slots.Count - 1)
                    {
                        inv.slots[i] = Slot.SetItemProperties(inv.slots[i], item, 1, durability.GetValueOrDefault());
                        amount--;

                        if (amount <= 0) break;
                        continue;
                    }
                    else if (!inv.slots[i].HasItem)
                    {
                        inv.slots[i] = Slot.SetItemProperties(inv.slots[i], item, 1, durability.GetValueOrDefault());
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
                if (inv.slots[i].HasItem) continue;
                if (!AcceptsSlotProtection(inv.slots[i], MethodType.Add) && !overrideSlotProtection) continue;
                if (!inv.slots[i].whitelist?.itemsList.Contains(item) ?? false) continue;

                else if (i < inv.slots.Count - 1)
                {
                    var maxAmount = item.maxAmount;

                    if (amount <= maxAmount)
                        inv.slots[i] = Slot.SetItemProperties(inv.slots[i], item, amount, durability.GetValueOrDefault());
                    else
                    {
                        inv.slots[i] = Slot.SetItemProperties(inv.slots[i], item, maxAmount, durability.GetValueOrDefault());
                        amount -= maxAmount;
                        if (amount > 0) continue;
                        else break;
                    }

                    break;
                }
                else if (!inv.slots[i].HasItem)
                {
                    InventoryHandler.AddItemEventArgs aea2 = new InventoryHandler.AddItemEventArgs(inv, true, false, item, amount, null);
                    var newSlot = inv.slots[i].amount;
                    amount -= item.maxAmount - newSlot;
                    newSlot = item.maxAmount;
                    inv.slots[i] = Slot.SetItemProperties(inv.slots[i], item, newSlot, durability.GetValueOrDefault());
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
        public static int AddItem(this Inventory inv, Item item, int amount, BroadcastEventType e = BroadcastEventType.AddItem, bool overrideSlotProtection = false, int? durability = null, Action callback = null)
        {
            //Validating Arguments
            if (!AcceptsInventoryProtection(inv, MethodType.Add)) return amount;

            if (inv == null)
            {
                Debug.LogError("Null inventory provided for AddItem");
                throw new ArgumentNullException("inv", "Null Inventory was provided");
            }
            if (item == null)
            {
                Debug.LogError("Null item provided for AddItem");
                throw new ArgumentNullException("item", "Null Item was provided");
            }
            
            if (durability == null) durability = item.maxDurability;

            //If the items is not marked as stackable it calls AddItemToNewSlot witch handles the rest
            if (!item.stackable) return AddItemToNewSlot(inv, item, amount, e);

            for (int i = 0; i < inv.slots.Count; i++)
            {
                if (inv.slots[i].item != item) continue;                                         // Must be same item
                if (inv.slots[i].amount == inv.slots[i].item.maxAmount) continue;                // Must fit at least one
                if (!AcceptsSlotProtection(inv.slots[i], MethodType.Add) && !overrideSlotProtection) 
                    continue;                                                                    // Must have a acceptable protection level

                if (!inv.slots[i].whitelist?.itemsList.Contains(item) ?? false) continue;        // Must be accepted in the whitelist since it may change in runtime

                var newSlot = inv.slots[i];                                                      // Tmp var

                if (newSlot.amount + amount <= item.maxAmount)                                   // If the entire amount fits on the slot
                {
                    newSlot.amount += amount;
                    amount = 0;
                    inv.slots[i] = newSlot;
                    break;
                }
                else if (newSlot.amount + amount > item.maxAmount)                               // If the entire amount doesn`t fit on the slot
                {
                    amount -= item.maxAmount - newSlot.amount;
                    newSlot.amount = item.maxAmount;
                    inv.slots[i] = newSlot;
                    if (amount > 0) continue;
                }
            }

            //If there are still Items to add and there are no other slot with the same item it adds to a new slot
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
        public static int AddItemToSlot(this Inventory inv, Item item, int amount, int slotNumber, BroadcastEventType e = BroadcastEventType.AddItem, bool overrideSlotProtection = false, int? durability = null, Action callback = null)
        {
            if (inv == null)
            {
                Debug.LogError("Null inventory provided for AddItemToSlot");
                throw new ArgumentNullException("inv", "Null inventory provided");
            }
            if (item == null)
            {
                Debug.LogError("Null item provided for AddItemToSlot");
                throw new ArgumentNullException("item", "Null item provided");
            }

            if (!AcceptsInventoryProtection(inv, MethodType.Add)) return amount;

            if (!AcceptsSlotProtection(inv.slots[slotNumber], MethodType.Add) && !overrideSlotProtection) return amount;

            if (!inv.slots[slotNumber].whitelist?.itemsList.Contains(item) ?? false) return amount;

            if (durability == null) durability = item.maxDurability;

            if (!item.stackable)
            {
                if (inv.slots[slotNumber].HasItem) return amount;
                inv.slots[slotNumber] = Slot.SetItemProperties(inv.slots[slotNumber], item, 1, durability.GetValueOrDefault() / amount);
                callback?.Invoke();
                InventoryHandler.AddItemEventArgs aea = new InventoryHandler.AddItemEventArgs(inv, false, true, item, amount, slotNumber);
                InventoryHandler.current.Broadcast(e, aea);
                return amount - 1 > 0 ? amount - 1 : 0;
            }

            if (!inv.slots[slotNumber].HasItem)
            {
                if (amount < item.maxAmount)
                    inv.slots[slotNumber] = Slot.SetItemProperties(inv.slots[slotNumber], item, amount, durability.GetValueOrDefault());
                else
                {
                    inv.slots[slotNumber] = Slot.SetItemProperties(inv.slots[slotNumber], item, item.maxAmount, durability.GetValueOrDefault());
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
                    inv.slots[slotNumber] = Slot.SetItemProperties(inv.slots[slotNumber], item, amount + inv.slots[slotNumber].amount, inv.slots[slotNumber].durability/*, inv.slots[slotNumber].totalDurability + totalDurability.GetValueOrDefault()*/);
                else
                {
                    int valueToReeturn = amount + inv.slots[slotNumber].amount - item.maxAmount;
                    inv.slots[slotNumber] = Slot.SetItemProperties(inv.slots[slotNumber], item, item.maxAmount, inv.slots[slotNumber].durability/*, inv.slots[slotNumber].totalDurability + totalDurability.GetValueOrDefault()*/);
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
        /// <returns>The RemoveItem or RemoveItemInSlot function return value</returns>
        public static bool DropItem(this Inventory inv, int amount, Vector3 dropPosition, Item item, BroadcastEventType e = BroadcastEventType.DropItem, bool overrideSlotProtecion = true)
        {
            if (inv == null)
            {
                Debug.LogError("Null inventory provided for DropItem");
                throw new ArgumentNullException("inv", "Null inventory provided");
            }

            if (!AcceptsInventoryProtection(inv, MethodType.Drop)) return false;

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

        /// <summary>
        /// Drops a item. Its a cover function that calls either the RemoveItem or the RemoveItemInSlot
        /// </summary>
        /// <param name="inv">The inventory in witch the item will be removed</param>
        /// <param name="amount">The amount of items to be removed</param>
        /// <param name="slot">The slot that will have item removed</param>
        /// <returns>The RemoveItem or RemoveItemInSlot function return value</returns>
        public static bool DropItem(this Inventory inv, int amount, Vector3 dropPosition, int slot, BroadcastEventType e = BroadcastEventType.DropItem, bool overrideSlotProtecion = true)
        {
            if (inv == null)
            {
                Debug.LogError("Null inventory provided for DropItem");
                throw new ArgumentNullException("inv", "Null inventory provided");
            }

            if (!AcceptsInventoryProtection(inv, MethodType.Drop)) return false;

            if (slot >= 0 && slot < inv.slots.Count)
            {
                return RemoveItemInSlot(inv, slot, amount, e, dropPosition, overrideSlotProtecion);
            }
            else
            {
                Debug.LogError($"Invalid slot number provided for DropItem; slot number: {slot}");
                throw new ArgumentOutOfRangeException("slot", "The slot number provided was out of the inventory slots bounds");
            }
        }

        /// <summary>
        /// Removes a certain item in a certain amount from the first apearence in certain inventory. When a slot runs out of items it goes to the next one with that item
        /// </summary>
        /// <param name="inv">The inventory in witch the item will be removed</param>
        /// <param name="item">The item that will be removed in the inventory</param>
        /// <param name="amount">The amount of items to be removed</param>
        /// <returns>True if it was able to remove the items False if it wasnt</returns>
        public static bool RemoveItem(this Inventory inv, Item item, int amount, BroadcastEventType e = BroadcastEventType.RemoveItem, Vector3? dropPosition = null, bool overrideSlotProtection = false)
        {
            if (inv == null)
            {
                Debug.LogError("Null inventory provided for RemoveItem");
                throw new ArgumentNullException("inv", "Null inventory provided");
            }
            if (item == null)
            {
                Debug.LogError("Null item provided for RemoveItem");
                throw new ArgumentNullException("item", "Null item provided");
            }

            if (!AcceptsInventoryProtection(inv, MethodType.Remove)) return false;

            int total = 0;
            for (int i = 0; i < inv.slots.Count; i++)
            {
                if (inv.slots[i].item == item && (AcceptsSlotProtection(inv.slots[i], MethodType.Remove) || overrideSlotProtection))
                {
                    total += inv.slots[i].amount;
                }
            }

            int index = 0;
            if (total >= amount)
            {
                for (int i = 0; i < inv.slots.Count; i++)
                {
                    if (inv.slots[i].item == item && (AcceptsSlotProtection(inv.slots[i], MethodType.Remove) || overrideSlotProtection))
                    {
                        int prevAmount = inv.slots[i].amount;
                        Slot slot = inv.slots[i];
                        slot.amount -= amount;
                        inv.slots[i] = slot;
                        if (slot.amount <= 0)
                            inv.slots[i] = Slot.SetItemProperties(inv.slots[i], nullSlot);
                        else break;
                        amount -= prevAmount;
                        index = i;
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
                item.OnDrop(inv, false, index, amount, false, dropPosition);
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
            if (inv == null)
            {
                Debug.LogError("Null inventory provided for RemoveItemInSlot");
                throw new ArgumentNullException("inv", "Null inventory provided");
            }

            if (!AcceptsInventoryProtection(inv, MethodType.Remove)) return false;

            if (!AcceptsSlotProtection(inv.slots[slot], MethodType.Remove) && !overrideSlotProtecion) return false;

            dropPosition = (dropPosition ?? new Vector3(0, 0, 0));
            InventoryHandler.RemoveItemEventArgs rea = new InventoryHandler.RemoveItemEventArgs(inv, false, amount, inv.slots[slot].item, slot);

            if (inv.slots[slot].amount == amount)
            {
                Item tmp = inv.slots[slot].item;
                inv.slots[slot] = Slot.SetItemProperties(inv.slots[slot], nullSlot);

                if (e == BroadcastEventType.DropItem)
                    tmp?.OnDrop(inv, true, slot, amount, false, dropPosition);
                else InventoryHandler.current.Broadcast(e, rea: rea);
                return true;
            }
            else if (inv.slots[slot].amount > amount)
            {
                Item tmp = inv.slots[slot].item;
                inv.slots[slot] = Slot.SetItemProperties(inv.slots[slot], inv.slots[slot].item, inv.slots[slot].amount - amount, 0);

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
        public static void UseItemInSlot(this Inventory inv, int slot, BroadcastEventType e = BroadcastEventType.UseItem, bool overrideSlotProtection = false)
        {
            if (inv == null)
            {
                Debug.LogError("Null inventory provided for UseItemInSlot");
                throw new ArgumentNullException("inv", "Null inventory provided");
            }

            if (!AcceptsInventoryProtection(inv, MethodType.Use)) return;
            if (!AcceptsSlotProtection(inv.slots[slot], MethodType.Use) && !overrideSlotProtection) return;

            if (inv.slots[slot].HasItem)
            {
                if (inv.slots[slot].item.destroyOnUse)
                {
                    Item it = inv.slots[slot].item;
                    if (it.hasDurability)
                    {
                        if(inv.slots[slot].durability > 0)
                        {
                            var tmp = inv.slots[slot];
                            Slot.SetDurability(ref tmp, inv.slots[slot].durability - 1);
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
                            Slot.SetDurability(ref tmp, inv.slots[slot].durability - 1);
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
        public static void UseItem(this Inventory inv, Item item, BroadcastEventType e = BroadcastEventType.UseItem, bool overrideSlotProtection = false)
        {
            if (inv == null)
            {
                Debug.LogError("Null inventory provided for UseItemInSlot");
                throw new ArgumentNullException("inv", "Null inventory provided");
            }

            if (!AcceptsInventoryProtection(inv, MethodType.Use)) return;

            for(int i = 0; i < inv.slots.Count; i++)
            {
                if (!inv.slots[i].HasItem) continue;
                if (inv.slots[i].item != item) continue;
                if (!AcceptsSlotProtection(inv.slots[i], MethodType.Use) && !overrideSlotProtection) continue;
                if (inv.slots[i].item.destroyOnUse)
                {
                    Item it = inv.slots[i].item;
                    if (it.hasDurability)
                    {
                        if (inv.slots[i].durability > 0)
                        {
                            var tmp = inv.slots[i];
                            Slot.SetDurability(ref tmp, inv.slots[i].durability - 1);
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
                            Slot.SetDurability(ref tmp, inv.slots[i].durability - 1);
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
        public static void SwapItemsInSlots(this Inventory inv, int nativeSlot, int targetSlot, BroadcastEventType e = BroadcastEventType.SwapItem, bool overrideSlotProtection = false)
        {
            if (inv == null)
            {
                Debug.LogError("Null inventory provided for SwapItemsInSlots");
                throw new ArgumentNullException("inv", "Null inventory provided");
            }

            if (inv[nativeSlot].item == null)
            {
                Debug.LogError("Null item provided for SwapItemsInCertainAmountInSlots");
                return;
            }

            if (!AcceptsInventoryProtection(inv, MethodType.LocalSwap))
                return;

            if (inv.slots[targetSlot].isProductSlot) return;

            if (!AcceptsSlotProtection(inv.slots[targetSlot], MethodType.Swap) && !overrideSlotProtection) return;
            if (!AcceptsSlotProtection(inv.slots[nativeSlot], MethodType.Swap) && !overrideSlotProtection) return;


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


            Slot tmpSlot = inv.slots[targetSlot];

            if (inv.slots[nativeSlot].isProductSlot || inv.slots[targetSlot].isProductSlot)
            {
                if (tmpSlot.item == null)
                {
                    inv.slots[targetSlot] = Slot.SetItemProperties(inv.slots[targetSlot], inv.slots[nativeSlot]);
                    inv.slots[nativeSlot] = Slot.SetItemProperties(inv.slots[nativeSlot], tmpSlot);

                    InventoryHandler.SwapItemsEventArgs sea2 = new InventoryHandler.SwapItemsEventArgs(inv, nativeSlot, targetSlot, inv.slots[targetSlot].item, tmpSlot.item, null);
                    InventoryHandler.current.Broadcast(e, sea: sea2);
                    return;
                }
                return;
            }

            inv.slots[targetSlot] = Slot.SetItemProperties(inv.slots[targetSlot], inv.slots[nativeSlot]);

            inv.slots[nativeSlot] = Slot.SetItemProperties(inv.slots[nativeSlot], tmpSlot);

            InventoryHandler.SwapItemsEventArgs sea = new InventoryHandler.SwapItemsEventArgs(inv, nativeSlot, targetSlot, inv.slots[targetSlot].item, tmpSlot.item, null);
            InventoryHandler.current.Broadcast(e, sea: sea);

        }

        /// <summary>
        ///Swap a certain amount of items in two slots. This function will stack items.
        /// </summary>
        /// <param name="inv">The inventary to have items swapped</param>
        /// <param name="nativeSlot">The slot to lose items</param>
        /// <param name="targetSlot">The slot to gain items</param>
        /// <param name="amount">The amount of items to be swaped</param>
        /// <returns>Returns the number of items that dind fit in the other slot</returns>
        public static int SwapItemsInCertainAmountInSlots(this Inventory inv, int nativeSlot, int targetSlot, int? _amount, BroadcastEventType e = BroadcastEventType.SwapItem, bool overrideSlotProtection = false)
        {
            //Validations
            if (inv == null)
            {
                Debug.LogError("Null inventory provided for SwapItemsInCertainAmountInSlots");
                throw new ArgumentNullException("inv", "Null inventory provided");
            }
            if (inv[nativeSlot].item == null)
                Debug.LogWarning("Null item provided for SwapItemsInCertainAmountInSlots");
                //return _amount ?? inv.slots[nativeSlot].amount;


            if (!inv[nativeSlot].item?.stackable ?? false)
            {
                inv.SwapItemsInSlots(nativeSlot, targetSlot);
                return 0;
            }

            if (!AcceptsInventoryProtection(inv, MethodType.LocalSwap))
                return _amount ?? inv.slots[nativeSlot].amount;

            if (inv.slots[targetSlot].isProductSlot) return (_amount ?? inv.slots[nativeSlot].amount);

            if (!AcceptsSlotProtection(inv.slots[targetSlot], MethodType.Swap) && !overrideSlotProtection) return (_amount ?? inv.slots[nativeSlot].amount);
            if (!AcceptsSlotProtection(inv.slots[nativeSlot], MethodType.Swap) && !overrideSlotProtection) return (_amount ?? inv.slots[nativeSlot].amount);


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

            int amount = (_amount ?? inv.slots[nativeSlot].amount);

            if (amount <= 0) return amount;
            InventoryHandler.SwapItemsEventArgs sea;
            if (amount > inv.slots[nativeSlot].amount) return amount;
            else if (inv.slots[targetSlot].item == null)
            {
                inv.slots[targetSlot] = Slot.SetItemProperties(
                    inv.slots[targetSlot],
                    inv.slots[nativeSlot].item,
                    amount,
                    inv.slots[nativeSlot].durability
                );

                inv.slots[nativeSlot] = Slot.SetItemProperties(
                    inv.slots[nativeSlot],
                    inv.slots[nativeSlot].item,
                    inv.slots[nativeSlot].amount - amount,
                    inv.slots[nativeSlot].durability
                    );

                if (inv.slots[nativeSlot].amount <= 0) inv.slots[nativeSlot] = Slot.SetItemProperties(inv.slots[nativeSlot], nullSlot);
            }
            else if (inv.slots[nativeSlot].item == inv.slots[targetSlot].item)
            {
                int remaning = AddItemToSlot(
                    inv,
                    inv.slots[nativeSlot].item,
                    amount,
                    targetSlot
                    /// totalDurability: inv.slots[nativeSlot].totalDurability
                    );

                inv.slots[nativeSlot] = Slot.SetItemProperties(
                    inv.slots[nativeSlot],
                    inv.slots[nativeSlot].item,
                    inv.slots[nativeSlot].amount - amount + remaning,
                    inv.slots[nativeSlot].durability
                );

                if (inv.slots[nativeSlot].amount <= 0)
                    inv.slots[nativeSlot] = Slot.SetItemProperties(inv.slots[nativeSlot], nullSlot);

                sea = new InventoryHandler.SwapItemsEventArgs(inv, nativeSlot, targetSlot, inv.slots[targetSlot].item, inv.slots[nativeSlot].item, amount - remaning);
                InventoryHandler.current.Broadcast(e, sea: sea);
                return remaning;
            }
            else SwapItemsInSlots(inv, nativeSlot, targetSlot);

            sea = new InventoryHandler.SwapItemsEventArgs(inv, nativeSlot, targetSlot, inv.slots[targetSlot].item, inv.slots[nativeSlot].item, amount);
            InventoryHandler.current.Broadcast(e, sea: sea);
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
        public static int SwapItemThruInventoriesSlotToSlot(this Inventory nativeInv, Inventory targetInv, int nativeSlotNumber, int targetSlotNumber, int amount, BroadcastEventType e = BroadcastEventType.SwapTrhuInventory, bool overrideSlotProtection = false)
        {
            if (nativeInv == null)
            {
                Debug.LogError("Null native inventory provided for SwapItemThruInventoriesSlotToSlot");
                throw new ArgumentNullException("nativeInv", "Null inventory provided");
            }
            if (targetInv == null)
            {
                Debug.LogError("Null target inventory provided for SwapItemThruInventoriesSlotToSlot");
                throw new ArgumentNullException("targetInv", "Null inventory provided");
            }

            if (nativeInv[nativeSlotNumber].item == null)
                Debug.LogWarning("Null item provided for SwapItemsInCertainAmountInSlots");

            if (!nativeInv[nativeSlotNumber].item?.stackable ?? false)
            {
                nativeInv.SwapItemsThruInventoriesInSlots(targetInv, nativeSlotNumber, targetSlotNumber);
                return 0;
            }

            if (!AcceptsInventoryProtection(nativeInv, MethodType.Swap)) return amount;
            if (!AcceptsInventoryProtection(targetInv, MethodType.Swap)) return amount;

            if (targetInv.slots[targetSlotNumber].isProductSlot) return amount;

            if (!AcceptsSlotProtection(nativeInv.slots[nativeSlotNumber], MethodType.Swap) && !overrideSlotProtection) return amount;
            if (!AcceptsSlotProtection(targetInv.slots[targetSlotNumber], MethodType.Swap) && !overrideSlotProtection) return amount;

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

            InventoryHandler.SwapItemsTrhuInvEventArgs siea;
            if (amount > nativeInv.slots[nativeSlotNumber].amount) return amount;
            else if (targetInv.slots[targetSlotNumber].item == null)
            {
                targetInv.slots[targetSlotNumber] = Slot.SetItemProperties(targetInv.slots[targetSlotNumber],
                    nativeInv.slots[nativeSlotNumber].item,
                    amount,
                    nativeInv.slots[nativeSlotNumber].durability
                );

                nativeInv.slots[nativeSlotNumber] = Slot.SetItemProperties(nativeInv.slots[nativeSlotNumber],
                    nativeInv.slots[nativeSlotNumber].item,
                    nativeInv.slots[nativeSlotNumber].amount - amount,
                    nativeInv.slots[nativeSlotNumber].durability
                );

                if (nativeInv.slots[nativeSlotNumber].amount <= 0) nativeInv.slots[nativeSlotNumber] = Slot.SetItemProperties(
                    nativeInv.slots[nativeSlotNumber],
                    nullSlot
                );
            }
            else if (nativeInv.slots[nativeSlotNumber].item == targetInv.slots[targetSlotNumber].item)
            {
                int remaning = AddItemToSlot(targetInv, nativeInv.slots[nativeSlotNumber].item, amount, targetSlotNumber);

                nativeInv.slots[nativeSlotNumber] = Slot.SetItemProperties(
                    nativeInv.slots[nativeSlotNumber],
                    nativeInv.slots[nativeSlotNumber].item,
                    nativeInv.slots[nativeSlotNumber].amount - amount + remaning,
                    nativeInv.slots[nativeSlotNumber].durability
                );

                if (nativeInv.slots[nativeSlotNumber].amount <= 0)
                    nativeInv.slots[nativeSlotNumber] = Slot.SetItemProperties(
                        nativeInv.slots[nativeSlotNumber],
                        nullSlot
                    );

                siea = new InventoryHandler.SwapItemsTrhuInvEventArgs(nativeInv, targetInv, nativeSlotNumber, targetSlotNumber, targetInv.slots[targetSlotNumber].item, nativeInv.slots[nativeSlotNumber].item, amount - remaning);
                InventoryHandler.current.Broadcast(e, siea: siea);

                return remaning;
            }
            else
            {
                Slot tmpSlot = targetInv.slots[targetSlotNumber];

                targetInv.slots[targetSlotNumber] = Slot.SetItemProperties(targetInv.slots[targetSlotNumber], nativeInv.slots[nativeSlotNumber]);

                nativeInv.slots[nativeSlotNumber] = Slot.SetItemProperties(nativeInv.slots[nativeSlotNumber], tmpSlot);
            }

            siea = new InventoryHandler.SwapItemsTrhuInvEventArgs(nativeInv, targetInv, nativeSlotNumber, targetSlotNumber, targetInv.slots[targetSlotNumber].item, nativeInv.slots[nativeSlotNumber].item, amount);
            InventoryHandler.current.Broadcast(e, siea: siea);
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
            if (nativeInv == null)
            {
                Debug.LogError("Null native inventory provided for SwapItemThruInventories");
                throw new ArgumentNullException("nativeInv", "Null inventory provided");
            }
            if (targetInv == null)
            {
                Debug.LogError("Null target inventory provided for SwapItemThruInventories");
                throw new ArgumentNullException("targetInv", "Null inventory provided");
            }

            if (!AcceptsInventoryProtection(nativeInv, MethodType.Swap)) return false;
            if (!AcceptsInventoryProtection(targetInv, MethodType.Swap)) return false;

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

        /// <summary>
        /// Literally swap slot A with slot B.
        /// </summary>
        /// <param name="nativeInv">The native inventory</param>
        /// <param name="targetInv">The target inventory</param>
        /// <param name="nativeSlot">The slot Number of the native inventory</param>
        /// <param name="targetSlot">The Slot numben of the target inventory</param>
        public static void SwapItemsThruInventoriesInSlots(this Inventory nativeInv, Inventory targetInv, int nativeSlot, int targetSlot, BroadcastEventType e = BroadcastEventType.SwapTrhuInventory, bool overrideSlotProtection = false)
        {
            if (nativeInv == null || targetInv == null)
            {
                Debug.LogError("Null inventory provided for SwapItemsInSlots");
                throw new ArgumentNullException(nativeInv == null ? "nativeInv" : "targetInv", "Null inventory provided");
            }

            if (!AcceptsInventoryProtection(nativeInv, MethodType.Swap)) return;
            if (!AcceptsInventoryProtection(targetInv, MethodType.Swap)) return;

            if (targetInv.slots[targetSlot].isProductSlot) return;

            if (!AcceptsSlotProtection(nativeInv.slots[nativeSlot], MethodType.Swap) && !overrideSlotProtection) return;
            if (!AcceptsSlotProtection(targetInv.slots[targetSlot], MethodType.Swap) && !overrideSlotProtection) return;


            //Verifys if the items to be swaped are in the whitelists
            bool whitelist =
            (nativeInv.slots[nativeSlot].whitelist?.itemsList.ContainsWNull(targetInv.slots[targetSlot].item)
            ??
            (targetInv.slots[targetSlot].whitelist == null ? true : targetInv.slots[targetSlot].whitelist.itemsList.ContainsWNull(nativeInv.slots[nativeSlot].item)))
            &&
            (targetInv.slots[targetSlot].whitelist?.itemsList.ContainsWNull(nativeInv.slots[nativeSlot].item)
            ??
            (nativeInv.slots[nativeSlot].whitelist == null ? true : nativeInv.slots[nativeSlot].whitelist.itemsList.ContainsWNull(targetInv.slots[targetSlot].item)));

            if (!whitelist) return;


            Slot tmpSlot = targetInv.slots[targetSlot];

            if (nativeInv.slots[nativeSlot].isProductSlot || targetInv.slots[targetSlot].isProductSlot)
            {
                if (tmpSlot.item == null)
                {
                    targetInv.slots[targetSlot] = Slot.SetItemProperties(targetInv.slots[targetSlot], nativeInv.slots[nativeSlot]);
                    nativeInv.slots[nativeSlot] = Slot.SetItemProperties(nativeInv.slots[nativeSlot], tmpSlot);

                    InventoryHandler.SwapItemsTrhuInvEventArgs siea2 = new InventoryHandler.SwapItemsTrhuInvEventArgs(nativeInv, targetInv, nativeSlot, targetSlot, nativeInv.slots[targetSlot].item, tmpSlot.item, null);
                    InventoryHandler.current.Broadcast(e, siea: siea2);
                    return;
                }
                return;
            }

            targetInv.slots[targetSlot] = Slot.SetItemProperties(targetInv.slots[targetSlot], nativeInv.slots[nativeSlot]);

            nativeInv.slots[nativeSlot] = Slot.SetItemProperties(nativeInv.slots[nativeSlot], tmpSlot);

            InventoryHandler.SwapItemsTrhuInvEventArgs siea = new InventoryHandler.SwapItemsTrhuInvEventArgs(nativeInv, targetInv, nativeSlot, targetSlot, nativeInv.slots[nativeSlot].item, tmpSlot.item, null);
            InventoryHandler.current.Broadcast(e, siea: siea);

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
        public static CraftItemData CraftItem(this Inventory inv, CraftItemData grid, Vector2Int gridSize, bool craftItem, bool allowPatternRecipe, int productSlots)
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
                    var recipeRes = CraftItem(inv, grid, craftItem, recipe, productSlots);
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
        public static CraftItemData CraftItem(this Inventory inv, CraftItemData grid, Vector2Int gridSize, bool craftItem, RecipeGroup asset, bool allowPatternRecipe, int productSlots)
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
                var recipeRes = CraftItem(inv, grid, craftItem, recipe, productSlots);
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
        public static CraftItemData CraftItem(this Inventory inv, CraftItemData grid, Vector2Int gridSize, bool craftItem, PatternRecipe pattern, int productSlots)
        {
            if (pattern.pattern.Length > grid.items.Length) return CraftItemData.nullData;
            if (pattern.products.Length > productSlots) return CraftItemData.nullData;
            else if (pattern.pattern.Length == grid.items.Length && pattern.amountPattern.Length == grid.amounts.Length)
            {
                if (Enumerable.SequenceEqual(pattern.pattern, grid.items) && SequenceEqualOrGreter(pattern.amountPattern, grid.amounts))
                {
                    if (craftItem)
                    {
                        bool canAdd = true;
                        int tmp = 0;
                        for (int h = grid.items.Length; h - grid.items.Length < productSlots; h++)
                        {
                            if (!inv.slots[h].HasItem) { tmp++; continue; }
                            for (int index = 0; index < pattern.products.Count(); index++)
                            {
                                if (inv.slots[h].item == pattern.products[index])
                                {
                                    if (inv.slots[h].amount + pattern.amountProducts[index] > inv.slots[h].item.maxAmount) { continue; }
                                    tmp++; 
                                    continue; 
                                }
                            }
                        }
                        if (tmp < pattern.products.Length)
                            canAdd = false;

                        if (canAdd)
                        {
                            int i = 0;
                            int offset = 0;
                            for (int k = grid.items.Length; k < inv.slots.Count; k++)
                            {
                                if (k > grid.items.Length - 1)
                                {
                                    if (k - grid.items.Length >= pattern.products.Length) break;

                                    AddOffset:
                                    if (k + offset >= inv.slots.Count) return CraftItemData.nullData;
                                    if (inv[k + offset].HasItem &&
                                            (inv[k + offset].item != pattern.products[k - grid.items.Length] ||
                                            (inv[k + offset].item == pattern.products[k - grid.items.Length] &&
                                            inv[k + offset].amount + pattern.amountProducts[k - grid.items.Length] > inv[k + offset].item.maxAmount)))
                                    {
                                        offset++;
                                        goto AddOffset;
                                    }

                                    i = inv.AddItemToSlot(pattern.products[k - grid.items.Length], pattern.amountProducts[k - grid.items.Length], k + offset, overrideSlotProtection: true);
                                    if (i > 0) return CraftItemData.nullData;
                                }
                            }
                            if (i > 0) return CraftItemData.nullData;


                            for (int k = 0; k < grid.items.Length; k++)
                            {
                                if (inv.slots[k].HasItem && k <= grid.items.Length - 1)
                                    inv.RemoveItemInSlot(k, pattern.amountPattern[k]);

                            }
                        }
                    }
                    return new CraftItemData(pattern.products, pattern.amountProducts);
                }
            }
            else if (pattern.pattern.Length < grid.items.Length)
            {
                int fit = (gridSize.y - pattern.gridSize.y + 1) * (gridSize.x - pattern.gridSize.x + 1);

                List<int> indexes;
                for (int i = 0; i < fit; i++)
                {
                    var result = CraftItem(inv, GetSectionFromGrid(grid, gridSize, pattern.gridSize, i, out indexes), pattern.gridSize, false, pattern, productSlots);
                    if (result.items != null)
                    {
                        bool canReturn = true;
                        for (int j = 0; j < grid.items.Length; j++)
                        {
                            if (indexes.Contains(j)) continue;
                            if (grid.items[j] != null) canReturn = false;
                        }
                        if (canReturn)
                        {
                            if (craftItem)
                            {
                                bool canAdd = true;
                                int tmp = 0;
                                for (int h = grid.items.Length; h - grid.items.Length < productSlots; h++)
                                {
                                    if (!inv.slots[h].HasItem) { tmp++; continue; }
                                    for (int index = 0; index < pattern.products.Count(); index++)
                                    {
                                        if (inv.slots[h].item == pattern.products[index])
                                        {
                                            if (inv.slots[h].amount + pattern.amountProducts[index] > inv.slots[h].item.maxAmount) { continue; }
                                            tmp++;
                                            continue;
                                        }
                                    }
                                }
                                if (tmp < pattern.products.Length)
                                    canAdd = false;

                                if (canAdd)
                                {
                                    int w = 0;
                                    int offset = 0;
                                    for (int k = grid.items.Length; k < inv.slots.Count; k++)
                                    {
                                        if (k > grid.items.Length - 1)
                                        {
                                            if (k - grid.items.Length >= pattern.products.Length) break;

                                            AddOffset:
                                            if (k + offset >= inv.slots.Count) return CraftItemData.nullData;
                                            if (inv[k + offset].HasItem &&
                                                    (inv[k + offset].item != pattern.products[k - grid.items.Length] ||
                                                    (inv[k + offset].item == pattern.products[k - grid.items.Length] &&
                                                    inv[k + offset].amount + pattern.amountProducts[k - grid.items.Length] > inv[k + offset].item.maxAmount)))
                                            {
                                                offset++;
                                                goto AddOffset;
                                            }

                                            w = inv.AddItemToSlot(pattern.products[k - grid.items.Length], pattern.amountProducts[k - grid.items.Length], k + offset, overrideSlotProtection: true);
                                            if (w > 0) return CraftItemData.nullData;
                                        }
                                    }
                                    if (w > 0) return CraftItemData.nullData;

                                    for (int v = 0; v < pattern.gridSize.y; v++)
                                    {
                                        for (int u = 0; u < pattern.gridSize.x; u++)
                                        {
                                            var index = (v * gridSize.x) + u;
                                            if (inv.slots[index].HasItem && index <= grid.items.Length - 1)
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
        public static CraftItemData CraftItem(this Inventory inv, CraftItemData grid, bool craftItem, Recipe recipe, int productSlots)
        {
            List<int> jumpIndexes = new List<int>();
            List<int> tmpjumpIndexes = new List<int>();
            List<int> removeAmount = new List<int>();
            if (recipe.products.Length > productSlots) return CraftItemData.nullData;
            for (int i = 0; i < grid.items.Length; i++)
            {
                for (int j = 0; j < recipe.numberOfFactors; j++)
                {
                    if (grid.items[i] == recipe.factors[j] && !tmpjumpIndexes.Contains(j))
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
            for (int j = 0; j < grid.items.Length; j++)
            {
                if (grid.items[j] != null && !jumpIndexes.Contains(j))
                {
                    canReturn = false;
                }
            }

            for (int i = 0; i < jumpIndexes.Count; i++)
            {
                if (grid.amounts[jumpIndexes[i]] < recipe.amountFactors[i])
                {
                    canReturn = false;
                }
            }

            if (canReturn)
            {
                if (craftItem)
                {
                    bool canAdd = true;
                    int tmp = 0;
                    for (int h = grid.items.Length; h - grid.items.Length < productSlots; h++)
                    {
                        if (!inv.slots[h].HasItem) { tmp++; continue; }
                        for (int index = 0; index < recipe.products.Count(); index++)
                        {
                            if (inv.slots[h].item == recipe.products[index])
                            {
                                if (inv.slots[h].amount + recipe.amountProducts[index] > inv.slots[h].item.maxAmount) { continue; }
                                tmp++;
                                continue;
                            }
                        }
                    }
                    if (tmp < recipe.products.Length)
                        canAdd = false;

                    if (canAdd)
                    {
                        int i = 0;
                        int offset = 0;
                        for (int k = grid.items.Length; k < inv.slots.Count; k++)
                        {
                            if (k > grid.items.Length - 1)
                            {
                                if (k - grid.items.Length >= recipe.products.Length) break;

                                AddOffset:
                                if(k + offset >= inv.slots.Count) return CraftItemData.nullData;
                                if (inv[k + offset].HasItem &&
                                        (inv[k + offset].item != recipe.products[k - grid.items.Length] ||
                                        (inv[k + offset].item == recipe.products[k - grid.items.Length] &&
                                        inv[k + offset].amount + recipe.amountProducts[k - grid.items.Length] > inv[k + offset].item.maxAmount)))
                                {
                                    offset++;
                                    goto AddOffset;
                                }

                                i = inv.AddItemToSlot(recipe.products[k - grid.items.Length], recipe.amountProducts[k - grid.items.Length], k + offset, overrideSlotProtection: true);
                                if (i > 0) return CraftItemData.nullData;
                            }
                        }
                        if (i > 0) return CraftItemData.nullData;

                        var index = 0;
                        for (int k = 0; k < grid.items.Length; k++)
                        {
                            if (inv.slots[k].HasItem && k <= grid.items.Length - 1)
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
        public static CraftItemData GetSectionFromGrid(CraftItemData originalGrid, Vector2Int originalGridSize, Vector2Int sectionSize, int offsetIndex, out List<int> usedIndexes)
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
                    returnGrid[i * sectionSize.x + j] = originalGrid.items[(i + offsety) * originalGridSize.x + j + offsetx];
                    returnIntGrid[i * sectionSize.x + j] = originalGrid.amounts[(i + offsety) * originalGridSize.x + j + offsetx];
                    usedIndexes.Add((i + offsety) * originalGridSize.x + j + offsetx);
                }
            }
            return new CraftItemData(returnGrid, returnIntGrid);
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
        /// <returns>Returns a clas containing 7 attributes: inventory(The inventory checked), slotsCheckced(The slots that where checked), slotsWithItem(The Slots in witch there is the item, does not need to have minimun amount), amout(The total amout of that item in the inventory), HasItem(If the item was found in the provided conditions), mustBeOnSameSlot(SelfExplanatory), checkedItem(The item that was checked)</returns>
        public static CheckItemData CheckItemInInventory(this Inventory inv, Item itemToCheck, int minAmount, InventoryProtection[] acceptableInvProtections = null, SlotProtection acceptableSlotProtections = AllSlotFlags, bool mustBeOnSameSlot = false)
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
        /// <returns>Returns a clas containing 7 attributes: inventory(The inventory checked), slotsCheckced(The slots that where checked), slotsWithItem(The Slots in witch there is the item, does not need to have minimun amount), amout(The total amout of that item in the inventory), HasItem(If the item was found in the provided conditions), mustBeOnSameSlot(SelfExplanatory), checkedItem(The item that was checked)</returns>
        public static CheckItemData CheckItemInInventory(this Inventory inv, Item itemToCheck, int minAmount, InventoryProtection[] acceptableInvProtections = null, SlotProtection acceptableSlotProtections = AllSlotFlags, bool mustBeOnSameSlot = false, params int[] slotsToCheck)
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

            //if (acceptableInvProtections == null) acceptableInvProtections = allInventoryProtections;

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

                if (!acceptableSlotProtections.HasFlag(inv.slots[slot].interative)) continue;

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
        public static int AddItemToNewSlot(this Inventory inv, Item item, int amount, int durability)
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
        public static int AddItemToNewSlot(this Inventory inv, Item item, int amount, int durability, Action callback)
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
        public static int AddItem(this Inventory inv, Item item, int amount, int durability)
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
        public static int AddItem(this Inventory inv, Item item, int amount, int durability, Action callback)
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
        public static int AddItemToSlot(this Inventory inv, Item item, int amount, int slotNumber)
        {
            return AddItemToSlot(inv, item, amount, slotNumber, e: BroadcastEventType.AddItem);
        }

        /// <summary>
        /// Adds a certain amount of an item to an specific slot. If it fills slot entirily it will return the remaning items to be stored.
        /// </summary>
        /// <param name="inv">The inventory in witch the item will be placed</param>
        /// <param name="item">The item that will be stored in the inventory</param>
        /// <param name="amount">The amount of items to be stored</param>
        /// <param name="slotNumber">The index of the slot to store the items</param>
        /// <returns>If the slot gets full and there are still items to store it will return the number of items remaining</returns>
        public static int AddItem(this Inventory inv, Item item, int amount, int slotNumber, int durability)
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
        public static int AddItem(this Inventory inv, Item item, int amount, int slotNumber, int durability, Action callback)
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
        public static int AddItemToSlot(this Inventory inv, Item item, int amount, int slotNumber, int durability)
        {
            return AddItemToSlot(inv, item, amount, slotNumber, durability: durability, e: BroadcastEventType.AddItem);
        }

        /// <summary>
        /// Adds a certain amount of an item to an specific slot. If it fills slot entirily it will return the remaning items to be stored.
        /// </summary>
        /// <param name="inv">The inventory in witch the item will be placed</param>
        /// <param name="item">The item that will be stored in the inventory</param>
        /// <param name="amount">The amount of items to be stored</param>
        /// <param name="slotNumber">The index of the slot to store the items</param>
        /// <returns>If the slot gets full and there are still items to store it will return the number of items remaining</returns>
        public static int AddItemToSlot(this Inventory inv, Item item, int amount, int slotNumber, int durability, Action callback)
        {
            return AddItemToSlot(inv, item, amount, slotNumber, durability: durability, callback: callback, e: BroadcastEventType.AddItem);
        }

        /// <summary>
        /// Adds a certain amount of an item to an specific slot. If it fills slot entirily it will return the remaning items to be stored.
        /// </summary>
        /// <param name="inv">The inventory in witch the item will be placed</param>
        /// <param name="item">The item that will be stored in the inventory</param>
        /// <param name="amount">The amount of items to be stored</param>
        /// <param name="slotNumber">The index of the slot to store the items</param>
        /// <returns>If the slot gets full and there are still items to store it will return the number of items remaining</returns>
        public static int AddItemToSlot(this Inventory inv, Item item, int amount, int slotNumber, Action callback)
        {
            return AddItemToSlot(inv, item, amount, slotNumber, callback: callback, e: BroadcastEventType.AddItem);
        }

        #endregion



        [Serializable]
        protected enum MethodType
        {
            Add = 0,
            Remove = 1,
            Use = 2,
            Swap = 3,
            LocalSwap = 4,
            Initialize = 5,
            Craft = 6,
            Utility = 7,
            Drop = 8
        }

        private static bool AcceptsSlotProtection(Slot slot, MethodType methodType)
        {
            if (slot.interative.Equals(SlotProtection.Locked)) return false;
            switch (methodType)
            {
                case MethodType.Add:
                    return slot.interative.HasFlag(AddFlags);
                case MethodType.Remove:
                    return slot.interative.HasFlag(RemoveFlags);
                case MethodType.Swap:
                    return slot.interative.HasFlag(SwapFlags);
                case MethodType.LocalSwap:
                    goto case MethodType.Swap;
                case MethodType.Use:
                    return slot.interative.HasFlag(UseFlags);
                default:
                    return slot.interative.HasFlag(AllSlotFlags);
            }
        }

        private static bool AcceptsInventoryProtection(Inventory inv, MethodType methodType)
        {
            if (inv == null) return false;
            if (inv.interactiable.Equals(InventoryProtection.Locked)) return false;
            switch (methodType)
            {
                case MethodType.Add:
                    return inv.interactiable.HasFlag(AddInvFlags);
                case MethodType.Remove:
                    return inv.interactiable.HasFlag(RemoveInvFlags);
                case MethodType.Swap:
                    return inv.interactiable.HasFlag(SwapInvFlags);
                case MethodType.LocalSwap:
                    return inv.interactiable.HasFlag(LocalSwapInvFlags);
                case MethodType.Use:
                    return inv.interactiable.HasFlag(UseInvFlags);
                case MethodType.Drop:
                    return (inv.interactiable & DropInvFlags) != DropInvFlags;
                default:
                    return inv.interactiable.HasFlag(AllInventoryFlags);
            }
        }
    }

    [Serializable]
    public struct Slot
    {
        //Item properties
        public int amount;
        public Item item;

        public bool HasItem 
        {
            get
            {
                if(item != null)
                {
                    hasItem = true;
                    return true;
                }
                return false;
            }
        }
        [SerializeField] private bool hasItem;              // For Inspector

        public int durability
        {
            get { return _durability; }
            set
            {
                if (value > (item?.maxDurability ?? int.MaxValue)) throw new Exception("The value provided for durability is greter than the max durablity\nIf your intentions are of using a durability greter then the max one use the SetDurability function with op=true");
                //_totalDurability += value - _durability;
                _durability = value;
            }
        }
        [SerializeField] private int _durability;     
        
        //Slot properties
        public bool isProductSlot;
        public SlotProtection interative;
        public ItemGroup whitelist;

        public readonly static Slot nullSlot = new Slot(null, 0, false, InventoryController.AllSlotFlags, null, 0);

        public int GetDurability() => durability; 
        public bool IsDurabilityValid => _durability <= (item?.maxDurability ?? 0);

        #region Setters

        public static bool SetDurability(ref Slot slot, int value, bool op = false)
        {
            if (op)
            {
                ///slot._totalDurability += value - slot._durability;
                slot._durability = value;
                return true;
            }
            if (slot.item == null || !slot.HasItem || value > slot.item.maxDurability || !slot.item.hasDurability)
                return false;
            ///slot._totalDurability += value - slot._durability;
            slot._durability = value;
            return true;
        }

        public static Slot Set(ref Slot slot, Item _item, int _amount, bool _isProductSlot, SlotProtection _interactive, ItemGroup _whitelist, int _durability)
            => slot = new Slot(_item, _amount, _isProductSlot, _interactive, _whitelist, _durability);   

        public static Slot Set(ref Slot slot, Slot _slot)
            => slot = new Slot(_slot.item, _slot.amount, _slot.isProductSlot, _slot.interative, _slot.whitelist, _slot.durability);

        public static Slot Set(Item _item, int _amount, bool _isProductSlot, SlotProtection _interactive, ItemGroup _whitelist, int _durability)
            => new Slot(_item, _amount, _isProductSlot, _interactive, _whitelist, _durability);


        public static Slot SetSlotProperties(ref Slot slot, Slot _slot)
            => slot = new Slot(slot.item, slot.amount, _slot.isProductSlot, _slot.interative, _slot.whitelist, slot.durability);

        public static Slot SetSlotProperties(ref Slot slot, bool _isProductSlot, SlotProtection _interative, ItemGroup _whitelist) 
            => slot = new Slot(slot.item, slot.amount, _isProductSlot, _interative, _whitelist, slot.durability);

        public static Slot SetSlotProperties(Slot slot, Slot _slot)
            => new Slot(slot.item, slot.amount, _slot.isProductSlot, _slot.interative, _slot.whitelist, slot.durability);

        public static Slot SetSlotProperties(Slot slot, bool _isProductSlot, SlotProtection _interative, ItemGroup _whitelist)
            => new Slot(slot.item, slot.amount, _isProductSlot, _interative, _whitelist, slot.durability);


        public static Slot SetItemProperties(ref Slot slot, Slot _slot)
            => slot = new Slot(_slot.item, _slot.amount, slot.isProductSlot, slot.interative, slot.whitelist, _slot.durability);
        /// No Code Here. Line 2020 is cursed
        public static Slot SetItemProperties(ref Slot slot, Item _item, int _amount, int _durability)
            => slot = new Slot(_item, _amount, slot.isProductSlot, slot.interative, slot.whitelist, _durability);

        public static Slot SetItemProperties(Slot slot, Slot _slot)
            => new Slot(_slot.item, _slot.amount, slot.isProductSlot, slot.interative, slot.whitelist, _slot.durability);

        public static Slot SetItemProperties(Slot slot, Item _item, int _amount, int _durability)
            => new Slot(_item, _amount, slot.isProductSlot, slot.interative, slot.whitelist, _durability);

        /**public static Slot SetItemProperties(Slot slot, Item _item, int _amount, bool _hasItem, uint _durability, uint _totalDurability)
            => new Slot(_item, _amount, _hasItem, slot.isProductSlot, slot.interative, slot.whitelist, _durability, _totalDurability);
        public static Slot SetItemProperties(ref Slot slot, Item _item, int _amount, bool _hasItem, uint _durability, uint _totalDurability)
            => slot = new Slot(_item, _amount, _hasItem, slot.isProductSlot, slot.interative, slot.whitelist, _durability, _totalDurability);

        (uint)(_item.maxDurability * (_amount - 1) + _durability)
        **/


        #endregion

        public Slot(Slot slot, bool _isProductSlot, SlotProtection _interactive, ItemGroup _whitelist)
        {
            item = slot.item;
            amount = slot.amount;
            hasItem = slot.HasItem;
            isProductSlot = _isProductSlot;
            interative = _interactive;
            whitelist = _whitelist;
            _durability = slot.durability;
            durability = slot.durability;
        }

        public Slot(Slot slot, bool _isProductSlot, SlotProtection _interactive)
        {
            item = slot.item;
            amount = slot.amount;
            hasItem = slot.HasItem;
            isProductSlot = _isProductSlot;
            interative = _interactive;
            whitelist = null;
            _durability = slot.durability;
            ///_totalDurability = 0;
            durability = slot.durability;
        }

        public Slot(Item _item)
        {
            item = _item;
            amount = 1;
            hasItem = _item != null;
            isProductSlot = false;
            interative = InventoryController.AllSlotFlags;
            whitelist = null;
            _durability = 0;
            durability = 0;
        }

        public Slot(Item _item, int _amount)
        {
            item = _item;
            amount = _amount;
            hasItem = _item != null;
            isProductSlot = false;
            interative = InventoryController.AllSlotFlags;
            whitelist = null;
            _durability = 0;
            durability = 0;
        }
        
        public Slot(Item _item, int _amount, int _durability)
        {
            item = _item;
            amount = _amount;
            hasItem = _item != null;
            isProductSlot = false;
            interative = InventoryController.AllSlotFlags;
            whitelist = null;
            this._durability = _durability;
            durability = _durability;           
        }

        public Slot(Item _item, int _amount, bool _isProductSlot)
        {
            item = _item;
            amount = _amount;
            hasItem = _item != null;
            isProductSlot = _isProductSlot;
            interative = InventoryController.AllSlotFlags;
            whitelist = null;
            _durability = 0;
            durability = 0;
        }
        
        public Slot(Item _item, int _amount, bool _isProductSlot, int _durability)
        {
            item = _item;
            amount = _amount;
            hasItem = _item != null;
            isProductSlot = _isProductSlot;
            interative = InventoryController.AllSlotFlags;
            whitelist = null;
            this._durability = _durability;
   
            durability = _durability;
        }

        public Slot(Item _item, int _amount, bool _isProductSlot, SlotProtection _interactive)
        {
            item = _item;
            amount = _amount;
            hasItem = _item != null;
            isProductSlot = _isProductSlot;
            interative = _interactive;
            whitelist = null;
            _durability = 0;
            durability = 0;
        }

        public Slot(Item _item, int _amount, bool _isProductSlot, SlotProtection _interactive, ItemGroup _whitelist)
        {
            item = _item;
            amount = _amount;
            hasItem = _item != null;
            isProductSlot = _isProductSlot;
            interative = _interactive;
            whitelist = _whitelist;
            _durability = 0;
            durability = 0;
        }
        
        public Slot(Item _item, int _amount, bool _isProductSlot, SlotProtection _interactive, ItemGroup _whitelist, int _durability)
        {
            item = _item;
            amount = _amount;
            hasItem = _item != null;
            isProductSlot = _isProductSlot;
            interative = _interactive;
            whitelist = _whitelist;
            this._durability = _durability;
            durability = _durability;
        }

        /**public Slot(Item _item, int _amount, bool _hasItem, bool _isProductSlot, SlotProtection _interactive, ItemGroup _whitelist, uint _durability, uint _totalDurability)
        {
            item = _item;
            amount = _amount;
            hasItem = _item != null;
            isProductSlot = _isProductSlot;
            interative = _interactive;
            whitelist = _whitelist;
            this._durability = _durability;
            ///this._totalDurability = 0;
            ///totalDurability = _totalDurability;
        }**/
        
        public static bool operator !(Slot s) => !s.HasItem;
        public static bool operator true(Slot s) => s.HasItem;
        public static bool operator false(Slot s) => !s.HasItem;
        public static bool operator >=(Slot s, Slot s2) => s.amount >= s2.amount;
        public static bool operator <=(Slot s, Slot s2) => s.amount <= s2.amount;
        public static bool operator <(Slot s, Slot s2) => s.amount < s2.amount;
        public static bool operator >(Slot s, Slot s2) => s.amount > s2.amount;
    } 

    [Serializable]
    public struct InventoryData
    {
        public Inventory[] inventories;

        public Inventory this[int i] => inventories[i];
    }

    [Serializable]
    public class CheckItemData
    {
        public readonly Inventory inventory;
        public readonly int[] slotsChecked;
        public readonly int[] slotsWithItem;
        public readonly int amount;
        public readonly bool hasItem;
        public readonly bool mustBeOnSameSlot;
        public readonly Item checkedItem;

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

        public static bool operator true(CheckItemData c) => c.hasItem;
        public static bool operator false(CheckItemData c) => !c.hasItem;
        public static bool operator !(CheckItemData c) => !c.hasItem;
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

        public static bool operator true(CraftItemData c) => c.items != null || c.amounts != null;
        public static bool operator false(CraftItemData c) => c.items == null || c.amounts == null;
    } 

    [Serializable, Flags]
    public enum InventoryProtection
    {
        Locked = 0,                                     //0b00000000
        InventoryToInventory = 1,                       //0b00000001
        SlotToSlot = 2,                                 //0b00000010
        Add = 4,                                        //0b00000100
        Remove = 8,                                     //0b00001000
        Use = 16,                                       //0b00010000
        Drop = 32                                       //0b0010000
    }

    [Serializable, Flags]
    public enum SlotProtection : short
    {
        Locked = 0,
        Add = 1,
        Remove = 2,
        Swap = 4,
        Use = 8
    }

}
