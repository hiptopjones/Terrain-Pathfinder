using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinHeap<T> where T : IComparable<T>
{
    private T[] Items { get; set; }
    private int NextIndex { get; set; }

    public MinHeap(int maxSize)
    {
        Items = new T[maxSize];
    }

    public void Add(T item)
    {
        if (NextIndex >= Items.Length)
        {
            throw new Exception("Max size of heap exceeded");
        }

        // Add new item to the bottom of the heap
        int index = NextIndex;
        Items[index] = item;
        NextIndex++;

        // Sort it up into place if necessary
        SortUp(index);

    }

    public T RemoveMin()
    {
        if (NextIndex == 0)
        {
            throw new Exception("No items in heap");
        }

        T minItem = Items[0];

        // Move the last item to the top
        int lastIndex = NextIndex - 1;
        Items[0] = Items[lastIndex];
        NextIndex--;

        // Sort it down into place if necessary
        SortDown(0);

        return minItem;
    }

    private int GetParentIndex(int i)
    {
        return (i - 1) / 2;
    }

    private int GetLeftChildIndex(int i)
    {
        return (i * 2) + 1;
    }

    private int GetRightChildIndex(int i)
    {
        return (i * 2) + 2;
    }

    private void SwapItems(int index1, int index2)
    {
        T item = Items[index1];
        Items[index1] = Items[index2];
        Items[index2] = item;
    }

    private void SortUp(int index)
    {
        while (true)
        {
            T item = Items[index];

            int parentIndex = GetParentIndex(index);

            // Exit if no movement required
            //  item >= Items[parentIndex]
            if (item.CompareTo(Items[parentIndex]) >= 0)
            {
                break;
            }

            SwapItems(index, parentIndex);
            index = parentIndex;
        }
    }

    private void SortDown(int index)
    {
        while (true)
        {
            int leftChildIndex = GetLeftChildIndex(index);
            int rightChildIndex = GetRightChildIndex(index);

            int smallestChildIndex = index;

            // Is left child valid and smaller than its parent
            //                                Items[leftChildIndex] < Items[smallestChildIndex]
            if (leftChildIndex < NextIndex && Items[leftChildIndex].CompareTo(Items[smallestChildIndex]) < 0)
            {
                smallestChildIndex = leftChildIndex;
            }

            // Is right child valid and smaller than the left child?
            //                                 Items[rightChildIndex] < Items[smallestChildIndex]
            if (rightChildIndex < NextIndex && Items[rightChildIndex].CompareTo(Items[smallestChildIndex]) < 0)
            {
                smallestChildIndex = rightChildIndex;
            }

            // Exit if no movement required
            if (smallestChildIndex == index)
            {
                break;
            }

            SwapItems(index, smallestChildIndex);
            index = smallestChildIndex;
        }
    }
}