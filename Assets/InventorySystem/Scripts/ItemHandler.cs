using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    public static ItemHandler current;
    public List<Item> allItemsInGame = new List<Item>();

    private void Start()
    {
        current = this;
        InventoryController.SetItems(allItemsInGame);
    }
}
