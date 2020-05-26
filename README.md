# UniversalInventorySystem [![Unity 2019.3 or later](https://img.shields.io/badge/unity-2019.3%20or%20later-green.svg?logo=unity&cacheSeconds=2592000)](https://unity3d.com/get-unity/download/archive)

A Universal open source Inventory System, made to be as customized as possible!

## Dependencies
- Text Mesh Pro


## Getting Started

In the InventorySystem folder there is a Example paste, By loading the scene you will have a example of what the system is caplable to. 

The scene is composed by three panels:

1. An inventory UI
2. An inventory UI that shares the same inventory as the 1 one
3. An inventory UI with a totally diferent inventory
4. A crafting grid

You can acces the ExampleScript to see how its coded

## A quick guide trhu the methods

* AddItemToNewSlot
```c#
public static int AddItemToNewSlot(this Inventory inv, Item item, int amount, BroadcastEventType e = BroadcastEventType.AddItem, bool overrideSlotProtection = false)
```
* AddItem
```c#
public static int AddItem(this Inventory inv, Item item, int amount, BroadcastEventType e = BroadcastEventType.AddItem, bool overrideSlotProtection = false)
```
* AddItemToSlot
```c#
public static int AddItemToSlot(this Inventory inv, Item item, int amount, int slotNumber, BroadcastEventType e = BroadcastEventType.AddItem, bool overrideSlotProtection = false)
```
* DropItem
```c#
public static bool DropItem(this Inventory inv, int amount, Vector3 dropPosition, Item item = null, int? slot = null, BroadcastEventType e = BroadcastEventType.DropItem)
```
* RemoveItem
```c#
public static bool RemoveItem(this Inventory inv, Item item, int amount, BroadcastEventType e = BroadcastEventType.RemoveItem, Vector3? dropPosition = null)
```
* RemoveItemInSlot
```c#
public static bool RemoveItemInSlot(this Inventory inv, int slot, int amount, BroadcastEventType e = BroadcastEventType.RemoveItem, Vector3? dropPosition = null)
```
* UseItemInSlot
```c#
public static void UseItemInSlot(this Inventory inv, int slot, BroadcastEventType e = BroadcastEventType.UseItem)
```
* SwapItemsInSlots
```c#
public static void SwapItemsInSlots(this Inventory inv, int nativeSlot, int targetSlot, BroadcastEventType e = BroadcastEventType.SwapItem)
```
* SwapItemsInCertainAmountInSlots
```c#
public static int SwapItemsInCertainAmountInSlots(this Inventory inv, int nativeSlot, int targetSlot, int? _amount, BroadcastEventType e = BroadcastEventType.SwapItem)
```
* SwapItemThruInventoriesSlotToSlot
```c#
public static int SwapItemThruInventoriesSlotToSlot(this Inventory nativeInv, Inventory targetInv, int nativeSlotNumber, int targetSlotNumber, int amount, BroadcastEventType e = BroadcastEventType.SwapTrhuInventory)
```
* SwapItemThruInventories
```c#
public static bool SwapItemThruInventories(this Inventory nativeInv, Inventory targetInv, Item item, int amount, BroadcastEventType e = BroadcastEventType.SwapTrhuInventory)
```
* CraftItem
```c#
public static Item[] CraftItem(this Inventory inv, Item[] grid, Vector2Int gridSize, bool craftItem, bool allowPatternRecipe, int productSlots)
```

* __InitializeInventory__*
```c#
public static Inventory InitializeInventory(this Inventory inv, BroadcastEventType e = BroadcastEventType.InitializeInventory)
```
* __InitializeInventoryFromAnotherInventory__*
```c#
public static Inventory InitializeInventoryFromAnotherInventory(this Inventory inv, Inventory modelInv, BroadcastEventType e = BroadcastEventType.InitializeInventory)
```

> *This functions are kind of special, if you are using a inventory you need to call this functions when you create them, the only exeption is when creating a inventory from the inventoryUI inspector

All the functions have a summary explaining what it does, what it returns and what are their params

