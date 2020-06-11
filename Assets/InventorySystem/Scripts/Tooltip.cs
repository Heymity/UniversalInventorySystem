using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UniversalInventorySystem 
{ 
    public class Tooltip : MonoBehaviour
    {
        [HideInInspector] public Canvas canvas;
        [HideInInspector] public InventoryUI invUI;
        [HideInInspector] public int slotNum;
        GameObject toolTip;

        private void Update()
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {
                var item = invUI.GetInventory().slots[slotNum].item;
                if (item == null) return;
                if (item.tooltip == null) return;
                if (!item.tooltip.useTooltip) return;

                if (!toolTip)
                {
                    if (!item.tooltip.usePrefab)
                    {
                        toolTip = new GameObject();
                        toolTip.transform.SetParent(canvas.transform);
                        toolTip.name = $"Tooltip {invUI.name} {name} {Random.Range(int.MinValue, int.MaxValue)}";

                        var sr = toolTip.AddComponent<Image>();
                        sr.raycastTarget = false;
                        sr.sprite = item.tooltip.sprite;
                        sr.color = item.tooltip.backgroudColor;

                        var toolTipText = new GameObject();
                        toolTipText.name = "title";
                        toolTipText.transform.SetParent(toolTip.transform);

                        var tmp = toolTipText.AddComponent<TextMeshProUGUI>();
                        tmp.text = item.tooltip.title;
                        tmp.color = item.tooltip.color;
                        tmp.raycastTarget = false;
                        (tmp.transform as RectTransform).sizeDelta = new Vector2(tmp.preferredWidth, tmp.preferredHeight);
                        tmp.alignment = TextAlignmentOptions.Center;

                        Vector2 padding = new Vector2(item.tooltip.padding.x * 10, item.tooltip.padding.y * 10);

                        var rt = toolTip.transform as RectTransform;
                        rt.localScale = new Vector3(item.tooltip.size.x / 2f, item.tooltip.size.y / 2f, 1);
                        rt.sizeDelta = new Vector2(tmp.preferredWidth + padding.x, tmp.preferredHeight + padding.y);
                    }
                    else 
                    {
                        toolTip = Instantiate(item.tooltip.tooltipPrefab, canvas.transform);
                        toolTip.name = $"Tooltip {invUI.name} {name} {Random.Range(int.MinValue, int.MaxValue)}";
                    }
                }

                Vector3 tooltipPos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
               toolTip.transform.position = tooltipPos;

                switch (item.tooltip.xalign)
                {
                    case XAligment.right:
                        (toolTip.transform as RectTransform).localPosition += new Vector3((toolTip.transform as RectTransform).rect.width / 4, 0, 0);
                        goto case XAligment.center;
                    case XAligment.center:
                        (toolTip.transform as RectTransform).localPosition += new Vector3(item.tooltip.margin.x, 0, 0);
                        break;
                    case XAligment.left:
                        (toolTip.transform as RectTransform).localPosition -= new Vector3((toolTip.transform as RectTransform).rect.width / 4, 0, 0);
                        goto case XAligment.center;
                }

                switch (item.tooltip.yalign)
                {
                    case YAligment.up:
                        (toolTip.transform as RectTransform).localPosition += new Vector3(0, (toolTip.transform as RectTransform).rect.height / 4, 0);
                        goto case YAligment.center;
                    case YAligment.center:
                        (toolTip.transform as RectTransform).localPosition += new Vector3(0, item.tooltip.margin.y, 0);
                        break;
                    case YAligment.down:
                        (toolTip.transform as RectTransform).localPosition -= new Vector3(0, (toolTip.transform as RectTransform).rect.height / 4, 0);
                        goto case YAligment.center;
                }
            }
            else
            {
                Destroy(toolTip, 0.0000001f);
                toolTip = null;
            }
        }
    }

    [System.Serializable]
    public class ToolTipInfo
    {
        public bool useTooltip;

        public bool usePrefab;
        public GameObject tooltipPrefab;

        public Sprite sprite;
        public XAligment xalign;
        public YAligment yalign;
        public Color backgroudColor;

        public Vector2 size;
        public Vector2 padding;
        public Vector2 margin;

        public string title;
        [TextArea] public string description;
        public Color color;
    }

    public enum XAligment
    {
        left = 0,
        center = 1,
        right = 2
    }
    public enum YAligment
    {
        up = 0,
        center = 1,
        down = 2
    }
}
