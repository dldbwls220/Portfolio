using UnityEngine;
using System;

public interface IHeapItem<T> : IComparable<T>
{
    int _heapIndex { get; set; }
}

public class Heap<T> where T : IHeapItem<T>
{
    T[] _item;
    int _currentItemCount;

    public int _Count => _currentItemCount;

    public Heap(int maxHeapSize)
    {
        _item = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item._heapIndex = _currentItemCount;
        _item[_currentItemCount] = item;
        SortUp(item);
        _currentItemCount++;
    }
    public T RemoveFirst()
    {
        T firstItem = _item[0];
        _currentItemCount--;
        _item[0] = _item[_currentItemCount];
        _item[0]._heapIndex = 0;
        SortDown(_item[0]);

        return firstItem;
    }
    public void UpdateItem(T item)
    {
        SortUp(item);
    }
    public bool Contains(T item)
    {
        return Equals(_item[item._heapIndex], item);
    }

    void Swap(T itemA, T itemB)
    {
        _item[itemA._heapIndex] = itemB;
        _item[itemB._heapIndex] = itemA;

        int tempIndex = itemA._heapIndex;
        itemA._heapIndex = itemB._heapIndex;
        itemB._heapIndex = tempIndex;
    }
    void SortUp(T item)
    {
        int parentIndex = (item._heapIndex - 1) / 2;
        while (true)
        {
            T parentItem = _item[parentIndex];
            if (item.CompareTo(parentItem) > 0)
                Swap(item, parentItem);
            else break;

            parentIndex = (item._heapIndex - 1) / 2;
        }
    }

    void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = item._heapIndex * 2 + 1;
            int childIndexRight = item._heapIndex * 2 + 2;
            int swapIndex = 0;


            if (childIndexLeft < _currentItemCount)
            {
                swapIndex = childIndexLeft;
                if (childIndexRight < _currentItemCount)
                {
                    if (_item[childIndexLeft].CompareTo(_item[childIndexRight]) < 0)
                        swapIndex = childIndexRight;
                }
                if (item.CompareTo(_item[swapIndex]) < 0)
                    Swap(item, _item[swapIndex]);
                else return;
            }
            else return;
        }
    }
}
