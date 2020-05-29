using UnityEngine;
using UniversalInventorySystem;

public class TestScript2 : MonoBehaviour
{
    Inventory inventory;
    public InventoryUI invUI;

    private void Start()
    {
        inventory = invUI.GetInventory();
    }
   
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            inventory.AddItem(InventoryHandler.current.GetItem(0, 0), 12);
    }
}
