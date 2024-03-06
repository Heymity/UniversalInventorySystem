using MolecularLib.InventorySystem.Items;

namespace MolecularLib.InventorySystem
{
    public interface IInventory<in TSlot, TStack> where TSlot : ISlot<TStack> where TStack : ItemStack
    {
        public bool AddItem(IItem item);
        public bool AddItem(IItemStack item);
        public bool AddItem(TSlot slot, IItemStack item);
        public bool AddItem(TSlot slot, IItem item);
        public bool AddItem(int slotId, IItemStack item);
        public bool AddItem(int slotId, IItem item);
        
        public bool RemoveItem(IItem item);
        public bool RemoveItem(IItemStack item);
        public bool RemoveItem(TSlot slot);
    }
}