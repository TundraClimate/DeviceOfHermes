using HarmonyLib;
using HarmonyExtension;

namespace System.Collections.Generic;

/// <summary>A extension of RefList</summary>
public static class RefListExtension
{
    /// <summary>Creates RefList</summary>
    public static RefList<T> AsRef<T>(this List<T> list) => new(list);
}

/// <summary>A list of element refs</summary>
public struct RefList<T>(List<T> origin)
{
    /// <summary>Impls GetEnumerator</summary>
    public RefListEnumerator<T> GetEnumerator()
        => new RefListEnumerator<T>(Items, Count);

    /// <summary>Impls Indexer</summary>
    public ref T this[int idx]
    {
        get => ref Items[idx];
    }

    private ref T[] Items => ref _itemsRef(_origin);

    private ref int Count => ref _countRef(_origin);

    private List<T> _origin = origin;

    private static AccessTools.FieldRef<List<T>, T[]> _itemsRef
        = typeof(List<T>).FieldRefAccess<T[]>("_items");

    private static AccessTools.FieldRef<List<T>, int> _countRef
        = typeof(List<T>).FieldRefAccess<int>("_size");
}

/// <summary>A enumerator of RefList</summary>
public struct RefListEnumerator<T>(T[] items, int count)
{
    private readonly T[] _items = items;
    private readonly int _count = count;
    private int _index = -1;

    /// <summary>Impls MoveNext</summary>
    public bool MoveNext() => _count > ++_index;

    /// <summary>Impls Current</summary>
    public ref T Current => ref _items[_index];
}
