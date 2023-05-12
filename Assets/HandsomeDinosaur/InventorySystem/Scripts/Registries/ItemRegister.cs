using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MolecularLib.InventorySystem.Items
{
    [CreateAssetMenu(menuName = "Inventory System/New ItemRegister", order = 1, fileName = "UnnamedItemRegister")]
    public class ItemRegister : ScriptableObject
    {
        [SerializeField] private string registerId;
        [SerializeField] private SerializableDictionary<string, Item> items;

        public string RegisterId => registerId;

        public IEnumerable<Item> Items => items.Values.AsEnumerable();

        public Item GetItemOfId(string id) => items[id];

        public bool TryGetItemOfId(string id, out Item item) => items.TryGetValue(id, out item);
    }
}
