namespace MolecularLib.InventorySystem.Items
{
    public interface IItem
    {
        public string Id { get; }
        public IItemData ModelItemData { get; }
    }
}