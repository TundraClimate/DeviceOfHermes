using System.Xml;
using System.Xml.Serialization;

namespace DeviceOfHermes.Resource;

/// <summary>A generic Xmls parser</summary>
/// <example><code>
/// var path = "data.xml";
///
/// var data = ReadXmlParser.Read&lt;Data&gt;(path);
/// </code></example>
public class ReadXmlParser
{
    /// <summary>Reads file then parse to T</summary>
    /// <param name="path">A xml path</param>
    /// <typeparam name="T">Type of Parse</typeparam>
    /// <returns>A result of read and parse</returns>
    /// <example><code>
    /// var path = "data.xml";
    ///
    /// var data = ReadXmlParser.Read&lt;Data&gt;(path);
    /// </code></example>
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
