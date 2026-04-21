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
        private static Transform createdSingletonsParent;
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
                if(createdSingletonsParent == null) createdSingletonsParent = new GameObject("Created Singletons").transform;
                T existingSingleton = GameObject.FindAnyObjectByType<T>();
                if(existingSingleton == null) // If there isnt a singleton in the scene, make one
                {
                    T newSingleton = new GameObject(typeof(T).ToString()).AddComponent<T>();
                    newSingleton.transform.parent = createdSingletonsParent;
                }
                else // Add that manually
                {
                    singletons.Add(typeof(T),existingSingleton);
                }
            }
            return (T) singletons[typeof(T)];
        }
        /// <summary>
        /// Adds itself to the singleton dictionary.
        /// If you override this, make sure you execute base.OnEnable().
        /// </summary>
        protected virtual void OnEnable()
        {
            if(singletons.ContainsKey(GetType()))
            {
                if(singletons[GetType()] == this) return;
                Debug.LogWarning("There is an extra instance of Singleton of type " + GetType() + " on gameobject \"" + name + "\"");
                return;
            }
            else singletons.Add(GetType(),this);
        }
        /// <summary>
        /// Removes itself from the singleton dictionary.
        /// If you override this, make sure you execute base.OnDisable().
        /// </summary>
        protected virtual void OnDisable()
        {
            if(singletons[GetType()] == this) singletons.Remove(GetType());
        }
    }
}
