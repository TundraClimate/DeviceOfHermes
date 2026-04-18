using UI;
using UnityEngine;
using HarmonyLib;
using HarmonyExtension;
using static HarmonyLib.AccessTools;

namespace DeviceOfHermes.Resource;

/// <summary>An additional artwork loader</summary>
/// <example><code>
/// var unitBufPath = Path.Combine(typeof(MyModInitializer).GetAsmDirectory(), "Artwork", "BattleUnitBuf");
///
/// Artwork.LoadBattleUnitBufSprites(path, true);
/// </code></example>
public static class Artwork
{
    static Artwork()
    {
        var harmony = new Harmony("DeviceOfHermes.Resource.Artwork");

        harmony.CreateClassProcessor(typeof(PatchArtwork.PatchOnInitStoryIconDic)).Patch();
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
    /// Lets see <see cref="Artwork.CreateSprite(byte[], float)"/>
    /// </remarks>
    /// <example><code>
    /// var sprite = Hermes.CreateSprite("image.png");
    /// </code></example>
    public static Sprite? CreateSprite(string path, float pixPerUnit = 50f)
    {
        var fileBytes = File.ReadAllBytes(path);

        return CreateSprite(fileBytes, pixPerUnit);
    }

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
        var sprite = CreateSprite(imgPath);

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
    /// Artwork.LoadBattleUnitBufSprites(path, true);
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

    /// <summary>Set new StoryIcon</summary>
    /// <param name="icon">A new icon</param>
    /// <param name="replace">Is replace if contains same type</param>
    /// <remarks>
    /// If calls on Initializer, set is lazy polling since manager not initialized.
    /// </remarks>
    /// <example><code>
    /// var sprite = Artwork.CreateSprite("sprite.png");
    ///
    /// Artwork.SetStoryIconSprite(new UIIconManager.IconSet()
    /// {
    ///     type = "MolarOffice",
    ///     icon = sprite,
    ///     iconGlow = sprite,
    /// }, true);
    /// </code></example>
    public static void SetStoryIconSprite(UIIconManager.IconSet icon, bool replace = false)
    {

        if (GameSceneManager.Instance is null)
        {
            _initializeStoryIconStash?.Add((icon, replace));

            return;
        }

        ref var dict = ref _storyIconRef(UISpriteDataManager.instance);

        if (dict.ContainsKey(icon.type))
        {
            if (replace)
            {
                dict[icon.type] = icon;
            }
            else
            {
                Hermes.Say($"Skipped: The StoryIcon '{icon.type}' is already exists.", MessageLevel.Warn);
            }

            return;
        }

        dict.Add(icon.type, icon);
    }

    /// <summary>Set new StoryIcon</summary>
    /// <param name="type">A keyword of icon type</param>
    /// <param name="icon">A new icon</param>
    /// <param name="iconGlow">A new icon glow</param>
    /// <param name="replace">Is replace if contains same type</param>
    /// <remarks>
    /// If calls on Initializer, set is lazy polling since manager not initialized.
    /// </remarks>
    /// <example><code>
    /// var sprite = Artwork.CreateSprite("sprite.png");
    ///
    /// Artwork.SetStoryIconSprite("MolarOffice", sprite, replace: true);
    /// </code></example>
    public static void SetStoryIconSprite(string type, Sprite icon, Sprite? iconGlow = null, bool replace = false)
    {
        iconGlow ??= icon;

        var iconSet = new UIIconManager.IconSet()
        {
            type = type,
            icon = icon,
            iconGlow = iconGlow,
        };

        SetStoryIconSprite(iconSet, replace);
    }

    /// <summary>Set new StoryIcon</summary>
    /// <param name="type">A keyword of icon type</param>
    /// <param name="iconPath">A new icon path</param>
    /// <param name="iconGlowPath">A new icon glow path</param>
    /// <param name="replace">Is replace if contains same type</param>
    /// <remarks>
    /// If calls on Initializer, set is lazy polling since manager not initialized.<br/>
    /// Overides type if set.
    /// </remarks>
    /// <example><code>
    /// Artwork.SetStoryIconSprite("MolarOffice.png", replace: true);
    /// </code></example>
    public static void SetStoryIconSprite(string iconPath, string? iconGlowPath = null, string? type = null, bool replace = false)
    {
        type ??= Path.GetFileName(iconPath)
            .Let(name => (name.EndsWith(".png") || name.EndsWith(".jpg")) ? name.Substring(0, name.Length - 4) : name)
            .Let(name => (name!.EndsWith(".jpeg") ? name!.Substring(0, name.Length - 5) : name));
        iconGlowPath ??= iconPath;

        var icon = Artwork.CreateSprite(iconPath);
        var iconGlow = Artwork.CreateSprite(iconGlowPath);

        var iconSet = new UIIconManager.IconSet()
        {
            type = type,
            icon = icon,
            iconGlow = iconGlow,
        };

        SetStoryIconSprite(iconSet, replace);
    }

    /// <summary>Loads all images with under <c>rootDirPath</c></summary>
    /// <param name="rootDirPath">A path that includes storyicon sprite image</param>
    /// <param name="glowSuffix">A suffix of glow sprite</param>
    /// <param name="replace">Is replace if contains same ID</param>
    /// <remarks>
    /// Method retrieves under path files on recursive.<br/>
    /// Only read the png or jpeg extension file.
    /// </remarks>
    /// <example><code>
    /// var storyIconPath = Path.Combine(typeof(MyModInitializer).GetAsmDirectory(), "Artwork", "StoryIcon");
    ///
    /// Artwork.LoadStoryIconSprites(path, "_Glow", true);
    /// </code></example>
    public static void LoadStoryIconSprites(string rootDirPath, string glowSuffix = "_Glow", bool replace = false)
    {
        if (!Directory.Exists(rootDirPath))
        {
            Hermes.Say($"Skipped: The rootDirPath '{rootDirPath}' is not exists.", MessageLevel.Warn);

            return;
        }

        var paths = Walkdir.GetFilesRecursive(rootDirPath);

        foreach (var path in paths)
        {
            if (!path.EndsWith(".png") && !path.EndsWith(".jpeg") && !path.EndsWith(".jpg"))
            {
                continue;
            }

            var type = Path.GetFileNameWithoutExtension(path);

            if (type.EndsWith(glowSuffix))
            {
                continue;
            }

            var ext = Path.GetExtension(path);
            var glowPath = string.Format("{0}{1}{2}", path.Substring(0, path.Length - ext.Length), glowSuffix, ext);

            var icon = Artwork.CreateSprite(path);
            var glow = Artwork.CreateSprite(glowPath);

            var iconSet = new UIIconManager.IconSet()
            {
                type = type,
                icon = icon,
                iconGlow = glow,
            };

            SetStoryIconSprite(iconSet, replace);
        }
    }

    private static readonly FieldRef<UISpriteDataManager, Dictionary<string, UIIconManager.IconSet>> _storyIconRef =
        typeof(UISpriteDataManager).FieldRefAccess<Dictionary<string, UIIconManager.IconSet>>("StoryIconDic");

    private static List<(UIIconManager.IconSet, bool)>? _initializeStoryIconStash = new();

    private class PatchArtwork
    {
        [HarmonyPatch(typeof(UISpriteDataManager), "Init")]
        public class PatchOnInitStoryIconDic
        {
            static void Postfix(Dictionary<string, UIIconManager.IconSet> ___StoryIconDic)
            {
                foreach (var (sic, replace) in _initializeStoryIconStash ?? new())
                {
                    SetStoryIconSprite(sic, replace);
                }

                _initializeStoryIconStash = null;
            }
        }
    }
}
