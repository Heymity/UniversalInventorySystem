using System.Diagnostics.Contracts;

namespace MolecularLib.InventorySystem.Items
{
    public interface IItemStack
    {
        public IItem ItemModel { get; }
        public IItemData Data { get; }
        
        [Pure]
        public bool CanMerge(IItemStack other);
        
        public (bool succes, IItemStack other) Merge(IItemStack other);
        public bool IsEmpty();
    }
    
    public interface IItemStack<T> : IItemStack
    {
        public T Amount { get; }
        
        public bool Add(T amount);
        
        public bool Remove(T amount);
        
        public T MaxStackSize();

    }
}