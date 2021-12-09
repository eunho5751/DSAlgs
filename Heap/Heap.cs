using System;

public enum HeapType
{
    Min,
    Max
}

public class Heap<T> where T : IComparable<T>
{
    private const int DefaultCapacity = 10;
    private const float CapacityMultiplier = 2f;

    private T[] _arr;
    private Func<T, T, bool> _swapComparer;

    public Heap(HeapType type) : this(type, DefaultCapacity)
    {

    }

    public Heap(HeapType type, int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than 0.");

        _arr = new T[capacity];
        if (type == HeapType.Min)
            _swapComparer = (x, y) => x.CompareTo(y) > 0;
        else if (type == HeapType.Max)
            _swapComparer = (x, y) => x.CompareTo(y) < 0;

        Type = type;
    }

    public T Peek()
    {
        if (Count == 0)
            throw new InvalidOperationException("Heap is empty.");
        
        return _arr[0];
    }

    public void Push(T value)
    {
        if (_arr.Length == Count)
        {
            int newCapacity = (int)(_arr.Length * CapacityMultiplier);
            T[] newArr = new T[newCapacity];

            for (int i = 0; i < _arr.Length; i++)
            {
                newArr[i] = _arr[i];
            }

            _arr = newArr;
        }

        _arr[Count] = value;

        int currentIdx = Count;
        while (currentIdx > 0)
        {
            int parentIdx = (currentIdx - 1) / 2;
            if (_swapComparer(_arr[parentIdx], _arr[currentIdx]))
            {
                T temp = _arr[parentIdx];
                _arr[parentIdx] = _arr[currentIdx];
                _arr[currentIdx] = temp;

                currentIdx = parentIdx;
            }
            else
            {
                break;
            }
        }
        
        Count++;
    }

    public T Pop()
    {
        if (Count == 0)
            throw new InvalidOperationException("Heap is empty.");

        T ret = _arr[0];
        _arr[0] = _arr[Count - 1];

        int currentIdx = 0;
        while (currentIdx < Count - 1)
        {
            int child = currentIdx * 2 + 1;
            if (child < Count - 1 && _swapComparer(_arr[child], _arr[child + 1]))
            {
                child += 1;
            }

            if (child <= Count - 1 && _swapComparer(_arr[currentIdx], _arr[child]))
            {
                T temp = _arr[child];
                _arr[child] = _arr[currentIdx];
                _arr[currentIdx] = temp;

                currentIdx = child;
            }
            else
            {
                break;
            }
        }

        Count--;
        return ret;
    }

    public bool IsEmpty()
    {
        return Count == 0;
    }

    public HeapType Type { get; }

    public int Count { get; private set; }
}