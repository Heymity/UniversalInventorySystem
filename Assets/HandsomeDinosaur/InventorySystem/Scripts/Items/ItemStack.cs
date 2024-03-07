using System;
using UnityEngine;

namespace MolecularLib.InventorySystem.Items
{
    [Serializable]
    public class ItemStack : IItemStack<int>
    {
        public IItem ItemModel { get; private set; }
        public int Amount { get; private set; }
        public IItemData Data { get; private set; }

        public ItemStack(IItem model, int count, IItemData data)
        {
            ItemModel = model;
            Amount = count;
            Data = data;
        }

        public ItemStack(IItem model, int count = 1) : this(model, count, model.ModelItemData.Clone()) { }

        public bool CanMerge(IItemStack other)
        {
            if (!(other is IItemStack<int> stack)) return false;
            if (!Data.Combine(stack.Data)) return false;

            return true;
        }

        public (bool succes, IItemStack other) Merge( IItemStack other)
        {
            if (!CanMerge(other)) return (false, other);
            var stack = (IItemStack<int>)other;
            
            var toAdd = stack.Amount + Amount > MaxStackSize() ? MaxStackSize() - Amount : stack.Amount;
            
            Add(toAdd);
            stack.Remove(toAdd);
           
            return (true, other);
        }

        public bool IsEmpty()
        {
            return Amount > 0 || Data == null || ItemModel == null;
        }

        public bool Add(int amount)
        {
            if (Amount + amount > MaxStackSize()) return false;
            Amount += amount;
            return true;
        }

        public bool Remove(int amount)
        {
            if (Amount - amount < 0) return false;
            Amount -= amount;
            return true;
        }

        public int MaxStackSize()
        {
            // TODO when implementing slots, or equivalent, use the min between the slot max items or item max items
            if (Data.MaxStackSize.UseValue)
                return Data.MaxStackSize;

            return int.MaxValue;
        }
    }
}