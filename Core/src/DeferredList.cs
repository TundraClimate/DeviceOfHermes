namespace System.Collections.Generic;

/// <summary>A extension of DeferredList</summary>
public static class DeferredListExtension
{
    /// <summary>Creates DeferredList</summary>
    public static DeferredList<T> AsDefer<T>(this List<T> list, out DeferredList<T> self)
    {
        self = new(list);

        return self;
    }
}

/// <summary>A list of deferred</summary>
public struct DeferredList<T>(List<T> items) : IEnumerable<T>
{
    /// <summary>Impls GetEnumerator</summary>
    public IEnumerator<T> GetEnumerator()
    {
        _remains += 1;

        return new DeferredListEnumerator<T>(_items, _remains, _adds, _rms);
    }

    /// <summary>Adds Adding pending list</summary>
    public void Add(T item)
    {
        _adds.Add(item);
    }

    /// <summary>Adds Removing pending list</summary>
    public void Remove(T item)
    {
        _rms.Add(item);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private List<T> _items = items;

    private List<T> _adds = new();

    private List<T> _rms = new();

    private int _remains = 0;
}

/// <summary>A enumerator of DeferredList</summary>
public struct DeferredListEnumerator<T>(List<T> items, int remains, List<T> adds, List<T> rms) : IEnumerator<T>
{
    /// <summary>Impls Current</summary>
    public T Current => _items[_index];

    object? IEnumerator.Current => Current;

    /// <summary>Impls MoveNext</summary>
    public bool MoveNext() => _items.Count > ++_index;

    /// <summary>Impls Reset</summary>
    public void Reset()
    {
        _index = -1;
    }

    /// <summary>Impls Dispose</summary>
    public void Dispose()
    {
        _remains -= 1;

        if (_remains == 0)
        {
            Flush();
        }
    }

    private void Flush()
    {
        foreach (var a in _adds)
        {
            _items.Add(a);
        }

        foreach (var r in _rms)
        {
            _items.Remove(r);
        }
    }

    private List<T> _items = items;

    private List<T> _adds = adds;

    private List<T> _rms = rms;

    private int _remains = remains;

    private int _index = -1;
}
