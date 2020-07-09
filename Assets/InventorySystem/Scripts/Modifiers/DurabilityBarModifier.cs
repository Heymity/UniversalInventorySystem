using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniversalInventorySystem;

public class DurabilityBarModifier : BaseUIModifier
{
    public Gradient gradient;
    public Color BackgroundColor;
    public Material mat;
    [Range(0, 100)]
    public float percentageX;
    [Range(0, 100)]
    public float percentageY;

    public Vector2 offset;

    List<(int index, GameObject go)> gos = new List<(int index, GameObject go)>();

    public void LateUpdate()
    {
        for(int i = 0; i < target.slots.Count; i++)
        {
            if (!target.GetInventory()[i]) continue;
            if (!target.GetInventory()[i].item.hasDurability) continue;

            bool hasChild = false;
            for(int j = 0; j < gos.Count; j++)
            {
                if (gos[j].index == i) hasChild = true;
            }

            if (!hasChild)
            {
                GameObject bar = new GameObject();
                bar.transform.SetParent(target.slots[i].transform);
                bar.name = "DurabilityBar";
                var img = bar.AddComponent<Image>();
                img.sprite = null;
                img.raycastTarget = false;
                
                img.material = new Material(mat);

                Vector2 newSize = new Vector2(
                    (target.slots[i].transform as RectTransform).sizeDelta.x * (percentageX / 100), 
                    (target.slots[i].transform as RectTransform).sizeDelta.y * (percentageY / 100)
                );
                (bar.transform as RectTransform).sizeDelta = newSize; 
                (bar.transform as RectTransform).localScale = (target.slots[i].transform as RectTransform).localScale;

                Vector2 newPos = new Vector2(
                    (target.slots[i].transform as RectTransform).position.x + offset.x,
                    (target.slots[i].transform as RectTransform).position.y + offset.y
                 );
                (bar.transform as RectTransform).position = newPos; 
                gos.Add((i, bar));
            }
        }

        GoBack:
        for(int i = 0; i < gos.Count; i++)
        {
            if (!target.GetInventory()[gos[i].index].hasItem)
            {
                Destroy(gos[i].go, 0.00000000001f);
                gos.RemoveAt(i);
                goto GoBack;
            }
            if(!target.GetInventory()[gos[i].index].item.hasDurability)
            {
                Destroy(gos[i].go, 0.00000000001f);
                gos.RemoveAt(i);
                goto GoBack;
            }

            float percentage = (float)target.GetInventory()[gos[i].index].durability / (float)target.GetInventory()[gos[i].index].item.maxDurability;
            var img = gos[i].go.GetComponent<Image>();
            img.sprite = null;
            img.material.SetFloat("_FillAmount", percentage);
            img.material.SetColor("_Color", gradient.Evaluate(percentage));
            img.material.SetColor("_BackGroundColor", BackgroundColor);

            Vector2 newSize = new Vector2(
                    (target.slots[gos[i].index].transform as RectTransform).sizeDelta.x * (percentageX / 100),
                    (target.slots[gos[i].index].transform as RectTransform).sizeDelta.y * (percentageY / 100)
                );
            (gos[i].go.transform as RectTransform).sizeDelta = newSize;

            Vector2 newPos = new Vector2(
                    (target.slots[gos[i].index].transform as RectTransform).position.x + offset.x,
                    (target.slots[gos[i].index].transform as RectTransform).position.y + offset.y
                 );
            (gos[i].go.transform as RectTransform).position = newPos;
        }
    }
}
