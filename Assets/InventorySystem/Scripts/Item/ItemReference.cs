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
 *  This is one of the most important base classes, its the item, as both scriptable object and normal class.
 */

namespace UniversalInventorySystem
{
    [System.Serializable]
    public class ItemReference
    {
        public bool useConstant;
        public ItemVariable variable;
        public Item constantValue;

        public Item Value
        {
            get => useConstant ? constantValue : variable.value;
        }

        public void SetItem(Item item)
        {
            constantValue = item;
            variable = null;
            useConstant = true;
        }

        public void SetItem(ItemVariable item)
        {
            variable = item;
            constantValue = null;
            useConstant = false;
        }

        public ItemReference(Item item)
        {
            constantValue = item;
            variable = null;
            useConstant = true;
        }

        public ItemReference(ItemVariable item)
        {
            variable = item;
            constantValue = null;
            useConstant = false;
        }

        public ItemReference(Item item, ItemVariable itemVariable, bool _useConstant)
        {
            variable = itemVariable;
            constantValue = item;
            useConstant = _useConstant;
        }

        public ItemReference()
        {
            variable = null;
            constantValue = null;
            useConstant = true;
        }

        public static bool operator ==(ItemReference a, ItemReference b) => a.Value == b.Value;
        public static bool operator ==(ItemReference a, Item b) => a.Value == b;
        public static bool operator !=(ItemReference a, Item b) => a.Value != b;
        public static bool operator !=(ItemReference a, ItemReference b) => a.Value != b.Value;

        public override bool Equals(object obj) => Value.Equals((obj as ItemReference).Value);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => $"UseConstant: {useConstant}; ConstantV: {constantValue}; ReferenceV: {variable}";

        public static implicit operator Item(ItemReference a) => a.Value;

        public static explicit operator ItemReference(Item a) => new ItemReference(a);
        public static explicit operator ItemReference(ItemVariable a) => new ItemReference(a);
    }
}