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

        public void SetInventory(Inventory inv)
        {
            constantValue = inv;
            variable = null;
            useConstant = true;
        }

        public void SetInventory(InventoryVarialbe inv)
        {
            variable = inv;
            constantValue = null;
            useConstant = false;
        }

        public InventoryReference(Inventory inv)
        {
            constantValue = inv;
            variable = null;
            useConstant = true;
        }

        public InventoryReference(InventoryVarialbe inv)
        {
            variable = inv;
            constantValue = null;
            useConstant = false;
        }

        public InventoryReference(Inventory inv, InventoryVarialbe invVariable, bool _useConstant)
        {
            variable = invVariable;
            constantValue = inv;
            useConstant = _useConstant;
        }

        public InventoryReference()
        {
            variable = null;
            constantValue = null;
            useConstant = true;
        }

        public static bool operator ==(InventoryReference a, InventoryReference b) => a.Value == b.Value;
        public static bool operator ==(InventoryReference a, Inventory b) => a.Value == b;
        public static bool operator !=(InventoryReference a, Inventory b) => a.Value != b;
        public static bool operator !=(InventoryReference a, InventoryReference b) => a.Value != b.Value;

        public override bool Equals(object obj) => Value.Equals((obj as InventoryReference).Value);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => $"UseConstant: {useConstant}; ConstantV: {constantValue}; ReferenceV: {variable}";

        public static implicit operator Inventory(InventoryReference a) => a.Value;
        public static explicit operator InventoryReference(Inventory a) => new InventoryReference(a);
        public static explicit operator InventoryReference(InventoryVarialbe a) => new InventoryReference(a);
    }
}
