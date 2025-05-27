using System;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPatterns
{
    public class Heap<T>
    {
        private List<T> items;
        private Func<T,T,bool> orderingFunction;
        /// <summary>
        /// Initializes the heap. You should provide an orderingFunction to determine which items float to the top of the heap.
        /// </summary>
        /// <param name="startingCapacity"> Used to initialize the list of items in this heap </param>
        /// <param name="orderingFunction"> Function that returns true if the left argument should be on top of the right argument. </param>
        public Heap(int startingCapacity, Func<T,T,bool> orderingFunction)
        {
            items = new List<T>(startingCapacity);
            this.orderingFunction = orderingFunction;
        }
        /// <summary>
        /// Initializes the heap with the given values. You should provide an orderingFunction to determine which items float to the top of the heap.
        /// </summary>
        /// <param name="orderingFunction"> Function that returns true if the left argument should be on top of the right argument. </param>
        public Heap(List<T> array, Func<T,T,bool> orderingFunction)
        {
            items = new List<T>(array.Count);
            this.orderingFunction = orderingFunction;
            for(int i = 0; i < array.Count; i++) Insert(array[i]);
        }
        public int Count => items.Count;
        /// <summary> Returns the top element of the heap, but does not remove it. </summary>
        public T Peek() => items[0];
        /// <summary>
        /// Inserts a new element to the heap and maintains the heap structure.
        /// </summary>
        public void Insert(T t)
        {
            items.Add(t);
            int curr = items.Count - 1;
            while(curr > 0)
            {
                int parent = ParentIndexOf(curr);
                if(!orderingFunction(items[curr], items[parent])) break;
                Swap(curr, parent);
                curr = parent;
            }
        }
        /// <summary>
        /// Removes the top element of the heap and maintains the heap structure.
        /// </summary>
        public T Extract()
        {
            T toRemove = items[0];
            Swap(0, items.Count-1);
            items.RemoveAt(items.Count-1);
            int curr = 0;
            int child;
            while (true)
            {
                int swapIndex = curr;
                child = LeftChildOf(curr); // Left child
                if (child < items.Count && orderingFunction(items[child], items[curr])) swapIndex = child;
                child += 1; // Right child
                if (child < items.Count && orderingFunction(items[child], items[swapIndex])) swapIndex = child;
                if (swapIndex == curr) break;
                Swap(curr, swapIndex);
                curr = swapIndex;
            }
            return toRemove;
        }
        /// <summary> Swap two indexes in this heap </summary>
        private void Swap(int a, int b)
        {
            T temp = items[a];
            items[a] = items[b];
            items[b] = temp;
        }
        /// <summary> Returns the parent of the given index </summary>
        private int ParentIndexOf(int index) => (index - 1)/2;
        /// <summary> Returns the left child of the given index. If the child is out of bounds of the heap, returns int.MinValue. </summary>
        private int LeftChildOf(int index)
        {
            return 2*index + 1;
        }
        /// <summary> Returns the right child of the given index. If the child is out of bounds of the heap, returns int.MinValue. </summary>
        private int RightChildOf(int index)
        {
            return 2*index + 2;
        }
    }
}
