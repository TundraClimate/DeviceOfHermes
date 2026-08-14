using System.Xml.Serialization;

namespace DeviceOfHermes.Data;

/// <summary>A template of OnlyCardXml root</summary>
public class OnlyCardXmlRoot
{
    /// <summary>The onlycards</summary>
    [XmlElement("OnlyCard")]
    public List<OnlyCardXmlInfo> info = new();
}
