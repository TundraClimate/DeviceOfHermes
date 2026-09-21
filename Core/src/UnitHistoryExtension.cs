using System.Runtime.CompilerServices;
using HarmonyLib;
using HarmonyExtension;

namespace DeviceOfHermes;

/// <summary>An extension of UnitBattleDataHistory</summary>
public static class UnitHistoryExtension
{
    internal static void Init()
    {
        var harmony = new Harmony("DeviceOfHermes.UnitHistoryExtension");

        harmony.CreateClassProcessor(typeof(PatchOnRoundStart)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnChargeUse)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnTakeDamage)).Patch();
    }

    extension(UnitBattleDataHistory history)
    {
        /// <summary>Returns number of taken damage by burn</summary>
        public int GetDamageByBurn() => DamageByBurn.GetValue(history, _ => new(0)).value;

        /// <summary>Returns number of taken damage by bleed</summary>
        public int GetDamageByBleed() => DamageByBleed.GetValue(history, _ => new(0)).value;

        /// <summary>Returns number of charge consumed with UseStack</summary>
        public int GetConsumedChargeStack() => ConsumedChargeStack.GetValue(history, _ => new(0)).value;

        /// <summary>Returns number of charge consumed with UseStack at this round</summary>
        public int GetConsumedChargeStackAtOneRound() => ConsumedChargeStackAtOneRound.GetValue(history, _ => new(0)).value;
    }

    static ConditionalWeakTable<UnitBattleDataHistory, Box<int>> DamageByBurn = new();

    static ConditionalWeakTable<UnitBattleDataHistory, Box<int>> DamageByBleed = new();

    static ConditionalWeakTable<UnitBattleDataHistory, Box<int>> ConsumedChargeStack = new();

    static ConditionalWeakTable<UnitBattleDataHistory, Box<int>> ConsumedChargeStackAtOneRound = new();

    [HarmonyPatch(typeof(UnitBattleDataHistory), "OnRoundStart")]
    class PatchOnRoundStart
    {
        static Exception Finalizer(Exception __exception, UnitBattleDataHistory __instance)
        {
            ConsumedChargeStackAtOneRound.GetValue(__instance, _ => new(0)).value = 0;

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleUnitBuf_warpCharge), "UseStack")]
    class PatchOnChargeUse
    {
        static Exception Finalizer(Exception __exception, BattleUnitBuf_warpCharge __instance, bool __result, int v)
        {
            if (__result)
            {
                ConsumedChargeStack.GetValue(__instance.Owner.history, _ => new(0)).value += v;
                ConsumedChargeStackAtOneRound.GetValue(__instance.Owner.history, _ => new(0)).value += v;
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "TakeDamage")]
    class PatchOnTakeDamage
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchEndForward(CodeMatch.IsLdarg(0), CodeMatch.IsLdloc(), CodeMatch.Calls(typeof(BattleUnitModel).Method("OnLoseHp")))
                .Advance(1)
                .Insert(CodeInstruction.Instance, CodeInstruction.Local(1), CodeInstruction.Arg(2), CodeInstruction.Arg(4), CodeInstruction.Call(typeof(PatchOnTakeDamage).Method("InjectMethod")));

            return matcher.Instructions();
        }

        static void InjectMethod(BattleUnitModel __instance, int dmg, DamageType dty, KeywordBuf buf)
        {
            if (dty is DamageType.Buf)
            {
                ConditionalWeakTable<UnitBattleDataHistory, Box<int>>? target = buf switch
                {
                    KeywordBuf.Burn => DamageByBurn,
                    KeywordBuf.Bleeding => DamageByBleed,

                    _ => null,
                };

                target?.GetValue(__instance.history, _ => new(0))?.value += dmg;
            }
        }
    }
}
