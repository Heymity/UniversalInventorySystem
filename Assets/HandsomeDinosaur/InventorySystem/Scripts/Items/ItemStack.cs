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

        public ItemStack(Item model, int count, IItemData data) : this(model, count)
        {
            Data = data.Clone();
        }

        public ItemStack(Item model, int count = 1)
        {
            ItemModel = model;
            Amount = count;
        }
    }
}