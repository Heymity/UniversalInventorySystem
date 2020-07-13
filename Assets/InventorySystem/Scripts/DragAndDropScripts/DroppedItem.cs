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
 *  This is responsable for the dropped item
 */ 

using UnityEngine;

namespace UniversalInventorySystem
{
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
}
