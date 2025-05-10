using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DesignPatterns
{
    public static class ListExtensions
    {
        /// <summary> Returns true if the index is inside the bounds of the list. </summary>
        public static bool ValidIndex<T>(this List<T> list, int index) => index >= 0 && index < list.Count;
    }
}
