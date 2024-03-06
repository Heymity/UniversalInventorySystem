using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MolecularLib.InventorySystem.Items.Interfaces;
using UnityEngine;

namespace MolecularLib.InventorySystem.Items
{
    [CreateAssetMenu(menuName = "Inventory System/New ItemRegister", order = 1, fileName = "UnnamedItemRegister")]
    public class ItemRegistrySO : ScriptableObject
    {
        //TODO I dont quite like this approach, it should be two separated things, the ItemRegistry and the SO, but the SO should have its own editor or something like that, not relying on the drawer for the ItemRegistry.
        
        [SerializeField] private ItemRegistry registry;

        public ItemRegistry Registry => registry;
    }
}