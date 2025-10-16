using System;

namespace DesignPatterns
{
    public class Fallback<T>
    {
        private readonly T _value;
        private readonly Func<T> _func;

        public Fallback(T value) { _value = value; _func = null; }
        public Fallback(Func<T> func) { _func = func; _value = default; }

        public T Get() => _func != null ? _func() : _value;

        public static implicit operator Fallback<T>(T value) => new Fallback<T>(value);
        public static implicit operator Fallback<T>(Func<T> func) => new Fallback<T>(func);
    }

    public static class UnityObjectExtensions
    {
        /// <summary> Equivalent to ?? but it handles Unity Null. Parameters should be Func<T> or T. Slowest IfNull. </summary>
        public static T IfNull<T>(this T self, params Fallback<T>[] fallbacks) where T : UnityEngine.Object
        {
            if (self != null) return self;
            foreach (Fallback<T> fallback in fallbacks)
            {
                T fallbackValue = fallback.Get();
                if (fallbackValue != null) return fallbackValue;
            }
            return self;
        }
        /// <summary> Equivalent to ?? but it handles Unity Null. Fastest IfNull. </summary>
        public static T IfNull<T>(this T self, params T[] fallbacks) where T : UnityEngine.Object
        {
            if (self != null) return self;
            foreach (T fallback in fallbacks)
            {
                if (fallback != null)
                    return fallback;
            }
            return self;
        }
        /// <summary> Equivalent to ?? but it handles Unity Null. </summary>
        public static T IfNull<T>(this T self, params Func<T>[] fallbacks) where T : UnityEngine.Object
        {
            if (self != null) return self;
            foreach (Func<T> fallback in fallbacks)
            {
                if (fallback == null) continue;
                T fallbackValue = fallback();
                if (fallbackValue != null) return fallbackValue;
            }
            return self;
        }
    }
}
