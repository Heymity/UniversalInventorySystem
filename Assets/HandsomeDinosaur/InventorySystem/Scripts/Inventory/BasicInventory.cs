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

        public bool AddItem(IItem item) => AddItem(new ItemStack(item));

        public bool AddItem(ItemStack item)
        {
            Slots.First(s => s.IsEmpty()).Stack = item;
            return true;
        }

        public bool AddItem(BasicSlot slot, IItem item) => AddItem(slot, new ItemStack(item));
        
        public bool AddItem(BasicSlot slot, ItemStack item)
        {
            throw new System.NotImplementedException();
        }

        public bool RemoveItem(IItem item)
        {
            throw new System.NotImplementedException();
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