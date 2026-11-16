using DeviceOfHermes.Boot;
using DeviceOfHermes.Resource;

namespace DeviceOfHermes.Data;

internal static class BookTagXmlLoader
{
    public static void Load()
    {
        foreach (var mod in HermesPreloader.ActiveMods)
        {
            if (!HermesConfigLoader.GetConfig(mod.invInfo.workshopInfo.uniqueId, out var conf))
            {
                continue;
            }

            var path = conf.data.BookTag.Trim();

            if (string.IsNullOrEmpty(path))
            {
                continue;
            }

            var file = Path.Combine(mod.dirInfo.FullName, path);

            if (!File.Exists(file) || Serde.FromXmlFile<BookTagXmlRoot>(file) is not BookTagXmlRoot root)
            {
                continue;
            }

            var pid = mod.invInfo.workshopInfo.uniqueId;

            foreach (var info in root.info)
            {
                var target = info.Target(pid);

                BookTag.SetTags(target, info.tags);

                Hermes.Say($"BookTagXml Load('{pid}'): {target} <= {string.Join(" ", info.tags)}");
            }
        }
    }
}
