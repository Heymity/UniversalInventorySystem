using MolecularLib.InventorySystem.Items;

namespace MolecularLib.InventorySystem
{
<<<<<<< HEAD
    public interface IInventory<in TSlot, in TStack> where TSlot : ISlot<TStack> where TStack : IItemStack
=======
    public interface IInventory<in TSlot, TStack> where TSlot : ISlot<TStack> where TStack : IItemStack
>>>>>>> f25b7a835c387b966cbc6e2e8bbdc46990081814
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