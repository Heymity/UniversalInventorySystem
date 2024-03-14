namespace MolecularLib.InventorySystem.Items
{
    public class BasicItem : IItem
    {
        private BasicItemData data;
        
        public string Id { get; set; }
        public IItemData ModelItemData => data;
    }
}