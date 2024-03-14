using MolecularLib.InventorySystem.Items;

namespace MolecularLib.InventorySystem
{
    public interface IInventory<in TSlot, in TStack> where TSlot : ISlot<TStack> where TStack : IItemStack
    {
        public bool AddItem(IItem item);
        public bool AddItem(TStack item);
        public bool AddItem(TSlot slot, TStack item);
        public bool AddItem(TSlot slot, IItem item);
        public bool AddItem(int slotId, TStack item);
        public bool AddItem(int slotId, IItem item);
        
        public bool RemoveItem(IItem item);
        public bool RemoveItem(TStack item);
        public bool RemoveItem(TSlot slot);
    }
}