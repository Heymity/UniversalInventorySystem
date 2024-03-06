using MolecularLib.InventorySystem.Items;

namespace MolecularLib.InventorySystem.Inventory
{
    public class BasicInventory : IInventory<BasicSlot, ItemStack>
    {
        public bool AddItem(IItem item)
        {
            throw new System.NotImplementedException();
        }

        public bool AddItem(IItemStack item)
        {
            throw new System.NotImplementedException();
        }

        public bool AddItem(BasicSlot slot, IItemStack item)
        {
            throw new System.NotImplementedException();
        }

        public bool AddItem(BasicSlot slot, IItem item)
        {
            throw new System.NotImplementedException();
        }

        public bool AddItem(int slotId, IItemStack item)
        {
            throw new System.NotImplementedException();
        }

        public bool AddItem(int slotId, IItem item)
        {
            throw new System.NotImplementedException();
        }
        
        
        public bool RemoveItem(IItem item)
        {
            throw new System.NotImplementedException();
        }

        public bool RemoveItem(IItemStack item)
        {
            throw new System.NotImplementedException();
        }

        public bool RemoveItem(BasicSlot slot)
        {
            throw new System.NotImplementedException();
        }
    }
}