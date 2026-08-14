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
            if (!HermesConfigLoader.GetConfig(mod.invInfo.workshopInfo.uniqueId, out var conf))
            {
                continue;
            }

            if (conf.localize.AbnoCard.Trim() is string abnocard && !string.IsNullOrEmpty(abnocard))
            {
                AbnoCardLocalizer.Load(Path.Combine(mod.dirInfo.FullName, string.Format(abnocard, lang)));
            }
        }
    }
}
