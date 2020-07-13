/*  Copyright 2020 Gabriel Pasquale Rodrigues Scavone
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 * 
 * 
 *  This is an example script of how to use the UIS
 * 
 */ 

using UnityEngine;
using UniversalInventorySystem;

public class ExampleScript : MonoBehaviour
{   
    Inventory inventory;
    public InventoryUI invUI;
    public InventoryUI invUI2;

    // To test the item drawer in inspector
    public Item testItem;
    public int slotAmount;

    private void Start()
    {
        //Inventory initialization
        inventory = new Inventory(slotAmount, true, InventoryController.AllInventoryFlags, true);
        inventory.Initialize();

        //InventoryUI initialization
        invUI.SetInventory(inventory);
        invUI2.SetInventory(inventory);

        //Events
        InventoryHandler invEvent = InventoryHandler.current;
        invEvent.OnAddItem += OnAddItem;
        invEvent.OnRemoveItem += OnRemoveItem;
    }

    private void Update()
    {
        //Adds a item
        if (Input.GetKeyDown(KeyCode.A))
            inventory.AddItem(InventoryHandler.current.GetItem(0, 0), 2);

        //Adds another type of item
        if (Input.GetKeyDown(KeyCode.D))
            inventory.AddItem(InventoryHandler.current.GetItem(0, 1), 2);

        //Checks an item in a inventory
        //Debug.Log(inventory.CheckItemInInventory(InventoryHandler.current.GetItem(0, 0), 4).hasItem);
    }

    //Callback function for when an item is removed from any inventory
    private void OnRemoveItem(object sender, InventoryHandler.RemoveItemEventArgs e)
    {
        Debug.Log("Remove (ExampleScript)");
    }

    //Callback function for when an item is added from any inventory
    private void OnAddItem(object sender, InventoryHandler.AddItemEventArgs e)
    {
        Debug.Log($"The item {e.itemAdded.name} was added (ExampleScript)");
    }

    //Unsubscribing the events if this object gets destoyed (better use the OnDisable func if your gameobj can be set inactive in hireachy)
    private void OnDestroy()
    {
        InventoryHandler.current.OnAddItem -= OnAddItem;
        InventoryHandler.current.OnRemoveItem -= OnRemoveItem;
    }

}
