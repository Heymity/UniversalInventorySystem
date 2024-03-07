namespace MolecularLib.InventorySystem.Items
{
    public interface IItemStack
    {
        public IItem ItemModel { get; }
        public IItemData Data { get; }
        public bool Merge(ref IItemStack other);
        public bool Merge(ref IItemStack other, int amount);
        public int Amount { get; }
        
        public bool IsEmpty();
        
        public bool Add(int amount);
        public bool CanAdd(int amount);
        
        public bool Remove(int amount);
        public bool CanRemove(int amount);
        
        public int MaxStackSize();
        public int MinStackSize();
    }
}