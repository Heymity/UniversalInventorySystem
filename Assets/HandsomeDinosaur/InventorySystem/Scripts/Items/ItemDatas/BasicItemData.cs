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
        
        public IItem ItemModel { get; set; }
        
        public Optional<int> MaxStackSize
        {
            get => _maxStackSize;
            set => _maxStackSize = value;
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
                ItemModel = this.ItemModel,
                ItemIcon = this.ItemIcon,
                DisplayName = this.DisplayName
            };
        }
        
        public bool CanCombine(in IItemData other)
        {
            return other is BasicItemData data && 
                   data.ItemModel.Id == this.ItemModel.Id &&
                   other.DisplayName == this.DisplayName && 
                   data.ItemIcon == this.ItemIcon;
        }

        public bool Combine(IItemData other)
        {
            return CanCombine(other);
        }
    }
}