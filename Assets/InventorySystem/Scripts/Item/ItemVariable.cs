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
 *  This is one of the most important base classes, its the item as a scriptable object.
 */

using UnityEngine;

namespace UniversalInventorySystem
{
    [
        CreateAssetMenu(fileName = "Item", menuName = "UniversalInventorySystem/Item", order = 115),
        System.Serializable
    ]
    public class ItemVariable : ScriptableObject
    {
        public Item value;
#pragma warning disable
        [SerializeField] private Item _value;
#pragma warning restore
        public void OnEnable()
        {
            if (!Application.isPlaying)
                SetValues();
        }

        public void OnValidate()
        {
            if (!Application.isPlaying)
                SetValues();
        }

        void SetValues()
        {
            value = new Item(_value);
        }

    }
}