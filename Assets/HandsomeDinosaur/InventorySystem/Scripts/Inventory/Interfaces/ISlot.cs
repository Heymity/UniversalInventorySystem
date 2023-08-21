using MolecularLib.InventorySystem.Items;

namespace MolecularLib.InventorySystem
{
    public interface ISlot<out TStack> where TStack : IItemStack
    {
        public int Id { get; }
        
        public TStack Stack { get; }
        
        public bool IsEmpty();
        
    }
}