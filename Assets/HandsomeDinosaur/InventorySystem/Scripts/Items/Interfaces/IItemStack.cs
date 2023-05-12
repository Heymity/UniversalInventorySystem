namespace MolecularLib.InventorySystem.Items
{
    public interface IItemStack
    {
        public IItem ItemModel { get; }
        public IItemData Data { get; }
    }
    
    public interface IItemStack<out T> : IItemStack
    {
        public T Amount { get; }
        
        public bool Merge(ref IItemStack other);
        
    }
}