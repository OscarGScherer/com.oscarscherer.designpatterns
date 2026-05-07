using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace DesignPatterns
{
    /// <summary>
    /// This provides:
    /// a dynamicaly allocated array;
    /// lookup, add, and remove via IDs (O(1), memory access is NOT sequential);
    /// lookup via indexes (O(1), memory access IS sequential)
    /// </summary>
    [Serializable]
    public class SparseSet<T>
    {
        [SerializeField] private T[] data = new T[0];
        [SerializeField] private int[] id_to_index = new int[0];
        [SerializeField] private int[] index_to_id = new int[0];

        [field: SerializeField] public int Count { get; private set; } = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T AccessByIndex(int index) => data[index];

        public bool Contains(int iD)
        {
            if (iD < 0 || iD >= id_to_index.Length) return false;
            int index = id_to_index[iD];
            return index >= 0 && index < Count;
        }

        public int Add(T item)
        {
            HandleResize(Count);

            int lastIndex = Count;

            // Put the new item at the end of the array
            data[lastIndex] = item;

            // Get the ID assosiated with the index of the new data
            int itemId = index_to_id[lastIndex];

            // If the id associated is not initialized, that means so isnt the index
            if (itemId == -1)
            {
                index_to_id[lastIndex] = lastIndex;
                id_to_index[lastIndex] = lastIndex;
                itemId = lastIndex;
            }

            Count++;
            return itemId;
        }

        public void Remove(int removeItemId)
        {
            if (removeItemId < 0 || removeItemId >= id_to_index.Length) throw new IndexOutOfRangeException();
            int removeItemIndex = id_to_index[removeItemId];

            // If we are removing the last element, all is needed is that the count be decremented
            if (removeItemIndex >= Count)
            {
                Count--;
                return;
            }

            int lastIndex = Count - 1;
            Swap(data, /*Indexes:*/ removeItemIndex, lastIndex); // Put the removed data on the end, so its index will be > Count

            // Keeping IDs stable
            id_to_index[removeItemId] = lastIndex;      // Ensuring the of the removed item still maps to it, just in case
            int lastItemId = index_to_id[lastIndex];
            id_to_index[lastItemId] = removeItemIndex;  // Ensuring the id of the item moved from the back maps to its new position

            // Keeping the indexes stable
            index_to_id[removeItemIndex] = lastItemId;
            index_to_id[lastIndex] = removeItemId;

            // Decrementing count to signify there is one less element
            Count--;
        }

        public void Clear()
        {
            Count = 0;
        }

        private void HandleResize(int index, int factor = 2)
        {
            if(index < data.Length) return;
            int newLength = Mathf.Max(index + 1, data.Length * factor);
            ResizeArray(ref data, newLength, default);
            ResizeArray(ref index_to_id, newLength, -1);
            ResizeArray(ref id_to_index, newLength, -1);
        }

        private void ResizeArray<TArray>(ref TArray[] oldArray, int size, TArray fillValue)
        {
            var newArray = new TArray[size];
            Array.Copy(oldArray, 0, newArray, 0, oldArray.Length);
            Array.Fill(newArray, fillValue, oldArray.Length, newArray.Length - oldArray.Length);
            oldArray = newArray;
        }

        private void Swap<T2>(T2[] array, int indexA, int indexB)
        {
            T2 temp = array[indexA];
            array[indexA] = array[indexB];
            array[indexB] = temp;
        }
    }
}