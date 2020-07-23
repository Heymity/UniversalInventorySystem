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
 *  This code is the Scriptable object of the Seed
 */

using System.Collections.Generic;
using UnityEngine;

namespace UniversalInventorySystem
{
    [CreateAssetMenu(fileName = "Seed", menuName = "UniversalInventorySystem/Seed", order = 82), System.Serializable]
    public class Seed : ScriptableObject
    {
        [SerializeField]
        public List<Slot> seedSlots;
    }
}
