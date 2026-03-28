using UnityEngine;

namespace System;

/// <summary>A main class of the DeviceOfHermes</summary>
/// <remarks>Hijacks the System and includes util</remarks>
/// <example>
/// <code>
/// Hermes.Say("Hermes says: Hello, rien.");
///
/// Hermes.Say("Prescript warning", MessageLevel.Warn);
///
/// Hermes.Say("*beep*", MessageLevel.Error);
/// </code>
/// </example>
public static class Hermes
{
    /// <summary>A wrapper of the UnityEngine.Debug.Log</summary>
    /// <param name="message">A message</param>
    /// <param name="lvl">Corresponds output level</param>
    /// <remarks>
    /// Says message with UnityEngine.Debug
    /// but dont needs the UnityEngine using.
    /// </remarks>
    /// <example><code>
    /// Hermes.Say("Debug message");
    /// </code></example>
    public static void Say(string? message, MessageLevel lvl = MessageLevel.Info)
    {
        switch (lvl)
        {
            case MessageLevel.Info:
                Debug.Log(message);

                break;
            case MessageLevel.Warn:
                Debug.LogWarning(message);

                break;
            case MessageLevel.Error:
                Debug.LogError(message);

                break;
        }
    }

    /// <summary>Stores data to global state</summary>
    /// <param name="data">A data that will store</param>
    /// <typeparam name="T">Corresponds state type</typeparam>
    /// <remarks>
    /// The setter also serves as Initializer,
    /// therefore must calls it on first.
    /// </remarks>
    /// <example><code>
    /// Hermes.Store(new DataType(12));
    ///
    /// int num = Hermes.Load&lt;DataType&gt;().Value;
    /// </code></example>
    public static void Store<T>(T data)
    {
        DataStorage<T>.Data = data;
    }

    /// <summary>Loads data from global state</summary>
    /// <typeparam name="T">Corresponds state type</typeparam>
    /// <returns>A data that loaded from global state</returns>
    /// <exception cref="System.InvalidOperationException">Throws when not initialized</exception>
    /// <remarks>
    /// Loads data that was stored.<br/>
    /// Dont calls when not state initialized.
    /// </remarks>
    /// <example><code>
    /// // var value = Hermes.Load&lt;DataType&gt;();
    /// // Throws InvalidOperationException by not initialized state
    ///
    /// Hermes.Store(new DataType(12));
    ///
    /// var number = Hermes.Load&lt;DataType&gt;().Value;
    /// </code></example>
    public static T Load<T>()
    {
        return DataStorage<T>.Data;
    }

    /// <summary>Try load data from global state</summary>
    /// <typeparam name="T">Corresponds state type</typeparam>
    /// <returns>The result of load by T</returns>
    /// <remarks>
    /// Loads data that was stored.<br/>
    /// Returns true when succeed by load.
    /// </remarks>
    /// <example><code>
    /// Hermes.Store(new DataType(12));
    ///
    /// if (Hermes.TryLoad(var out data))
    /// {
    ///     var num = data.Value;
    /// }
    /// </code></example>
    public static bool TryLoad<T>(out T? data)
    {
        data = DataStorage<T>._data;

        return data is not null;
    }

    /// <summary>Creates UnityEngine.Sprite from bytes</summary>
    /// <param name="bytes">The bytes that read by image</param>
    /// <param name="pixPerUnit">Pixel length used by Sprite.CreateSprite</param>
    /// <returns>A object of Sprite</returns>
    /// <remarks>
    /// The bytes that read image will convert with <see cref="UnityEngine.ImageConversion"/>. <br/>
    /// Returns null when can not convertion.
    /// </remarks>
    /// <example><code>
    /// var sprite = Hermes.CreateSprite(imageBytes);
    /// </code></example>
    public static Sprite? CreateSprite(byte[] bytes, float pixPerUnit = 50f)
    {
        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!ImageConversion.LoadImage(texture, bytes))
        {
            return null;
        }

        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            pixPerUnit
        );
    }

    /// <summary>Creates UnityEngine.Sprite from image path</summary>
    /// <param name="path">A path that read image</param>
    /// <param name="pixPerUnit">Pixel length used by Sprite.CreateSprite</param>
    /// <returns>A object of Sprite</returns>
    /// <remarks>
    /// Lets see <see cref="Hermes.CreateSprite(byte[], float)"/>
    /// </remarks>
    /// <example><code>
    /// var sprite = Hermes.CreateSprite("image.png");
    /// </code></example>
    public static Sprite? CreateSprite(string path, float pixPerUnit = 50f)
    {
        var fileBytes = File.ReadAllBytes(path);

        return CreateSprite(fileBytes, pixPerUnit);
    }
}

/// <summary>Log level used by <see cref="Hermes.Say(string?, MessageLevel)"/></summary>
public enum MessageLevel
{
    /// <summary>Info</summary>
    Info,
    /// <summary>Warn</summary>
    Warn,
    /// <summary>Error</summary>
    Error,
}

internal static class DataStorage<T>
{
    internal static T? _data;

    public static T Data
    {
        get => _data ?? throw new InvalidOperationException($"Data of {nameof(T)} must was initialized");
        set => _data = value;
    }
}
