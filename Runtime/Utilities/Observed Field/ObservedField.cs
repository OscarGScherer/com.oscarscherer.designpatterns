using System;
using UnityEngine;

namespace DesignPatterns
{
    [Serializable]
    public class ObservedField<T>
    {
        [SerializeField] private T _value = default;
        public Action<T> onChange = DoNothingAction;
        public Func<T, T> preChangeTransformer = DoNothingFunc;
        public T value
        {
            get => _value;
            set
            {
                _value = value;
                OnChange();
            }
        }

        public ObservedField() { }
        public ObservedField(T initialValue) => _value = initialValue;

        private void OnChange()
        {
            _value = preChangeTransformer(_value);
            onChange(_value);
        }

        private static void DoNothingAction(T input) { }
        private static T DoNothingFunc(T input) => input;

        public static implicit operator T(ObservedField<T> of) => of.value;
    }
}
