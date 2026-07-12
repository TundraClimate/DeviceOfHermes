using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DeviceOfHermes;

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

    /// <summary>Move unit</summary>
    public static IEnumerator UnitMoving(BattleUnitView view, Vector3 src, Vector3 dst, float duration, float speedMul = 1f)
    {
        var elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime * speedMul;

            var t = elapsed / duration;

            view.WorldPosition = Vector3.Lerp(src, dst, t);

            yield return null;
        }

        view.WorldPosition = dst;
    }

    /// <summary>Move unit with easing</summary>
    public static IEnumerator UnitEaseMoving(BattleUnitView view, Vector3 src, Vector3 dst, float duration, float speedMul = 1f)
    {
        var elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime * speedMul;

            var t = 1f - Mathf.Pow(1f - elapsed / duration, 3f);

            view.WorldPosition = Vector3.Lerp(src, dst, t);

            yield return null;
        }

        view.WorldPosition = dst;
    }

    /// <summary>Play earthquake</summary>
    public static IEnumerator EarthQuake(float time = 1f, float speedBase = 60f, float shake = 0.2f)
    {
        var cam = BattleCamManager.Instance?.EffectCam;

        if (cam is null)
        {
            yield break;
        }

        var effect = cam.gameObject.GetComponent<CameraFilterPack_FX_EarthQuake>();

        if (effect is null)
        {
            effect = cam.gameObject.AddComponent<CameraFilterPack_FX_EarthQuake>();
        }

        var elapsed = 0f;

        while (time > elapsed)
        {
            elapsed += Time.deltaTime;

            effect.Speed = speedBase * (1f - elapsed);
            effect.X = shake * (1f - elapsed);
            effect.Y = shake * (1f - elapsed);

            yield return null;
        }

        UnityObject.Destroy(effect);

        yield break;
    }
}
