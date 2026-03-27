using UnityEngine;

namespace System;

public static class Hermes
{
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

    public static void Store<T>(T data)
    {
        DataStorage<T>.Data = data;
    }

    public static T Load<T>()
    {
        return DataStorage<T>.Data;
    }

    public static bool TryLoad<T>(out T? data)
    {
        data = DataStorage<T>._data;

        return data is not null;
    }

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

    public static Sprite? CreateSprite(string path, float pixPerUnit = 50f)
    {
        var fileBytes = File.ReadAllBytes(path);

        return CreateSprite(fileBytes, pixPerUnit);
    }
}

public enum MessageLevel
{
    Info,
    Warn,
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
