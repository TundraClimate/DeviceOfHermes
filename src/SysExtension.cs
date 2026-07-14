using System.Text;
using HarmonyExtension;
using UnityEngine;
using UnityEngine.UI;

namespace System;

/// <summary>Respects the functions in Rust</summary>
public static class Extension
{
    /// <summary>Returns min</summary>
    public static int Min(this int n1, int n2)
    {
        return Math.Min(n1, n2);
    }

    /// <summary>Returns max</summary>
    public static int Max(this int n1, int n2)
    {
        return Math.Max(n1, n2);
    }

    /// <summary>Strips prefix and returns null when not matches</summary>
    public static string? StripPrefix(this string original, string strip)
    {
        if (original.StartsWith(strip))
        {
            return original.Substring(strip.Length);
        }

        return null;
    }

    /// <summary>Strips suffix and returns null when not matches</summary>
    public static string? StripSuffix(this string original, string strip)
    {
        if (original.EndsWith(strip))
        {
            return original.Substring(0, (original.Length - strip.Length).Max(0));
        }

        return null;
    }

    /// <summary>Renamed by Select</summary>
    public static IEnumerable<V> Map<T, V>(this IEnumerable<T> enumerable, Func<T, V> pred)
    {
        return enumerable.Select(pred);
    }

    /// <summary>Renamed by Where</summary>
    public static IEnumerable<T> Filter<T>(this IEnumerable<T> enumerable, Func<T, bool> pred)
    {
        return enumerable.Where(pred);
    }

    /// <summary>Renamed by Select Where</summary>
    public static IEnumerable<V> FilterMap<T, V>(this IEnumerable<T> enumerable, Func<T, V> pred)
    {
        return enumerable.Select(val => pred(val)).Where(val => val is not null);
    }

