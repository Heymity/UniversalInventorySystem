using MolecularLib.InventorySystem.Items;

namespace MolecularLib.InventorySystem.Inventory
{
    public class BasicSlot : ISlot<ItemStack>
    {
        public int Id { get; }
        public ItemStack Stack { get; }
        
        
        // TODO implement a way to set this values from unity and from code. Some sort of SlotConfig (per inventory and not perhaps)
        public Optional<int> MaxSlotCapacity()
        {
            return int.MaxValue;
        }

        public Optional<int> MinSlotCapacity()
        {
            return 0;
        }

        public bool IsEmpty() => Stack is null || Stack.IsEmpty();
    }
}