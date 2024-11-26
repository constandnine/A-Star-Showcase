using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Heap<T> where T : IHeapItem<T>
{
    private T[] items;
    private int currentItemCount;


    public Heap(int maximumHeapSize)
    {
        items = new T[maximumHeapSize];
    }


    public T RemoveFirstItem() // Fixed typo from "RemoveFisrtItem"
    {
        T firstItem = items[0];


        currentItemCount--;


        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;


        SortDown(items[0]);


        return firstItem;
    }


    public void UpdateItems(T item)
    {
        SortUp(item);
    }


    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }


    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }


    public void AddItem(T item)
    {
        item.HeapIndex = currentItemCount;

        items[currentItemCount] = item;
        
        
        SortUp(item);
        
        
        currentItemCount++;
    }


    public void SortDown(T item)
    {
        while (true)
        {
            int leftChildIndex = item.HeapIndex * 2 + 1;
            int rightChildIndex = item.HeapIndex * 2 + 2;


            int swapIndex;

            if (leftChildIndex < currentItemCount)
            {
                swapIndex = leftChildIndex;


                if (rightChildIndex < currentItemCount)
                {
                    if (items[leftChildIndex].CompareTo(items[rightChildIndex]) < 0)
                    {
                        swapIndex = rightChildIndex;
                    }
                }

                       
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }

                else
                {
                    return;
                }
            }

            else
            {
                return;
            }
        }
    }


    public void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;


        while (true)
        {
            if (parentIndex < 0) 
            { 
                return; 
            }


            T parentItem = items[parentIndex];

            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem); // Fixed missing logic to swap when the item is greater
            }

            else
            {
                break;
            }


            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }


    public void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;


        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}


public interface IHeapItem<T> : IComparable<T>
{
    public int HeapIndex
    {
        get;
        set;
    }
}
