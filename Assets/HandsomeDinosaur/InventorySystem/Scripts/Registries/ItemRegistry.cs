using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MolecularLib.InventorySystem.Items.Interfaces;
using UnityEngine;

namespace MolecularLib.InventorySystem.Items
{
    [System.Serializable]
    public class ItemRegistry : IItemRegistry
    {
        [SerializeField] private string registerId;
        [SerializeField] private SerializableDictionary<string, Item> items;

        public Item this[string id] => items[id];
        
        public string RegisterId => registerId;

        public IEnumerable<Item> Items => items.Values.AsEnumerable();

        public Item GetItemOfId(string id) => items[id];

        public bool TryGetItemOfId(string id, out Item item) => items.TryGetValue(id, out item);

        public bool RegisterItem(string id, Item item)
        {
            if (items.ContainsKey(id)) return false;
            items.Add(id, item);

            return true;
        }

        public IEnumerator<IItem> GetEnumerator() => items.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
