using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MolecularLib.InventorySystem.Items.Interfaces
{
    public interface IItemRegistry : IEnumerable<IItem>
    {
        IItem this[string id] { get; }

        string RegisterId { get; }

        IEnumerable<IItem> Items { get; }

        IItem GetItemOfId(string id);

        bool TryGetItemOfId(string id, out IItem item);

        bool RegisterItem(string id, IItem item);
    }
}