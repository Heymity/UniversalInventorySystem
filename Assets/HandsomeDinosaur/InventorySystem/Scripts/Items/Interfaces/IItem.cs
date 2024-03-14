namespace MolecularLib.InventorySystem.Items
{
    public interface IItem
    {
        string Id { get; set; }
        IItemData ModelItemData { get; }
    }
}