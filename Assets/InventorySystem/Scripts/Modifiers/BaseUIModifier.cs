using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniversalInventorySystem
{ 
    [RequireComponent(typeof(InventoryUI))]
    public class BaseUIModifier : MonoBehaviour
    {
        protected InventoryUI OriginalTarget { get; private set; }

        protected InventoryUI target;

        public InventoryUI GetTarget() => target;
        public InventoryUI GetOriginalTarget() => OriginalTarget;
        public InventoryUI SetTarget(InventoryUI _target) => target = _target;

        public void Start()
        { 
            target = GetComponent<InventoryUI>();
            OriginalTarget = target;
        }

        public void OnEnable()
        {
            target = GetComponent<InventoryUI>();
            OriginalTarget = target;
        }
    }
}