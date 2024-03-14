using UnityEngine;

namespace MolecularLib.InventorySystem.Items.SODefinitions
{
    public class SOItem<TItem> : ScriptableObject, IItem where TItem : IItem
    {
        public string Id
        {
            get => item.Id;
            set => item.Id = value;
        }

        public IItemData ModelItemData => item.ModelItemData;

        [SerializeField] private TItem item;
    }
}