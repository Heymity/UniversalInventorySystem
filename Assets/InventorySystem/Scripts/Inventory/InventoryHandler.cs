/*  Copyright 2020 Gabriel Pasquale Rodrigues Scavone
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 * 
 * 
 *  This code is responsable for the events of the Inventory System, and it also contains the items and recipes desired to be used; This code must be placed in object in game    
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniversalInventorySystem
{
    /// <summary>
    /// Put this script on a always active GameObject.
    /// </summary>
    [Serializable]
    public class InventoryHandler : MonoBehaviour
    {
        private void Awake()
        {
            if (current == null) current = this;
        }
        private void Start()
        {
            if (current == null) current = this;
        }
        private void OnEnable()
        {
            if (current == null) current = this;
        }

        public bool autoSaveOnChange;

        #region Item Handler

        //--------ITEM HANDLER--------//

        [Header("Items Handler")]
        public List<ItemGroup> itemAssets;

        public ItemGroup GetItemAssetAtIndex(int index) { return itemAssets[index]; }

        public ItemGroup GetItemAssetWithName(string _strId)
        {
            foreach (ItemGroup i in itemAssets)
            {
                if (i.strId == _strId) return i;
            }

            return null;
        }

        public ItemGroup GetItemAssetWithID(int id)
        {
            foreach (ItemGroup i in itemAssets)
            {
                if (i.id == id) return i;
            }

            return null;
        }

        public List<ItemGroup> OrderItemsAssetById()
        {
            return InsertionSort(itemAssets);
        }

        static List<ItemGroup> InsertionSort(List<ItemGroup> inputArray)
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

        public Item GetItem(int iAssetIndex, int itemIndex) { return GetItemAssetWithID(iAssetIndex).GetItemWithID(itemIndex); }

        public Item GetItemWithName(int id, string itemName) { return GetItemAssetWithID(id).GetItemWithName(itemName); }

        public Item GetItemWithName(string itemAssetStrId, string itemName) { return GetItemAssetWithName(itemAssetStrId).GetItemWithName(itemName); }

        #endregion

        #region RecipeHandler

        //--------RECIPE-HANDLER--------//

        [Header("Recipe Handler")]
        public List<RecipeGroup> recipeAssets = new List<RecipeGroup>();

        public RecipeGroup GetRecipeAssetAtIndex(int index) { return recipeAssets[index]; }

        public RecipeGroup GetRecipeAssetWithName(string _strId)
        {
            foreach (RecipeGroup i in recipeAssets)
            {
                if (i.strId == _strId) return i;
            }

            return null;
        }

        public RecipeGroup GetRecipeAssetWithID(int id)
        {
            foreach (RecipeGroup i in recipeAssets)
            {
                if (i.id == id) return i;
            }

            return null;
        }

        public List<RecipeGroup> OrderRecipeAssetById()
        {
            return InsertionSort(recipeAssets);
        }

        static List<RecipeGroup> InsertionSort(List<RecipeGroup> inputArray)
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

        public Recipe GetRecipeWithName(int id, string recipeName) { return GetRecipeAssetWithID(id).GetRecipeWithName(recipeName); }

        public Recipe GetRecipeWithName(string recipeAssetStrId, string recipeName) { return GetRecipeAssetWithName(recipeAssetStrId).GetRecipeWithName(recipeName); }

        public Recipe GetRecipeAtIndex(int recipeAssetIndex, int recipeIndex) { return recipeAssets[recipeAssetIndex].recipesList[recipeIndex]; }

        public PatternRecipe GetPatternRecipeWithName(int id, string recipeName) { return GetRecipeAssetWithID(id).GetRecipePatternWithKey(recipeName); }

        PatternRecipe GetPatternRecipeWithName(string recipeAssetStrId, string recipeName) { return GetRecipeAssetWithName(recipeAssetStrId).GetRecipePatternWithKey(recipeName); }

        public PatternRecipe GetPatternRecipeAtIndex(int recipeAssetIndex, int recipeIndex) { return recipeAssets[recipeAssetIndex].receipePatternsList[recipeIndex]; }

        public PatternRecipe GetPatternRecipeWithID(int recipeAssetID, int patternRecipeID)
        {
            RecipeGroup asset = GetRecipeAssetWithID(recipeAssetID);

            if (asset != null)
            {
                foreach (PatternRecipe i in asset.receipePatternsList)
                {
                    if (i.id == patternRecipeID) return i;
                }
            }
            return null;
        }

        #endregion

        #region Controller
        //------CONTROLLER------//
        public static InventoryHandler current;

        public event EventHandler<AddItemEventArgs> OnAddItem;
        public event EventHandler<RemoveItemEventArgs> OnRemoveItem;
        public event EventHandler<SwapItemsEventArgs> OnSwapItem;
        public event EventHandler<SwapItemsTrhuInvEventArgs> OnSwapTrhuInventory;
        public event EventHandler<UseItemEventArgs> OnUseItem;
        public event EventHandler<DropItemEventArgs> OnDropItem;
        public event EventHandler<AddItemEventArgs> OnPickUpItem;
        public event EventHandler<InitializeInventoryEventArgs> OnInitializeInventory;
        public event EventHandler<CraftItemEventArgs> OnCraftItem;

        public event EventHandler<EventArgs> OnChange;

        public class AddItemEventArgs : EventArgs
        {
            public Inventory inv;
            public bool addedToNewSlot;
            public bool addedToSlot;
            public Item itemAdded;
            public int amount;
            public int? slotNumber;

            public AddItemEventArgs(Inventory _inv, bool _addedToNewSlot, bool _addedToSlot, Item _itemAdded, int _amount, int? _slotNumber)
            {
                inv = _inv;
                addedToNewSlot = _addedToNewSlot;
                addedToSlot = _addedToSlot;
                itemAdded = _itemAdded;
                amount = _amount;
                slotNumber = _slotNumber;
            }
        }
        public class RemoveItemEventArgs : EventArgs
        {
            public Inventory inv;
            public bool removedByUI;
            public int amount;
            public Item item;
            public int? slot;

            public RemoveItemEventArgs(Inventory _inv, bool _removedByUI, int _amount, Item _item, int? _slot)
            {
                inv = _inv;
                removedByUI = _removedByUI;
                amount = _amount;
                item = _item;
                slot = _slot;
            }
        }
        public class SwapItemsEventArgs : EventArgs
        {
            public Inventory inv;
            public int nativeSlot;
            public int targetSlot;
            public Item nativeItem;
            public Item targetItem;
            public int? amount;

            public SwapItemsEventArgs(Inventory _inv, int _nativeSlot, int _targetSlot, Item _nativeItem, Item _targetItem, int? _amount)
            {
                inv = _inv;
                nativeItem = _nativeItem;
                targetItem = _targetItem;
                nativeSlot = _nativeSlot;
                targetSlot = _targetSlot;
                amount = _amount;
            }
        }
        public class SwapItemsTrhuInvEventArgs : EventArgs
        {
            public Inventory nativeInv;
            public Inventory targetInv;
            public int? nativeSlot;
            public int? targetSlot;
            public Item nativeItem;
            public Item targetItem;
            public int? amount;

            public SwapItemsTrhuInvEventArgs(Inventory _nativeInv, Inventory _targetInv, int? _nativeSlot, int? _targetSlot, Item _nativeItem, Item _targetItem, int? _amount)
            {
                nativeInv = _nativeInv;
                targetInv = _targetInv;
                nativeItem = _nativeItem;
                targetItem = _targetItem;
                nativeSlot = _nativeSlot;
                targetSlot = _targetSlot;
                amount = _amount;
            }
        }
        public class UseItemEventArgs : EventArgs
        {
            public Inventory inv;
            public Item item;
            public int slot;
            //public uint durability

            public UseItemEventArgs(Inventory _inv, Item _item, int _slot)
            {
                inv = _inv;
                item = _item;
                slot = _slot;
            }
        }
        public class DropItemEventArgs : EventArgs
        {
            public Inventory inv;
            public bool takenFromSpecificSlot;
            public int slot;
            public Item item;
            public int amount;
            public bool droppedByUI;
            public UnityEngine.Vector3 positionDropped;
            public bool useGeralDropBehaviour;

            public DropItemEventArgs(Inventory _inv, bool _takenFromSpecificSlot, int _slot, Item _item, int _amount, bool _droppedByUI, UnityEngine.Vector3 _positionDropped, bool _useGeralDropBehaviour)
            {
                inv = _inv;
                takenFromSpecificSlot = _takenFromSpecificSlot;
                slot = _slot;
                item = _item;
                amount = _amount;
                droppedByUI = _droppedByUI;
                positionDropped = _positionDropped;
                useGeralDropBehaviour = _useGeralDropBehaviour;
            }
        }
        public class InitializeInventoryEventArgs : EventArgs
        {
            public Inventory inventory;

            public InitializeInventoryEventArgs(Inventory _inv)
            {
                inventory = _inv;
            }
        }
        public class CraftItemEventArgs : EventArgs
        {
            public Inventory inv;
            public bool pattern;
            public CraftItemData result;
            public CraftItemData grid;
            public bool wasCrafted;
            public Recipe recipe;
            public PatternRecipe patternRecipe;

            public CraftItemEventArgs(Inventory inv, bool pattern, CraftItemData result, CraftItemData grid, bool wasCrafted, Recipe recipe, PatternRecipe patternRecipe)
            {
                this.inv = inv;
                this.pattern = pattern;
                this.result = result;
                this.grid = grid;
                this.wasCrafted = wasCrafted;
                this.recipe = recipe;
                this.patternRecipe = patternRecipe;
            }
        }

        public void Broadcast(BroadcastEventType e,
                              AddItemEventArgs aea = null,
                              RemoveItemEventArgs rea = null,
                              SwapItemsEventArgs sea = null,
                              SwapItemsTrhuInvEventArgs siea = null,
                              UseItemEventArgs uea = null,
                              DropItemEventArgs dea = null,
                              InitializeInventoryEventArgs iea = null,
                              CraftItemEventArgs cia = null)
        {
            //Debug.Log($"Broadcasting event {e}");
            switch (e)
            {
                case BroadcastEventType.AddItem:
                    OnAddItem?.Invoke(this, aea);
                    OnChange?.Invoke(this, aea);
                    break;
                case BroadcastEventType.RemoveItem:
                    OnRemoveItem?.Invoke(this, rea);
                    OnChange?.Invoke(this, rea);
                    break;
                case BroadcastEventType.SwapItem:
                    OnSwapItem?.Invoke(this, sea);
                    OnChange?.Invoke(this, sea);
                    break;
                case BroadcastEventType.SwapTrhuInventory:
                    OnSwapTrhuInventory?.Invoke(this, siea);
                    OnChange?.Invoke(this, siea);
                    break;
                case BroadcastEventType.UseItem:
                    OnUseItem?.Invoke(this, uea);
                    OnChange?.Invoke(this, uea);
                    break;
                case BroadcastEventType.DropItem:
                    OnDropItem?.Invoke(this, dea);
                    OnChange?.Invoke(this, dea);
                    break;
                case BroadcastEventType.PickUpItem:
                    OnPickUpItem?.Invoke(this, aea);
                    OnChange?.Invoke(this, aea);
                    break;
                case BroadcastEventType.InitializeInventory:
                    OnInitializeInventory?.Invoke(this, iea);
                    OnChange?.Invoke(this, iea);
                    break;
                case BroadcastEventType.Craft:
                    OnCraftItem?.Invoke(this, cia);
                    OnChange?.Invoke(this, cia);
                    break;
                default:
                    break;
            }
            if (autoSaveOnChange)
            {
                InventoryController.SaveInventoryData();
            }
        }
        #endregion

        #region UI

        //-------UI--------//

        public EventHandler<OnToggleInventoryEventArgs> OnToggleInventory;
        public EventHandler<OnDragItemEventArgs> OnDragItem;
        public EventHandler<OnDropItemUIEventArgs> OnDropItemUI;

        public class OnToggleInventoryEventArgs : EventArgs
        {
            public Inventory inv;
            public bool isActive;

            public OnToggleInventoryEventArgs(Inventory _inv, bool _isActive)
            {
                inv = _inv;
                isActive = _isActive;
            }
        }

        public class OnDragItemEventArgs : EventArgs
        {
            public Inventory inv;
            public UnityEngine.Vector3 pos;
            public GameObject slot;

            public OnDragItemEventArgs(Inventory _inv, UnityEngine.Vector3 _pos, GameObject _slot)
            {
                inv = _inv;
                pos = _pos;
                slot = _slot;
            }
        }

        public class OnDropItemUIEventArgs : EventArgs
        {
            public Inventory inv;
            public int amount;
            public UnityEngine.Vector3 pos;
            public Item item;

            public OnDropItemUIEventArgs(Inventory _inv, int _amount, UnityEngine.Vector3 _pos, Item _item)
            {
                inv = _inv;
                amount = _amount;
                pos = _pos;
                item = _item;
            }
        }

        public void BroadcastUIEvent(BroadcastEventType e,
                                     OnToggleInventoryEventArgs oti = null,
                                     OnDragItemEventArgs odi = null,
                                     OnDropItemUIEventArgs drop = null)
        {
            switch (e)
            {
                case BroadcastEventType.UIToggled:
                    OnToggleInventory?.Invoke(this, oti);
                    break;
                case BroadcastEventType.ItemDragged:
                    OnDragItem?.Invoke(this, odi);
                    break;
                case BroadcastEventType.DropItem:
                    OnDropItemUI?.Invoke(this, drop);
                    break;
                default:
                    break;
            }
        }
        #endregion
    }

    public enum BroadcastEventType
    {
        None = 0,
        AddItem = 1,
        RemoveItem = 2,
        SwapItem = 3,
        SwapTrhuInventory = 4,
        DropItem = 5,
        PickUpItem = 6,
        UseItem = 7,
        InitializeInventory = 8,
        UIToggled = 9,
        ItemDragged = 10,
        Craft = 11
    }
}

