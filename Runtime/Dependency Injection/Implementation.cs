using System;
using UnityEngine;

namespace DesignPatterns
{
    [Serializable]
    public abstract class Implementation
    {
        [SerializeField] protected Component component;
        public virtual bool Accepts(Component component) => false;
        public virtual Type Expects() => null;
    }

    [Serializable]
    public class Implementation<T> : Implementation
    {
        private T _value;
        public T value
        {
            get
            {
                if (component == null) return default;
                if (_value == null) _value = (T)(object)component;
                return _value;
            }
        }
        public override bool Accepts(Component component) => component is T;
        public override Type Expects() => typeof(T);
    }
}