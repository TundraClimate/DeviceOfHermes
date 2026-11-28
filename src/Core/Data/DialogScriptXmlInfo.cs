using System.Xml.Serialization;

namespace DeviceOfHermes.Data;

/// <summary>A dialog script appends</summary>
[XmlRoot("Character")]
public class DialogScriptXmlInfo
{
    /// <summary>ID</summary>
    [XmlAttribute("ID")]
    public int id;

    /// <summary>Script</summary>
    [XmlElement("DialogScript")]
    public string script = "";
}
