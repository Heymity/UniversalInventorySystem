using MolecularLib.InventorySystem.Items;

namespace MolecularLib.InventorySystem.Inventory
{
    public class BasicSlot : ISlot<ItemStack>
    {
        public int Id { get; }
        public ItemStack Stack { get; }
    }
}