using System;
using UnityEngine;

namespace DesignPatterns
{
    public static class ArrayExtensions
    {
        /// <summary> Returns true if the index is inside the bounds of the array. </summary>
        public static bool ValidIndex<T>(this T[] array, int index) => index >= 0 && index < array.Length;
        /// <summary>
        /// Makes an array of the given length, with all indexes initialized with the value
        /// </summary>
        public static T[] RepeatForArray<T>(this T value, int length)
        {
            T[] array = new T[length];
            array.SetAllValuesTo(value);
            return array;
        }
        /// <summary>
        /// Returns the index of value in the array, returns -1 if not present. Uses object.Equals().
        /// </summary>
        public static int IndexOf<T>(this T[] array, T value)
        {
            for(int i = 0; i < array.Length; i++)
            {
                if(array[i].Equals(value)) return i;
            }
            return -1;
        }
        /// <summary>
        /// Set all values in the array to the specified value.
        /// </summary>
        public static void SetAllValuesTo<T>(this T[] array, T value)
        {
            for(int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }
        /// <summary>
        /// Sets the given values in the array starting from the given index.
        /// </summary>
        public static void SetRange<T>(this T[] array, int start, params T[] values)
        {
            for(int i = start, j = 0; i < array.Length && j < values.Length; i++, j++)
            {
                array[i] = values[j];
            }
        }
    }
}
