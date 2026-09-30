using Microsoft.CodeAnalysis;

internal record struct Result<T>
{
    public static Result<T> Ok(T value)
    {
        return new() { kind = Kind.Ok, Value = value };
    }

    public static Result<T> Err(Diagnostic value)
    {
        return new() { kind = Kind.Err, diagnostic = value };
    }

    public bool IsOk => kind is Kind.Ok;

    public T? value => Value;

    public Diagnostic? Error => diagnostic;

    public bool TryGetValue(out T? value)
    {
        value = Value;

        return IsOk;
    }

    Kind kind;

    T? Value;

    Diagnostic? diagnostic;

    enum Kind
    {
        Ok,
        Err,
    }
}
