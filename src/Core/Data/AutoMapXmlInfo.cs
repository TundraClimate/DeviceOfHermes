using System.Xml.Serialization;

namespace DeviceOfHermes.Data;

/// <summary>A template of AutoMapXml</summary>
public class AutoMapXmlInfo
{
    /// <summary>A key attaches MapInfo</summary>
    [XmlAttribute("MapInfo")]
    public string key = "";

    /// <summary>A bg sprite path</summary>
    [XmlElement("BgImage")]
    public string bg = "";

    /// <summary>A floor sprite path</summary>
    [XmlElement("FloorImage")]
    public string floor = "";

    /// <summary>A bgm audio path</summary>
    [XmlElement("BGM")]
    public List<string> bgm = new();

    /// <summary>A dialog color code</summary>
    [XmlElement("DialogColor")]
    public string dlgColor = "#FFFFFF";

    /// <summary>A dialog ids</summary>
    [XmlElement("Dialog")]
    public List<string> dialog = new();

    /// <summary>A options</summary>
    [XmlElement("Option")]
    public List<NewMapOptions> option = new();
}
