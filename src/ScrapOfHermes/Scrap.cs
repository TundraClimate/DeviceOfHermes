using UnityEngine;

namespace ScrapOfHermes;

/// <summary>A scrap loader</summary>
public class Scrap : MonoBehaviour
{
    /// <summary>Loads and instatiate the scrap</summary>
    public static T Load<T>(Transform parent)
    {
        return UnityEngine.Object.Instantiate(AssetLoader.bundle.LoadAsset<GameObject>(typeof(T).Name), parent).GetComponent<T>();
    }
}
