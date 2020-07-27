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
 *  This code is the Scriptable object of the inventory
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniversalInventorySystem
{
    [
        CreateAssetMenu(fileName = "Inventory", menuName = "UniversalInventorySystem/Inventory", order = 81),
        Serializable
    ]
    public class Inventory : ScriptableObject
    {
#pragma warning disable
        [SerializeField] private List<Slot> inventorySlots = new List<Slot>();
        [SerializeField] private int _slotAmounts = 0;
        [SerializeField] private int _id = 0;
        [SerializeField] private string _key;

        [SerializeField] private InventoryProtection _interactiable = InventoryController.AllInventoryFlags;
#pragma warning restore

        [HideInInspector]
        public Seed[] seeds;

        public List<Slot> slots;
        public int SlotAmount => slots.Count;
        public int id;
        public string key;

        public InventoryProtection interactiable;

        public bool HasInitialized => hasInitialized;
        private bool hasInitialized;

        public Slot this[int i]
        {
            get { return slots[i]; }
            set { slots[i] = value; }
        }
        public static bool operator true(Inventory inv) => inv?.slots != null;
        public static bool operator false(Inventory inv) => inv?.slots == null;

        public void OnEnable()
        {
            //if (!Application.isPlaying)
            SetValues();
            hasInitialized = false;
            Initialize();
        }

        public void OnValidate()
        {
            if(!Application.isPlaying)
                SetValues();
        }

        void SetValues()
        {
            slots = new List<Slot>(inventorySlots);
            //SlotAmount = _slotAmounts;
            id = _id;
            interactiable = _interactiable;
            if (string.IsNullOrEmpty(_key)) { key = name; _key = name; }
            else key = _key;
        }

        /// <summary>
        /// This function must be called when a inventory is being created. It fills the inventory if null Slots if the list of slots is null or have less elments than inv.SlotAmount, give an id to the inventory and add it to the list of inventories in the InventoryController. This function dont need to be called if you are using an loading system;
        /// </summary>
        /// <returns>The initiaized inventory</returns>
        public Inventory Initialize(bool loadSeeds = true, BroadcastEventType e = BroadcastEventType.InitializeInventory)
        {
            if (this == null)
            {
                Debug.LogError("Null inventory provided for Initialize");
                throw new ArgumentNullException("inv", "Null inventory provided");
            }
            //Debug.Log("Init");
            if (hasInitialized) return this;

            if (seeds == null) seeds = new Seed[] { };
            if (seeds.Length >= 1 && loadSeeds) 
                LoadSeed(0);
            else
            {
                if (slots == null) slots = new List<Slot>();
                if (slots.Count != SlotAmount)
                {
                    for (int i = 0; i < SlotAmount; i++)
                    {
                        if (i < slots.Count) continue;
                        else
                            slots.Add(Slot.nullSlot);
                    }
                }
            }

            hasInitialized = true;
            if (!InventoryController.inventories.Contains(this)) 
                InventoryController.inventories.Add(this);
            InventoryHandler.InitializeInventoryEventArgs iea = new InventoryHandler.InitializeInventoryEventArgs(this);
            if (InventoryHandler.current != null)
                InventoryHandler.current.Broadcast(e, iea: iea);
            return this;
        }

        public bool LoadSeed(int index)
        {
            Seed seed = seeds[index];
            if (seed == null) return false;
            slots = new List<Slot>(seed.seedSlots);
            return true;
        }
    }
}
