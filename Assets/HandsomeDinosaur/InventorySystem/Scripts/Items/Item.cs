using System;
using MolecularLib.PolymorphismSupport;
using UnityEngine;

namespace MolecularLib.InventorySystem.Items
{
    [Serializable]
    public sealed class Item : IItem
    {
        [SerializeField] private string id;

        [SerializeField] private PolymorphicVariable<IItemData> itemData;

        public string Id => id;
        public IItemData ModelItemData => itemData.Value;
        
    }
}