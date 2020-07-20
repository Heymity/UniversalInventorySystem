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
 *  This is one of the most important base classes, its the item, its a scriptable object.
 */ 

using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

namespace UniversalInventorySystem
{
    [ 
        CreateAssetMenu(fileName = "Item", menuName = "UniversalInventorySystem/Item", order = 115), 
        Serializable
    ]
    public class Item : ScriptableObject
    {
        public string itemName;
        public int id;
        public Sprite sprite;
        public int maxAmount;
        public bool destroyOnUse;
        public int useHowManyWhenUsed;
        public bool stackable;
        public int durability;
        public bool hasDurability;
        public bool showAmount;
        /**public bool stackAlways;
        public bool stackOnMaxDurabiliy;
        public bool stackOnSpecifDurability;
        public StackOptions stackOptions;
        public List<uint> stackDurabilities;*/
        public List<DurabilityImage> durabilityImages
        {
            get
            {
                return _durabilityImages;
            }
            set
            {
                _durabilityImages = SortDurabilityImages(value);
            }
        }
        [SerializeField]
        private List<DurabilityImage> _durabilityImages;
        public MonoScript onUseFunc;
        public MonoScript optionalOnDropBehaviour;
        public ToolTipInfo tooltip;

        public void OnEnable()
        {
            _durabilityImages = SortDurabilityImages(_durabilityImages);
        }

        public void OnUse(Inventory inv, int slot)
        {
            if (onUseFunc == null) return;
            InventoryHandler.UseItemEventArgs uea = new InventoryHandler.UseItemEventArgs(inv, this, slot);
            object[] tmp = new object[2] { this, uea };

            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            MethodInfo monoMethod = onUseFunc.GetClass().GetMethod("OnUse", flags);
            if (monoMethod == null) Debug.LogError($"The script provided ({onUseFunc.name}) on item {itemName} does not contain, or its not accesible, the expected function OnUse.\n Check if this function exists and if the provided script derives from IUsable");
            else monoMethod.Invoke(Activator.CreateInstance(onUseFunc.GetClass()), tmp);

        }

        public void OnDrop(Inventory inv, bool tss, int slot, int amount, bool dbui, Vector3? pos)
        {
            if ((inv.interactiable & InventoryController.DropInvFlags) != InventoryController.DropInvFlags) return;

            if (optionalOnDropBehaviour == null)
            {
                InventoryHandler.DropItemEventArgs dea = new InventoryHandler.DropItemEventArgs(inv, tss, slot, this, amount, dbui, pos.GetValueOrDefault(), true);

                InventoryHandler.current.Broadcast(BroadcastEventType.DropItem, dea: dea);
            }
            else
            {
                InventoryHandler.DropItemEventArgs dea = new InventoryHandler.DropItemEventArgs(inv, tss, slot, this, amount, dbui, pos.GetValueOrDefault(), false);
                object[] tmp = new object[2] { this, dea };

                BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

                MethodInfo monoMethod = optionalOnDropBehaviour.GetClass().GetMethod("OnDropItem", flags);

                if (monoMethod == null) Debug.LogError($"The script provided ({optionalOnDropBehaviour.name}) on item {itemName} does not contain, or its not accesible, the expected function OnDropItem.\n Check if this function exists and if the provided script derives from DropBehaviour");
                else monoMethod.Invoke(Activator.CreateInstance(optionalOnDropBehaviour.GetClass()), tmp);
            }
        }

        public static List<DurabilityImage> SortDurabilityImages(List<DurabilityImage> inputArray)
        {
            if (inputArray == null) return inputArray;
            for (int i = 0; i < inputArray.Count - 1; i++)
            {
                for (int j = i + 1; j > 0; j--)
                {
                    if (inputArray[j - 1].durability > inputArray[j].durability)
                    {
                        checked
                        {
                            int temp = inputArray[j - 1].durability;
                            inputArray[j - 1].durability = inputArray[j].durability;
                            inputArray[j].durability = temp;
                        }
                    }
                }
            }
            return inputArray;
        }       
    }

    [Serializable]
    public class DurabilityImage : object
    {
        [SerializeField] public string imageName;
        [SerializeField] public Sprite sprite;
        [SerializeField] public int durability;
    }

    /*public enum StackOptions
    {
        Split = 0,
        TakeFromAll = 1,
        Mantain = 2,
    }*/
}
