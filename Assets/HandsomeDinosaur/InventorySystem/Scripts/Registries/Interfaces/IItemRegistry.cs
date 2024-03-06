using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MolecularLib.InventorySystem.Items.Interfaces
{
    public interface IItemRegistry : IEnumerable<IItem>
    {
        Item this[string id] { get; }

        string RegisterId { get; }

        IEnumerable<Item> Items { get; }

        Item GetItemOfId(string id);

        bool TryGetItemOfId(string id, out Item item);

        bool RegisterItem(string id, Item item);
    }
}