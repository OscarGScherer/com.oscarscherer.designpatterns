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
    public static class EventBus<T>
    {
        public class BusEvent
        {
            public readonly string name;
            public UnityEvent<T> unityEvent = new();
            public List<T> eventHistory = new List<T>();
            public BusEvent(string name) => this.name = name;
        }

        private static Dictionary<string, BusEvent> namedEvents = new();

        /// <summary>
        /// Register to some event tied to eventEnum value and the type T.
        /// </summary>
        /// <param name="eventEnum"> Enum value of the event. </param>
        /// <param name="action"> The action to add as listener. </param>
        /// <param name="getUpToDateWithEventHistory">
        /// Whether to immediatly call the action will all the parameters already raised prior to this action being registered.
        /// </param>
        public static void RegisterToEvent<E>(E eventEnum, UnityAction<T> action, bool getUpToDateWithEventHistory = false)
        where E : Enum
        {
            string eventName = GetEventName(eventEnum);
            if (!namedEvents.ContainsKey(eventName)) namedEvents.Add(eventName, new BusEvent(eventName));
            if (getUpToDateWithEventHistory)
            {
                foreach (T t in namedEvents[eventName].eventHistory)
                    action.Invoke(t);
            }
            namedEvents[eventName].unityEvent.AddListener(action);
        }

        /// <summary>
        /// Unregister to the event tied to the eventEnum value and type T.
        /// </summary>
        /// <param name="eventEnum"> Enum value of the event. </param>
        /// <param name="action">Action to remove as listener.</param>
        public static void UnregisterFromEvent<E>(E eventEnum, UnityAction<T> action)
        where E : Enum
        {
            string eventName = GetEventName(eventEnum);
            if (!namedEvents.ContainsKey(eventName)) return;
            namedEvents[eventName].unityEvent.RemoveListener(action);
        }
        /// <summary>
        /// Raise the event tied to the eventEnum value and type T, passing along the given param.
        /// </summary>
        /// <param name="eventEnum"> Enum value of the event. </param>
        /// <param name="param">
        /// The parameter that will be passed to all listeners.
        /// </param>
        /// <param name="addToEventHistory">
        /// Whether to add the given param to the event raise history.
        /// </param>
        public static void RaiseEvent<E>(E eventEnum, T param, bool addToEventHistory = false)
        where E : Enum
        {
            string eventName = GetEventName(eventEnum);
            if (!namedEvents.ContainsKey(eventName)) namedEvents.Add(eventName, new BusEvent(eventName));
            if (addToEventHistory) namedEvents[eventName].eventHistory.Add(param);
            namedEvents[eventName].unityEvent.Invoke(param);
        }
        /// <summary>
        /// Helper functions that converts the enum and argument type into a string to lookup the event dictionary.
        /// </summary>
        private static string GetEventName<E>(E eventEnum) where E : Enum => $"{typeof(T)}|{typeof(E)}|{eventEnum}";
    }
}
