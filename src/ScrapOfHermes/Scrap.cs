using UnityEngine;

namespace ScrapOfHermes;

/// <summary>A scrap loader</summary>
public class Scrap : MonoBehaviour
{
    /// <summary>Loads and instatiate the scrap</summary>
    public static void Load<T>(Transform parent)
    {
        UnityEngine.Object.Instantiate(AssetLoader.bundle.LoadAsset<GameObject>(typeof(T).Name), parent);
    }
}
