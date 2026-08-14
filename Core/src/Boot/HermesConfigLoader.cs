using System.Diagnostics.CodeAnalysis;
using Mod;
using DeviceOfHermes.Resource;

namespace DeviceOfHermes.Boot;

internal static class HermesConfigLoader
{
    public static void Load()
    {
        ConfigDict.Clear();

        foreach (var mod in HermesPreloader.ActiveMods)
        {
            var pid = mod.invInfo.workshopInfo.uniqueId;

            if (pid == "DeviceOfHermes")
            {
                continue;
            }

            try
            {
                var file = Path.Combine(mod.dirInfo.FullName, "Hermes.toml");

                if (File.Exists(file))
                {
                    var conf = Serde.FromTomlFile<HermesConfig>(file);

                    if (conf is not null)
                    {
                        ConfigDict[pid] = (conf, mod);

                        continue;
                    }
                }

                ConfigDict[pid] = (new(), mod);
            }
            catch (Exception)
            {
                Hermes.Say($"HermesConfigLoader(pid: {pid}): 'Hermes.toml' Load failed", MessageLevel.Warn);

                continue;
            }
        }
    }

    public static bool GetConfig(string pid, [NotNullWhen(true)] out HermesConfig? config)
    {
        if (!ConfigDict.TryGetValue(pid, out var res))
        {
            config = null;

            return false;
        }

        config = res.Item1;

        return true;
    }

    public static Dictionary<string, (HermesConfig, ModContentInfo)> ConfigDict = new();
}
