using MolecularLib.InventorySystem.Items;

namespace MolecularLib.InventorySystem
{
    public interface IInventory
    {
        public bool AddItem(IItem item);
        public bool AddItem(IItemStack item);
    }
}