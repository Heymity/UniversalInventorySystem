﻿using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[AddComponentMenu("UniversalInventorySystem/Item"), CreateAssetMenu(fileName = "Item", menuName = "UniversalInventorySystem/Item", order = 1), System.Serializable]
public class Item : ScriptableObject
{
    public string itemName;
    public int id;
    public Sprite sprite;
    public int maxAmount;
    public bool destroyOnUse;
    public int useHowManyWhenUsed;
    public bool stackable;
    public MonoScript onUseFunc;
    public MonoScript optionalOnDropBehaviour;

    public void OnUse(Inventory inv, int slot)
    {
        Debug.Log("UsingItem");
        InventoryHandler.UseItemEventArgs uea = new InventoryHandler.UseItemEventArgs(inv, this, slot);
        object[] tmp = new object[2] { this, uea };

        BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        MethodInfo monoMethod = onUseFunc.GetClass().GetMethod("OnUse", flags);
        if (monoMethod == null) Debug.LogError($"The script provided ({onUseFunc.name}) on item {itemName} does not contain, or its not accesible, the expected function OnUse.\n Check if this function exists and if the provided script derives from IUsable");
        else monoMethod.Invoke(Activator.CreateInstance(onUseFunc.GetClass()), tmp);

    }

    public void OnDrop(Inventory inv, bool tss, int? slot, int amount, bool dbui, Vector3? pos)
    {
        if (optionalOnDropBehaviour == null)
        {
            InventoryHandler.DropItemEventArgs dea = new InventoryHandler.DropItemEventArgs(inv, tss, slot, this, amount, dbui, pos.GetValueOrDefault(), true);
            InventoryHandler.current.Broadcast(BroadcastEventType.DropItem, dea: dea);
        } else
        {
            InventoryHandler.DropItemEventArgs dea = new InventoryHandler.DropItemEventArgs(inv, tss, slot, this, amount, dbui, pos.GetValueOrDefault(), false);
            object[] tmp = new object[2] { this, dea };

            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            MethodInfo monoMethod = optionalOnDropBehaviour.GetClass().GetMethod("OnDropItem", flags);

            if (monoMethod == null) Debug.LogError($"The script provided ({optionalOnDropBehaviour.name}) on item {itemName} does not contain, or its not accesible, the expected function OnDropItem.\n Check if this function exists and if the provided script derives from DropBehaviour");
            else monoMethod.Invoke(Activator.CreateInstance(optionalOnDropBehaviour.GetClass()), tmp);
        }
    }
}