using System.Diagnostics.Contracts;

namespace MolecularLib.InventorySystem.Items
{
    public interface IItemStack
    {
        public IItem ItemModel { get; }
        public IItemData Data { get; }
<<<<<<< HEAD
        
        [Pure]
        public bool CanMerge(IItemStack other);
        
        public (bool succes, IItemStack other) Merge(IItemStack other);
=======
        public bool Merge(ref IItemStack other);
        public bool Merge(ref IItemStack other, int amount);
        public int Amount { get; }
        
>>>>>>> f25b7a835c387b966cbc6e2e8bbdc46990081814
        public bool IsEmpty();
        
        public bool Add(int amount);
        public bool CanAdd(int amount);
        
        public bool Remove(int amount);
        public bool CanRemove(int amount);
        
        public int MaxStackSize();
        public int MinStackSize();
    }
}