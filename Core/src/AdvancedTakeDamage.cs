using System.Collections.Concurrent;
using HarmonyLib;
using LOR_DiceSystem;

namespace DeviceOfHermes;

/// <summary>The useful TakeDamage</summary>
public static class AdvancedTakeDamage
{
    static AdvancedTakeDamage()
    {
        var harmony = new Harmony("DeviceOfHermes.TakeDamage");

        harmony.CreateClassProcessor(typeof(PatchOnDamageTaken)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnDamaged)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnDamageEffectWithoutResist)).Patch();
    }

    /// <summary>Deals Damage with specific detail and resist</summary>
    public static void DealDamage(
        this BattleUnitModel owner,
        int baseDmg,
        BehaviourDetail detail,
        DamageType type = DamageType.Attack,
        BattleUnitModel? attacker = null,
        KeywordBuf keyword = KeywordBuf.None,
        AtkResist resist = AtkResist.None
    )
    {
        resist = resist is AtkResist.None ? owner.GetResistHP(detail) : resist;

        if (owner.battleCardResultLog is not null)
        {
            LogStateData.GetOrAdd(owner.battleCardResultLog, _ => new() { detail = detail, resist = resist });
        }

        OtherData.GetOrAdd(owner, _ => new() { detail = detail, resist = resist });

        var dmg = baseDmg * BookModel.GetResistRate(resist);

        owner.TakeDamage(((int)dmg), type: type, attacker: attacker, keyword: keyword);

        if (owner.battleCardResultLog is not null)
        {
            LogStateData.Remove(owner.battleCardResultLog, out _);
        }

        OtherData.Remove(owner, out _);
    }

    static ConcurrentDictionary<BattleCardTotalResult, Context> LogStateData = new();

    static ConcurrentDictionary<BattleUnitModel, Context> OtherData = new();

    class Context
    {
        public BehaviourDetail detail;

        public AtkResist resist;
    }

    [HarmonyPatch(typeof(BattleCardTotalResult), "SetDamageTaken")]
    class PatchOnDamageTaken
    {
        static void Prefix(BattleCardTotalResult __instance, ref BehaviourDetail detail, ref AtkResist atkResist)
        {
            if (LogStateData.TryGetValue(__instance, out var ctx))
            {
                detail = ctx.detail;
                atkResist = ctx.resist;
            }
        }
    }

    [HarmonyPatch(typeof(BattleUnitView), "Damaged")]
    class PatchOnDamaged
    {
        static void Prefix(BattleUnitView __instance, ref BehaviourDetail detail, ref AtkResist atkResist)
        {
            if (OtherData.TryGetValue(__instance.model, out var ctx))
            {
                detail = ctx.detail;
                atkResist = ctx.resist;
            }
        }
    }

    [HarmonyPatch(typeof(AttackEffectManager), "CreateDamagedTextEffectWithoutResist")]
    class PatchOnDamageEffectWithoutResist
    {
        static bool Prefix(int damage, int colorIdx, BattleUnitModel unit)
        {
            if (OtherData.TryGetValue(unit, out var ctx))
            {
                AttackEffectManager.Instance.CreateDamagedTextEffect(damage, ctx.detail, unit, null, ctx.resist, false, colorIdx);

                return false;
            }

            return true;
        }
    }
}
