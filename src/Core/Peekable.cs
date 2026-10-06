namespace System.Collections.Generic;

/// <summary>
/// Provides a wrapper over an enumerator that supports peeking the next element
/// without consuming it.
/// </summary>
/// <remarks>
/// This class wraps an <see cref="IEnumerator"/> and allows inspecting the next
/// element using <see cref="Peek(out T?)"/> without advancing the sequence.
/// <para/>
/// When <see cref="Peek(out T?)"/> is called, the value is cached internally.
/// The subsequent call to <see cref="MoveNext(out T?)"/> will return the same value.
/// </remarks>
/// <example>
/// <code>
/// var source = new List&lt;int&gt; { 1, 2, 3 };
/// var it = new Peekable&lt;int&gt;(source);
///
/// if (it.Peek(out var next))
/// {
///     Console.WriteLine(next); // 1 (not consumed yet)
/// }
///
/// it.MoveNext(out var current);
/// Console.WriteLine(current); // 1
/// </code>
/// </example>
public class Peekable<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Peekable{T}"/> class
    /// from the specified enumerable source.
    /// </summary>
    /// <param name="enumrable">The source enumerable.</param>
    public Peekable(IEnumerable enumrable)
    {
        _inner = enumrable.GetEnumerator();
    }

    /// <summary>
    /// Retrieves the next element without consuming it.
    /// </summary>
    /// <param name="peeked">
    /// When this method returns, contains the next element if available;
    /// otherwise, the default value.
    /// </param>
    /// <returns>
    /// <c>true</c> if the next element exists; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method advances the underlying enumerator only once and caches
    /// the value internally so it can be returned again by
    /// <see cref="MoveNext(out T?)"/>.
    /// </remarks>
    public bool Peek(out T? peeked)
    {
        if (!this._hasPeeked)
        {
            if (!this._inner.MoveNext())
            {
                peeked = default(T);

                return false;
            }

            this._peeked = (T)this._inner.Current;
            this._hasPeeked = true;
        }

        peeked = this._peeked;

        return true;
    }

    /// <summary>
    /// Advances to the next element and returns it.
    /// </summary>
    /// <param name="current">
    /// When this method returns, contains the current element if available;
    /// otherwise, the default value.
    /// </param>
    /// <returns>
    /// <c>true</c> if the next element exists; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// If <see cref="Peek(out T?)"/> was previously called, this method
    /// returns the cached value and clears the internal state.
    /// </remarks>
    public bool MoveNext(out T? current)
    {
        if (!this.Peek(out T? peeked))
        {
            current = peeked;

            return false;
        }

        current = _peeked;

        this._hasPeeked = false;

        return true;
    }

    private IEnumerator _inner;

    private T? _peeked;

    private bool _hasPeeked = false;
}
