using MolecularLib.Helpers;

namespace MolecularLib.InventorySystem.Items
{
    public interface IItemData 
    {
        public string DisplayName { get; }
        
        public Optional<int> MaxStackSize { get; }
        public Optional<int> MinStackSize { get; }
        
        public IItemData Clone();
        
        public bool CanCombine(in IItemData other);
        
        public bool Combine(IItemData other);
    }
}