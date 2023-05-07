using System;
using MolecularLib.PolymorphismSupport;
using UnityEngine;

namespace MolecularLib.InventorySystem
{
    [Serializable]
    public sealed class Item
    {
        [SerializeField] private string id;

        [SerializeField] private PolymorphicVariable<ItemData> itemData;

        public string Id => id;
        public Sprite ItemIcon => itemData.Value.ItemIcon;
        public string DisplayName => itemData.Value.DisplayName;
        public ItemData ModelItemData => itemData.Value;
    }
}