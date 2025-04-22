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
            public BusEvent() => this.name = "unnamed";
        }

        private static Dictionary<string, BusEvent> namedEvents = new();
        private static Dictionary<Type, BusEvent> generalEvents = new();

        //===================================================================//
        // TYPE EVENTS                                                       //
        //===================================================================//

        /// <summary>
        /// Registers to some event tied to the given type T.
        /// </summary>
        /// <param name="action">
        /// The action to add as listener.
        /// </param>
        /// <param name="getUpToDateWithEventHistory">
        /// Whether to immediatly call the action will all the parameters already raised prior to this action being registered.
        /// </param>
        public static void RegisterToGeneralEvent(UnityAction<T> action, bool getUpToDateWithEventHistory = false)
        {
            if(!generalEvents.ContainsKey(typeof(T))) generalEvents.Add(typeof(T), new BusEvent());
            if(getUpToDateWithEventHistory)
            {
                foreach(T t in generalEvents[typeof(T)].eventHistory)
                    action.Invoke(t);
            }
            generalEvents[typeof(T)].unityEvent.AddListener(action);
        }
        /// <summary>
        /// Unregister to the event tied to the type T.
        /// </summary>
        /// <param name="action">Action to remove as listener.</param>
        public static void UnregisterFromGeneralEvent(UnityAction<T> action)
        {
            if(!generalEvents.ContainsKey(typeof(T))) return;
            generalEvents[typeof(T)].unityEvent.RemoveListener(action);
        }
        /// <summary>
        /// Raise the event tied to the type T, passing along the given param.
        /// </summary>
        /// <param name="param">
        /// The parameter that will be passed to all listeners.
        /// </param>
        /// <param name="addToEventHistory">
        /// Whether to add the given param to the event raise history .
        /// </param>
        public static void RaiseGeneralEvent(T param, bool addToEventHistory = false)
        {
            if(!generalEvents.ContainsKey(typeof(T))) generalEvents.Add(typeof(T), new BusEvent());
            if(addToEventHistory) generalEvents[typeof(T)].eventHistory.Add(param);
            generalEvents[typeof(T)].unityEvent.Invoke(param);
        }
        
        //===================================================================//
        // NAMED EVENTS                                                      //
        //===================================================================//

        /// <summary>
        /// Register to some event tied to eventName and the type T.
        /// </summary>
        /// <param name="eventName"> The name of the event. </param>
        /// <param name="action"> The action to add as listener. </param>
        /// <param name="getUpToDateWithEventHistory">
        /// Whether to immediatly call the action will all the parameters already raised prior to this action being registered.
        /// </param>
        public static void RegisterToNamedEvent(string eventName, UnityAction<T> action, bool getUpToDateWithEventHistory = false)
        {
            eventName = typeof(T) + ":" + eventName;
            if(!namedEvents.ContainsKey(eventName)) namedEvents.Add(eventName, new BusEvent(eventName));
            if(getUpToDateWithEventHistory)
            {
                foreach(T t in namedEvents[eventName].eventHistory)
                    action.Invoke(t);
            }
            namedEvents[eventName].unityEvent.AddListener(action);
        }
        /// <summary>
        /// Unregister to the event tied to the eventName and type T.
        /// </summary>
        /// <param name="eventName"> The name of the event. </param>
        /// <param name="action">Action to remove as listener.</param>
        public static void UnregisterFromNamedEvent(string eventName, UnityAction<T> action)
        {
            eventName = typeof(T) + ":" + eventName;
            if(!namedEvents.ContainsKey(eventName)) return;
            namedEvents[eventName].unityEvent.RemoveListener(action);
        }
        /// <summary>
        /// Raise the event tied to the eventName and type T, passing along the given param.
        /// </summary>
        /// <param name="eventName"> The name of the event. </param>
        /// <param name="param">
        /// The parameter that will be passed to all listeners.
        /// </param>
        /// <param name="addToEventHistory">
        /// Whether to add the given param to the event raise history.
        /// </param>
        public static void RaiseNamedEvent(string eventName, T param, bool addToEventHistory = false)
        {
            eventName = typeof(T) + ":" + eventName;
            if(!namedEvents.ContainsKey(eventName)) namedEvents.Add(eventName, new BusEvent(eventName));
            if(addToEventHistory) namedEvents[eventName].eventHistory.Add(param);
            namedEvents[eventName].unityEvent.Invoke(param);
        }
    }
}
