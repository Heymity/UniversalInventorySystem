using MolecularLib.InventorySystem.Items;

namespace MolecularLib.InventorySystem.Inventory
{
    public class BasicInventory : IInventory<BasicSlot, ItemStack>
    {
        public bool AddItem(IItem item)
        {
            throw new System.NotImplementedException();
        }
<<<<<<< HEAD
        //TODO Change the return type  of all the add methods to return something like (bool, ItemStack, FailReason)
        public bool AddItem(IItem item) => AddItem(new ItemStack(item));

        public bool AddItem(ItemStack item)
        {
            BasicSlot slot = null;
            foreach (var s in Slots)
            {
                if (s.Stack.CanMerge(item))
                {
                    slot = s; 
                    break;
                }
                if (slot is null && s.IsEmpty()) slot = s;
            }
            if (slot is null) return false;
            
            slot.Stack.Merge(item);
            
            return true;
=======

        public bool AddItem(ItemStack item)
        {
            throw new System.NotImplementedException();
>>>>>>> f25b7a835c387b966cbc6e2e8bbdc46990081814
        }

        public bool AddItem(BasicSlot slot, ItemStack item)
        {
            if (!slot.Stack.CanMerge(item)) return false;
            slot.Stack.Merge(item);
            return true;
        }

        public bool AddItem(BasicSlot slot, IItem item)
        {
            throw new System.NotImplementedException();
        }

        public bool AddItem(int slotId, ItemStack item)
        {
            throw new System.NotImplementedException();
        }

        public bool AddItem(int slotId, IItem item)
        {
            throw new System.NotImplementedException();
        }

        public bool RemoveItem(IItem item)
        {
            Slots.First(s => s.Stack.ItemModel.ModelItemData == item.ModelItemData).Stack = null;
            return true;
        }

        public bool RemoveItem(ItemStack item)
        {
            throw new System.NotImplementedException();
        }

        public bool RemoveItem(BasicSlot slot)
        {
            throw new System.NotImplementedException();
        }
    }
}