namespace MolecularLib.InventorySystem.Items
{
    public interface IItemData 
    {
        public string DisplayName { get; }
        
        public IItemData Clone();
    }
}