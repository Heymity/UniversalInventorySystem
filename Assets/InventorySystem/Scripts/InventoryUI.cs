using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

//Todo
//Transparent slots when dragging

[System.Serializable]
public class InventoryUI : MonoBehaviour
{

    public bool generateUIFromSlotPrefab;
    public GameObject generatedUIParent;

    public GameObject slotPrefab;

    public Canvas canvas;

    public GameObject DontDropItemRect;

    public GameObject[] slots;

    public GameObject dragObj;

    public Color outlineColor;

    public float outlineSize;

    public bool hideInventory;

    public KeyCode toggleKey;
    public GameObject togglableObject;

    public Inventory inv;

    [HideInInspector]
    public bool isDraging;
    [HideInInspector]
    public int? dragSlotNumber = null;
    [HideInInspector]
    public bool shouldSwap;
    public void SetInventory(Inventory _inv) => inv = _inv;
    public Inventory GetInventory() => inv;
    
    public void Start()
    {
        if (!inv.hasInitializated) inv.InitializeInventory();
        InventoryEventsItemsHandler.current.OnDragItem += OnDragItem;

        if(inv.interactiable != IteractiableTypes.Locked)
        {
            var b = Instantiate(dragObj, canvas.transform);
            b.name = $"DRAGITEMOBJ_{name}_{UnityEngine.Random.Range(int.MinValue, int.MaxValue)}";
            b.AddComponent<DragSlot>();
            b.SetActive(false);
            dragObj = b;
        }

        InventoryController.inventoriesUI.Add(this);
        if (!generateUIFromSlotPrefab)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].name = i.ToString();
                var tmp = slots[i].transform.GetComponentInChildren<ItemDragHandler>();
                tmp.canvas = canvas;
                tmp.invUI = this;
            }
        }
        ItemDropHandler idh;
        if(!canvas.TryGetComponent(out idh)) canvas.gameObject.AddComponent<ItemDropHandler>();
    }

    List<GameObject> GenerateUI(int slotAmount)
    {
        List<GameObject> gb = new List<GameObject>();
        for(int i = 0;i < slotAmount; i++)
        {
            var g = Instantiate(slotPrefab, Vector3.zero, Quaternion.identity);
            g.transform.SetParent(generatedUIParent.transform);
            g.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            var tmp = g.transform.GetComponentInChildren<ItemDragHandler>();
            tmp.canvas = canvas;
            tmp.invUI = this;
            g.name = i.ToString();
            gb.Add(g);
        }
        slots = gb.ToArray();
        return gb;
    }

    bool hasGenerated = false;
    public void Update()
    {
        if (generateUIFromSlotPrefab && !hasGenerated)
        {
            GenerateUI(inv.slotAmounts);
            hasGenerated = true;
        }

        if (hideInventory)
        {
            if (Input.GetKeyDown(toggleKey))
            {
                togglableObject.SetActive(!togglableObject.activeInHierarchy);
            }
        }

        for (int i = 0;i < inv.slots.Count; i++)
        {
            if (inv.slots[i].item == null)
            {
                slots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                slots[i].transform.GetChild(0).GetComponent<Image>().color = new Color(0,0,0,0);
                slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
                
                continue;
            }
            slots[i].transform.GetChild(0).GetComponent<Image>().sprite = inv.slots[i].item.sprite;
            slots[i].transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1);
            slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = inv.slots[i].amount.ToString();
            if (dragObj.GetComponent<DragSlot>().GetSlotNumber() == i && isDraging)
            {
                if(inv.slots[i].amount - dragObj.GetComponent<DragSlot>().GetAmount() == 0)
                {
                    slots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                    slots[i].transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
                } else slots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (inv.slots[i].amount - dragObj.GetComponent<DragSlot>().GetAmount()).ToString();
            }

            slots[i].GetComponent<Button>().onClick.RemoveAllListeners();
            var index = i;
            slots[i].GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log($"Slot {slots[index].name} was clicked");
                inv.UseItemInSlot(index);             
            });
        }
    }

    public void OnDragItem(object sender, InventoryEventsItemsHandler.OnDragItemEventArgs e)
    {
       
    }
}
