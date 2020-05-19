using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultUseBehaviour
{
    public void OnUse(object sender, InventoryHandler.UseItemEventArgs e)
    {
        Debug.Log("Use Item Behaviour");
    }
}
