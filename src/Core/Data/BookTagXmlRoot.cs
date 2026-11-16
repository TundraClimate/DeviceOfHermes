using System.Xml.Serialization;

namespace DeviceOfHermes.Data;

/// <summary>A template of BookTagXml root</summary>
public class BookTagXmlRoot
{
    /// <summary>The informations</summary>
    [XmlElement("Book")]
    public List<BookTagXmlInfo> info = new();
}
