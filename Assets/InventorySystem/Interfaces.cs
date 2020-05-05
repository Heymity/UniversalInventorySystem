using UnityEngine.EventSystems;

/// <summary>
/// For this interface to work in callback it gamaobject muust have Tag "Drop"
/// </summary>
public interface IDroppable : IEventSystemHandler
{
    void OnDrop();
}

public interface IPickUppable
{
    void OnPickUp();
}
