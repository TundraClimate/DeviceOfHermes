using HarmonyLib;
using HarmonyExtension;

namespace DeviceOfHermes;

/// <summary>A extension with Non-patched methods</summary>
public static class NoPatchMethod
{
    internal static void Init()
    {
        var harmony = new Harmony("DeviceOfHermes.NoPatchMethod");

        harmony.CreateReversePatcher(typeof(BattleUnitModel).Method("Die"), new HarmonyMethod(typeof(NoPatchMethod).Method("DieNoPatch"))).Patch();
        harmony.CreateReversePatcher(typeof(BattleDiceBehavior).Method("GiveDamage"), new HarmonyMethod(typeof(NoPatchMethod).Method("GiveDamageNoPatch"))).Patch();
        harmony.CreateReversePatcher(typeof(StageController).Method("StartAction"), new HarmonyMethod(typeof(NoPatchMethod).Method("StartActionNoPatch"))).Patch();
        harmony.CreateReversePatcher(typeof(StageController).Method("StartParrying"), new HarmonyMethod(typeof(NoPatchMethod).Method("StartParryingNoPatch"))).Patch();
    }

    /// <summary>No patches BattleUnitModel.Die()</summary>
    [HarmonyReversePatch(HarmonyReversePatchType.Original)]
    public static void DieNoPatch(this BattleUnitModel __instance, BattleUnitModel? attacker = null, bool callEvent = true)
        => throw new NotImplementedException();

    /// <summary>No patches BattleDiceBehavior.GiveDamage()</summary>
    [HarmonyReversePatch(HarmonyReversePatchType.Original)]
    public static void GiveDamageNoPatch(this BattleDiceBehavior __instance, BattleUnitModel target)
        => throw new NotImplementedException();

    /// <summary>No patches StageController.StartAction()</summary>
    [HarmonyReversePatch(HarmonyReversePatchType.Original)]
    public static void StartActionNoPatch(this StageController __instance, BattlePlayingCardDataInUnitModel card)
        => throw new NotImplementedException();

    /// <summary>No patches StageController.StartParrying()</summary>
    [HarmonyReversePatch(HarmonyReversePatchType.Original)]
    public static void StartParryingNoPatch(this StageController __instance, BattlePlayingCardDataInUnitModel cardA, BattlePlayingCardDataInUnitModel cardB)
        => throw new NotImplementedException();
}
