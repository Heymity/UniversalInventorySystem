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
 *  This code is responsable for the UI of the inventory
 *  
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace UniversalInventorySystem
{
    [System.Serializable]
    public class InventoryUI : MonoBehaviour
    {
        //Slots
        public bool generateUIFromSlotPrefab;
        public GameObject generatedUIParent;

        public GameObject slotPrefab;

        public Canvas canvas;

        public GameObject DontDropItemRect;

        public List<GameObject> slots;

        public bool showAmount = true;

        public GameObject dragObj;

        public bool hideDragObj;
        public bool useOnClick;

        //Sahder
        public Color outlineColor;

        public float outlineSize;

        //Toggle inventory
        public bool hideInventory;

        public KeyCode toggleKey;
        public GameObject togglableObject;

        public bool dropOnCloseCrafting = false;
        public Vector3 dropPos = Vector3.zero;
        public Vector3 randomFactor = Vector3.zero;

        //Inv
        public InventoryReference inv;

        //Craft
        public bool isCraftInventory;

        public Vector2Int gridSize;

        public bool allowsPatternCrafting;

        public int[] productSlotsIndex;

        [HideInInspector]
        public bool isDraging;
        [HideInInspector]
        public int? dragSlotNumber = null;
        [HideInInspector]
        public bool shouldSwap;
        [HideInInspector]
        public List<ItemReference> pattern = new List<ItemReference>();
        [HideInInspector]
        public List<int> amount = new List<int>();

        public Inventory GetInventory() => inv.Value;

        public void Start()
        {
            if (inv == null) return;
            if (inv.Value == null) return;

            var b = Instantiate(dragObj, canvas.transform);
            b.name = $"DRAGITEMOBJ_{name}_{UnityEngine.Random.Range(int.MinValue, int.MaxValue)}";
            b.AddComponent<DragSlot>();
            b.SetActive(false);
            if (hideDragObj) b.hideFlags = HideFlags.HideInHierarchy;
            dragObj = b;

            InventoryController.inventoriesUI.Add(this);
            if (!generateUIFromSlotPrefab)
            {
                for (int i = 0; i < slots.Count; i++)
                {
                    slots[i].name = i.ToString();
                    for (int j = 0; j < slots[i].transform.childCount; j++)
                    {
                        Image image;
                        if (slots[i].transform.GetChild(j).TryGetComponent<Image>(out image))
                        {
                            ItemDragHandler drag;
                            if (slots[i].transform.GetChild(j).TryGetComponent(out drag))
                            {
                                drag.canvas = canvas;
                                drag.invUI = this;
                            }
                            else
                            {
                                drag = slots[i].transform.GetChild(j).gameObject.AddComponent<ItemDragHandler>();
                                drag.canvas = canvas;
                                drag.invUI = this;
                            }

                            Tooltip tooltip;
                            if (slots[i].transform.GetChild(j).TryGetComponent(out tooltip))
                            {
                                tooltip.canvas = canvas;
                                tooltip.invUI = this;
                                tooltip.slotNum = i;
                            } else
                            {
                                tooltip = slots[i].transform.GetChild(j).gameObject.AddComponent<Tooltip>();
                                tooltip.canvas = canvas;
                                tooltip.invUI = this;
                                tooltip.slotNum = i;
                            }
                        }
                    }
                }
            }

            if (!canvas.TryGetComponent(out ItemDropHandler _)) canvas.gameObject.AddComponent<ItemDropHandler>();
         
            if (isCraftInventory)
            {
                for (int i = 0; i < gridSize.x * gridSize.y; i++)
                {
                    pattern.Add(null);
                    amount.Add(0);
                }

                for (int i = 0; i < productSlotsIndex.Length; i++)
                { 
                    inv.Value.slots[productSlotsIndex[i]] = Slot.SetSlotProperties(inv.Value[i], true, SlotProtection.Remove | SlotProtection.Swap, null);
                }

            }
        }

        List<GameObject> GenerateUI(int slotAmount)
        {
            List<GameObject> gb = new List<GameObject>();
            for (int i = 0; i < slotAmount; i++)
            {
                var g = Instantiate(slotPrefab, Vector3.zero, Quaternion.identity);
                g.transform.SetParent(generatedUIParent.transform);
                g.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                var tmp = g.transform.GetComponentInChildren<ItemDragHandler>();
                tmp.canvas = canvas;
                tmp.invUI = this;
                g.name = i.ToString();
                gb.Add(g);

                for (int j = 0; j < g.transform.childCount; j++)
                {
                    Image image;
                    if (g.transform.GetChild(j).TryGetComponent<Image>(out image))
                    {
                        ItemDragHandler drag;
                        if (g.transform.GetChild(j).TryGetComponent(out drag))
                        {
                            drag.canvas = canvas;
                            drag.invUI = this;
                        }
                        else
                        {
                            drag = g.transform.GetChild(j).gameObject.AddComponent<ItemDragHandler>();
                            drag.canvas = canvas;
                            drag.invUI = this;
                        }

                        Tooltip tooltip;
                        if (g.transform.GetChild(j).TryGetComponent(out tooltip))
                        {
                            tooltip.canvas = canvas;
                            tooltip.invUI = this;
                            tooltip.slotNum = i;
                        }
                        else
                        {
                            tooltip = g.transform.GetChild(j).gameObject.AddComponent<Tooltip>();
                            tooltip.canvas = canvas;
                            tooltip.invUI = this;
                            tooltip.slotNum = i;
                        }
                    }
                }
            }
            slots = gb;
            return gb;
        }

        bool hasGenerated = false;
        public void Update()
        {
            if (inv == null) return;
            if (inv.Value == null) return;

            //Initialize if not yet
            if (!inv.Value.HasInitialized)
                inv.Value.Initialize();
            
            //Create UI
            if (generateUIFromSlotPrefab && !hasGenerated)
            {
                GenerateUI(inv.Value.SlotAmount);
                hasGenerated = true;
            }

            ///TODO: Add Event here
            //Hiding Inventory
            if (hideInventory)
            {
                if (Input.GetKeyDown(toggleKey) && !isDraging)
                {
                    if (isCraftInventory && dropOnCloseCrafting)
                    {
                        for (int i = 0; i < inv.Value.slots.Count; i++)
                        {
                            var item = inv.Value.slots[i];
                            Vector3 finalDropPos = dropPos;
                            finalDropPos.x += Random.Range(-randomFactor.x, randomFactor.x);
                            finalDropPos.y += Random.Range(-randomFactor.y, randomFactor.y);
                            finalDropPos.z += Random.Range(-randomFactor.z, randomFactor.z);
                            inv.Value.DropItem(item.amount, finalDropPos, slot: i);
                        }
                    }
                    togglableObject.SetActive(!togglableObject.activeInHierarchy);
                }
            }

            //Iterating slots go
            for (int i = 0; i < inv.Value.slots.Count; i++)
            {
                // Create pattern grid
                if (isCraftInventory && i < pattern.Count)
                {
                    pattern[i] = inv.Value.slots[i].itemValue;
                    amount[i] = inv.Value.slots[i].amount;
                }
                if (i >= slots.Count) break;                                        // dont know why its here but I am afraid to remove it

                // Rendering null Slot
                Image image;
                TextMeshProUGUI text;
                if (inv.Value.slots[i].Item == null)
                {
                    for (int j = 0; j < slots[i].transform.childCount; j++)
                    {

                        if (slots[i].transform.GetChild(j).TryGetComponent(out image))
                        {
                            image.sprite = null;
                            image.color = new Color(0, 0, 0, 0);
                        }
                        else if (slots[i].transform.GetChild(j).TryGetComponent(out text))
                            text.text = "";
                    }
                    continue;
                }

                // Rendering slot
                for (int j = 0; j < slots[i].transform.childCount; j++)
                {
                    if (slots[i].transform.GetChild(j).TryGetComponent(out image))
                    {
                        if (inv.Value.slots[i].ItemInstance.hasDurability)
                        {
                            if (inv.Value.slots[i].ItemInstance.durabilityImages.Count > 0)
                            {
                                image.sprite = GetNearestSprite(inv.Value, inv.Value.slots[i].durability, i);
                                image.color = new Color(1, 1, 1, 1);
                            }
                            else
                            {
                                image.sprite = inv.Value.slots[i].ItemInstance.sprite;
                                image.color = new Color(1, 1, 1, 1);
                            }
                        }
                        else
                        {
                            image.sprite = inv.Value.slots[i].ItemInstance.sprite;
                            image.color = new Color(1, 1, 1, 1);
                        }
                    }
                    else if (slots[i].transform.GetChild(j).TryGetComponent(out text) && showAmount && inv.Value[i].ItemInstance.showAmount)
                        text.text = inv.Value.slots[i].amount.ToString();
                    else if (slots[i].transform.GetChild(j).TryGetComponent(out text))
                        text.text = "";
                }

                if (dragObj.GetComponent<DragSlot>().GetSlotNumber() == i && isDraging)
                {
                    if (inv.Value.slots[i].amount - dragObj.GetComponent<DragSlot>().GetAmount() == 0)
                    {
                        for (int j = 0; j < slots[i].transform.childCount; j++)
                        {

                            if (slots[i].transform.GetChild(j).TryGetComponent(out image))
                            {
                                image.sprite = null;
                                image.color = new Color(0, 0, 0, 0);
                            }
                            else if (slots[i].transform.GetChild(j).TryGetComponent(out text))
                                text.text = "";
                        }
                    }
                    else
                    {
                        for (int j = 0; j < slots[i].transform.childCount; j++)
                        {
                            if (slots[i].transform.GetChild(j).TryGetComponent(out text) && showAmount && inv.Value[i].ItemInstance.showAmount)
                                text.text = (inv.Value.slots[i].amount - dragObj.GetComponent<DragSlot>().GetAmount()).ToString();
                            else if (slots[i].transform.GetChild(j).TryGetComponent(out text))
                                text.text = "";
                        }
                    }
                }

                if (!isCraftInventory)
                {
                    slots[i].GetComponent<Button>().onClick.RemoveAllListeners();
                    var index = i;
                    slots[i].GetComponent<Button>().onClick.AddListener(() =>
                    {
                    //Debug.Log($"Slot {slots[index].name} was clicked");
                        if (useOnClick)
                            inv.Value.UseItemInSlot(index);
                    });
                }
            }

            //Dont use on click if is crafting inventory
            if (isCraftInventory)
            {
                CraftItemData products = inv.Value.CraftItem(new CraftItemData(pattern.ToArray(), amount.ToArray()), gridSize, false, true, productSlotsIndex.Length);

                List<ItemReference> productsItem = new List<ItemReference>();
                if (products != CraftItemData.nullData && products.items.Length <= productSlotsIndex.Length)
                {
                    if (products.items.Length == productSlotsIndex.Length)
                    {
                        for (int k = 0; k < products.items.Length; k++)
                        {
                            productsItem.Add(inv.Value.slots[gridSize.x * gridSize.y + k].itemValue ?? products.items[k]);
                        }
                    }
                    else
                    {
                        for(int i = 0; i < productSlotsIndex.Length - products.items.Length + 1; i++)
                        {
                            productsItem = new List<ItemReference>();
                            for (int k = 0; k < products.items.Length; k++)
                            {
                                if (gridSize.x * gridSize.y + k + i >= inv.Value.slots.Count) break;
                                if (inv.Value.slots[gridSize.x * gridSize.y + k + i].Item == products.items[k] || inv.Value.slots[gridSize.x * gridSize.y + k + i].Item == null)
                                {
                                    productsItem.Add(inv.Value.slots[gridSize.x * gridSize.y + k + i].itemValue ?? products.items[k]);
                                    if (Enumerable.SequenceEqual(products.items, productsItem.ToArray()))
                                    {
                                        i = int.MaxValue - 1;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                }

                int productIndex = 0;
                for (int i = 0; i < productSlotsIndex.Length; i++)
                {
                    // If there is a item in the product slot it renders it and go to the next one
                    if (inv.Value.slots[productSlotsIndex[i]].HasItem)
                    {
                        // Iterating the childs
                        for (int j = 0; j < slots[productSlotsIndex[i]].transform.childCount; j++)
                        {
                            if (slots[productSlotsIndex[i]].transform.GetChild(j).TryGetComponent(out Image image))
                            {
                                // Having durability it renders the corresponding durability image
                                if (inv.Value.slots[productSlotsIndex[i]].ItemInstance.hasDurability)
                                {
                                    if (inv.Value.slots[productSlotsIndex[i]].ItemInstance.durabilityImages.Count > 0)
                                    {
                                        image.sprite = GetNearestSprite(inv.Value, inv.Value.slots[productSlotsIndex[i]].durability, productSlotsIndex[i]);
                                        image.color = new Color(1, 1, 1, 1);
                                    }
                                    else
                                    {
                                        image.sprite = inv.Value.slots[productSlotsIndex[i]].ItemInstance.sprite;
                                        image.color = new Color(1, 1, 1, 1);
                                    }
                                    //productIndex++;
                                }
                                else
                                {
                                    image.sprite = inv.Value.slots[productSlotsIndex[i]].ItemInstance.sprite;
                                    image.color = new Color(1, 1, 1, 1);
                                    //productIndex++;
                                }
                            }
                            else if (slots[productSlotsIndex[i]].transform.GetChild(j).TryGetComponent(out TextMeshProUGUI text) && showAmount && inv.Value[productSlotsIndex[i]].ItemInstance.showAmount)
                                text.text = inv.Value.slots[productSlotsIndex[i]].amount.ToString();
                            else if (slots[productSlotsIndex[i]].transform.GetChild(j).TryGetComponent(out text))
                                text.text = "";
                        }

                        if(products != null && products != CraftItemData.nullData)
                            if (inv.Value[productSlotsIndex[i]].Item == (products?.items[productIndex] ?? null) && 
                                inv.Value[productSlotsIndex[i]].amount + (products?.amounts[productIndex] ?? int.MaxValue) <= inv.Value[productSlotsIndex[i]].Item?.maxAmount
                                ) 
                                productIndex++;

                        //For click and drag
                        slots[productSlotsIndex[i]].GetComponent<Button>().onClick.RemoveAllListeners();
                        var index = i;
                        slots[productSlotsIndex[i]].GetComponent<Button>().onClick.AddListener(() =>
                        {
                            if (products.items != null && products.items.Length <= productSlotsIndex.Length)
                            {
                                // If you dont want the other of the items in the product slot to matter this line should be different, It shoud check if the
                                // List have the same items, not the same sequence.
                                if (Enumerable.SequenceEqual(products.items, productsItem.ToArray()))
                                {
                                    inv.Value.CraftItem(new CraftItemData(pattern.ToArray(), amount.ToArray()), gridSize, true, true, productSlotsIndex.Length);
                                }
                            }
                        });
                    }
                    else if (products.items != null && products.items.Length <= productSlotsIndex.Length && productIndex < products.items.Length)
                    {
                        bool nextIndex = false;
                        for (int j = 0; j < slots[productSlotsIndex[i]].transform.childCount; j++)
                        {
                            // Iterating the childs
                            if (slots[productSlotsIndex[i]].transform.GetChild(j).TryGetComponent(out Image image))
                            {
                                if (products.items[productIndex].Value.hasDurability)
                                {
                                    if (products.items[productIndex].Value.durabilityImages.Count > 0)
                                    {
                                        image.sprite = GetNearestSprite(products.items[productIndex], products.items[productIndex].Value.durability);
                                        image.color = new Color(1, 1, 1, .7f);
                                    }
                                    else
                                    {
                                        image.sprite = products.items[productIndex].Value.sprite;
                                        image.color = new Color(1, 1, 1, .7f);
                                    }
                                    nextIndex = true;
                                }
                                else
                                {
                                    image.sprite = products.items[productIndex].Value.sprite;
                                    image.color = new Color(1, 1, 1, .7f);
                                    nextIndex = true;
                                }
                            }
                            else if (slots[productSlotsIndex[i]].transform.GetChild(j).TryGetComponent(out TextMeshProUGUI text) && showAmount && products.items[productIndex].Value.showAmount)
                            {
                                text.text = products.amounts[productIndex].ToString();
                                nextIndex = true;
                            }
                            else if (slots[productSlotsIndex[i]].transform.GetChild(j).TryGetComponent(out text))
                            {
                                text.text = "";
                                nextIndex = true;
                            }
                        }

                        if(nextIndex)
                            productIndex++;

                        //For click and drag
                        slots[productSlotsIndex[i]].GetComponent<Button>().onClick.RemoveAllListeners();
                        var index = i;
                        slots[productSlotsIndex[i]].GetComponent<Button>().onClick.AddListener(() =>
                        {
                            //Debug.Log($"Product slot {slots[index].name} was clicked");
                            inv.Value.CraftItem(new CraftItemData(pattern.ToArray(), amount.ToArray()), gridSize, true, true, productSlotsIndex.Length);
                        });

                    }
                    else
                    {
                        if (!inv.Value.slots[productSlotsIndex[i]].HasItem)
                        {
                            for (int j = 0; j < slots[i].transform.childCount; j++)
                            {
                                if (slots[productSlotsIndex[i]].transform.GetChild(j).TryGetComponent(out Image image))
                                {
                                    image.sprite = null;
                                    image.color = new Color(0, 0, 0, 0);
                                }
                                else if (slots[productSlotsIndex[i]].transform.GetChild(j).TryGetComponent(out TextMeshProUGUI text))
                                    text.text = "";
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Get nearest sprite of durability
        /// </summary>
        /// <param name="inv">inventory</param>
        /// <param name="durability">Usualy the durability in the slot</param>
        /// <param name="slot">slot number</param>
        /// <returns>The nearest Sprite</returns>
        public static Sprite GetNearestSprite(Inventory inv, int durability, int slot)
        {
            var minDif = int.MaxValue;
            var index = 0;
            for(int i = inv.slots[slot].Item.durabilityImages.Count - 1; i >= 0;i--)
            {
                int dif = inv.slots[slot].Item.durabilityImages[i].durability - durability;
                if (dif < 0) break;
                if (dif < minDif)
                {
                    minDif = dif;
                    index = i;
                }
            }
            return inv.slots[slot].Item.durabilityImages[index].sprite;
        }

        /// <summary>
        /// Get nearest sprite of durability
        /// </summary>
        /// <param name="item">The item with durability</param>
        /// <param name="durability">The atual durability</param>
        /// <returns>The nearest Sprite</returns>
        public static Sprite GetNearestSprite(Item item, int durability)
        {
            var minDif = int.MaxValue;
            var index = 0;
            for (int i = item.durabilityImages.Count - 1; i >= 0; i--)
            {
                int dif = item.durabilityImages[i].durability - durability;
                if (dif < 0) break;
                if (dif < minDif)
                {
                    minDif = dif;
                    index = i;
                }
            }
            return item.durabilityImages[index].sprite;
        }

        public void OnDisable()
        {
            InventoryController.inventoriesUI.Remove(this);
        }
    }
}