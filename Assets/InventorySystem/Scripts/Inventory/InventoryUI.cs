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
        public Inventory inv;

        //Craft
        public bool isCraftInventory;

        public Vector2Int gridSize;

        public bool allowsPatternCrafting;

        public GameObject[] productSlots;

        [HideInInspector]
        public bool isDraging;
        [HideInInspector]
        public int? dragSlotNumber = null;
        [HideInInspector]
        public bool shouldSwap;
        [HideInInspector]
        public List<Item> pattern = new List<Item>();
        public List<int> amount = new List<int>();

        public void SetInventory(Inventory _inv) => inv = _inv;
        public Inventory GetInventory() => inv;

        public void Start()
        {
            if (isCraftInventory)
            {
                inv.slotAmounts += productSlots.Length;

                foreach (GameObject g in productSlots)
                {
                    slots.Add(g);
                }
            }

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
            ItemDropHandler idh;
            if (!canvas.TryGetComponent(out idh)) canvas.gameObject.AddComponent<ItemDropHandler>();

            if (isCraftInventory)
            {
                for (int i = 0; i < gridSize.x * gridSize.y; i++)
                {
                    pattern.Add(null);
                    amount.Add(0);
                }

                for (int i = gridSize.x * gridSize.y; i < inv.slots.Count; i++)
                {
                    var slot = inv.slots[i];
                    inv.slots[i] = new Slot(slot.item, slot.amount, slot.hasItem, true, SlotProtection.OnlyRemove, null);
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
            if (!inv.hasInitializated)
                inv.Initialize();
            
            if (generateUIFromSlotPrefab && !hasGenerated)
            {
                GenerateUI(inv.slotAmounts);
                hasGenerated = true;
            }

            if (hideInventory)
            {
                if (Input.GetKeyDown(toggleKey) && !isDraging)
                {
                    if (isCraftInventory && dropOnCloseCrafting)
                    {
                        for (int i = 0; i < inv.slots.Count; i++)
                        {
                            var item = inv.slots[i];
                            Vector3 finalDropPos = dropPos;
                            finalDropPos.x += Random.Range(-randomFactor.x, randomFactor.x);
                            finalDropPos.y += Random.Range(-randomFactor.y, randomFactor.y);
                            finalDropPos.z += Random.Range(-randomFactor.z, randomFactor.z);
                            inv.DropItem(item.amount, finalDropPos, slot: i);
                        }
                    }
                    togglableObject.SetActive(!togglableObject.activeInHierarchy);
                }
            }

            for (int i = 0; i < inv.slots.Count; i++)
            {
                if (isCraftInventory && i < pattern.Count)
                {
                    pattern[i] = inv.slots[i].item;
                    amount[i] = inv.slots[i].amount;
                }
                if (i >= slots.Count) break;
                Image image;
                TextMeshProUGUI text;
                if (inv.slots[i].item == null)
                {
                    for (int j = 0; j < slots[i].transform.childCount; j++)
                    {

                        if (slots[i].transform.GetChild(j).TryGetComponent<Image>(out image))
                        {
                            image.sprite = null;
                            image.color = new Color(0, 0, 0, 0);
                        }
                        else if (slots[i].transform.GetChild(j).TryGetComponent(out text))
                            text.text = "";
                    }
                    continue;
                }

                for (int j = 0; j < slots[i].transform.childCount; j++)
                {

                    if (slots[i].transform.GetChild(j).TryGetComponent<Image>(out image))
                    {
                        if (inv.slots[i].item.hasDurability)
                        {
                            if (inv.slots[i].item.durabilityImages.Count > 0)
                            {
                                image.sprite = GetNearestSprite(inv, inv.slots[i].durability, i);
                                image.color = new Color(1, 1, 1, 1);
                            }
                        }
                        else
                        {
                            image.sprite = inv.slots[i].item.sprite;
                            image.color = new Color(1, 1, 1, 1);
                        }
                    }
                    else if (slots[i].transform.GetChild(j).TryGetComponent(out text))
                        text.text = inv.slots[i].amount.ToString();
                }

                if (dragObj.GetComponent<DragSlot>().GetSlotNumber() == i && isDraging)
                {
                    if (inv.slots[i].amount - dragObj.GetComponent<DragSlot>().GetAmount() == 0)
                    {
                        for (int j = 0; j < slots[i].transform.childCount; j++)
                        {

                            if (slots[i].transform.GetChild(j).TryGetComponent<Image>(out image))
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
                            if (slots[i].transform.GetChild(j).TryGetComponent(out text))
                                text.text = (inv.slots[i].amount - dragObj.GetComponent<DragSlot>().GetAmount()).ToString();
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
                            inv.UseItemInSlot(index);
                    });
                }
            }

            if (isCraftInventory)
            {
                CraftItemData products = inv.CraftItem((pattern.ToArray(), amount.ToArray()), gridSize, false, true, productSlots.Length);

                List<Item> productsItem = new List<Item>();
                if (products != CraftItemData.nullData && products.items.Length <= productSlots.Length)
                {
                    for (int k = 0; k < products.items.Length; k++)
                    {
                        productsItem.Add(inv.slots[gridSize.x * gridSize.y + k].item ?? products.items[k]);
                    }

                }

                int productIndex = 0;
                for (int i = 0; i < productSlots.Length; i++)
                {
                    if (inv.slots[(gridSize.x * gridSize.y) + i].hasItem)
                    {
                        for (int j = 0; j < slots[(gridSize.x * gridSize.y) + i].transform.childCount; j++)
                        {
                            Image image;
                            TextMeshProUGUI text;
                            if (slots[(gridSize.x * gridSize.y) + i].transform.GetChild(j).TryGetComponent<Image>(out image))
                            {
                                image.sprite = inv.slots[(gridSize.x * gridSize.y) + i].item.sprite;
                                image.color = new Color(1, 1, 1, 1);
                                productIndex++;
                            }
                            else if (slots[(gridSize.x * gridSize.y) + i].transform.GetChild(j).TryGetComponent(out text))
                                text.text = inv.slots[(gridSize.x * gridSize.y) + i].amount.ToString();
                        }

                        //For click and drag
                        productSlots[i].GetComponent<Button>().onClick.RemoveAllListeners();
                        var index = i;
                        productSlots[i].GetComponent<Button>().onClick.AddListener(() =>
                        {
                        //Debug.Log($"Product slot {slots[index].name} was clicked");
                        if (products.items != null && products.items.Length <= productSlots.Length)
                            {
                                if (Enumerable.SequenceEqual(products.items, productsItem.ToArray()))
                                {
                                    inv.CraftItem((pattern.ToArray(), amount.ToArray()), gridSize, true, true, productSlots.Length);
                                }
                            }
                        });
                    }
                    else if (products.items != null && products.items.Length <= productSlots.Length && productIndex < products.items.Length)
                    {

                        for (int j = 0; j < slots[(gridSize.x * gridSize.y) + i].transform.childCount; j++)
                        {
                            Image image;
                            TextMeshProUGUI text;
                            if (slots[(gridSize.x * gridSize.y) + i].transform.GetChild(j).TryGetComponent(out image))
                            {
                                image.sprite = products.items[productIndex].sprite;
                                image.color = new Color(1, 1, 1, .7f);
                                productIndex++;
                            }
                            else if (productSlots[i].transform.GetChild(j).TryGetComponent(out text))
                                text.text = products.amounts[i].ToString();
                        }

                        //For click and drag
                        productSlots[i].GetComponent<Button>().onClick.RemoveAllListeners();
                        var index = i;
                        productSlots[i].GetComponent<Button>().onClick.AddListener(() =>
                        {
                        //Debug.Log($"Product slot {slots[index].name} was clicked");
                        inv.CraftItem((pattern.ToArray(), amount.ToArray()), gridSize, true, true, productSlots.Length);
                        });

                    }
                    else
                    {
                        if (!inv.slots[(gridSize.x * gridSize.y) + i].hasItem)
                        {
                            for (int j = 0; j < slots[i].transform.childCount; j++)
                            {
                                Image image;
                                TextMeshProUGUI text;
                                if (slots[(gridSize.x * gridSize.y) + i].transform.GetChild(j).TryGetComponent<Image>(out image))
                                {
                                    image.sprite = null;
                                    image.color = new Color(0, 0, 0, 0);
                                }
                                else if (productSlots[i].transform.GetChild(j).TryGetComponent(out text))
                                    text.text = "";
                            }
                        }
                    }
                }
            }
        }

        public static Sprite GetNearestSprite(Inventory inv, int durability, int slot)
        {
            var minDif = int.MaxValue;
            var index = 0;
            for(int i = inv.slots[slot].item.durabilityImages.Count - 1; i >= 0;i--)
            {
                int dif = inv.slots[slot].item.durabilityImages[i].durability - durability;
                if (dif < 0) break;
                if (dif < minDif)
                {
                    minDif = dif;
                    index = i;
                }
            }
            return inv.slots[slot].item.durabilityImages[index].sprite;
        }
    }
}