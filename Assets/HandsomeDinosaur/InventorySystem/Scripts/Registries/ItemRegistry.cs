using System.Collections;
using System;
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
        [SerializeField] private SerializableDictionary<string, IItem> items;

        public IItem this[string id] => items[id];
        
        public string RegisterId => registerId;

        public IEnumerable<IItem> Items => items.Values.AsEnumerable();

        public IItem GetItemOfId(string id) => items[id];

        public bool TryGetItemOfId(string id, out IItem item) => items.TryGetValue(id, out item);

        public bool RegisterItem(string id, IItem item)
        {
            if (items.ContainsKey(id)) return false;
            items.Add(id, item);

            return true;
        }

        public void OnValidate()
        {
            foreach (var itemDefinitionKp in items)
            {
                var itemDef = itemDefinitionKp.Value;
                if (itemDef == null) continue;
                //TODO TODO TODO FIX THIS MERGE //itemDef.Id = itemDefinitionKp.Key;
            }
        }

        public IEnumerator<IItem> GetEnumerator() => items.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
