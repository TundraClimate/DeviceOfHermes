using DeviceOfHermes.Boot;
using DeviceOfHermes.Resource;

namespace DeviceOfHermes.Data;

internal static class EmotionCardXmlLoader
{
    public static void Load()
    {
        foreach (var mod in HermesPreloader.ActiveMods)
        {
            var file = Path.Combine(mod.dirInfo.FullName, "Assemblies", "HermesData", "EmotionCard.xml");

            if (!File.Exists(file))
            {
                continue;
            }

            var root = Serde.FromXmlFile<EmotionCardXmlRoot>(file);

            if (root is null)
            {
                continue;
            }

            var pid = mod.invInfo.workshopInfo.uniqueId;

            foreach (var info in root.emotionCardXmlList)
            {
                EmotionCard.Add(pid, info);
            }
        }
    }
}
