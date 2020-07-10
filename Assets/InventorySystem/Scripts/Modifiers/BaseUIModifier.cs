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
 *  This is the base class for making modifiers to the inventoryUI
 */  

using UnityEngine;

namespace UniversalInventorySystem
{ 
    [RequireComponent(typeof(InventoryUI))]
    public class BaseUIModifier : MonoBehaviour
    {
        protected InventoryUI OriginalTarget { get; private set; }

        protected InventoryUI target;

        public InventoryUI GetTarget() => target;
        public InventoryUI GetOriginalTarget() => OriginalTarget;
        public InventoryUI SetTarget(InventoryUI _target) => target = _target;

        public void Start()
        { 
            target = GetComponent<InventoryUI>();
            OriginalTarget = target;
        }

        public void OnEnable()
        {
            target = GetComponent<InventoryUI>();
            OriginalTarget = target;
        }
    }
}