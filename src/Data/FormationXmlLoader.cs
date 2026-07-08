using DeviceOfHermes.Boot;
using DeviceOfHermes.Resource;

namespace DeviceOfHermes.Data;

internal static class FormationXmlLoader
{
    public static void Load()
    {
        foreach (var mod in HermesPreloader.ActiveMods)
        {
            var file = Path.Combine(mod.dirInfo.FullName, "Assemblies", "HermesData", "Formation.xml");

            if (!File.Exists(file))
            {
                continue;
            }

            var root = Serde.FromXmlFile<FormationXmlRoot>(file);

            if (root is null)
            {
                continue;
            }

            var pid = mod.invInfo.workshopInfo.uniqueId;

            foreach (var info in root.list)
            {
                Hermes.Say($"pid: {pid}, id: {info.id}");

                Formation.Add(pid, info);
            }
        }
    }
}
