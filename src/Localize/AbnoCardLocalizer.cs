using DeviceOfHermes.Resource;
using LOR_XML;

namespace DeviceOfHermes.Localize;

internal static class AbnoCardLocalizer
{
    public static void Load(string localizeDirPath)
    {
        var files = Path.Combine(localizeDirPath, "AbnoCard");

        List<AbnormalityCard> data = new();

        foreach (var file in Walkdir.GetFilesRecursive(files))
        {
            var root = Serde.FromXmlFile<AbnormalityCardsRoot>(file);

            if (root is null)
            {
                continue;
            }

            data.AddRange(root.sephirahList.FlatMap(s => s.list));
        }

        TextModel.SetAbnormalityCards(data);
    }
}
