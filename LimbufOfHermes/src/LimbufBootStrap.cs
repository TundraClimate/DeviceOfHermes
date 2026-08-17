global using UnityObject = UnityEngine.Object;
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
        LocalizeEtcText(lang);
    }

    static void LocalizeEffectText(string lang)
    {
        var data = _manifest.FirstOrDefault((name) => name.Key.EndsWith($"{lang}_EffectText.xml")).Value;

        var text = Encoding.UTF8.GetString(data);
        var xml = Serde.FromXmlStr<BattleEffectTextRoot>(text);

        TextModel.SetBattleEffectTexts(xml!.effectTextList, true);
    }

    static void LocalizeEtcText(string lang)
    {
        var data = _manifest.FirstOrDefault((name) => name.Key.EndsWith($"{lang}_EtcText.xml")).Value;

        var text = Encoding.UTF8.GetString(data);
        var xml = Serde.FromXmlStr<EtcDataXmlRoot>(text);

        TextModel.SetTextData(xml!.localize.Map(info => (info.id, info.Text)), true);
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

/// <summary>Localize template</summary>
[System.Xml.Serialization.XmlRoot("localize")]
public class EtcDataXmlRoot
{
    /// <summary>texts</summary>
    [System.Xml.Serialization.XmlElement("text")]
    public List<EtcDataXmlInfo> localize = new();
}

/// <summary>Localize template</summary>
public class EtcDataXmlInfo
{
    /// <summary>id</summary>
    [System.Xml.Serialization.XmlAttribute]
    public string id = "";

    /// <summary>text</summary>
    [System.Xml.Serialization.XmlText]
    public string Text = "";
}
