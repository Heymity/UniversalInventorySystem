using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[AddComponentMenu("UniversalInventorySystem/Item")]
[CreateAssetMenu(fileName = "Item", menuName = "UniversalInventorySystem/Item", order = 1), System.Serializable]
public class Item : ScriptableObject
{
    public string itemName;
    public int id;
    public Sprite sprite;
    public int maxAmount;
    public bool destroyOnUse;
    public int useHowManyWhenUsed;
    public bool stackable;
    public MonoScript OnUseFunc;

    public void OnUse(Inventory inv, int slot)
    {
        Debug.Log("UsingItem");
        InventoryEventHandler.UseItemEventArgs uea = new InventoryEventHandler.UseItemEventArgs(inv, this, slot);
        object[] tmp = new object[2] { this, uea };

        BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        MethodInfo[] monoMethods = OnUseFunc.GetClass().GetMethods(flags);

        monoMethods[0].Invoke(Activator.CreateInstance(OnUseFunc.GetClass()), tmp);

    }
}
