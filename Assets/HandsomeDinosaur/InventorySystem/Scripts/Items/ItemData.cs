using System;
using UnityEngine;

namespace MolecularLib.InventorySystem
{
    [Serializable]
    public class ItemData : IItemData
    {
        public Sprite ItemIcon;
        public string DisplayName;

        public IItemData Clone()
        {
            return new ItemData
            {
                ItemIcon = this.ItemIcon,
                DisplayName = this.DisplayName
            };
        }
    }
}