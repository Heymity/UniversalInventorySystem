using MolecularLib.Helpers;
using UnityEngine;

namespace MolecularLib
{
    public class VolatileScriptableObject<TData> : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] protected TData runtimeValue;
        
        [SerializeField] protected TData editorSavedValue;
        protected TData Value
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    return editorSavedValue;
#endif
                return runtimeValue;
            }
            set
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    editorSavedValue = value;
#endif
                runtimeValue = value;
            }
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
                runtimeValue = DeepCopy(editorSavedValue);
        }

        public void OnBeforeSerialize()
        {
            
        }

        protected void CopyRuntimeValuesToEditorSaved()
        {
            editorSavedValue = DeepCopy(runtimeValue);
        }

        public void OnAfterDeserialize()
        {
            if (PlayStatus.IsPlaying) return;
            
            runtimeValue = DeepCopy(editorSavedValue);
        }
        
        private static TData DeepCopy(TData objToCopy)
        {
            var json = JsonUtility.ToJson(objToCopy);
            var obj = JsonUtility.FromJson<TData>(json);
            return obj;
        }
    }
}
