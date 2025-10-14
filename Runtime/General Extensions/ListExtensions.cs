using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DesignPatterns
{
    public static class ListExtensions
    {
        /// <summary> Returns true if the index is inside the bounds of the list. </summary>
        public static bool ValidIndex<T>(this List<T> list, int index) => index >= 0 && index < list.Count;

        /// <summary> Adds item to list and returns the list. Intended when you want to chain with other list operations. </summary>
        public static List<T> CAdd<T>(this List<T> list, T t)
        {
            list.Add(t);
            return list;
        }
        /// <summary> Adds collection of items to list and returns the list. Intended when you want to chain with other list operations. </summary>
        public static List<T> CAddRange<T>(this List<T> list, IEnumerable<T> enumerable)
        {
            foreach (T t in enumerable) list.Add(t);
            return list;
        }
        /// <summary> Returns true if the list contains every element in the given enumerable. </summary>
        public static bool ContainsAll<T>(this List<T> list, IEnumerable<T> enumerable)
        {
            foreach (T t in enumerable) if (!list.Contains(t)) return false;
            return true;
        }
    }
}
