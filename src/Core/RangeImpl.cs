using System.Collections;

namespace System;

/// <summary>A re-implementation the netcore Index</summary>
public readonly struct Index(int value, bool fromEnd = false) : IEquatable<Index>
{
    /// <summary>Returns value</summary>
    public int Value { get; } = value;

    /// <summary>Returns true if end that</summary>
    public bool IsFromEnd { get; } = fromEnd;

    /// <summary>Returns the offset number</summary>
    public int GetOffset(int length) => IsFromEnd ? length - Value : Value;

    /// <summary>Impl implicit</summary>
    public static implicit operator Index(int value) => new Index(value);

    /// <summary>Equals</summary>
    public bool Equals(Index other) => Value == other.Value && IsFromEnd == other.IsFromEnd;

    /// <summary>Impl Eq</summary>
    public static bool operator ==(Index self, Index other) => self.Equals(other);

    /// <summary>Impl Neq</summary>
    public static bool operator !=(Index self, Index other) => !self.Equals(other);

    /// <summary>Impl Equals</summary>
    public override bool Equals(object obj) => obj is Index other && Equals(other);

    /// <summary>Impl GetHashCode</summary>
    public override int GetHashCode() => (Value, IsFromEnd).GetHashCode();
}

/// <summary>A re-implementation the netcore Range</summary>
public readonly struct Range(Index start, Index end) : IEquatable<Range>, IEnumerable<int>
{
    /// <summary>Returns the start number</summary>
    public Index Start { get; } = start;

    /// <summary>Returns the end number</summary>
    public Index End { get; } = end;

    /// <summary>Equals</summary>
    public bool Equals(Range other) => Start.Equals(other.Start) && End.Equals(other.End);

    /// <summary>Impl Eq</summary>
    public static bool operator ==(Range self, Range other) => self.Equals(other);

    /// <summary>Impl Neq</summary>
    public static bool operator !=(Range self, Range other) => !self.Equals(other);

    /// <summary>Impl Equals</summary>
    public override bool Equals(object obj) => obj is Range other && Equals(other);

    /// <summary>Impl GetHashCode</summary>
    public override int GetHashCode() => (Start, End).GetHashCode();

    /// <summary>Impl GetEnumerator</summary>
    public IEnumerator<int> GetEnumerator() => new RangeEnumerator(Start.Value, End.Value);

    /// <summary>Impl GetEnumerator</summary>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>An enumerator the Range</summary>
public struct RangeEnumerator(int start, int end) : IEnumerator<int>
{
    /// <summary>Returns the Current number</summary>
    public int Current => _min + _offset;

    object IEnumerator.Current => Current;

    /// <summary>Impl MoveNext</summary>
    public bool MoveNext() => reverse ? _min + --_offset > _max : _max > _min + ++_offset;

    /// <summary>Impl Dispose</summary>
    public void Dispose()
    {
    }

    /// <summary>Impl Reset</summary>
    public void Reset() => _offset = reverse ? 1 : -1;

    int _offset = start > end ? 1 : -1;

    int _min = start;

    int _max = end;

    bool reverse = start > end;
}
