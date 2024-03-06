using System;
using MolecularLib.Helpers;
using UnityEngine;

namespace MolecularLib.InventorySystem.Items
{
    [Serializable]
    public class BasicItemData : IItemData
    {
        private string _displayName;
        private Sprite _itemIcon;
        private Optional<int> _maxStackSize;
        private Optional<int> _minStackSize;

        public Optional<int> MaxStackSize
        {
            get => _maxStackSize;
            set => _maxStackSize = value;
        }
        
        public Optional<int> MinStackSize
        {
            get => _minStackSize;
            set => _minStackSize = value;
        }
        
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
            return other is BasicItemData data && 
                   other.DisplayName == this.DisplayName && 
                   data.ItemIcon == this.ItemIcon;
        }

        public bool Combine(IItemData other)
        {
            return CanCombine(other);
        }
    }
}