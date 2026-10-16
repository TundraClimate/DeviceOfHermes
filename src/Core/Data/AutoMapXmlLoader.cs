using System.Globalization;
using UnityEngine;
using DeviceOfHermes.Boot;
using DeviceOfHermes.Resource;

namespace DeviceOfHermes.Data;

internal static class AutoMapXmlLoader
{
    public static void Load()
    {
        foreach (var mod in HermesPreloader.ActiveMods)
        {
            if (!HermesConfigLoader.GetConfig(mod.invInfo.workshopInfo.uniqueId, out var conf))
            {
                continue;
            }

            var path = conf.data.AutoMap.Trim();
            var artworks = conf.resource.AutoMapArtwork.Trim();
            var audios = conf.resource.AutoMapAudio.Trim();

            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(artworks) || string.IsNullOrEmpty(audios))
            {
                continue;
            }

            var file = Path.Combine(mod.dirInfo.FullName, path);

            if (!File.Exists(file) || Serde.FromXmlFile<AutoMapXmlRoot>(file) is not AutoMapXmlRoot root)
            {
                continue;
            }

            var pid = mod.invInfo.workshopInfo.uniqueId;

            foreach (var info in root.info)
            {
                var bgPath = Path.Combine(mod.dirInfo.FullName, artworks, info.bg);

                if (!File.Exists(bgPath) || Artwork.CreateSprite(bgPath) is not Sprite bg)
                {
                    Hermes.Say($"AutoMap: image file '{pid}:{bgPath}' is not exists", MessageLevel.Warn);

                    continue;
                }

                var floorPath = Path.Combine(mod.dirInfo.FullName, artworks, info.floor);

                if (!File.Exists(floorPath) || Artwork.CreateSprite(floorPath) is not Sprite floor)
                {
                    Hermes.Say($"AutoMap: image file '{pid}:{floorPath}' is not exists", MessageLevel.Warn);

                    continue;
                }

                var audioPath = Path.Combine(mod.dirInfo.FullName, audios);
                var bgm = new List<AudioClip>();

                foreach (var bgmPath in info.bgm)
                {
                    var p = Path.Combine(audioPath, bgmPath);

                    if (!File.Exists(p) || Audio.ReadFromMp3(p) is not AudioClip clip)
                    {
                        Hermes.Say($"AutoMap: audio file '{pid}:{p}' is not exists", MessageLevel.Warn);

                        break;
                    }

                    bgm.Add(clip);
                }

                if (GetColor(info.dlgColor) is not Color dlgColor)
                {
                    Hermes.Say($"AutoMap: color code '{pid}:{info.dlgColor}' is not valid", MessageLevel.Warn);

                    continue;
                }

                var opt = NewMapOptions.None;

                foreach (var o in info.option)
                {
                    opt |= o;
                }

                AutoMapSetter.SetMapFactory(info.key, f => f.CreateNewMap(bg, floor, bgm.ToArray(), dlgColor, info.dialog, opt));
            }
        }
    }

    static Color? GetColor(string code)
    {
        if (code.Length != 7 || code.StripPrefix("#") is not string color)
        {
            return null;
        }

        if (!byte.TryParse(color.Substring(0, 2), NumberStyles.HexNumber, null, out var r)
            || !byte.TryParse(color.Substring(2, 2), NumberStyles.HexNumber, null, out var g)
            || !byte.TryParse(color.Substring(4, 2), NumberStyles.HexNumber, null, out var b))
        {
            return null;
        }

        return new Color32(r, g, b, 255);
    }
}
