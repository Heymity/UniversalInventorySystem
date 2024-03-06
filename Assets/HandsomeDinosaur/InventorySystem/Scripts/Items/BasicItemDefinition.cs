using UnityEngine;

namespace MolecularLib.InventorySystem.Items
{
    [CreateAssetMenu(menuName = "Inventory System/New Basic Item", order = 1, fileName = "UnnamedItem")]
    public class BasicItemDefinition : AbstractItemDefinition<BasicItemData>
    {
    }
}