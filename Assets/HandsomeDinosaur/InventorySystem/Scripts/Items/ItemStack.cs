using System;
using UnityEngine;

namespace MolecularLib.InventorySystem.Items
{
    [Serializable]
    public class ItemStack : IItemStack
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

        public bool Merge(ref IItemStack stack, int amount)
        {
            if (stack is null) return false;
            if (!Data.Combine(stack.Data)) return false;

            if (!CanAdd(amount) || !CanRemove(amount)) return false;
            
            Add(amount);
            stack.Remove(amount);

            return true;
        }
        
        public bool Merge(ref IItemStack stack)
        {
            if (stack is null) return false;
            if (!Data.Combine(stack.Data)) return false;
            
            var toAdd = stack.Amount + Amount > MaxStackSize() ? MaxStackSize() - Amount : stack.Amount;

            //TODO Account for MinStackSize when removing.

            return Merge(ref stack, toAdd);
        }

        public bool IsEmpty()
        {
            return Amount > 0 || Data == null || ItemModel == null;
        }

        public bool CanAdd(int amount) => Amount + amount <= MaxStackSize();
        public bool CanRemove(int amount) => Amount - amount >= MinStackSize();
        
        public bool Add(int amount)
        {
            if (CanAdd(amount)) return false;
            Amount += amount;
            return true;
        }

        public bool Remove(int amount)
        {
            if (CanRemove(amount)) return false;
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

        public int MinStackSize()
        {
            if (Data.MinStackSize.UseValue)
                return Data.MinStackSize;

            return 0;
        }
    }
}