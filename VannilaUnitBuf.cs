using HarmonyLib;

namespace DeviceOfHermes;

public class VannilaUnitBuf
{
    public static void SetMaxForcely<T>(int max)
        where T : BattleUnitBuf, new()
    {
        var target = typeof(T).Method("OnAddBuf");

        _forcelyMax.Add((KeywordBuf)typeof(T).Property("bufType").GetValue(new T()), max);

        _harmony.Patch(target, prefix: new HarmonyMethod(typeof(VannilaUnitBuf).Method("PrefixForcely")));
    }

    public static void SetMaxIf<T>(int max, Func<BattleUnitBuf, BattleUnitModel?, bool> cond)
        where T : BattleUnitBuf, new()
    {
        var target = typeof(T).Method("OnAddBuf");

        _ifMax.Add((KeywordBuf)typeof(T).Property("bufType").GetValue(new T()), new() { (cond, max) });

        if (_harmony.GetPatchedMethods().All(mes => mes != target))
        {
            _harmony.Patch(target, prefix: new HarmonyMethod(typeof(VannilaUnitBuf).Method("PrefixIf")));
        }
    }

    public static void AddMaxIf<T>(int max, Func<BattleUnitBuf, BattleUnitModel?, bool> cond)
        where T : BattleUnitBuf, new()
    {
        var target = typeof(T).Method("OnAddBuf");
        var kbf = (KeywordBuf)typeof(T).Property("bufType").GetValue(new T());

        if (_ifMax.ContainsKey(kbf))
        {
            _ifMax[kbf].Add((cond, max));
        }
        else
        {
            _ifMax.Add(kbf, new() { (cond, max) });
        }

        if (_harmony.GetPatchedMethods().All(mes => mes != target))
        {
            _harmony.Patch(target, prefix: new HarmonyMethod(typeof(VannilaUnitBuf).Method("PrefixIf")));
        }
    }

    static bool PrefixForcely(BattleUnitBuf __instance, int addedStack)
    {
        if (_forcelyMax.TryGetValue(__instance.bufType, out var max))
        {
            __instance.stack = max.Min(__instance.stack + addedStack);

            return false;
        }

        return true;
    }

    static bool PrefixIf(BattleUnitBuf __instance, int addedStack)
    {
        if (_ifMax.TryGetValue(__instance.bufType, out var conds))
        {
            foreach (var res in conds)
            {
                var cond = res.Item1;
                var max = res.Item2;

                var _owner = _ownerRef(__instance);

                if (cond(__instance, _owner))
                {
                    __instance.stack = max.Min(__instance.stack + addedStack);

                    return false;
                }
            }
        }

        return true;
    }

    private static Dictionary<KeywordBuf, int> _forcelyMax = new();

    private static Dictionary<KeywordBuf, List<(Func<BattleUnitBuf, BattleUnitModel?, bool>, int)>> _ifMax = new();

    private static AccessTools.FieldRef<BattleUnitBuf, BattleUnitModel?> _ownerRef =
        typeof(BattleUnitBuf).FieldRefAccess<BattleUnitModel?>("_owner");

    private static Harmony _harmony = new Harmony("DeviceOfHermes.VannilaUnitBuf");
}
