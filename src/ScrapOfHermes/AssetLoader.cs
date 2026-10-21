using UnityEngine;

namespace ScrapOfHermes;

internal static class AssetLoader
{
    internal static AssetBundle bundle = AssetBundle.LoadFromStream(typeof(AssetLoader).Assembly.GetManifestResourceStream("ScrapOfHermes.public.scrapofhermes.assetbundle"));
}
