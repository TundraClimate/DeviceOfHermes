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
        harmony.CreateReversePatcher(typeof(BattleUnitBuf_burn).Method("OnRoundEnd"), new HarmonyMethod(typeof(NoPatchMethod).Method("BurnOnRoundEndNoPatch"))).Patch();
        harmony.CreateReversePatcher(typeof(BattleUnitBuf_bleeding).Method("AfterDiceAction"), new HarmonyMethod(typeof(NoPatchMethod).Method("BleedingAfterDiceActionNoPatch"))).Patch();
        harmony.CreateReversePatcher(typeof(BattleUnitBuf_Decay).Method("OnRoundEnd"), new HarmonyMethod(typeof(NoPatchMethod).Method("DecayOnRoundEndNoPatch"))).Patch();
        harmony.CreateReversePatcher(typeof(BattleUnitBuf_fairy).Method("AfterDiceAction"), new HarmonyMethod(typeof(NoPatchMethod).Method("FairyAfterDiceActionNoPatch"))).Patch();
        harmony.CreateReversePatcher(typeof(BattleUnitBuf_Alriune_Debuf).Method("OnRoundEndTheLast"), new HarmonyMethod(typeof(NoPatchMethod).Method("AlriuneDebufOnRoundEndTheLastNoPatch"))).Patch();
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

    /// <summary>No patches BattleUnitBuf_burn.OnRoundEnd()</summary>
    [HarmonyReversePatch(HarmonyReversePatchType.Original)]
    public static void BurnOnRoundEndNoPatch(this BattleUnitBuf_burn __instance)
        => throw new NotImplementedException();

    /// <summary>No patches BattleUnitBuf_bleeding.AfterDiceAction()</summary>
    [HarmonyReversePatch(HarmonyReversePatchType.Original)]
    public static void BleedingAfterDiceActionNoPatch(this BattleUnitBuf_bleeding __instance, BattleDiceBehavior behavior)
        => throw new NotImplementedException();

    /// <summary>No patches BattleUnitBuf_Decay.OnRoundEnd()</summary>
    [HarmonyReversePatch(HarmonyReversePatchType.Original)]
    public static void DecayOnRoundEndNoPatch(this BattleUnitBuf_Decay __instance)
        => throw new NotImplementedException();

    /// <summary>No patches BattleUnitBuf_fairy.AfterDiceAction()</summary>
    [HarmonyReversePatch(HarmonyReversePatchType.Original)]
    public static void FairyAfterDiceActionNoPatch(this BattleUnitBuf_fairy __instance)
        => throw new NotImplementedException();

    /// <summary>No patches BattleUnitBuf_Alriune_Debuf.OnRoundEndTheLast()</summary>
    [HarmonyReversePatch(HarmonyReversePatchType.Original)]
    public static void AlriuneDebufOnRoundEndTheLastNoPatch(this BattleUnitBuf_Alriune_Debuf __instance)
        => throw new NotImplementedException();
}
