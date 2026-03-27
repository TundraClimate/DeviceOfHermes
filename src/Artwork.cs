using UnityEngine;

namespace DeviceOfHermes.Resource;

public static class Artwork
{
    public static void SetBattleUnitBufSprite(string unitBufId, Sprite sprite, bool replace = false)
    {
        var iconDict = BattleUnitBuf._bufIconDictionary;

        if (iconDict.ContainsKey(unitBufId))
        {
            if (replace)
            {
                iconDict[unitBufId] = sprite;
            }
            else
            {
                Hermes.Say($"Skipped: The keywordId '{unitBufId}' is already exists.", MessageLevel.Warn);
            }

            return;
        }

        iconDict.Add(unitBufId, sprite);
    }

    public static void SetBattleUnitBufSprite(string imgPath, bool replace = false)
    {
        var imgFileName = Path.GetFileName(imgPath);
        string imgId;

        if (imgFileName.EndsWith(".png") || imgFileName.EndsWith(".jpg"))
        {
            imgId = imgFileName.Substring(0, imgFileName.Length - 4);
        }
        else if (imgFileName.EndsWith(".jpeg"))
        {
            imgId = imgFileName.Substring(0, imgFileName.Length - 5);
        }
        else
        {
            Hermes.Say($"Skipped: Not supported file name the '{imgFileName} by '{imgPath}", MessageLevel.Warn);

            return;
        }

        SetBattleUnitBufSprite(imgId, imgPath, replace);
    }

    public static void SetBattleUnitBufSprite(string unitBufId, string imgPath, bool replace = false)
    {
        var sprite = Hermes.CreateSprite(imgPath);

        if (sprite is null)
        {
            Hermes.Say($"Skipped: Specified imgPath the '{imgPath}' is incorrect path.", MessageLevel.Warn);

            return;
        }

        SetBattleUnitBufSprite(unitBufId, sprite, replace);
    }

    public static void LoadBattleUnitBufSprites(string rootDirPath, bool replace = false)
    {
        if (!Directory.Exists(rootDirPath))
        {
            Hermes.Say($"Skipped: The rootDirPath '{rootDirPath}' is not exists.", MessageLevel.Warn);

            return;
        }

        var paths = Walkdir.GetFilesRecursive(rootDirPath);

        foreach (var path in paths)
        {
            SetBattleUnitBufSprite(path, replace);
        }
    }
}
