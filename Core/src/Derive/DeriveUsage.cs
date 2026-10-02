namespace DeviceOfHermes.Derive;

/// <summary>An attribute of represents usage</summary>
[AttributeUsage(AttributeTargets.Method)]
public class DeriveUsageAttribute(Type cls, string method, Type[]? args = null) : Attribute
{
    /// <summary>defines class</summary>
    public Type cls = cls;

    /// <summary>target method</summary>
    public string method = method;

    /// <summary>target args</summary>
    public Type[]? args = args;
}
