using UnityEngine;
using UnityEngine.EventSystems;

namespace UniversalInventorySystem
{
    /// <summary>
    /// Remember to subscribe this functioon to the InventoyrEventHandler in order to work
    /// </summary>
    public interface IDropBehaviour
    {
        void OnDropItem(object sender, InventoryHandler.DropItemEventArgs e);
    }

    /// <summary>
    /// Remember to subscribe this functioon to the InventoyrEventHandler in order to work
    /// </summary>
    public interface IPickUpBehaviour
    {
        void OnPickUp(object sender, InventoryHandler.AddItemEventArgs e);
    }

    public interface IUsable
    {
        void OnUse(object sender, InventoryHandler.UseItemEventArgs e);
    }



    public abstract class DropBehaviour : MonoBehaviour, IDropBehaviour
    {
        public virtual void OnEnable()
        {
            InventoryHandler.current.OnDropItem += OnDropItem;
        }

        public virtual void OnDestroy()
        {
            InventoryHandler.current.OnDropItem -= OnDropItem;
        }

        public abstract void OnDropItem(object sender, InventoryHandler.DropItemEventArgs e);
    }
}
