namespace DeviceOfHermes.Derive;

/// <summary>An attribute of derive for auto implements</summary>
[AttributeUsage(AttributeTargets.Class)]
public class DeriveAttribute(params Type[] template) : Attribute
{
    /// <summary>template</summary>
    public Type[] template = template;
}
