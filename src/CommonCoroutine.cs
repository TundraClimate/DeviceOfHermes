using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DeviceOfHermes.UI;

/// <summary>A list of Coroutines</summary>
public class CommonCoroutine
{
    /// <summary>Fadeout Image</summary>
    public static IEnumerator ImageFadeout(Image image, float wait, float duration)
    {
        var elapsed = 0f;
        var color = image.color;

        yield return new WaitForSeconds(wait);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            color.a = 1f - t;

            image.color = color;

            yield return null;
        }

        color.a = 0;
        image.color = color;
    }

    /// <summary>Fadeout Image and destroy</summary>
    public static IEnumerator ImageFadeoutAndDestroy(Image image, float wait, float duration)
    {
        yield return ImageFadeout(image, wait, duration);

        var go = image.gameObject;

        UnityEngine.Object.Destroy(go);
    }
}
