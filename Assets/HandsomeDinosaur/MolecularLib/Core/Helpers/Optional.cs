using System;
using UnityEngine;

namespace MolecularLib.Helpers
{
    [Serializable]
    public class Optional<T>
    {
        [SerializeField] private T value;
        [SerializeField] private bool useValue;

        public Optional()
        {
            useValue = false;
        }
        
        public Optional(T value, bool useValue)
        {
            Value = value;
            UseValue = useValue;
        }

        public T Value
        {
            get => value;
            set => this.value = value;
        }

        public bool UseValue
        {
            get => useValue;
            set => useValue = value;
        }

        public bool HasValue => Value != null;

        public static implicit operator Optional<T>(T value) => new Optional<T>(value, true);

        public static implicit operator T(Optional<T> optional) => optional.Value;
        public static implicit operator bool(Optional<T> optional) => optional.UseValue;
    }
}
