using System;
using UnityEngine;
using UnityEngine.Events;

namespace DesignPatterns
{
    [Serializable]
    public class ObservedField<T>
    {
        [SerializeField] private T _value = default;
        public T value
        {
            get => _value;
            set
            {
                _value = value;
            }
        }

        public Func<T, T> preChangeTransformers = NoTransformation;
        public Action<T> onChange = DoNothing;

        public ObservedField() { }
        public ObservedField(T initialValue) => this._value = initialValue;

        private void OnChange()
        {
            T currentValue = _value;
            foreach (Func<T, T> transformer in preChangeTransformers.GetInvocationList())
                currentValue = transformer != null ? transformer(currentValue) : currentValue;
            _value = currentValue;
            onChange(_value);
        }

        private static T NoTransformation(T obj) => obj;
        private static void DoNothing(T obj) { }
    }
}
