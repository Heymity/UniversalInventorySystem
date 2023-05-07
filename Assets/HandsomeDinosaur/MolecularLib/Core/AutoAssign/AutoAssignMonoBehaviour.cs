using UnityEngine;

namespace MolecularLib.AutoAssign
{
    public class AutoAssignMonoBehaviour : MonoBehaviour
    {
        protected virtual void Awake()
        {
            this.AutoAssign();
        }
    }
}