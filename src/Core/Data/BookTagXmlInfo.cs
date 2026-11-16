using System.Xml.Serialization;

namespace DeviceOfHermes.Data;

/// <summary>A template of BookTagXml</summary>
public class BookTagXmlInfo
{
    internal LorId Target(string defaultPid) => LorId.MakeLorId(new LorIdXml(pid, id), defaultPid);

    /// <summary>A target of keypage pid</summary>
    [XmlAttribute("Pid")]
    public string pid = "";

    /// <summary>A target of keypage id</summary>
    [XmlAttribute("ID")]
    public int id = -1;

    /// <summary>A list of tags</summary>
    [XmlElement("Tag")]
    public List<string> tags = new();
}
