using UnityEngine;
using UnityEngine.Events;

namespace DesignPatterns
{
    public abstract class ProjectVariable<T> : ScriptableObject
    {
        [TextArea] [SerializeField] private string description;
        [SerializeField] private T _value;
        public T value {
            get => _value;
            set {
                T prev = _value;
                _value = value;
                onChange.Invoke(prev, value);
            }
        }
        public UnityEvent<T,T> onChange;
    }
}
