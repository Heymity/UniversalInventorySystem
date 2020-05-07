using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandler : MonoBehaviour
{
    public List<Item> allItemsInGame = new List<Item>();

    private void Start()
    {
        InventoryController.SetItems(allItemsInGame);
    }
}
