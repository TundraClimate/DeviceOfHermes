using UnityEngine;
using NAudio.Wave;

namespace DeviceOfHermes.Resource;

/// <summary>A utility class of audio</summary>
public static class Audio
{
    /// <summary>Read mp3 file to AudioClip</summary>
    public static AudioClip? ReadFromMp3(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        using var reader = new Mp3FileReader(path);
        using var ms = new MemoryStream();

        var buffer = new byte[8192];
        var bytesRead = 0;

        while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
        {
            ms.Write(buffer, 0, bytesRead);
        }

        var pcmBytes = ms.ToArray();

        if (reader.WaveFormat.Encoding is not WaveFormatEncoding.Pcm || reader.WaveFormat.BitsPerSample != 16)
        {
            return null;
        }

        var sampleCount = pcmBytes.Length / 2;
        var samples = new float[sampleCount];

        for (var i = 0; i < sampleCount; i++)
        {
            samples[i] = BitConverter.ToInt16(pcmBytes, i * 2) / 32768f;
        }

        var channels = reader.WaveFormat.Channels;
        var frequency = reader.WaveFormat.SampleRate;
        var frames = sampleCount / channels;

        return AudioClip.Create(Path.GetFileNameWithoutExtension(path), frames, channels, frequency, false).Also(c => c.SetData(samples, 0));
    }
}
