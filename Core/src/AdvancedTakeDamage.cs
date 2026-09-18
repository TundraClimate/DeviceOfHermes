using System.Reflection.Emit;
using HarmonyLib;
using HarmonyExtension;
using LOR_DiceSystem;
using UnityEngine;

namespace DeviceOfHermes;

/// <summary>The useful TakeDamage</summary>
public static class AdvancedTakeDamage
{
    static AdvancedTakeDamage()
    {
        var harmony = new Harmony("DeviceOfHermes.TakeDamage");

        harmony.CreateClassProcessor(typeof(PatchOnDamageTaken)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnBreakDamageTaken)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnDamaged)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnBreakDamaged)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnDamageEffectWithoutResist)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnDamageEffect)).Patch();
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

        var ctx = new Context() { detail = detail, resist = resist };
        var dmg = baseDmg * BookModel.GetResistRate(resist);

        InitData(owner, ctx);

        owner.TakeDamage(((int)dmg), type: type, attacker: attacker, keyword: keyword);

        RestoreData(owner);
    }

    /// <summary>Deals Damage with styled image</summary>
    public static void StyledDamage(this BattleUnitModel owner, int dmg, Sprite image, DamageType type = DamageType.Buf, KeywordBuf keyword = KeywordBuf.None)
    {
        var ctx = new Context() { detail = BehaviourDetail.None, resist = (AtkResist)image.GetInstanceID(), img = image };

        InitData(owner, ctx);

        owner.TakeDamage(dmg, type: type, keyword: keyword);

        RestoreData(owner);
    }

    /// <summary>Deals BreakDamage with styled image</summary>
    public static void StyledBreakDamage(this BattleUnitModel owner, int dmg, Sprite image, DamageType type = DamageType.Buf, KeywordBuf keyword = KeywordBuf.None)
    {
        var ctx = new Context() { detail = BehaviourDetail.None, resist = (AtkResist)image.GetInstanceID(), img = image };

        InitBreakData(owner, ctx);

        owner.TakeBreakDamage(dmg, type: type, keyword: keyword);

        RestoreBreakData(owner);
    }

    static void InitData(BattleUnitModel owner, Context ctx)
    {
        if (owner.battleCardResultLog is not null)
        {
            LogStateData[owner.battleCardResultLog] = ctx;
        }

        OtherData[owner] = ctx;
    }

    static void RestoreData(BattleUnitModel owner)
    {
        if (owner.battleCardResultLog is not null)
        {
            LogStateData.Remove(owner.battleCardResultLog, out _);
        }

        OtherData.Remove(owner, out _);
    }

    static void InitBreakData(BattleUnitModel owner, Context ctx)
    {
        if (owner.battleCardResultLog is not null)
        {
            BreakLogStateData[owner.battleCardResultLog] = ctx;
        }

        BreakOtherData[owner] = ctx;
    }

    static void RestoreBreakData(BattleUnitModel owner)
    {
        if (owner.battleCardResultLog is not null)
        {
            BreakLogStateData.Remove(owner.battleCardResultLog, out _);
        }

        BreakOtherData.Remove(owner, out _);
    }

    static Dictionary<BattleCardTotalResult, Context> LogStateData = new();

    static Dictionary<BattleUnitModel, Context> OtherData = new();

    static Dictionary<BattleCardTotalResult, Context> BreakLogStateData = new();

    static Dictionary<BattleUnitModel, Context> BreakOtherData = new();

    static Dictionary<AtkResist, Sprite> SpriteData = new();

    class Context
    {
        public BehaviourDetail detail;

        public AtkResist resist;

        public Sprite? img;
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

                if (ctx.img is not null)
                {
                    SpriteData[ctx.resist] = ctx.img;
                }
            }
        }
    }

    [HarmonyPatch(typeof(BattleCardTotalResult), "SetBreakDmgTaken")]
    class PatchOnBreakDamageTaken
    {
        static void Prefix(BattleCardTotalResult __instance, ref BehaviourDetail detail, ref AtkResist atkResist)
        {
            if (BreakLogStateData.TryGetValue(__instance, out var ctx))
            {
                detail = ctx.detail;
                atkResist = ctx.resist;

                if (ctx.img is not null)
                {
                    SpriteData[ctx.resist] = ctx.img;
                }
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

                if (ctx.img is not null)
                {
                    SpriteData[ctx.resist] = ctx.img;
                }
            }
        }
    }

    [HarmonyPatch(typeof(BattleUnitView), "BreakDamaged")]
    class PatchOnBreakDamaged
    {
        static void Prefix(BattleUnitView __instance, ref BehaviourDetail detail, ref AtkResist atkResist)
        {
            if (BreakOtherData.TryGetValue(__instance.model, out var ctx))
            {
                detail = ctx.detail;
                atkResist = ctx.resist;

                if (ctx.img is not null)
                {
                    SpriteData[ctx.resist] = ctx.img;
                }
            }
        }
    }

    [HarmonyPatch(typeof(AttackEffectManager), "CreateDamagedTextEffectWithoutResist")]
    class PatchOnDamageEffectWithoutResist
    {
        static bool Prefix(int damage, int colorIdx, BattleUnitModel unit)
        {
            Context ctx;

            if (OtherData.TryGetValue(unit, out ctx))
            {
                if (ctx.img is not null)
                {
                    SpriteData[ctx.resist] = ctx.img;
                }

                AttackEffectManager.Instance.CreateDamagedTextEffect(damage, ctx.detail, unit, null, ctx.resist, false, colorIdx);

                return false;
            }

            if (BreakOtherData.TryGetValue(unit, out ctx))
            {
                if (ctx.img is not null)
                {
                    SpriteData[ctx.resist] = ctx.img;
                }

                AttackEffectManager.Instance.CreateDamagedTextEffect(damage, ctx.detail, unit, null, ctx.resist, false, colorIdx);

                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(AttackEffectManager), "CreateDamagedTextEffect")]
    class PatchOnDamageEffect
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.End()
                .MatchEndBackwards(CodeMatch.IsOpCode(OpCodes.Ret))
                .Insert(
                    CodeInstruction.Local(0),
                    CodeInstruction.Arg(2),
                    CodeInstruction.Arg(5),
                    CodeInstruction.Call(typeof(PatchOnDamageEffect).Method("InjectMethod"))
                );

            return matcher.Instructions();
        }

        static void InjectMethod(DamageTextEffect effect, BehaviourDetail detail, AtkResist atkResist)
        {
            if (detail is BehaviourDetail.None && SpriteData.TryGetValue(atkResist, out var img))
            {
                effect.img_resistIconBg.enabled = false;
                effect.img_resistIconFg.enabled = false;
                effect.txt_resist.enabled = false;
                effect.img_resistIcon.enabled = true;

                effect.img_resistIcon.color = new(1, 1, 1, 1);
                effect.img_resistIcon.sprite = img;
            }
        }
    }
}
