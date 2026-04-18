using HarmonyLib;
using HarmonyExtension;

namespace DeviceOfHermes;

/// <summary>Modify the vannila unitbufs</summary>
/// <remarks>
/// This class is helper of HarmonyPatch for Vannila unitbufs.<br/>
/// When called functions then instantly patches for correspond method.
/// </remarks>
/// <example><code>
/// // Change the warpCharge max stack to 20
/// VannilaUnitBuf.SetMaxForcely&lt;BattleUnitBuf_warpCharge&gt;(20);
///
/// // Change the smoke max stack to 20 if owner has a PassiveAbility_cigar
/// VannilaUnitBuf.AddMaxIf&lt;BattleUnitBuf_smoke&gt;(20, (buf, owner) => owner?.passiveDetail.HasPassive&lt;PassiveAbility_cigar&gt;() == true);
/// </code></example>
public class VannilaUnitBuf
{
    /// <summary>Set max stack forcely</summary>
    /// <param name="max">A number of max stack</param>
    /// <typeparam name="T">A target of vannila unitbuf</typeparam>
    /// <example><code>
    /// VannilaUnitBuf.SetMaxForcely&lt;BattleUnitBuf_warpCharge&gt;(20);
    /// </code></example>
    public static void SetMaxForcely<T>(int max)
        where T : BattleUnitBuf, new()
    {
        var target = typeof(T).Method("OnAddBuf");

        _forcelyMax.Add((KeywordBuf)typeof(T).Property("bufType").GetValue(new T()), max);

        _harmony.Patch(target, prefix: new HarmonyMethod(typeof(VannilaUnitBuf).Method("PrefixForcely")));
    }

    /// <summary>Set max stack if cond</summary>
    /// <param name="max">A number of max stack</param>
    /// <param name="cond">Applies max if returns true</param>
    /// <typeparam name="T">A target of vannila unitbuf</typeparam>
    /// <remarks>
    /// Each calls then reset cond.
    /// </remarks>
    /// <example><code>
    /// VannilaUnitBuf.SetMaxIf&lt;BattleUnitBuf_smoke&gt;(20, (buf, owner) => owner?.passiveDetail.HasPassive&lt;PassiveAbility_cigar&gt;() == true);
    /// </code></example>
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

    /// <summary>Set max stack if cond</summary>
    /// <param name="max">A number of max stack</param>
    /// <param name="cond">Applies max if returns true</param>
    /// <typeparam name="T">A target of vannila unitbuf</typeparam>
    /// <example><code>
    /// VannilaUnitBuf.AddMaxIf&lt;BattleUnitBuf_smoke&gt;(20, (buf, owner) => owner?.passiveDetail.HasPassive&lt;PassiveAbility_cigar&gt;() == true);
    /// </code></example>
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
