namespace System;

public static class Extension
{
    public static int Min(this int n1, int n2)
    {
        return Math.Min(n1, n2);
    }

    public static int Max(this int n1, int n2)
    {
        return Math.Max(n1, n2);
    }

    public static string? StripPrefix(this string original, string strip)
    {
        if (original.StartsWith(strip))
        {
            return original.Substring(strip.Length);
        }

        return null;
    }

    public static string? StripSuffix(this string original, string strip)
    {
        if (original.EndsWith(strip))
        {
            return original.Substring(0, (original.Length - strip.Length).Max(0));
        }

        return null;
    }

    public static IEnumerable<V> Map<T, V>(this IEnumerable<T> enumerable, Func<T, V> pred)
    {
        return enumerable.Select(pred);
    }

    public static IEnumerable<T> Filter<T>(this IEnumerable<T> enumerable, Func<T, bool> pred)
    {
        return enumerable.Where(pred);
    }

    public static IEnumerable<V> FilterMap<T, V>(this IEnumerable<T> enumerable, Func<T, V> pred)
    {
        return enumerable.Select(val => pred(val)).Where(val => val is not null);
    }

    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable)
    {
        return enumerable.SelectMany(val => val);
    }

    public static IEnumerable<(int, T)> Enumerate<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.Select((val, idx) => (idx, val));
    }

    public static List<T> Collect<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.ToList();
    }

    public static R Fold<T, R>(this IEnumerable<T> enumerable, R root, Func<R, T, R> acc)
    {
        var res = root;

        foreach (var elem in enumerable)
        {
            res = acc(res, elem);
        }

        return res;
    }

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

    public static void Foreach<T>(this IEnumerable<T> enumerable, Action<T> each)
    {
        foreach (T elem in enumerable)
        {
            each(elem);
        }
    }

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

    public static T? Inspect<T>(this T? nullable, Action<T> inspect)
    {
        if (nullable is not null)
        {
            inspect(nullable);
        }

        return nullable;
    }

    public static Faction FaceTo(this Faction faction)
    {
        return faction switch
        {
            Faction.Enemy => Faction.Player,
            _ => Faction.Enemy,
        };
    }

    public static List<BattleDiceCardModel> GetHands(this BattleUnitModel? owner, Func<BattleDiceCardModel, bool>? filter = null)
    {
        List<BattleDiceCardModel> list = new();
        var f = filter ??= _ => true;

        owner?.allyCardDetail?.GetHand()?.Filter(f)?.Inspect(hands => list.AddRange(hands));

        return list;
    }

    public static void Say(this BattleUnitModel owner, string txt)
    {
        BattleManagerUI.Instance.ui_unitListInfoSummary.DisplayDlg(txt, owner, false, MentalState.Positive);
    }

    public static string GetAsmDirectory(this Type ty)
    {
        return Path.GetDirectoryName(ty.Assembly.Location);
    }
}
