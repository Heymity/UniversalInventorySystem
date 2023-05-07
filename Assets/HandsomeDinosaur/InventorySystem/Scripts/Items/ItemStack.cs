using System;
using UnityEngine;

namespace MolecularLib.InventorySystem
{
    [Serializable]
    public class ItemStack
    {
        public Item ItemModel { get; private set; }
        public int Amount { get; private set; }
        public ItemData Data { get; private set; }
    }
}