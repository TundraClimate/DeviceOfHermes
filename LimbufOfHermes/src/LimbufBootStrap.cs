using System.Text;
using LOR_XML;
using DeviceOfHermes.Boot;
using DeviceOfHermes.Resource;

namespace LimbufOfHermes.Boot;

internal class LimbufBootStrap : HermesInitializer
{
    public void OnInitMod()
    {
        LoadAllBufIcons();

        TextModel.OnLoadLocalize += OnLocalize;
    }

    static void LoadAllBufIcons()
    {
        foreach (var (name, data) in _manifest)
        {
            if (!name.EndsWith(".png"))
            {
                continue;
            }

            var id = name.Split('.')[2];

            Artwork.SetBattleUnitBufSprite(id, Artwork.CreateSprite(data)!);
        }
    }

    static void OnLocalize(string lang)
    {
        lang = lang.ToLower();

        LocalizeEffectText(lang);
    }

    static void LocalizeEffectText(string lang)
    {
        var data = _manifest.FirstOrDefault((name) => name.Key.EndsWith($"{lang}_EffectText.xml")).Value;

        var text = Encoding.UTF8.GetString(data);
        var xml = Serde.FromXmlStr<BattleEffectTextRoot>(text);

        TextModel.SetBattleEffectTexts(xml!.effectTextList, true);
    }

    static Dictionary<string, byte[]> InitEmbeds()
    {
        var assembly = typeof(LimbufBootStrap).Assembly;
        var result = new Dictionary<string, byte[]>();

        foreach (var resourceName in assembly.GetManifestResourceNames())
        {
            using var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
            {
                continue;
            }

            using var ms = new MemoryStream();

            stream.CopyTo(ms);

            result[resourceName] = ms.ToArray();
        }

        return result;
    }

    private static Dictionary<string, byte[]> _manifest = InitEmbeds();
}
