using System;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Events;

namespace DesignPatterns
{
    /// <summary>
    /// Middleman class to register and raise events, from and to anywhere in your project,
    /// passing any generic argument you need.
    /// Use it to communicate between systems that you don't wan't to directly couple.
    /// </summary>
    /// /// <typeparam name="E">
    /// Enum used as key for the event.
    /// </typeparam>
    /// <typeparam name="T">
    /// The type of the paramenter the event takes.
    /// </typeparam>
    public static class EventBus<E,T> where E : Enum
    {
        public class BusEvent
        {
            public readonly string name;
            public UnityEvent<T> unityEvent = new();
            public BusEvent(string name) => this.name = name;
        }

        private static int offset;
        private static BusEvent[] events;

        private static int EnumToIndex(E key)
        {
            return offset + UnsafeUtility.As<E, int>(ref key);
        }

        static EventBus()
        {
            if (Enum.GetUnderlyingType(typeof(E)) != typeof(int)) 
                throw new ArgumentException($"The enum type {typeof(E)} must have int values to be used as an EventBus key.");

            var enumValues = (int[]) Enum.GetValues(typeof(E));
            if (enumValues.Length == 0)
            {
                events = new BusEvent[0];
                return;
            }
            int min = enumValues.Min();
            offset = min < 0 ? Mathf.Abs(min) : 0;
            events = new BusEvent[enumValues.Max() - enumValues.Min() + 1];
            foreach (E enumValue in Enum.GetValues(typeof(E)))
                events[EnumToIndex(enumValue)] = new(enumValue.ToString());
        }

        /// <summary>
        /// Register to some event tied to eventEnum value and the type T.
        /// </summary>
        /// <param name="eventEnum"> Enum value of the event. </param>
        /// <param name="action"> The action to add as listener. </param>
        public static void RegisterToEvent(E eventEnum, UnityAction<T> action)
        {
            events[EnumToIndex(eventEnum)].unityEvent.AddListener(action);
        }

        /// <summary>
        /// Unregister to the event tied to the eventEnum value and type T.
        /// </summary>
        /// <param name="eventEnum"> Enum value of the event. </param>
        /// <param name="action">Action to remove as listener.</param>
        public static void UnregisterFromEvent(E eventEnum, UnityAction<T> action)
        {
            events[EnumToIndex(eventEnum)].unityEvent.RemoveListener(action);
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
            events[EnumToIndex(eventEnum)].unityEvent.Invoke(param);
        }
    }
}
