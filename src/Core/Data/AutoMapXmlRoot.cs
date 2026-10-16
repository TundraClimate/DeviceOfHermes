using System.Xml.Serialization;

namespace DeviceOfHermes.Data;

/// <summary>A template of AutoMapXml root</summary>
public class AutoMapXmlRoot
{
    /// <summary>A MapSettings</summary>
    [XmlElement("AutoMap")]
    public List<AutoMapXmlInfo> info = new();
}
