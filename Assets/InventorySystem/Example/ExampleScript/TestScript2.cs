using UnityEngine;

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
            inventory.AddItem(InventoryEventsItemsHandler.current.GetItem(0, 0), 12);
    }
}
