using System.Xml.Serialization;

namespace DeviceOfHermes.Data;

/// <summary>A dialog script apeends</summary>
[XmlRoot("BattleDialogRoot")]
public class DialogScriptXmlRoot
{
    /// <summary>Characters</summary>
    [XmlElement("Character")]
    public List<DialogScriptXmlInfo> list = [];
}
