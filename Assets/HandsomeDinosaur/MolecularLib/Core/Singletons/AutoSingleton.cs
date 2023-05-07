using UnityEngine;

namespace MolecularLib
{
    /// <summary>
    /// Derive from this class to have a Current (Singleton) property to your class. The Singleton will be assigned used the FindObjectOfType&lt;T&gt; method in a lazy way (will only call the method when used and then cache it for later use)
    /// </summary>
    /// <typeparam name="T">Your MonoBehaviour derived singleton Type</typeparam>
    public class AutoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _current;
        public static T Current => _current ??= FindObjectOfType<T>() ?? CreateSingleton();

        private static T CreateSingleton()
        {
            var singleton = new GameObject(typeof(T).Name);
            _current = singleton.AddComponent<T>();
            return _current;
        }
    }
}
