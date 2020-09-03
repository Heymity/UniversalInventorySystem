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

namespace UniversalInventorySystem
{
    [System.Serializable]
    public class InventoryReference
    {
        public bool useConstant = true;
        public Inventory constantValue;
        public InventoryVarialbe variable;

        public Inventory Value
        {
            get => useConstant ? constantValue : variable.value;
        }
    }
}
