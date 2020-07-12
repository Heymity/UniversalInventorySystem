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
 * 
 *  This script is responsable for saving the info of the slot beeing dragged 
 */ 

using UnityEngine;

namespace UniversalInventorySystem
{
    public class DragSlot : MonoBehaviour
    {
        [HideInInspector] public int amount;
        [HideInInspector] public int durability;
        [HideInInspector] public Item item;
        [HideInInspector] public int slotNumber;
        [HideInInspector] public InventoryUI invUI;
        [HideInInspector] public Inventory inv;

        public void SetAmount(int _amount) => amount = _amount;
        public void SetDurability(int _durability) => durability = _durability;
        public void SetItem(Item _item) => item = _item;
        public void SetSlotNumber(int num) => slotNumber = num;
        public void SetInventoryUI(InventoryUI _invUI) => invUI = _invUI;
        public void SetInventory(Inventory _inv) => inv = _inv;

        public int GetAmount() => amount;
        public int GetDurability() => durability;
        public Item GetItem() => item;
        public int GetSlotNumber() => slotNumber;
        public InventoryUI GetInventoryUI() => invUI;
        public Inventory GetInventory() => inv;
    }
}
