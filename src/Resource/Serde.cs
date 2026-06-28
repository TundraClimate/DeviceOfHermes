using System.Xml;
using System.Xml.Serialization;

namespace DeviceOfHermes.Resource;

/// <summary>Serialize and deserialize</summary>
public static class Serde
{
    /// <summary>Deserialize from xmlreader</summary>
    public static T? FromXml<T>(XmlReader reader)
    {
        var serde = new XmlSerializer(typeof(T));

        try
        {
            return (T)serde.Deserialize(reader);
        }
        catch (InvalidOperationException e)
        {
            Hermes.Say($"Xml parse failed: Readed content is not deserializable", MessageLevel.Warn);

            Hermes.Say(e.InnerException?.Message ?? "Unknown infomation", MessageLevel.Warn);

            return default(T);
        }
    }

    /// <summary>Deserialize from string xml</summary>
    public static T? FromXmlStr<T>(string content)
    {
        using var reader = new StringReader(content);

        var settings = new XmlReaderSettings()
        {
            IgnoreComments = true,
            IgnoreWhitespace = true,
            CloseInput = true,
        };

        using var xmlReader = XmlReader.Create(reader, settings);

        return FromXml<T>(xmlReader);
    }

    /// <summary>Deserialize from xml file</summary>
    public static T? FromXmlFile<T>(string path)
    {
        if (!File.Exists(path))
        {
            Hermes.Say($"Read file failed: '{path}' is not exists.", MessageLevel.Warn);

            return default(T);
        }

        using var reader = new StreamReader(path);

        var settings = new XmlReaderSettings()
        {
            IgnoreComments = true,
            IgnoreWhitespace = true,
            CloseInput = true,
        };

        using var xmlReader = XmlReader.Create(reader, settings);

        return FromXml<T>(xmlReader);
    }

    /// <summary>Serialize to writer</summary>
    public static void ToXml<T>(T value, XmlWriter writer)
    {
        var serializer = new XmlSerializer(typeof(T));

        var ns = new XmlSerializerNamespaces();

        ns.Add("", "");

        serializer.Serialize(writer, value, ns);
    }

    /// <summary>Serialize to string</summary>
    public static string ToXmlStr<T>(T value)
    {
        var settings = new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = false,
            CloseOutput = true,
        };

        using var sw = new StringWriter();
        using var writer = XmlWriter.Create(sw, settings);

        ToXml(value, writer);

        return sw.ToString();
    }

    /// <summary>Serialize to file</summary>
    public static void ToXmlFile<T>(T value, string path)
    {
        var settings = new XmlWriterSettings
        {
            Indent = true,
            OmitXmlDeclaration = false,
            CloseOutput = true,
        };

        using var writer = XmlWriter.Create(path, settings);

        ToXml(value, writer);
    }
}

/// <summary>A generic Xmls parser</summary>
/// <example><code>
/// var path = "data.xml";
///
/// var data = ReadXmlParser.Read&lt;Data&gt;(path);
/// </code></example>
[Obsolete]
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
