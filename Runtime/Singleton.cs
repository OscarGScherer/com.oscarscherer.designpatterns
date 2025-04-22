using System;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns
{
    /// <summary>
    /// Base Singleton class. Inherit from it to make your own singleton.
    /// </summary>
    public abstract class Singleton : MonoBehaviour
    {
        private static Transform parent;
        private static Dictionary<Type, Singleton> singletons = new Dictionary<Type, Singleton>();
        /// <summary>
        /// Gets a Singleton of type T. If the singleton doesn't exists it is instantiated.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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
        /// <summary>
        /// Properly adds itself to the singleton dictionary.
        /// If you override this, make sure you execute base.OnAwake().
        /// </summary>
        protected virtual void OnAwake()
        {
            if(singletons.ContainsKey(GetType()))
            {
                Debug.LogWarning("There is an extra instance of Singleton of type " + GetType() + " on gameobject \"" + name + "\"");
                return;
            }
            singletons.Add(GetType(),this);
        }
        /// <summary>
        /// Properly removes itself from the singleton dictionary.
        /// If you override this, make sure you execute base.OnDestroy().
        /// </summary>
        protected virtual void OnDestroy()
        {
            if(singletons[GetType()] == this) singletons.Remove(GetType());
        }
    }
}
