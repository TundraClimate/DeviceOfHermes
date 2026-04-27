using System.Text;
using LOR_DiceSystem;
using HarmonyExtension;

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

    /// <summary>Returns opponent</summary>
    public static Faction FaceTo(this Faction faction)
    {
        return faction switch
        {
            Faction.Enemy => Faction.Player,
            _ => Faction.Enemy,
        };
    }

    /// <summary>Returns unit hands with filter</summary>
    public static List<BattleDiceCardModel> GetHands(this BattleUnitModel? owner, Func<BattleDiceCardModel, bool>? filter = null)
    {
        List<BattleDiceCardModel> list = new();
        var f = filter ??= _ => true;

        owner?.allyCardDetail?.GetHand()?.Filter(f)?.Let(hands => list.AddRange(hands));

        return list;
    }

    /// <summary>Returns directory of ty found assembly</summary>
    public static string GetAsmDirectory(this Type ty)
    {
        return Path.GetDirectoryName(ty.Assembly.Location);
    }

    /// <summary>Creates new <see cref="BattleDiceBehavior"/></summary>
    public static BattleDiceBehavior CreateBattleDiceBehavior(
        this BattlePlayingCardDataInUnitModel playcard,
        DiceBehaviour behaviour,
        int idx = -1
    )
    {
        var beh = new BattleDiceBehavior()
        {
            card = playcard,
            behaviourInCard = behaviour,
            abilityList = string.IsNullOrEmpty(behaviour.Script) ?
                new() : [AssemblyManager.Instance.CreateInstance_DiceCardAbility(behaviour.Script)],
        };

        foreach (var abi in beh.abilityList)
        {
            abi.behavior = beh;
        }

        beh.SetIndex(0 > idx ? playcard?.cardBehaviorQueue?.Last()?.Index ?? 0 : idx);

        return beh;
    }

    /// <summary>Creates new <see cref="BattlePlayingCardDataInUnitModel"/></summary>
    public static BattlePlayingCardDataInUnitModel CreatePlayingCard(
        this BattleUnitModel owner,
        DiceCardXmlInfo cardInfo,
        BattleUnitModel? target = null,
        int targetSlotOrder = -1,
        int speedDiceResultValue = 0,
        List<BattleUnitModel>? subTargets = null
    )
    {
        var card = BattleDiceCardModel.CreatePlayingCard(cardInfo);

        card.owner = owner;

        var playcard = new BattlePlayingCardDataInUnitModel()
        {
            owner = owner,
            card = card,
            target = target,
            targetSlotOrder = targetSlotOrder,
            earlyTarget = target,
            earlyTargetOrder = targetSlotOrder,
            cardAbility = card.CreateDiceCardSelfAbilityScript(),
            cardBehaviorQueue = new(),
            speedDiceResultValue = speedDiceResultValue,
            subTargets = subTargets?
                .Filter(unit => unit != owner)
                .Map(unit => new BattlePlayingCardDataInUnitModel.SubTarget()
                {
                    target = unit,
                    targetSlotOrder = RandomUtil.Range(0, unit.speedDiceResult.Count)
                })
                .Collect()
        };

        playcard.cardAbility?.card = playcard;

        foreach (var (i, beh) in cardInfo.DiceBehaviourList.Enumerate())
        {
            playcard.cardBehaviorQueue.Enqueue(playcard.CreateBattleDiceBehavior(beh, i));
        }

        return playcard;
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
}
