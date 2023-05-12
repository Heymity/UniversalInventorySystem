namespace MolecularLib.InventorySystem.Items
{
    public interface IItemData 
    {
        public string DisplayName { get; }
        
        public IItemData Clone();
        
        public bool CanCombine(in IItemData other);
        
        public bool Combine(ref IItemData other);
    }
}