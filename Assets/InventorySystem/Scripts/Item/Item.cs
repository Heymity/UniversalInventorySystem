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
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UniversalInventorySystem
{
    [Serializable]
    public class Item
    {
        public string name;
        public int id;
        public Sprite sprite;

        public int maxAmount;
        public bool destroyOnUse;
        public int useHowManyWhenUsed;

        public bool stackable;
        public int durability;
        public bool hasDurability;
        public bool showAmount;

        [DontValidateOnValueEqual]
        public List<DurabilityImage> durabilityImages
        {
            get
            {
                return SortDurabilityImages(_durabilityImages);
            }
            set
            {
                _durabilityImages = SortDurabilityImages(value);
            }
        }
        [SerializeField, DontValidateOnValueEqual]
        private List<DurabilityImage> _durabilityImages;

        public MonoScript onUseFunc;
        public MonoScript optionalOnDropBehaviour;

        [DontValidateOnValueEqual]
        public ToolTipInfo tooltip;

        public virtual void OnUse(Inventory inv, int slot)
        {
            InventoryHandler.UseItemEventArgs uea = new InventoryHandler.UseItemEventArgs(inv, this, slot);
            if (onUseFunc == null)
            {
                InventoryHandler.current.Broadcast(BroadcastEventType.UseItem, uea: uea);
                return;
            }

            object[] tmp = new object[2] { this, uea };

            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            MethodInfo monoMethod = onUseFunc.GetClass().GetMethod("OnUse", flags);
            if (monoMethod == null) Debug.LogError($"The script provided ({onUseFunc.name}) on item {name} does not contain, or its not accesible, the expected function OnUse.\n Check if this function exists and if the provided script derives from IUsable");
            else monoMethod.Invoke(Activator.CreateInstance(onUseFunc.GetClass()), tmp);

            InventoryHandler.current.Broadcast(BroadcastEventType.UseItem, uea: uea);
        }

        public virtual void OnDrop(Inventory inv, bool fromSpecificSlot, int slot, int amount, bool dbui, Vector3? pos)
        {
            if ((inv.interactiable & InventoryController.DropInvFlags) != InventoryController.DropInvFlags) return;

            if (optionalOnDropBehaviour == null)
            {
                InventoryHandler.DropItemEventArgs dea = new InventoryHandler.DropItemEventArgs(inv, fromSpecificSlot, slot, this, amount, dbui, pos.GetValueOrDefault(), true);

                InventoryHandler.current.Broadcast(BroadcastEventType.DropItem, dea: dea);
            }
            else
            {
                InventoryHandler.DropItemEventArgs dea = new InventoryHandler.DropItemEventArgs(inv, fromSpecificSlot, slot, this, amount, dbui, pos.GetValueOrDefault(), false);
                object[] tmp = new object[2] { this, dea };

                BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

                MethodInfo monoMethod = optionalOnDropBehaviour.GetClass().GetMethod("OnDropItem", flags);

                if (monoMethod == null) Debug.LogError($"The script provided ({optionalOnDropBehaviour.name}) on item {name} does not contain, or its not accesible, the expected function OnDropItem.\n Check if this function exists and if the provided script derives from DropBehaviour");
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

        public bool ValueEqual(Item second) => ValueEqual(this, second);

        public static bool ValueEqual(Item first, Item second)
        {
            if ((first?.Equals(null) ?? true) ^ (second?.Equals(null) ?? true)) return false;
            else if ((first?.Equals(null) ?? true) && (second?.Equals(null) ?? true)) return true;
            if (first.GetType() != second.GetType()) return false;
            if (first.Equals(second)) return true;

            BindingFlags bf = BindingFlags.Public | BindingFlags.Instance;
            MemberTypes mt = MemberTypes.Field | MemberTypes.Property;
            MemberInfo[] publicMembers = first.GetType().FindMembers(mt, bf, ValidatePublicFunc, null);
            MemberInfo[] privateMembers = first.GetType().FindMembers(mt, BindingFlags.NonPublic | BindingFlags.Instance, ValidationFunc, null);

            List<MemberInfo> members = new List<MemberInfo>();
            members.AddRange(publicMembers);
            members.AddRange(privateMembers);

            foreach(MemberInfo mi in members)
            {
                switch (mi.MemberType)
                {
                    case MemberTypes.Field:
                        var field = mi as FieldInfo;
                        var firstField = field.GetValue(first);
                        var secondField = field.GetValue(second);
                        ///Debug.Log($"Bool: {!firstField.Equals(secondField)} Values: {firstField}, {secondField}"); //This line will give error when monoscripts are null. Just a note ;)
                        if (firstField == null)
                        {
                            if (secondField == null)
                            {
                                break;
                            }
                            return false;
                        }
                        if (!firstField.Equals(secondField)) 
                            return false;
                        break;
                    case MemberTypes.Property:
                        var prop = mi as PropertyInfo;
                        var firstProp = prop.GetValue(first);
                        var secondProp = prop.GetValue(second);
                        ///Debug.Log($"Bool: {!firstField.Equals(secondField)} Values: {firstField}, {secondField}"); //This line will give error when monoscripts are null. Just a note ;)
                        if (firstProp == null)
                        {
                            if (secondProp == null)
                            {
                                break;
                            }
                            return false;
                        }
                        if (!firstProp.Equals(secondProp))
                            return false;
                        break;
                }
            }

            return true;
        }

        protected static string[] dontValidate = new string[1] { "name" };
        protected static bool ValidationFunc(MemberInfo mi, object search)
        {
            foreach (string s in dontValidate)
                if (mi.Name == s) return false;

            if (mi.GetCustomAttribute<ValidateOnValueEqualAttribute>() != null)
                return true;
            return false;
        }

        protected static bool ValidatePublicFunc(MemberInfo mi, object search)
        {
            foreach(string s in dontValidate)
                if (mi.Name == s) return false;

            if (mi.GetCustomAttribute<DontValidateOnValueEqualAttribute>() != null)
                return false;
            return true;
        }

        public Item ShallowCopy() => (Item)MemberwiseClone();

        public Item()
        {
            name = "New Item";
            id = name.GetHashCode();
            sprite = null;

            maxAmount = 0;
            destroyOnUse = false;
            useHowManyWhenUsed = 0;

            stackable = false;
            durability = 0;
            hasDurability = false;
            showAmount = false;
            _durabilityImages = null;
            durabilityImages = null;


            onUseFunc = null;
            optionalOnDropBehaviour = null;

            tooltip = null;
        }

        public static bool operator false(Item item) => item == null;
        public static bool operator !(Item item) => item == null;
        public static bool operator true(Item item) => item != null;

        public static bool operator ==(Item a, ItemReference b) => a == (b?.Value ?? null);
        public static bool operator !=(Item a, ItemReference b) => a != (b?.Value ?? null);

        public static bool operator ==(Item a, Item b) => ValueEqual(a, b);
        public static bool operator !=(Item a, Item b) => !ValueEqual(a, b);

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    
    [Serializable]
    public class DurabilityImage : object
    {
        [SerializeField] public string imageName;
        [SerializeField] public Sprite sprite;
        [SerializeField] public int durability;
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ValidateOnValueEqualAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DontValidateOnValueEqualAttribute : Attribute { }
}
