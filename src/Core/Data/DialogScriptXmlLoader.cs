using DeviceOfHermes.Boot;
using DeviceOfHermes.Resource;

namespace DeviceOfHermes.Data;

internal static class DialogScriptXmlLoader
{
    public static void Load()
    {
        foreach (var mod in HermesPreloader.ActiveMods)
        {
            var relative = mod?.invInfo?.fileInfo?.dialogFile?.relativePath;

            if (string.IsNullOrEmpty(relative))
            {
                continue;
            }

            relative = relative!.StartsWith("/") || relative.StartsWith("\\") ? relative[1..] : relative;

            var path = Path.Combine(mod!.dirInfo.FullName, relative);

            if (!File.Exists(path) || Serde.FromXmlFile<DialogScriptXmlRoot>(path) is not DialogScriptXmlRoot root)
            {
                continue;
            }

            var pid = mod.invInfo.workshopInfo.uniqueId;

            foreach (var book in root.list)
            {
                if (string.IsNullOrEmpty(book.script))
                {
                    continue;
                }

                var lid = new LorId(pid, book.id);

                CustomDialogueScript.SetDialogScript(lid, book.script);

                Hermes.Say($"DialogScriptXml Load('{pid}:{book.id}'): {book.script}");
            }
        }
    }
}
