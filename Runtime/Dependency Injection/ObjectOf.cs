using System;
using UnityEngine;

namespace DesignPatterns.DependencyInjection
{
    [Serializable]
    public class ObjectOf<I> : ISerializationCallbackReceiver
    {
        [SerializeReference] public I value;
        public ObjectOf()
        {
            value = DI<I>.HasInstancer() ? DI<I>.Instantiate() : default;
        }

        public void OnAfterDeserialize()
        {
            if (value == null && DI<I>.HasInstancer()) value = DI<I>.Instantiate();
        }

        public void OnBeforeSerialize() { /* NOOP */ }
    }
}