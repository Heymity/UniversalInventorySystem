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
 *  This script handles the tooltip of items
 *  
 */ 

using System.Collections.Generic;
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
        RectTransform tooltipRect;

        private void Update()
        {
            if (invUI == null) return;
            if (invUI.GetInventory() == null) return;
            if (RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {
                var item = invUI.GetInventory().slots[slotNum].Item;
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

                        Vector2 padding = new Vector2(item.tooltip.padding.x * 10, item.tooltip.padding.y * 10);

                        float height = 0;
                        float width = 0;

                        List<GameObject> tttexts = new List<GameObject>();
                        for (int i = 0; i < item.tooltip.texts.Count; i++)
                        {
                            var toolTipText = new GameObject();
                            toolTipText.name = $"text {i}";
                            toolTipText.transform.SetParent(toolTip.transform);

                            var tmp = toolTipText.AddComponent<TextMeshProUGUI>();
                            tmp.font = item.tooltip.texts[i].font;
                            tmp.text = item.tooltip.texts[i].text;
                            tmp.color = item.tooltip.texts[i].color;
                            tmp.fontSize = item.tooltip.texts[i].fontSize;
                            tmp.raycastTarget = false;
                            tmp.fontStyle = item.tooltip.texts[i].fontStyles;
                            tmp.alignment = item.tooltip.texts[i].alignOptions;
                            (tmp.transform as RectTransform).sizeDelta = new Vector2(tmp.preferredWidth <= item.tooltip.maxWidth - Mathf.Abs(padding.x) ? tmp.preferredWidth : item.tooltip.maxWidth - Mathf.Abs(padding.x), tmp.preferredHeight);

                            tttexts.Add(toolTipText);

                            height += tmp.preferredHeight;
                            width = width <= tmp.preferredWidth ? tmp.preferredWidth : _ = width;
                        }

                        float tmpheight = 0;
                        for(int i = 0; i < tttexts.Count;i++)
                        {
                            GameObject g = tttexts[i];
                            float addingheight = -(tmpheight - (height / 2) + g.GetComponent<TextMeshProUGUI>().preferredHeight / 2);

                            switch (item.tooltip.texts[i].aligmentOption)
                            {
                                case AligmentOption.percentage:
                                    (g.transform as RectTransform).anchorMin = new Vector2(
                                        item.tooltip.texts[i].pixelOrPercentage / 100,
                                        (g.transform as RectTransform).anchorMin.y
                                    );
                                    (g.transform as RectTransform).anchorMax = new Vector2(
                                        item.tooltip.texts[i].pixelOrPercentage / 100,
                                        (g.transform as RectTransform).anchorMax.y
                                    );
                                    switch (item.tooltip.texts[i].pivot)
                                    {
                                        case XAligment.center:
                                            (g.transform as RectTransform).localPosition += new Vector3(0, addingheight, 0);
                                            break;
                                        case XAligment.left:
                                            (g.transform as RectTransform).localPosition += new Vector3(((g.transform as RectTransform).sizeDelta.x / 2) + item.tooltip.margin.x, addingheight, 0);
                                            break;
                                        case XAligment.right:
                                            (g.transform as RectTransform).localPosition += new Vector3(-((g.transform as RectTransform).sizeDelta.x / 2) - item.tooltip.margin.x, addingheight, 0);
                                            break;
                                    }
                                    break;

                                case AligmentOption.pixel:
                                    switch (item.tooltip.texts[i].pivot)
                                    {
                                        case XAligment.center:
                                            (g.transform as RectTransform).localPosition += new Vector3(item.tooltip.texts[i].pixelOrPercentage, addingheight, 0);
                                            break;
                                        case XAligment.right:
                                            (g.transform as RectTransform).localPosition += new Vector3(((g.transform as RectTransform).sizeDelta.x / 2) + item.tooltip.margin.x + item.tooltip.texts[i].pixelOrPercentage, addingheight, 0);
                                            break;
                                        case XAligment.left:
                                            (g.transform as RectTransform).localPosition += new Vector3(-((g.transform as RectTransform).sizeDelta.x / 2) - item.tooltip.margin.x + item.tooltip.texts[i].pixelOrPercentage, addingheight, 0);
                                            break;
                                    }
                                    break;

                                case AligmentOption.preDefined:
                                    switch (item.tooltip.texts[i].pivot)
                                    {
                                        case XAligment.center:
                                            (g.transform as RectTransform).localPosition += new Vector3(0, addingheight, 0);
                                            break;
                                        case XAligment.left:
                                            (g.transform as RectTransform).anchorMin = new Vector2(0, (g.transform as RectTransform).anchorMin.y);
                                            (g.transform as RectTransform).anchorMax = new Vector2(0, (g.transform as RectTransform).anchorMax.y);
                                            (g.transform as RectTransform).localPosition += new Vector3(((g.transform as RectTransform).sizeDelta.x / 2) + item.tooltip.margin.x, addingheight, 0);
                                            break;
                                        case XAligment.right:
                                            (g.transform as RectTransform).anchorMin = new Vector2(1, (g.transform as RectTransform).anchorMin.y);
                                            (g.transform as RectTransform).anchorMax = new Vector2(1, (g.transform as RectTransform).anchorMax.y);
                                            (g.transform as RectTransform).localPosition += new Vector3(-((g.transform as RectTransform).sizeDelta.x / 2) - item.tooltip.margin.x, addingheight, 0);
                                            break;
                                    }
                                    break;
                            }
                            tmpheight += g.GetComponent<TextMeshProUGUI>().preferredHeight;
                        }

                        height += padding.x;
                        width += padding.y;

                        width = width <= item.tooltip.maxWidth ? width : item.tooltip.maxWidth;

                        var rt = toolTip.transform as RectTransform;
                        rt.localScale = new Vector3(item.tooltip.size.x / 2f, item.tooltip.size.y / 2f, 1);
                        rt.sizeDelta = new Vector2(width, height);
                    }
                    else 
                    {
                        toolTip = Instantiate(item.tooltip.tooltipPrefab, canvas.transform);
                        toolTip.name = $"Tooltip {invUI.name} {name} {Random.Range(int.MinValue, int.MaxValue)}";
                    }
                }
                Vector3 tooltipPos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
               toolTip.transform.position = tooltipPos;

                switch (item.tooltip.xAligmentOption)
                {
                    case AligmentOption.percentage:
                        switch (item.tooltip.xAlign)
                        {
                            case XAligment.right:
                                (toolTip.transform as RectTransform).localPosition += new Vector3((toolTip.transform as RectTransform).rect.width / 2 * toolTip.transform.localScale.x, 0, 0);
                                (toolTip.transform as RectTransform).localPosition -= new Vector3((item.tooltip.xPixelOrPercentage / 100) * ((toolTip.transform as RectTransform).rect.width), 0, 0);
                                (toolTip.transform as RectTransform).localPosition += new Vector3(item.tooltip.margin.x, 0, 0);
                                break;
                            case XAligment.center:
                                (toolTip.transform as RectTransform).localPosition -= new Vector3(((toolTip.transform as RectTransform).rect.width / toolTip.transform.localScale.x), 0, 0);
                                (toolTip.transform as RectTransform).localPosition += new Vector3(item.tooltip.xPixelOrPercentage / 100 * (toolTip.transform as RectTransform).rect.width, 0, 0);
                                (toolTip.transform as RectTransform).localPosition += new Vector3(item.tooltip.margin.x, 0, 0);
                                break;
                            case XAligment.left:
                                (toolTip.transform as RectTransform).localPosition -= new Vector3(((toolTip.transform as RectTransform).rect.width / 2 * toolTip.transform.localScale.x), 0, 0);
                                (toolTip.transform as RectTransform).localPosition += new Vector3((item.tooltip.xPixelOrPercentage / 100) * ((toolTip.transform as RectTransform).rect.width), 0, 0);
                                (toolTip.transform as RectTransform).localPosition += new Vector3(item.tooltip.margin.x, 0, 0);
                                break;
                        }
                        break;
                    case AligmentOption.preDefined:
                        switch (item.tooltip.xAlign)
                        {
                            case XAligment.right:
                                (toolTip.transform as RectTransform).localPosition += new Vector3((toolTip.transform as RectTransform).rect.width / 2 * toolTip.transform.localScale.x, 0, 0);
                                goto case XAligment.center;
                            case XAligment.center:
                                (toolTip.transform as RectTransform).localPosition += new Vector3(item.tooltip.margin.x, 0, 0);
                                break;
                            case XAligment.left:
                                (toolTip.transform as RectTransform).localPosition -= new Vector3((toolTip.transform as RectTransform).rect.width / 2 * toolTip.transform.localScale.x, 0, 0);
                                goto case XAligment.center;
                        }
                        break;
                    case AligmentOption.pixel:
                        switch (item.tooltip.xAlign)
                        {
                            case XAligment.right:
                                (toolTip.transform as RectTransform).localPosition += new Vector3((toolTip.transform as RectTransform).rect.width / 2 * toolTip.transform.localScale.x, 0, 0);
                                (toolTip.transform as RectTransform).localPosition -= new Vector3(item.tooltip.xPixelOrPercentage, 0, 0);
                                (toolTip.transform as RectTransform).localPosition += new Vector3(item.tooltip.margin.x, 0, 0);
                                break;
                            case XAligment.center:
                                (toolTip.transform as RectTransform).localPosition -= new Vector3(((toolTip.transform as RectTransform).rect.width / toolTip.transform.localScale.x), 0, 0);
                                (toolTip.transform as RectTransform).localPosition += new Vector3(item.tooltip.xPixelOrPercentage, 0, 0);
                                (toolTip.transform as RectTransform).localPosition += new Vector3(item.tooltip.margin.x, 0, 0);
                                break;
                            case XAligment.left:
                                (toolTip.transform as RectTransform).localPosition -= new Vector3(((toolTip.transform as RectTransform).rect.width / 2 * toolTip.transform.localScale.x), 0, 0);
                                (toolTip.transform as RectTransform).localPosition += new Vector3(item.tooltip.xPixelOrPercentage, 0, 0);
                                (toolTip.transform as RectTransform).localPosition += new Vector3(item.tooltip.margin.x, 0, 0);
                                break;
                        }
                        break;
                }
               
                switch (item.tooltip.yAligmentOption)
                {
                    case AligmentOption.percentage:
                        switch (item.tooltip.yAlign)
                        {
                            case YAligment.up:
                                (toolTip.transform as RectTransform).localPosition += new Vector3(0, (toolTip.transform as RectTransform).rect.height / 2 * toolTip.transform.localScale.y, 0);
                                (toolTip.transform as RectTransform).localPosition -= new Vector3(0, (item.tooltip.yPixelOrPercentage / 100) * ((toolTip.transform as RectTransform).rect.height), 0);
                                (toolTip.transform as RectTransform).localPosition += new Vector3(0, item.tooltip.margin.y, 0);
                                break;
                            case YAligment.center:
                                (toolTip.transform as RectTransform).localPosition -= new Vector3(0, ((toolTip.transform as RectTransform).rect.height / toolTip.transform.localScale.y), 0);
                                (toolTip.transform as RectTransform).localPosition += new Vector3(0, item.tooltip.yPixelOrPercentage / 100 * (toolTip.transform as RectTransform).rect.height, 0);
                                (toolTip.transform as RectTransform).localPosition += new Vector3(0, item.tooltip.margin.y, 0);
                                break;
                            case YAligment.down:
                                (toolTip.transform as RectTransform).localPosition -= new Vector3(0, ((toolTip.transform as RectTransform).rect.height / 2 * toolTip.transform.localScale.y), 0);
                                (toolTip.transform as RectTransform).localPosition += new Vector3(0, (item.tooltip.yPixelOrPercentage / 100) * ((toolTip.transform as RectTransform).rect.height), 0);
                                (toolTip.transform as RectTransform).localPosition += new Vector3(0, item.tooltip.margin.y, 0);
                                break;
                        }
                        break;
                    case AligmentOption.preDefined:
                        switch (item.tooltip.yAlign)
                        {
                            case YAligment.up:
                                (toolTip.transform as RectTransform).localPosition += new Vector3(0, (toolTip.transform as RectTransform).rect.height / 2 * toolTip.transform.localScale.y, 0);
                                goto case YAligment.center;
                            case YAligment.center:
                                (toolTip.transform as RectTransform).localPosition += new Vector3(0, item.tooltip.margin.y, 0);
                                break;
                            case YAligment.down:
                                (toolTip.transform as RectTransform).localPosition -= new Vector3(0, (toolTip.transform as RectTransform).rect.height / 2 * toolTip.transform.localScale.y, 0);
                                goto case YAligment.center;
                        }
                        break;
                    case AligmentOption.pixel:
                        switch (item.tooltip.yAlign)
                        {
                            case YAligment.up:
                                (toolTip.transform as RectTransform).localPosition += new Vector3(0, (toolTip.transform as RectTransform).rect.height / 2 * toolTip.transform.localScale.y, 0);
                                (toolTip.transform as RectTransform).localPosition -= new Vector3(0, item.tooltip.yPixelOrPercentage, 0);
                                (toolTip.transform as RectTransform).localPosition += new Vector3(0, item.tooltip.margin.y, 0);
                                break;
                            case YAligment.center:
                                (toolTip.transform as RectTransform).localPosition -= new Vector3(0, ((toolTip.transform as RectTransform).rect.height / toolTip.transform.localScale.y), 0);
                                (toolTip.transform as RectTransform).localPosition += new Vector3(0, item.tooltip.yPixelOrPercentage, 0);
                                (toolTip.transform as RectTransform).localPosition += new Vector3(0, item.tooltip.margin.y, 0);
                                break;
                            case YAligment.down:
                                (toolTip.transform as RectTransform).localPosition -= new Vector3(0, ((toolTip.transform as RectTransform).rect.height / 2 * toolTip.transform.localScale.y), 0);
                                (toolTip.transform as RectTransform).localPosition += new Vector3(0, item.tooltip.yPixelOrPercentage, 0);
                                (toolTip.transform as RectTransform).localPosition += new Vector3(0, item.tooltip.margin.y, 0);
                                break;
                        }
                        break;
                }
                //TODO Consider Percentage and pixel. For percentage subtracting it from 100 and using it should do it
                if (item.tooltip.autoReAlign)
                {
                    tooltipRect = toolTip.transform as RectTransform;
                    Vector3[] corners = new Vector3[4];
                    tooltipRect.GetWorldCorners(corners);

                    //X
                    bool outRight = Camera.main.WorldToViewportPoint(corners[2]).x > 1 - item.tooltip.snapMargin.x && Camera.main.WorldToViewportPoint(corners[3]).x > 1 - item.tooltip.snapMargin.x;
                    bool outLeft = Camera.main.WorldToViewportPoint(corners[0]).x < 0 + item.tooltip.snapMargin.x && Camera.main.WorldToViewportPoint(corners[1]).x < 0 + item.tooltip.snapMargin.x;
                    //Y
                    bool outUp = Camera.main.WorldToViewportPoint(corners[1]).y > 1 - item.tooltip.snapMargin.y&& Camera.main.WorldToViewportPoint(corners[2]).y > 1 - item.tooltip.snapMargin.y;
                    bool outDown = Camera.main.WorldToViewportPoint(corners[3]).y < 0 + item.tooltip.snapMargin.y && Camera.main.WorldToViewportPoint(corners[0]).y < 0 + item.tooltip.snapMargin.y;
                    
                    if(item.tooltip.autoRealignOptions == AutoRealignOptions.snapToSide)
                    {
                        //Debug.Log("Snap");
                        if (outRight)
                        {
                            //Debug.Log("outRight");
                            toolTip.transform.position = new Vector3(Camera.main.ViewportToWorldPoint(new Vector3(1 - item.tooltip.snapTo.x, 0, 0), Camera.main.stereoActiveEye).x, toolTip.transform.position.y, toolTip.transform.position.z);
                            (toolTip.transform as RectTransform).localPosition -= new Vector3((toolTip.transform as RectTransform).rect.width / 2 * toolTip.transform.localScale.x, 0, 0);
                        }
                        if (outLeft)
                        {
                            //Debug.Log("outLeft");
                            toolTip.transform.position = new Vector3(Camera.main.ViewportToWorldPoint(new Vector3(item.tooltip.snapTo.x, 0, 0), Camera.main.stereoActiveEye).x, toolTip.transform.position.y, toolTip.transform.position.z);
                            (toolTip.transform as RectTransform).localPosition += new Vector3((toolTip.transform as RectTransform).rect.width / 2 * toolTip.transform.localScale.x, 0, 0);
                        }
                        if (outUp)
                        {
                            //Debug.Log("outUp");
                            toolTip.transform.position = new Vector3(toolTip.transform.position.x, Camera.main.ViewportToWorldPoint(new Vector3(0, 1 - item.tooltip.snapTo.y, 0)).y, toolTip.transform.position.z);
                            (toolTip.transform as RectTransform).localPosition -= new Vector3(0, (toolTip.transform as RectTransform).rect.height / 2 * toolTip.transform.localScale.y, 0);
                        }
                        if (outDown)
                        {
                            //Debug.Log("outDown");
                            toolTip.transform.position = new Vector3(toolTip.transform.position.x, Camera.main.ViewportToWorldPoint(new Vector3(0, item.tooltip.snapTo.y, 0)).y, toolTip.transform.position.z);
                            (toolTip.transform as RectTransform).localPosition += new Vector3(0, (toolTip.transform as RectTransform).rect.height / 2 * toolTip.transform.localScale.y, 0);
                        }
                    }
                    else if (item.tooltip.autoRealignOptions == AutoRealignOptions.switchSide)
                    {
                        Debug.Log("Switch");
                        if (outRight)
                        {
                            Debug.Log("outRight");
                            int tmp = 4;
                            if (item.tooltip.xAlign == XAligment.right) tmp = 2;
                            (toolTip.transform as RectTransform).localPosition -= new Vector3((toolTip.transform as RectTransform).rect.width / tmp, 0, 0);
                            (toolTip.transform as RectTransform).localPosition += new Vector3(tmp == 2 ? -2 : -1 * item.tooltip.margin.x, 0, 0);   
                        }
                        if (outLeft)
                        {
                            Debug.Log("outLeft");
                            int tmp = 4;
                            if (item.tooltip.xAlign == XAligment.left) tmp = 2;
                            (toolTip.transform as RectTransform).localPosition += new Vector3((toolTip.transform as RectTransform).rect.width / tmp, 0, 0);
                            (toolTip.transform as RectTransform).localPosition += new Vector3(tmp == 2 ? -2 : -1 * item.tooltip.margin.x, 0, 0);
                        }
                        if (outUp)
                        {
                            Debug.Log("outUp");
                            int tmp = 4;
                            if (item.tooltip.yAlign == YAligment.up) tmp = 2;
                            (toolTip.transform as RectTransform).localPosition -= new Vector3(0, (toolTip.transform as RectTransform).rect.height / tmp, 0);
                            (toolTip.transform as RectTransform).localPosition += new Vector3(0, tmp == 2 ? -2 : -1 * item.tooltip.margin.y, 0);  
                        }
                        if (outDown)
                        {
                            int tmp = 4;
                            if (item.tooltip.yAlign == YAligment.down) tmp = 2;
                            (toolTip.transform as RectTransform).localPosition += new Vector3(0, (toolTip.transform as RectTransform).rect.height / tmp, 0);
                            (toolTip.transform as RectTransform).localPosition += new Vector3(0, tmp == 2 ? -2 : -1 * item.tooltip.margin.y, 0);
                            Debug.Log("outDown");
                        }
                    }            
                }
            }
            else
            {
                Destroy(toolTip, 0.0000001f);
                toolTip = null;
            }
        }

        void OnDrawGizmos()
        {
            if (!toolTip) return;
            var item = invUI.GetInventory().slots[slotNum].Item;
            if (item == null) return;
            if (item.tooltip == null) return;

            //tooltipRect = toolTip.transform as RectTransform;
            Vector3[] corners = new Vector3[4];
            tooltipRect.GetWorldCorners(corners);
            //Debug.Log($"Gizmos 2: {Camera.main.WorldToViewportPoint(corners[2]).x} 3: {Camera.main.WorldToViewportPoint(corners[3]).x}");
            for (var i = 0; i < corners.Length; i++)
            {
                    //Debug.Log($"Corner {i} {corners[i]}");
                if (Camera.main.WorldToViewportPoint(corners[i]).x > 1 || Camera.main.WorldToViewportPoint(corners[i]).x < 0 || Camera.main.WorldToViewportPoint(corners[i]).y > 1 || Camera.main.WorldToViewportPoint(corners[i]).y < 0)
                    Gizmos.color = Color.red;
                else if (Camera.main.WorldToViewportPoint(corners[i]).x == 1 || Camera.main.WorldToViewportPoint(corners[i]).x == 0 || Camera.main.WorldToViewportPoint(corners[i]).y == 1 || Camera.main.WorldToViewportPoint(corners[i]).y == 0)
                    Gizmos.color = Color.blue;
                else if (Camera.main.WorldToViewportPoint(corners[i]).x > 1 - item.tooltip.snapMargin.x || Camera.main.WorldToViewportPoint(corners[i]).x < 0 + item.tooltip.snapMargin.x || Camera.main.WorldToViewportPoint(corners[i]).y > 1 - item.tooltip.snapMargin.y || Camera.main.WorldToViewportPoint(corners[i]).y < 0 + item.tooltip.snapMargin.y)
                    Gizmos.color = Color.yellow;
                else Gizmos.color = Color.green;
                Gizmos.DrawSphere(corners[i], .25f);
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

        public AligmentOption xAligmentOption;
        public XAligment xAlign;
        public float xPixelOrPercentage;

        public AligmentOption yAligmentOption;
        public YAligment yAlign;
        public float yPixelOrPercentage;
        
        public bool autoReAlign;
        public AutoRealignOptions autoRealignOptions;
        public Vector2 snapMargin;
        public Vector2 snapTo;

        public Color backgroudColor;

        public Vector2 size;
        public Vector2 padding;
        public Vector2 margin;
        public float maxWidth;

        public List<TooltipText> texts;

        [System.Serializable]
        public class TooltipText
        {
            [TextArea] 
            public string text;
            public TextAlignmentOptions alignOptions;
            public FontStyles fontStyles;
            public TMP_FontAsset font;
            public int fontSize;
            public Color color;

            public AligmentOption aligmentOption;
            public XAligment pivot;
            public float pixelOrPercentage;
        }
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
    public enum AligmentOption
    {
        preDefined = 0,
        percentage = 1,
        pixel = 2,
    }
    public enum AutoRealignOptions
    {
        snapToSide = 0,
        switchSide = 1,
        //fit = 2, 
    }
}
