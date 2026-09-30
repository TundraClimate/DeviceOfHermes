namespace DeviceOfHermes.Derive;

/// <summary>An attribute of represents usage</summary>
[AttributeUsage(AttributeTargets.Class)]
public class DeriveUsageAttribute(Type cls, string method) : Attribute
{
    /// <summary>defines class</summary>
    public Type cls = cls;

    /// <summary>target method</summary>
    public string method = method;
}
