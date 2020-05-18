using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript2 : MonoBehaviour
{
    Inventory inventory;
    public Item testItem;
    public int slotAmount;
    public InventoryUI invUI;

    private void Start()
    {
        inventory = new Inventory(slotAmount, true, IteractiableTypes.Any, true);
        inventory.InitializeInventory();
        invUI.SetInventory(inventory);
    }
   
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            inventory.AddItem(testItem, 12);
    }

}
