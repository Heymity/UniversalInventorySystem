using System;
using MolecularLib.Helpers;
using UnityEngine;
using UnityEngine.Serialization;

namespace MolecularLib.InventorySystem.Items
{
    [Serializable]
    public class BasicItemData : IItemData
    {
        [SerializeField] private string displayName;
        [SerializeField] private Sprite itemIcon;
        [SerializeField] private Optional<int> maxStackSize;
        
        public IItem ItemModel { get; set; }
        
        public Optional<int> MaxStackSize
        {
            get => maxStackSize;
            set => maxStackSize = value;
        }
        
        public Sprite ItemIcon
        {
            get => itemIcon;
            set => itemIcon = value;
        }

        public string DisplayName
        {
            get => displayName;
            set => displayName = value;
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