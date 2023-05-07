using System.Collections.Generic;
using MolecularLib.PolymorphismSupport;
using UnityEngine;

namespace MolecularLib.InventorySystem
{
    [CreateAssetMenu(menuName = "Molecular Lib/New ItemRegister", order = 1, fileName = "UnnamedItemRegister")]
    public class ItemRegister : ScriptableObject
    {
        [SerializeField] private string _registerId;
        [SerializeField] private List<Item> _items;

        public PolymorphicVariable<ItemData> a;

        public string RegisterId => _registerId;
    }
}
