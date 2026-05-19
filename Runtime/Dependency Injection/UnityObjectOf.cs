using System;
using UnityEngine;

namespace DesignPatterns
{
    [Serializable]
    public abstract class UnityObjectOf
    {
        [SerializeField] protected UnityEngine.Object component;
        public virtual bool Accepts(UnityEngine.Object component) => false;
        public virtual Type Expects() => null;
    }

    [Serializable]
    public class UnityObjectOf<T> : UnityObjectOf
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
            set
            {
                if (value is not UnityEngine.Object) return;
                component = (UnityEngine.Object)(object)value;
                _value = value;
            }
        }
        public override bool Accepts(UnityEngine.Object component) => component is T;
        public override Type Expects() => typeof(T);
    }
}