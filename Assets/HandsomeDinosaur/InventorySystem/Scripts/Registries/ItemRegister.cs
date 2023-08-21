using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MolecularLib.InventorySystem.Items
{
    [CreateAssetMenu(menuName = "Inventory System/New ItemRegister", order = 1, fileName = "UnnamedItemRegister")]
    public class ItemRegister : ScriptableObject
    {
        [SerializeField] private string registerId;
        [SerializeField] private SerializableDictionary<string, AbstractItemDefinition> items;

        public AbstractItemDefinition this[string id] => items[id];
        
        public string RegisterId => registerId;

        public IEnumerable<AbstractItemDefinition> Items => items.Values.AsEnumerable();

        public AbstractItemDefinition GetItemOfId(string id) => items[id];

        public bool TryGetItemOfId(string id, out AbstractItemDefinition item) => items.TryGetValue(id, out item);

        public bool RegisterItem(string id, AbstractItemDefinition item)
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
                itemDef.Id = itemDefinitionKp.Key;
            }
        }
    }
}
