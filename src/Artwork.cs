using UnityEngine;

namespace DeviceOfHermes.Resource;

/// <summary>An additional artwork loader</summary>
/// <remarks>
/// The additional artworks that loaded by <see cref="Artwork"/> will clear when main initializer.<br/>
/// therefore needs the lazy load and on-initialize hook.
/// </remarks>
/// <example><code>
/// var unitBufPath = Path.Combine(typeof(MyModInitializer).GetAsmDirectory(), "Artwork", "BattleUnitBuf");
///
/// TextModel.OnLoadLocalize += lang => 
/// {
///     Artwork.LoadBattleUnitBufSprites(path, true);
/// }
/// </code></example>
public static class Artwork
{
    /// <summary>Set <see cref="BattleUnitBuf"/> with id</summary>
    /// <param name="unitBufId">Specific ID</param>
    /// <param name="sprite">A sprite that shown</param>
    /// <param name="replace">Is replace if contains same ID</param>
    /// <remarks>
    /// The unitBufId is correspond the <c>BattleUnitBuf.keywordIconId</c> by default<br/>
    /// </remarks>
    /// <example><code>
    /// var unitBuf = Hermes.CreateSprite("MyUnitBuf.png");
    ///
    /// Artwork.SetBattleUnitBufSprite("MyUnitBuf", unitBuf);
    /// </code></example>
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

    /// <summary>Set <see cref="BattleUnitBuf"/> with path</summary>
    /// <param name="imgPath">A unit texure path</param>
    /// <param name="replace">Is replace if contains same ID</param>
    /// <remarks>
    /// An Id uses non-extension filename
    /// </remarks>
    /// <example><code>
    /// Artwork.SetBattleUnitBufSprite("MyUnitBuf.png");
    /// </code></example>
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

    /// <summary>Set <see cref="BattleUnitBuf"/> with path</summary>
    /// <param name="unitBufId">Specific ID</param>
    /// <param name="imgPath">A unit texure path</param>
    /// <param name="replace">Is replace if contains same ID</param>
    /// <remarks>
    /// Overrides Id with <c>unitBufId</c>.
    /// </remarks>
    /// <example><code>
    /// Artwork.SetBattleUnitBufSprite("UnitBuf", "MyUnitBuf.png");
    /// </code></example>
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

    /// <summary>Loads all images with under <c>rootDirPath</c></summary>
    /// <param name="rootDirPath">A path that includes unitbuf sprite image</param>
    /// <param name="replace">Is replace if contains same ID</param>
    /// <remarks>
    /// Method retrieves under path files on recursive.<br/>
    /// Only read the png or jpeg extension file.
    /// </remarks>
    /// <example><code>
    /// var unitBufPath = Path.Combine(typeof(MyModInitializer).GetAsmDirectory(), "Artwork", "BattleUnitBuf");
    ///
    /// TextModel.OnLoadLocalize += lang => 
    /// {
    ///     Artwork.LoadBattleUnitBufSprites(path, true);
    /// }
    /// </code></example>
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
