using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item", order = 1), System.Serializable]
public class Item : ScriptableObject
{
    public string itemName;
    public int id;
    public Sprite sprite;
    public int maxAmount;
    public bool destroyOnUse;
    public int useHowManyWhenUsed;
    public bool stackable;

    public void OnUse()
    {
        Debug.Log("UsingItem");
    }
}
