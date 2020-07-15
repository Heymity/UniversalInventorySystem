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
        CreateAssetMenu(fileName = "Inventory", menuName = "UniversalInventorySystem/Inventory", order = 1),
        Serializable
    ]
    public class Inventory : ScriptableObject
    {
        [SerializeField] private Slot[] _slots;
        [SerializeField] private int _slotAmounts;
        [SerializeField] private int _id;

        [SerializeField] private InventoryProtection _interactiable;

        public List<Slot> slots { get; set; }
        public int slotAmounts { get; set; }
        public int id { get; set; }

        public InventoryProtection interactiable { get; set; }

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
            SetValues();
            Initialize();
        }

        public void OnValidate()
        {
            SetValues();
        }

        void SetValues()
        {
            slots = _slots.ToList();
            slotAmounts = _slotAmounts;
            id = _id;
            interactiable = _interactiable;
        }

        /// <summary>
        /// This function must be called when a inventory is being created. It fills the inventory if null Slots if the list of slots is null or have less elments than inv.slotAmounts, give an id to the inventory and add it to the list of inventories in the InventoryController. This function dont need to be called if you are using an loading system;
        /// </summary>
        /// <returns>The initiaized inventory</returns>
        public Inventory Initialize(BroadcastEventType e = BroadcastEventType.InitializeInventory)
        {
            if (this == null)
            {
                Debug.LogError("Null inventory provided for Initialize");
                throw new ArgumentNullException("inv", "Null inventory provided");
            }

            if (hasInitialized) return this;

            if (slots == null) slots = new List<Slot>();
            if (slots.Count != slotAmounts)
            {
                for (int i = 0; i < slotAmounts; i++)
                {
                    if (i < slots.Count) continue;
                    else
                        slots.Add(Slot.nullSlot);
                }
            }

            hasInitialized = true;
            InventoryController.inventories.Add(this);
            InventoryHandler.InitializeInventoryEventArgs iea = new InventoryHandler.InitializeInventoryEventArgs(this);
            if (InventoryHandler.current != null)
                InventoryHandler.current.Broadcast(e, iea: iea);
            return this;
        }

        [Serializable]
        public class InventoryData
        {
            [SerializeField] public List<Slot> slots;
            [SerializeField] public int slotAmounts;
            [SerializeField] public int id;

            [SerializeField] public InventoryProtection interactiable;

            public InventoryData(Inventory inv)
            {
                slots = inv.slots;
                slotAmounts = inv.slotAmounts;
                id = inv.id;
                interactiable = inv.interactiable;
            }
        }

    }
}
