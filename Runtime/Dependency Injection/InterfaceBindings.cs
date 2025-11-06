using UnityEngine;
using System.Collections.Generic;
using System;

namespace DesignPatterns.DependencyInjection
{
    [CreateAssetMenu(menuName = "Interface Bindings")]
    public class InterfaceBindings : ScriptableObject
    {
        public List<InterfaceBinding> this[Scope scope] => _bindings[(int)scope].list;
        [SerializeField] private List<InterfaceBindingList> _bindings;
    }

    [Serializable]
    public class InterfaceBindingList
    {
        public List<InterfaceBinding> list;
    }

    [Serializable]
    public class InterfaceBinding : ISerializationCallbackReceiver
    {
        // Serialized
        public string interfaceAQN, concreteAQN;
        [SerializeReference] public object defaultValue = null;

        // Non serializable
        public Type interfaceType, concreteType;

        public void OnAfterDeserialize()
        {
            interfaceType = AQNToType(interfaceAQN);
            concreteType = AQNToType(concreteAQN);
        }

        public static Type AQNToType(string aqn) => aqn == null ? null : Type.GetType(aqn);

        public void OnBeforeSerialize()
        {
            interfaceAQN = interfaceType?.AssemblyQualifiedName ?? interfaceAQN;
            concreteAQN = concreteType?.AssemblyQualifiedName ?? concreteAQN;
        }
    }
}