using System.Collections.Generic;
using System.Linq;
using MolecularLib.InventorySystem.Items;

namespace MolecularLib.InventorySystem.Inventory
{
    public class BasicInventory : IInventory<BasicSlot, ItemStack>
    {
        public List<BasicSlot> Slots { get; }

        public BasicInventory(int slotCount)
        {
            Slots = new List<BasicSlot>();
            for (var i = 0; i < slotCount; i++)
            {
                Slots.Add(new BasicSlot());
            }
        }
        //TODO Change the return type  of all the add methods to return something like (bool, ItemStack, FailReason)
        public bool AddItem(IItem item) => AddItem(new ItemStack(item));

        public bool AddItem(ItemStack item)
        {
            BasicSlot slot = null;
            foreach (var s in Slots)
            {
                if (s.Stack.CanMerge(item))
                {
                    slot = s; 
                    break;
                }
                if (slot is null && s.IsEmpty()) slot = s;
            }
            if (slot is null) return false;
            
            slot.Stack.Merge(item);
            
            return true;
        }

        public bool AddItem(BasicSlot slot, IItem item) => AddItem(slot, new ItemStack(item));
        
        public bool AddItem(BasicSlot slot, ItemStack item)
        {
            if (!slot.Stack.CanMerge(item)) return false;
            slot.Stack.Merge(item);
            return true;
        }

        public bool RemoveItem(IItem item)
        {
            Slots.First(s => s.Stack.ItemModel.ModelItemData == item.ModelItemData).Stack = null;
            return true;
        }

        public bool RemoveItem(ItemStack item)
        {
            throw new System.NotImplementedException();
        }

        public bool RemoveItem(BasicSlot slot)
        {
            throw new System.NotImplementedException();
        }
    }
}