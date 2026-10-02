namespace DeviceOfHermes.Derive;

/// <summary>An attribute of derives priority</summary>
[AttributeUsage(AttributeTargets.Method)]
public class DerivePriorityAttribute(int priority) : Attribute
{
    /// <summary>priority</summary>
    public int priority = priority;
}