    /// <summary>SelectMany and less 1 depth</summary>
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable)
    {
        return enumerable.SelectMany(val => val);
    }

    /// <summary>Returns enumerable with index</summary>
    public static IEnumerable<(int, T)> Enumerate<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.Select((val, idx) => (idx, val));
    }

    /// <summary>Renamed by ToList</summary>
    public static List<T> Collect<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.ToList();
    }

    /// <summary>Compute new Value with enumerable</summary>
    public static R Fold<T, R>(this IEnumerable<T> enumerable, R root, Func<R, T, R> acc)
    {
        var res = root;

        foreach (var elem in enumerable)
        {
            res = acc(res, elem);
        }

        return res;
    }

    /// <summary>Compute new Value with enumerable</summary>
    public static T? Reduce<T>(this IEnumerable<T> enumerable, Func<T, T, T> acc)
    {
        T? res = default(T);

        foreach (var elem in enumerable)
        {
            if (res is null)
            {
                res = elem;

                continue;
            }

            res = acc(res, elem);
        }

        return res;
    }

    /// <summary>Iterate the enumerable</summary>
    public static void Foreach<T>(this IEnumerable<T> enumerable, Action<T> each)
    {
        foreach (T elem in enumerable)
        {
            each(elem);
        }
    }

    /// <summary>Iterate the enumerable with breakable</summary>
    public static bool TryForeach<T, R>(this IEnumerable<T> enumerable, Func<T, R?> each)
    {
        foreach (T elem in enumerable)
        {
            var res = each(elem);

            if (res is null)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Iterate the enumerable, returns self</summary>
    public static IEnumerable<T> Inspect<T>(this IEnumerable<T> enumerable, Action<T> inspect)
    {
        foreach (var elem in enumerable)
        {
            if (elem is not null)
            {
                inspect(elem);
            }
        }

        return enumerable;
    }

    /// <summary>Runs fn with it</summary>
    public static R? Let<T, R>(this T it, Func<T, R> fn)
    {
        if (it is null)
        {
            return default(R);
        }

        return fn(it);
    }

    /// <summary>Runs fn with it</summary>
    public static void Let<T>(this T it, Action<T> fn)
    {
        if (it is not null)
        {
            fn(it);
        }
    }

    /// <summary>Runs fn with it and returns self</summary>
    public static T Also<T>(this T it, Action<T> fn)
    {
        if (it is not null)
        {
            fn(it);
        }

        return it;
    }

    /// <summary>Returns some if true else returns null</summary>
    public static T? Then<T>(this bool it, Func<T> f)
    {
        if (it)
        {
            return f();
        }
        else
        {
            return default(T);
        }
    }

    /// <summary>Returns opponent</summary>
    public static Faction FaceTo(this Faction faction)
    {
        return faction switch
        {
            Faction.Enemy => Faction.Player,
            _ => Faction.Enemy,
        };
    }

    /// <summary>Returns directory of ty found assembly</summary>
    public static string GetAsmDirectory(this Type ty)
    {
        return Path.GetDirectoryName(ty.Assembly.Location);
    }

    extension(Faction faction)
    {
        /// <summary>Get alive units on faction</summary>
        public List<BattleUnitModel> GetAlives()
        {
            return BattleObjectManager.instance.GetAliveList(faction);
        }

        /// <summary>Get alive units on faction</summary>
        public List<BattleUnitModel> AliveUnits => faction.GetAlives();
    }

    extension(BattleUnitBuf buf)
    {
        /// <summary>Get keywordId</summary>
        public string KeywordId => (string)typeof(BattleUnitBuf).Property("keywordId").GetValue(buf);

        /// <summary>Get keywordIconId</summary>
        public string KeywordIconId => (string)typeof(BattleUnitBuf).Property("keywordIconId").GetValue(buf);

        /// <summary>Creates pretty string</summary>
        public string ToPrettyString()
        {
            var builder = new StringBuilder();

            builder.AppendLine($"{buf.GetType().Name}");
            builder.AppendLine($"KeywordId: {buf.KeywordId}");
            builder.AppendLine($"BufType: {buf.bufType}");
            builder.AppendLine($"PositiveType: {buf.positiveType}");
            builder.AppendLine($"Displayed Name: {buf.bufActivatedNameWithStack}");
            builder.AppendLine($"Displayed Desc: {buf.bufActivatedText}");
            builder.AppendLine($"Hide: {buf.Hide}");
            builder.AppendLine($"Destroyed: {buf.IsDestroyed()}");

            return builder.ToString();
        }
    }

    extension(GameObject ob)
    {
        /// <summary>Adds new child object to self </summary>
        public GameObject AddChildObject(string? name = null, string? layerName = null)
        {
            var go = string.IsNullOrEmpty(name) ? new GameObject() : new GameObject(name);

            if (layerName is not null)
            {
                go.layer = LayerMask.NameToLayer(layerName);
            }

            go.transform.SetParent(ob.transform, false);

            return go;
        }

        /// <summary>Move to anchor</summary>
        public GameObject MoveTo(Vector2 anchor)
        {
            var rect = ob.GetComponent<RectTransform>() switch
            {
                RectTransform rt => rt,
                null => ob.AddComponent<RectTransform>(),
            };

            rect.Let(rt =>
            {
                rt.anchorMin = rt.anchorMax = anchor;
                rt.offsetMin = rt.offsetMax = rt.anchoredPosition = Vector2.zero;
            });

            return ob;
        }

        /// <summary>Add container to self</summary>
        public GameObject AddContainer(Action<GameObject> f)
        {
            var container = ob.AddChildObject(layerName: LayerMask.LayerToName(ob.layer));

            f(container);

            return ob;
        }

        /// <summary>Set image with Sprite</summary>
        public Image SetImage(Sprite sprite, Vector2? sizeDelta = null)
        {
            var image = ob.GetComponent<Image>() switch
            {
                Image img => img,
                null => ob.AddComponent<Image>(),
            };

            return image.Also(img =>
            {
                img.sprite = sprite;

                if (sizeDelta is null)
                {
                    img.SetNativeSize();
                }
                else
                {
                    img.rectTransform.sizeDelta = sizeDelta.Value;
                }
            });
        }
    }

    extension(CanvasGroup cg)
    {
        /// <summary>Enable CanvasGroup</summary>
        public void Enable()
        {
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        /// <summary>Disable CanvasGroup</summary>
        public void Disable()
        {
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }

        /// <summary>Show CanvasGroup</summary>
        public void Show()
        {
            cg.alpha = 1f;
        }

        /// <summary>Hide CanvasGroup</summary>
        public void Hide()
        {
            cg.alpha = 0f;
        }
    }
}
