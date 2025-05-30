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
        public static List<T> Add<T>(this List<T> list, T t)
        {
            list.Add(t);
            return list;
        }
        /// <summary> Adds collection of items to list and returns the list. Intended when you want to chain with other list operations. </summary>
        public static List<T> AddRange<T>(this List<T> list, IEnumerable<T> enumerable)
        {
            foreach (T t in enumerable) list.Add(t);
            return list;
        }
    }
}
