using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Slots config")]
    public bool generateUIFromSlotPrefab;
    public GameObject generatedUIParent;
    [Tooltip("Make sure that the first child is a image and the second a text")]
    public GameObject slotPrefab;
    [Space]
    public Canvas canvas;
    [Space]
    public GameObject[] slots;
    [Space] [Header("Toggle inventory")]
    public bool hideInventory;
    [Tooltip("There is no need to assign this varible if hideInventory is set to False")]
    public KeyCode toggleKey;
    public GameObject togglableObject;
    [Space] [Header("Inventory")]
    public Inventory inv;

    public void SetInventory(Inventory _inv) => inv = _inv;
    public Inventory GetInventory() => inv;

    public void Start()
    {
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

    }

    public List<GameObject> GenerateUI(int slotAmount)
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

            slots[i].GetComponent<Button>().onClick.RemoveAllListeners();
            var index = i;
            slots[i].GetComponent<Button>().onClick.AddListener(() =>
            {
                Debug.Log($"Slot {slots[index].name} was clicked");
                inv.UseItemInSlot(index);             
            });
        }
    }

}
