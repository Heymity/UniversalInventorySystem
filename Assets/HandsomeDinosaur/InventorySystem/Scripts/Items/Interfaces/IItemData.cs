using MolecularLib.Helpers;

namespace MolecularLib.InventorySystem.Items
{
    public interface IItemData 
    {
        public IItem ItemModel { get; }
        
        public string DisplayName { get; }
        
        public Optional<int> MaxStackSize { get; }
        
        public IItemData Clone();
        
        public bool CanCombine(in IItemData other);
        
        public bool Combine(IItemData other);
    }
}