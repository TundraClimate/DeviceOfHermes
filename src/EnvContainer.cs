using GameSave;

namespace DeviceOfHermes.Resource;

/// <summary>Mod enviroment container</summary>
public class EnvContainer
{
    private EnvContainer(string path, ContainerInfo info)
    {
        _container = path;
        _info = info;
    }

    /// <summary>Creates new Container</summary>
    public static EnvContainer? Register<T>()
        where T : ModPackage, new()
    {
        var path = Path.Combine(SaveManager.savePath, "EnvContainer", PackageInfo<T>.Id);
        var content = new ContainerInfo();
        var res = new EnvContainer(path, content);

        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var containerJson = Path.Combine(path, "container.json");

            if (!File.Exists(containerJson))
            {
                Serde.ToJsonFile(content, containerJson);
            }
            else
            {
                res._info = Serde.FromJsonFile<ContainerInfo>(containerJson)
                    ?? throw new InvalidOperationException($"{PackageInfo<T>.Id}/container.json");

                foreach (var crate in content.Crates)
                {
                    res.AddCrate(crate);
                }
            }
        }
        catch (IOException e)
        {
            Hermes.Say($"EnvContainer '{PackageInfo<T>.Id}' failed register: {e.Message}", MessageLevel.Error);
            Hermes.Say($"{e.StackTrace}", MessageLevel.Error);

            return null;
        }
        catch (InvalidOperationException)
        {
            throw;
        }

        return res;
    }

    /// <summary>Add new Crate or load Crate</summary>
    public EnvCrate? AddCrate(string name)
    {
        if (name == "container")
        {
            Hermes.Say("Crate name 'container' not available", MessageLevel.Warn);

            return null;
        }

        var crate = EnvCrate.Register(_container, name);

        if (crate is not null)
        {
            _crates.TryAdd(name, crate);
        }

        return crate;
    }

    /// <summary>Saves this</summary>
    public void Save()
    {
        _info.Crates = _crates.Keys.Collect();

        Serde.ToJsonFile(_info, Path.Combine(_container, "container.json"));

        foreach (var (_, crate) in _crates)
        {
            crate.Save();
        }
    }

    private Dictionary<string, EnvCrate> _crates = new();

    private string _container;

    private ContainerInfo _info;
}

/// <summary>A dataset on env</summary>
public class EnvCrate
{
    private EnvCrate(Dictionary<string, CrateData?> data, string dir, string file)
    {
        _data = data;
        _dir = dir;
        _file = file;
    }

    internal static EnvCrate? Register(string containerPath, string crateName)
    {
        var filePath = Path.Combine(containerPath, $"{crateName}.json");
        var dirPath = Path.Combine(containerPath, $"{crateName}.d");
        var name = Path.GetFileName(containerPath);
        Dictionary<string, CrateData?> data = new();

        try
        {
            if (!File.Exists(filePath))
            {
                Serde.ToJsonFile(data, filePath);
            }
            else
            {
                data = Serde.FromJsonFile<Dictionary<string, CrateData?>>(filePath)
                    ?? throw new InvalidOperationException($"{name}/{crateName}");
            }

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }
        catch (IOException e)
        {
            Hermes.Say($"EnvContainer crate '{name}:{crateName}' failed register: {e.Message}", MessageLevel.Error);
            Hermes.Say($"{e.StackTrace}", MessageLevel.Error);

            return null;
        }
        catch (InvalidOperationException)
        {
            throw;
        }

        return new EnvCrate(data, dirPath, filePath);
    }

    /// <summary>Access value</summary>
    public object? this[string key]
    {
        get => GetValue(key);
        set => SetValue(key, value);
    }

    private object? GetValue(string key)
    {
        return _data.TryGetValue(key, out var res).Then(() => res?.Value);
    }

    private void SetValue(string key, object? value)
    {
        if (value is null && _data.ContainsKey(key))
        {
            _data[key] = null;

            return;
        }

        if (!_data.TryAdd(key, new CrateData() { Value = value }))
        {
            _data[key]?.Value = value;
        }
    }

    /// <summary>Load data</summary>
    public byte[]? GetData(string key)
    {
        var path = _data.TryGetValue(key, out var data).Then(() => data?.Path);

        if (path is null || !File.Exists(path))
        {
            return null;
        }

        using var stream = File.OpenRead(Path.Combine(_dir, path));
        using (MemoryStream ms = new MemoryStream())
        {
            stream.CopyTo(ms);

            return ms.ToArray();
        }
    }

    /// <summary>Set data</summary>
    public void SetData(string key, string name, byte[] data)
    {
        if (!_data.TryAdd(key, new CrateData() { Path = name }))
        {
            _data[key]?.Path = name;
        }

        File.WriteAllBytes(Path.Combine(_dir, name), data);
    }

    /// <summary>Saves this</summary>
    public void Save()
    {
        Serde.ToJsonFile(_data, _file);
    }

    private Dictionary<string, CrateData?> _data;

    private string _dir;

    private string _file;
}

internal class ContainerInfo
{
    public List<string> Crates { get; set; } = new();
}

internal class CrateData
{
    public object? Value { get; set; }

    public string? Path { get; set; }
}
