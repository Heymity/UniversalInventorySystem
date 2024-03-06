using System;
using MolecularLib.PolymorphismSupport;
using UnityEditor;
using UnityEngine;

namespace MolecularLib.InventorySystem.Items
{
    [Serializable]
    public abstract class AbstractItemDefinition : ScriptableObject, IItem
    {
        [SerializeField, HideInInspector] private string id;

        public virtual string Id
        {
            get => id;
            internal set => id = value;
        }
        public abstract IItemData ModelItemData { get; }
    }

    [Serializable]
    public abstract class AbstractItemDefinition<T> : AbstractItemDefinition where T : IItemData
    {
        [SerializeField] private T itemData;
        
        public override IItemData ModelItemData => itemData;
    }

}