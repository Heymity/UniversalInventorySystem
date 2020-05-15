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

    public Item GetItemAtIndex(int index) { return allItemsInGame[index]; }

    public Item GetItemWithName(string name)
    {
        foreach(Item i in allItemsInGame)
        {
            if (i.name == name) return i;
        }

        return null;
    }

    public Item GetItemWithID(int id)
    {
        foreach (Item i in allItemsInGame)
        {
            if (i.id == id) return i;
        }

        return null;
    }

    public List<Item> OrderItemsById()
    {
        return InsertionSort(allItemsInGame);
    }

    static List<Item> InsertionSort(List<Item> inputArray)
    {
        for (int i = 0; i < inputArray.Count - 1; i++)
        {
            for (int j = i + 1; j > 0; j--)
            {
                if (inputArray[j - 1].id > inputArray[j].id)
                {
                    int temp = inputArray[j - 1].id;
                    inputArray[j - 1].id = inputArray[j].id;
                    inputArray[j].id = temp;
                }
            }
        }
        return inputArray;
    }
}
