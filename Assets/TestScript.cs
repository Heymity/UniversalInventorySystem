using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class TestScript : MonoBehaviour
{
    Inventory inventory;
    public Item testItem;
    public int slotAmount;
    public InventoryUI invUI;
    public int amountToTake;

    private void Start()
    {
        inventory = new Inventory(slotAmount, true, IteractiableTypes.Any, true);
        inventory.InitializeInventory();
        invUI.SetInventory(inventory);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            inventory.AddItem(testItem, 2);
        if (Input.GetKeyDown(KeyCode.S))
            inventory.SwapItemsInCertainAmountInSlots(0, 10, amountToTake);
    }
}
