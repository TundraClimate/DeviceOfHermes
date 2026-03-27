namespace System.Collections.Generic;

public class Peekable<T>
{
    public Peekable(IEnumerable enumrable)
    {
        _inner = enumrable.GetEnumerator();
    }

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
