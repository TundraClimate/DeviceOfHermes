using System.Xml;
using System.Xml.Serialization;

namespace DeviceOfHermes.Resource;

public class ReadXmlParser
{
    public static T? Read<T>(string path)
    {
        if (!File.Exists(path))
        {
            Hermes.Say($"Read file failed: '{path}' is not exists.", MessageLevel.Warn);

            return default(T);
        }

        using var reader = new StreamReader(path);
        var serde = new XmlSerializer(typeof(T));

        var settings = new XmlReaderSettings()
        {
            IgnoreComments = true,
            IgnoreWhitespace = true,
        };

        using var xmlReader = XmlReader.Create(reader, settings);

        try
        {
            return (T)serde.Deserialize(xmlReader);
        }
        catch (Exception e)
        {
            Hermes.Say($"Xml parse failed: Readed content that from '{path}' is not deserializable", MessageLevel.Warn);

            Hermes.Say(e.Message ?? "Unknown infomation", MessageLevel.Warn);

            return default(T);
        }
    }
}
