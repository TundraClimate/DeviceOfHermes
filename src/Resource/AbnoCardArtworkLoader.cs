using System.Reflection;
using UI;
using UnityEngine.UI;
using HarmonyLib;
using HarmonyExtension;
using DeviceOfHermes.Boot;

namespace DeviceOfHermes.Resource;

internal static class AbnoCardArtworkLoader
{
    public static void Load()
    {
        var harmony = new Harmony("DeviceOfHermes.Resource.AbnoCardArtworkLoader");

        harmony.CreateClassProcessor(typeof(PatchCardUI)).Patch();

        foreach (var mod in HermesPreloader.ActiveMods)
        {
            var files = Path.Combine(mod.dirInfo.FullName, "Assemblies", "HermesResource", "AbnoCardArtwork");

            if (!Directory.Exists(files))
            {
                continue;
            }

            Artwork.LoadAbnoCardArtworks(files);
        }
    }

    [HarmonyPatch]
    class PatchCardUI
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return typeof(EmotionPassiveCardUI).Method("SetSprites");
            yield return typeof(UIEmotionPassiveCardInven).Method("SetSprites");
        }

        static Exception Finalizer(Exception __exception, Image ____artwork, EmotionCardXmlInfo ____card)
        {
            if (Artwork.TryGetAbnoCardArtwork(____card.Artwork, out var stored))
            {
                ____artwork.sprite = stored;
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(UIAbnormalityCardPreviewSlot), "Init")]
    class PatchPreview
    {
        static Exception Finalizer(Exception __exception, Image ___artwork, EmotionCardXmlInfo card)
        {
            if (Artwork.TryGetAbnoCardArtwork(card.Artwork, out var stored))
            {
                ___artwork.sprite = stored;
            }

            return __exception;
        }
    }
}
