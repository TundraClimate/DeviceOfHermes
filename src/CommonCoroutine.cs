using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DeviceOfHermes.UI;

/// <summary>A list of Coroutines</summary>
public class CommonCoroutine
{
    /// <summary>Fadein CanvasGroup</summary>
    public static IEnumerator CanvasGroupFadein(CanvasGroup cg, float duration)
    {
        var elapsed = 0f;

        cg.alpha = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            cg.alpha = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            yield return null;
        }

        cg.alpha = 1;
    }

    /// <summary>Fadeout CanvasGroup</summary>
    public static IEnumerator CanvasGroupFadeout(CanvasGroup cg, float wait, float duration)
    {
        var elapsed = 0f;

        yield return new WaitForSeconds(wait);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            cg.alpha = 1f - Mathf.SmoothStep(0f, 1f, elapsed / duration);

            yield return null;
        }

        cg.alpha = 0;
    }

    /// <summary>Fadein Image</summary>
    public static IEnumerator ImageFadein(Image image, float duration)
    {
        var elapsed = 0f;
        var color = image.color;

        color.a = 0;

        image.color = color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            color.a = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            image.color = color;

            yield return null;
        }

        color.a = 1;
        image.color = color;
    }

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
}
