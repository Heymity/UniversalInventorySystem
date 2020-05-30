using UniversalInventorySystem;
using UnityEngine;

public class MyInventory : MonoBehaviour
{
    public Inventory myInv;

    public Item item;

    public void Start()
    {
        myInv.slotAmounts = 10;
        myInv.InitializeInventory();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) myInv.AddItem(item, 1);
    }
}