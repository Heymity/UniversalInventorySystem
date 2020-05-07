using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DroppedItem : MonoBehaviour
{
    SpriteRenderer sr;
    public int amount;

    private void OnEnable()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetSprite(Sprite s) 
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = s; 
    }

    public void SetAmount(int _amount) => amount = _amount;

    private void Update()
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, Camera.main.ScreenToWorldPoint(Input.mousePosition)))
        {
            Debug.Log("Pick Up");
        }
    }
}
