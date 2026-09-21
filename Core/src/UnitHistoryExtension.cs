using System.Runtime.CompilerServices;
using HarmonyLib;

namespace DeviceOfHermes;

/// <summary>An extension of UnitBattleDataHistory</summary>
public static class UnitHistoryExtension
{
    internal static void Init()
    {
        var harmony = new Harmony("DeviceOfHermes.UnitHistoryExtension");

        harmony.CreateClassProcessor(typeof(PatchOnRoundStart)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnChargeUse)).Patch();
    }

    extension(UnitBattleDataHistory history)
    {
        /// <summary>Returns number of charge consumed with UseStack</summary>
        public int GetConsumedChargeStack() => ConsumedChargeStack.GetValue(history, _ => new(0)).value;

        /// <summary>Returns number of charge consumed with UseStack at this round</summary>
        public int GetConsumedChargeStackAtOneRound() => ConsumedChargeStackAtOneRound.GetValue(history, _ => new(0)).value;
    }

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
}
