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
 *  This is basically a group of items, an item database.
 */ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniversalInventorySystem
{
    [AddComponentMenu("UniversalInventorySystem/ItemGroup"), CreateAssetMenu(fileName = "ItemGroup", menuName = "UniversalInventorySystem/ItemGroup", order = 1), System.Serializable]
    public class ItemGroup : ScriptableObject
    {
        public List<Item> itemsList = new List<Item>();
        public string strId;
        public int id;

        public Item GetItemAtIndex(int index) { return itemsList[index]; }

        public Item GetItemWithName(string name)
        {
            foreach (Item i in itemsList)
            {
                if (i.name == name) return i;
            }

            return null;
        }

        public Item GetItemWithID(int id)
        {
            foreach (Item i in itemsList)
            {
                if (i.id == id) return i;
            }

            return null;
        }

        public List<Item> OrderItemsById()
        {
            return InsertionSort(itemsList);
        }

        static List<Item> InsertionSort(List<Item> inputArray)
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
    }

    public static class ItemAssetController
    {
        public static bool ContainsWNull(this List<Item> items, Item item)
        {
            if (item == null)
            {
                return true;
            }
            else
            {
                return items.Contains(item);
            }
        }
    }
}
