using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ExampleScript : MonoBehaviour
{
    Inventory inventory;
    public GameObject inventory2;
    public Item testItem;
    public int slotAmount;
    public InventoryUI invUI;
    public int amountToTake;

    private void Start()
    {
        inventory = new Inventory(slotAmount, true, IteractiableTypes.Any, true);
        inventory.InitializeInventory();
        invUI.SetInventory(inventory);
        InventoryEventHandler invEvent = InventoryEventHandler.current;
        invEvent.OnAddItem += OnAddItem;
        invEvent.OnRemoveItem += OnRemoveItem;
        invEvent.OnDropItem += OnDropItem;
    }

    private void OnDropItem(object sender, InventoryEventHandler.DropItemEventArgs e)
    {
        Debug.Log("Drop");
        e.inv.RemoveItemInSlot(e.slot.GetValueOrDefault(), e.amount);
    }

    private void OnRemoveItem(object sender, InventoryEventHandler.RemoveItemEventArgs e)
    {
        Debug.Log("Remove");
    }

    private void OnDestroy()
    {
        InventoryEventHandler.current.OnAddItem -= OnAddItem;
    }

    private void OnAddItem(object sender, InventoryEventHandler.AddItemEventArgs e)
    {
        Debug.Log($"The item {e.itemAdded.name} was added");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            inventory.AddItem(testItem, 2);
        if (Input.GetKeyDown(KeyCode.S))
            inventory.SwapItemsInCertainAmountInSlots(0, 10, amountToTake);
        if (Input.GetKeyDown(KeyCode.O))
            inventory.SwapItemThruInventoriesSlotToSlot(inventory2.GetComponent<InventoryUI>().GetInventory(), 0, 1, amountToTake);
        if (Input.GetKeyDown(KeyCode.L))
            inventory.SwapItemThruInventories(inventory2.GetComponent<InventoryUI>().GetInventory(), testItem, amountToTake);
    }
}
