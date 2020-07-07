using UnityEngine;
using UniversalInventorySystem;

[RequireComponent(typeof(InventoryUI))]
public class TestScript : MonoBehaviour
{
    Inventory inventory;
    InventoryUI invUI;

    private void Start()
    {
        invUI = GetComponent<InventoryUI>();
        inventory = invUI.GetInventory();
    }
   
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            inventory.AddItem(InventoryHandler.current.GetItem(0, 0), 12);
    }
}
