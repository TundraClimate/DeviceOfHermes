using System.Xml.Serialization;

namespace DeviceOfHermes.Data;

/// <summary>A template of OnlyCardXml</summary>
public class OnlyCardXmlInfo
{
    internal LorId Target(string defaultPid) => LorId.MakeLorId(new LorIdXml(pid, id), defaultPid);

    internal List<LorId> Cards(string defaultPid)
    {
        List<LorId> res = new();

        LorId.InitializeLorIds(cards, res, defaultPid);

        return res;
    }

    /// <summary>A target of keypage pid</summary>
    [XmlAttribute("Pid")]
    public string pid = "";

    /// <summary>A target of keypage id</summary>
    [XmlAttribute("ID")]
    public int id = -1;

    /// <summary>A list of onlypage</summary>
    [XmlElement("Card")]
    public List<LorIdXml> cards = new();
}
