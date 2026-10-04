using System.Reflection;
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
        harmony.CreateClassProcessor(typeof(PatchOnRecoverPP)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnSpendCost)).Patch();
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

        /// <summary>Returns number of playpoint recoverd</summary>
        public int GetRecoveredPlaypoint() => RecoveredPlaypoint.GetValue(history, _ => new(0)).value;

        /// <summary>Returns number of playpoint recoverd at this round</summary>
        public int GetRecoveredPlaypointAtOneRound() => RecoveredPlaypointAtOneRound.GetValue(history, _ => new(0)).value;

        /// <summary>Returns number of playpoint recoverd at previous round</summary>
        public int GetRecoveredPlaypointAtPrevRound() => RecoveredPlaypointAtPrevRound.GetValue(history, _ => new(0)).value;

        /// <summary>Returns number of playpoint spended</summary>
        public int GetSpendedCost() => SpendedCost.GetValue(history, _ => new(0)).value;

        /// <summary>Returns number of playpoint spended at this round</summary>
        public int GetSpendedCostAtOneRound() => SpendedCostAtOneRound.GetValue(history, _ => new(0)).value;

        /// <summary>Returns number of playpoint spended at previous round</summary>
        public int GetSpendedCostAtPrevRound() => SpendedCostAtPrevRound.GetValue(history, _ => new(0)).value;
    }

    static ConditionalWeakTable<UnitBattleDataHistory, Box<int>> DamageByBurn = new();

    static ConditionalWeakTable<UnitBattleDataHistory, Box<int>> DamageByBleed = new();

    static ConditionalWeakTable<UnitBattleDataHistory, Box<int>> ConsumedChargeStack = new();

    static ConditionalWeakTable<UnitBattleDataHistory, Box<int>> ConsumedChargeStackAtOneRound = new();

    static ConditionalWeakTable<UnitBattleDataHistory, Box<int>> RecoveredPlaypoint = new();

    static ConditionalWeakTable<UnitBattleDataHistory, Box<int>> RecoveredPlaypointAtOneRound = new();

    static ConditionalWeakTable<UnitBattleDataHistory, Box<int>> RecoveredPlaypointAtPrevRound = new();

    static ConditionalWeakTable<UnitBattleDataHistory, Box<int>> SpendedCost = new();

    static ConditionalWeakTable<UnitBattleDataHistory, Box<int>> SpendedCostAtOneRound = new();

    static ConditionalWeakTable<UnitBattleDataHistory, Box<int>> SpendedCostAtPrevRound = new();

    [HarmonyPatch(typeof(UnitBattleDataHistory), "OnRoundStart")]
    class PatchOnRoundStart
    {
        static Exception Finalizer(Exception __exception, UnitBattleDataHistory __instance)
        {
            ConsumedChargeStackAtOneRound.GetValue(__instance, _ => new(0)).value = 0;

            RecoveredPlaypointAtOneRound.GetValue(__instance, _ => new(0)).Let(num =>
            {
                RecoveredPlaypointAtPrevRound.GetValue(__instance, _ => new(0)).value = num.value;

                num.value = 0;
            });

            SpendedCostAtOneRound.GetValue(__instance, _ => new(0)).Let(num =>
            {
                SpendedCostAtPrevRound.GetValue(__instance, _ => new(0)).value = num.value;

                num.value = 0;
            });

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

    [HarmonyPatch]
    class PatchOnRecoverPP
    {
        static IEnumerable<MethodInfo> TargetMethods()
        {
            yield return typeof(BattlePlayingCardSlotDetail).Method("RecoverPlayPoint");
            yield return typeof(BattlePlayingCardSlotDetail).Method("RecoverPlayPointByCard");
        }

        static Exception Finalizer(Exception __exception, BattlePlayingCardSlotDetail __instance, int value)
        {
            RecoveredPlaypoint.GetValue(_selfRef(__instance).history, _ => new(0)).value += value;
            RecoveredPlaypointAtOneRound.GetValue(_selfRef(__instance).history, _ => new(0)).value += value;

            return __exception;
        }

        static AccessTools.FieldRef<BattlePlayingCardSlotDetail, BattleUnitModel> _selfRef
            = typeof(BattlePlayingCardSlotDetail).FieldRefAccess<BattleUnitModel>("_self");
    }

    [HarmonyPatch(typeof(BattlePlayingCardSlotDetail), "SpendCost")]
    class PatchOnSpendCost
    {
        static Exception Finalizer(Exception __exception, BattlePlayingCardSlotDetail __instance, int value)
        {
            SpendedCost.GetValue(_selfRef(__instance).history, _ => new(0)).value += value;
            SpendedCostAtOneRound.GetValue(_selfRef(__instance).history, _ => new(0)).value += value;

            return __exception;
        }

        static AccessTools.FieldRef<BattlePlayingCardSlotDetail, BattleUnitModel> _selfRef
            = typeof(BattlePlayingCardSlotDetail).FieldRefAccess<BattleUnitModel>("_self");
    }
}
