using System;
using UnityEngine;

namespace MolecularLib.InventorySystem.Items
{
    [Serializable]
    public class BasicItemData : IItemData
    {
        private string _displayName;
        private Sprite _itemIcon;

        public Sprite ItemIcon
        {
            get => _itemIcon;
            set => _itemIcon = value;
        }

        public string DisplayName
        {
            get => _displayName;
            set => _displayName = value;
        }

        public IItemData Clone()
        {
            return new BasicItemData
            {
                ItemIcon = this.ItemIcon,
                DisplayName = this.DisplayName
            };
        }

        public bool CanCombine(in IItemData other)
        {
            return other.GetType() == typeof(BasicItemData) && other.DisplayName == this.DisplayName && ((BasicItemData)other).ItemIcon == this.ItemIcon;
        }

        public bool Combine(ref IItemData other)
        {
            if (!CanCombine(other)) return false;

            return true;
        }
    }
}