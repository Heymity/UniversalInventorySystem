using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class ExampleScript : MonoBehaviour
{
    Inventory inventory;
    public Item testItem;
    public int slotAmount;
    public InventoryUI invUI;

    private void Start()
    {
        //Inventory initialization
        inventory = new Inventory(slotAmount, true, IteractiableTypes.Any, true);
        inventory.InitializeInventory();

        //InventoryUI initialization
        invUI.SetInventory(inventory);

        //Events
        InventoryEventHandler invEvent = InventoryEventHandler.current;
        invEvent.OnAddItem += OnAddItem;
        invEvent.OnRemoveItem += OnRemoveItem;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            inventory.AddItem(testItem, 2);
    }

    //Callback function for when an item is removed from any inventory
    private void OnRemoveItem(object sender, InventoryEventHandler.RemoveItemEventArgs e)
    {
        Debug.Log("Remove");
    }

    //Callback function for when an item is added from any inventory
    private void OnAddItem(object sender, InventoryEventHandler.AddItemEventArgs e)
    {
        Debug.Log($"The item {e.itemAdded.name} was added");
    }

    //Unsubscribing the events if this object gets destoyed (better use the OnDisable func if your gameobj can be set inactive in hireachy)
    private void OnDestroy()
    {
        InventoryEventHandler.current.OnAddItem -= OnAddItem;
        InventoryEventHandler.current.OnRemoveItem -= OnRemoveItem;
    }

}
