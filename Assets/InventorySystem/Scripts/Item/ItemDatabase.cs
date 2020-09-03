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

using System.Collections.Generic;
using UnityEngine;

namespace UniversalInventorySystem
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "UniversalInventorySystem/ItemDatabase", order = 116), System.Serializable]
    public class ItemDatabase : ScriptableObject
    {
        public List<ItemReference> itemsList = new List<ItemReference>();
        public string strId;
        public int id;

        public ItemReference GetItemAtIndex(int index) => itemsList[index]; 

        public ItemReference this[int i]
        {
            get => itemsList[i];
        }

        public ItemReference GetItemWithName(string name)
        {
            foreach (ItemReference i in itemsList)
            {
                if (i.Value.name == name) return i;
            }

            return null;
        }

        public ItemReference GetItemWithID(int id)
        {
            foreach (ItemReference i in itemsList)
            {
                if (i.Value.id == id) return i;
            }

            return null;
        }

        public List<ItemReference> OrderItemsById()
        {
            return InsertionSort(itemsList);
        }

        static List<ItemReference> InsertionSort(List<ItemReference> inputArray)
        {
            for (int i = 0; i < inputArray.Count - 1; i++)
            {
                for (int j = i + 1; j > 0; j--)
                {
                    if (inputArray[j - 1].Value.id > inputArray[j].Value.id)
                    {
                        int temp = inputArray[j - 1].Value.id;
                        inputArray[j - 1].Value.id = inputArray[j].Value.id;
                        inputArray[j].Value.id = temp;
                    }
                }
            }
            return inputArray;
        } 
    }

    public static class ItemAssetController
    {
        public static bool ContainsWNull(this List<ItemReference> items, Item itemToCompare)
        {
            if (itemToCompare == null)
            {
                return true;
            }
            else
            {
                return items.ConvertAll((ItemReference item) => item.Value).Contains(itemToCompare);
            }
        }

        public static bool Contains(this List<ItemReference> items, Item itemToCompare)
        {
            return items.ConvertAll((ItemReference item) => item.Value).Contains(itemToCompare);
        }
    }
}
