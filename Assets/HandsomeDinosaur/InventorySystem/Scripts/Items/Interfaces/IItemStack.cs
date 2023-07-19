namespace MolecularLib.InventorySystem.Items
{
    public interface IItemStack
    {
        public IItem ItemModel { get; }
        public IItemData Data { get; }
    }
    
    public interface IItemStack<T> : IItemStack
    {
        public T Amount { get; }
        
        public bool Merge(ref IItemStack other);

        public bool Add(T amount);
        
        public bool Remove(T amount);
        
        public T MaxStackSize();

    }
}