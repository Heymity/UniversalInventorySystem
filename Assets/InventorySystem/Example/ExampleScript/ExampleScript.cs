using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    Inventory inventory;
    public InventoryUI invUI;
    public InventoryUI invUI2;

    public Item testItem;
    public int slotAmount;

    private void Start()
    {
        //Inventory initialization
        inventory = new Inventory(slotAmount, true, InventoryProtection.Any, true);
        inventory.InitializeInventory();

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
