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
 *  This script contain the interfaces and abstract classes from the inventory system
 * 
 */ 

using UnityEngine;

namespace UniversalInventorySystem
{
    /// <summary>
    /// Remember to subscribe this functioon to the InventoyrEventHandler in order to work
    /// </summary>
    public interface IDropBehaviour
    {
        void OnDropItem(object sender, InventoryHandler.DropItemEventArgs e);
    }

    /// <summary>
    /// Remember to subscribe this functioon to the InventoyrEventHandler in order to work
    /// </summary>
    public interface IPickUpBehaviour
    {
        void OnPickUp(object sender, InventoryHandler.AddItemEventArgs e);
    }

    public interface IUsable
    {
        void OnUse(object sender, InventoryHandler.UseItemEventArgs e);
    }



    public abstract class DropBehaviour : MonoBehaviour, IDropBehaviour
    {
        public virtual void OnEnable()
        {
            InventoryHandler.current.OnDropItem += OnDropItem;
        }

        public virtual void OnDestroy()
        {
            InventoryHandler.current.OnDropItem -= OnDropItem;
        }

        public abstract void OnDropItem(object sender, InventoryHandler.DropItemEventArgs e);
    }
}
