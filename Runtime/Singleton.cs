using System;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns
{
    public abstract class Singleton : MonoBehaviour
    {
        private static Transform parent;
        private static Dictionary<Type, Singleton> singletons = new Dictionary<Type, Singleton>();

        public static T Get<T>() where T : Singleton
        {
            if(!singletons.ContainsKey(typeof(T)))
            {
                if(parent == null) parent = new GameObject("Singletons").transform;
                T newSingleton = new GameObject(typeof(T).ToString()).AddComponent<T>();
                singletons.Add(typeof(T),newSingleton);
            }
            return (T) singletons[typeof(T)];
        }

        protected virtual void OnAwake()
        {
            if(singletons.ContainsKey(GetType()))
            {
                Debug.LogWarning("There is an extra instance of Singleton of type " + GetType() + " on gameobject \"" + name + "\"");
                return;
            }
            singletons.Add(GetType(),this);
        }

        protected virtual void OnDestroy()
        {
            if(singletons[GetType()] == this) singletons.Remove(GetType());
        }
    }
}
