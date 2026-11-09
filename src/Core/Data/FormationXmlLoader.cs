using DeviceOfHermes.Boot;
using DeviceOfHermes.Resource;

namespace DeviceOfHermes.Data;

internal static class FormationXmlLoader
{
    public static void Load()
    {
        foreach (var mod in HermesPreloader.ActiveMods)
        {
            if (!HermesConfigLoader.GetConfig(mod.invInfo.workshopInfo.uniqueId, out var conf))
            {
                continue;
            }

            var path = conf.data.Formation.Trim();

            if (string.IsNullOrEmpty(path))
            {
                continue;
            }

            var file = Path.Combine(mod.dirInfo.FullName, path);

            if (!File.Exists(file) || Serde.FromXmlFile<FormationXmlRoot>(file) is not FormationXmlRoot root)
            {
                continue;
            }

            var pid = mod.invInfo.workshopInfo.uniqueId;

            foreach (var info in root.list)
            {
                Formation.Add(pid, info);

                Hermes.Say($"FormationXml Load('{pid}'): {info.id}");
            }
        }
    }
}
