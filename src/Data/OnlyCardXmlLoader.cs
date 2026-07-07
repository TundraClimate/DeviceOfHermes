using DeviceOfHermes.Boot;
using DeviceOfHermes.Resource;

namespace DeviceOfHermes.Data;

internal static class OnlyCardXmlLoader
{
    public static void Load()
    {
        foreach (var mod in HermesPreloader.ActiveMods)
        {
            var file = Path.Combine(mod.dirInfo.FullName, "Assemblies", "HermesData", "OnlyCard.xml");

            if (!File.Exists(file))
            {
                continue;
            }

            var root = Serde.FromXmlFile<OnlyCardXmlRoot>(file);

            if (root is null)
            {
                continue;
            }

            var pid = mod.invInfo.workshopInfo.uniqueId;

            foreach (var info in root.info)
            {
                var ao = new AdditonalOnlyCard(info.Target(pid));

                ao.AddCards(info.Cards(pid).ToArray());
            }
        }
    }
}
