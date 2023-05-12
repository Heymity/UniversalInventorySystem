using System;

namespace MolecularLib.InventorySystem.Items
{
    [Serializable]
    public class ItemStack : IItemStack<int>
    {
        public IItem ItemModel { get; private set; }
        public int Amount { get; private set; }
        public IItemData Data { get; private set; }

        public ItemStack(Item model, int count, IItemData data) : this(model, count)
        {
            Data = data;
        }

        public ItemStack(Item model, int count = 1)
        {
            ItemModel = model;
            Amount = count;
            Data = model.ModelItemData.Clone();
        }

        public bool Merge(ref IItemStack other)
        {
            if (!(other is IItemStack<int> stack)) return false;
            if (stack.ItemModel != ItemModel) return false;
            // TODO this needs to change because not always will two data fully merge, maybe the combine will give two datas.
            var stackData = stack.Data;
            if (!stack.Data.Combine(ref stackData)) return false;
            
            return true;
        }

    }
}