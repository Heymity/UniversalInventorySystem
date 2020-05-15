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
    public MonoScript OptionalOnDropBehaviour;

    public void OnUse(Inventory inv, int slot)
    {
        Debug.Log("UsingItem");
        InventoryEventHandler.UseItemEventArgs uea = new InventoryEventHandler.UseItemEventArgs(inv, this, slot);
        object[] tmp = new object[2] { this, uea };

        BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        MethodInfo monoMethod = OnUseFunc.GetClass().GetMethod("OnUse", flags);
        if (monoMethod == null) Debug.LogError($"The script provided ({OnUseFunc.name}) on item {itemName} does not contain, or its not accesible, the expected function OnUse.\n Check if this function exists and if the provided script derives from IUsable");
        else monoMethod.Invoke(Activator.CreateInstance(OnUseFunc.GetClass()), tmp);

    }

    public void OnDrop(Inventory inv, bool tss, int? slot, int amount, bool dbui, Vector3? pos)
    {
        if (OptionalOnDropBehaviour == null)
        {
            InventoryEventHandler.DropItemEventArgs dea = new InventoryEventHandler.DropItemEventArgs(inv, tss, slot, this, amount, dbui, pos.GetValueOrDefault(), true);
            InventoryEventHandler.current.Broadcast(BroadcastEventType.DropItem, dea: dea);
        } else
        {
            InventoryEventHandler.DropItemEventArgs dea = new InventoryEventHandler.DropItemEventArgs(inv, tss, slot, this, amount, dbui, pos.GetValueOrDefault(), false);
            object[] tmp = new object[2] { this, dea };

            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            MethodInfo monoMethod = OptionalOnDropBehaviour.GetClass().GetMethod("OnDropItem", flags);

            if (monoMethod == null) Debug.LogError($"The script provided ({OptionalOnDropBehaviour.name}) on item {itemName} does not contain, or its not accesible, the expected function OnDropItem.\n Check if this function exists and if the provided script derives from DropBehaviour");
            else monoMethod.Invoke(Activator.CreateInstance(OptionalOnDropBehaviour.GetClass()), tmp);
        }
    }
}
