using System;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace DesignPatterns
{
    [Serializable]
    public class EnumArray<E, T> : ISerializationCallbackReceiver
        where E : Enum
    {
        public static readonly int START_OFFSET, LENGTH;

        static EnumArray()
        {
            var enumValues = EnumToInt((E[])Enum.GetValues(typeof(E)));
            START_OFFSET = enumValues.Min() * -1;
            LENGTH = enumValues.Max() - enumValues.Min();
        }

        [SerializeField] private T[] values;

        public T this[E key]
        {
            get => values[EnumToInt(key) + START_OFFSET];
            set => values[EnumToInt(key) + START_OFFSET] = value;
        }

        public EnumArray()
        {
            values = new T[LENGTH];
        }

        #region Helpers

        private static int EnumToInt(E value)
        {
            return UnsafeUtility.SizeOf(Enum.GetUnderlyingType(typeof(E))) switch
            {
                1 => UnsafeUtility.As<E, byte>(ref value),
                2 => UnsafeUtility.As<E, short>(ref value),
                4 => UnsafeUtility.As<E, int>(ref value),
                _ => throw new NotSupportedException($"Enum {typeof(E)} cannot be used in EnumArray because it's size is 8 bytes (long), it must be at most 4 bytes (int).")
            };
        }

        private static int[] EnumToInt(E[] values)
        {
            int[] arr = new int[values.Length];
            for (int i = 0; i < arr.Length; i++) arr[i] = EnumToInt(values[i]);
            return arr;
        }

        #endregion

        #region ISerializationCallbackReceiver

        public void OnBeforeSerialize() { /* NOOP */ }

        public void OnAfterDeserialize()
        {
            if(LENGTH != values.Length)
            {
                T[] newValues = new T[LENGTH];
                Array.Copy(values, newValues, LENGTH);
                values = newValues;
            }
        }

        #endregion
    
    }
}