using DeviceOfHermes.Boot;
using DeviceOfHermes.Resource;

namespace DeviceOfHermes.Localize;

internal static class Localizer
{
    public static void Init()
    {
        TextModel.OnLoadLocalize += OnLocalize;
    }

    static void OnLocalize(string lang)
    {
        foreach (var mod in HermesPreloader.ActiveMods)
        {
            var localize = Path.Combine(mod.dirInfo.FullName, "Assemblies", "HermesLocalize", lang);

            if (!Directory.Exists(localize))
            {
                continue;
            }

            AbnoCardLocalizer.Load(localize);
        }
    }
}
