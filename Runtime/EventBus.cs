using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace DesignPatterns
{
    public static class EventBus<T>
    {
        public class NamedEvent
        {
            public readonly string name;
            public UnityEvent<T> unityEvent;
            public Type type;
            public NamedEvent(string name)
            {
                this.name = name;
                this.unityEvent = new UnityEvent<T>();
            }
        }

        private static Dictionary<string, NamedEvent> namedEvents = new();
        private static Dictionary<Type, UnityEvent<T>> generalEvents = new ();

        public static void RegisterToGeneralEvent(UnityAction<T> action)
        {
            if(!generalEvents.ContainsKey(typeof(T))) generalEvents.Add(typeof(T), new UnityEvent<T>());
            generalEvents[typeof(T)].AddListener(action);
        }
        public static void UnregisterFromGeneralEvent(UnityAction<T> action)
        {
            if(!generalEvents.ContainsKey(typeof(T))) return;
            generalEvents[typeof(T)].RemoveListener(action);
        }
        public static void RaiseGeneralEvent(T param)
        {
            if(!generalEvents.ContainsKey(typeof(T))) generalEvents.Add(typeof(T), new UnityEvent<T>());
            generalEvents[typeof(T)].Invoke(param);
        }
        
        public static void RegisterToNamedEvent(string eventName, UnityAction<T> action)
        {
            eventName = typeof(T) + ":" + eventName;
            if(!namedEvents.ContainsKey(eventName)) namedEvents.Add(eventName, new NamedEvent(eventName));
            namedEvents[eventName].unityEvent.AddListener(action);
        }
        public static void UnregisterFromNamedEvent(string eventName, UnityAction<T> action)
        {
            eventName = typeof(T) + ":" + eventName;
            if(!namedEvents.ContainsKey(eventName)) return;
            namedEvents[eventName].unityEvent.RemoveListener(action);
        }
        public static void RaiseNamedEvent(string eventName, T param)
        {
            eventName = typeof(T) + ":" + eventName;
            if(!namedEvents.ContainsKey(eventName)) namedEvents.Add(eventName, new NamedEvent(eventName));
            namedEvents[eventName].unityEvent.Invoke(param);
        }
    }
}
