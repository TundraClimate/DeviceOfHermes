namespace DeviceOfHermes.Derive;

/// <summary>An attribute of derive defines</summary>
[AttributeUsage(AttributeTargets.Class)]
public class DeriveTemplateAttribute(Type target) : Attribute
{
    /// <summary>target</summary>
    public Type target = target;
}
