using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace DesignPatterns
{
    /// <summary>
    /// Middleman class to register and raise events, from and to anywhere in your project,
    /// passing any generic argument you need.
    /// Use it to communicate between systems that you don't wan't to directly couple.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the paramenter the event takes.
    /// </typeparam>
    public static class EventBus<T, E> where E: Enum
    {
        public class BusEvent
        {
            public readonly string name;
            public UnityEvent<T> unityEvent = new();
            public BusEvent(string name) => this.name = name;
        }

        private static Dictionary<(Type,Enum), BusEvent> namedEvents = new();

        /// <summary>
        /// Register to some event tied to eventEnum value and the type T.
        /// </summary>
        /// <param name="eventEnum"> Enum value of the event. </param>
        /// <param name="action"> The action to add as listener. </param>
        public static void RegisterToEvent(E eventEnum, UnityAction<T> action)
        {
            (Type, Enum) key = (typeof(T), eventEnum);
            BusEvent busEvent;
            if (!namedEvents.TryGetValue(key, out busEvent))
            {
                busEvent = new BusEvent($"{typeof(T).Name}|{eventEnum}");
                namedEvents.Add(key, busEvent);
            }
            busEvent.unityEvent.AddListener(action);
        }

        /// <summary>
        /// Unregister to the event tied to the eventEnum value and type T.
        /// </summary>
        /// <param name="eventEnum"> Enum value of the event. </param>
        /// <param name="action">Action to remove as listener.</param>
        public static void UnregisterFromEvent(E eventEnum, UnityAction<T> action)
        {
            (Type, Enum) key = (typeof(T), eventEnum);
            BusEvent busEvent;
            if (!namedEvents.TryGetValue(key, out busEvent)) return;
            busEvent.unityEvent.RemoveListener(action);
        }

        /// <summary>
        /// Raise the event tied to the eventEnum value and type T, passing along the given param.
        /// </summary>
        /// <param name="eventEnum"> Enum value of the event. </param>
        /// <param name="param">
        /// The parameter that will be passed to all listeners.
        /// </param>
        public static void RaiseEvent(E eventEnum, T param)
        {
            (Type, Enum) key = (typeof(T), eventEnum);
            BusEvent busEvent;
            if (!namedEvents.TryGetValue(key, out busEvent))
            {
                busEvent = new BusEvent($"{typeof(T).Name}|{eventEnum}");
                namedEvents.Add(key, busEvent);
            }
            busEvent.unityEvent.Invoke(param);
        }
    }
}
