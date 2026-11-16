using DeviceOfHermes.Resource;

namespace DeviceOfHermes.Localize;

internal static class BookTagLocalizer
{
    public static void Load(string localizeDirPath)
    {
        if (!Directory.Exists(localizeDirPath))
        {
            return;
        }

        foreach (var file in Walkdir.GetFilesRecursive(localizeDirPath))
        {
            var dict = Serde.FromTomlFile<Dictionary<string, string>>(file);

            if (dict is null)
            {
                continue;
            }

            TextModel.SetTextData(dict.Map(e => ($"DeviceOfHermes_BookTag_{e.Key}", e.Value)), true);
        }
    }

    public static void LoadBuiltin(string lang)
    {
        var builtin = lang switch
        {
            "jp" => HermesConstants.BuiltinBookTagLocalizeJP,

            _ => new(),
        };

        TextModel.SetTextData(builtin.Map(e => ($"DeviceOfHermes_BookTag_{e.Key}", e.Value)), true);
    }
}
