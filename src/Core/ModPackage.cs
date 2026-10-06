namespace DeviceOfHermes;

/// <summary>A unit of mod</summary>
public interface ModPackage
{
    /// <summary>An id</summary>
    public string packageId { get; }
}

/// <summary>A information of package</summary>
public static class PackageInfo<T>
    where T : ModPackage, new()
{
    static PackageInfo()
    {
        Id = new T().packageId;
    }

    /// <summary>Returns package id</summary>
    public static string Id { get; private set; }

    /// <summary>Returns mod assemblies location</summary>
    public static string AsmPath => typeof(T).GetType().Assembly.Location;
}
