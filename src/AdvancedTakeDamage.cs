using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using LOR_DiceSystem;

namespace DeviceOfHermes;

/// <summary>The useful TakeDamage</summary>
public static class AdvancedTakeDamage
{
    static AdvancedTakeDamage()
    {
        var harmony = new Harmony("DeviceOfHermes.TakeDamage");

        harmony.CreateClassProcessor(typeof(AdvancedTakeDamagePatch.PatchTakeDamage)).Patch();
    }

    /// <summary>Deals Damage with specific detail and resist</summary>
    /// <param name="target">An attacking target</param>
    /// <param name="baseDmg">A base damage before resist</param>
    /// <param name="detail">The damage detail only atk type</param>
    /// <param name="attacker">An attacker unit</param>
    /// <param name="resist">Take resist override</param>
    /// <remarks>
    /// Use default resistance if resist override is null.<br/>
    /// baseDmg is affected by resistance but not affected reducation passive, buf and etc.
    /// </remarks>
    /// <example><code>
    /// target.DealDamage(999, BehaviourDetail.Slash);
    /// </code></example>
    public static void DealDamage(this BattleUnitModel target, int baseDmg, BehaviourDetail detail, BattleUnitModel? attacker = null, AtkResist? resist = null)
    {
        table.Remove(target);
        table.Add(target, new ATDContext() { detail = detail, resist = resist });

        var dmg = baseDmg * BookModel.GetResistRate(resist ?? target.GetResistHP(detail));

        target.TakeDamage(((int)dmg), attacker: attacker);
    }

    internal static ConditionalWeakTable<BattleUnitModel, ATDContext> table = new();
}

internal class ATDContext
{
    public BehaviourDetail detail;

    public AtkResist? resist;
}

internal class AdvancedTakeDamagePatch
{
    [HarmonyPatch(typeof(BattleUnitModel), "TakeDamage")]
    public class PatchTakeDamage
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var target = AccessTools.Method(typeof(BattleCardTotalResult), "SetDamageTaken");
            var inject = AccessTools.Method(typeof(PatchTakeDamage), "SetCustomizeTaken");

            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(CodeMatch.Calls(target))
                .Repeat(m =>
                {
                    m.SetInstruction(new CodeInstruction(OpCodes.Call, inject))
                        .Insert(new CodeInstruction(OpCodes.Ldarg_0))
                        .Advance(1);
                });

            return matcher.Instructions();
        }

        static void SetCustomizeTaken(BattleCardTotalResult instance, int dmg, int maxValue, BehaviourDetail detail, AtkResist atkResist, BattleUnitModel target)
        {
            if (AdvancedTakeDamage.table.TryGetValue(target, out var ctx))
            {
                instance.SetDamageTaken(dmg, maxValue, ctx.detail, ctx.resist ?? atkResist);

                return;
            }

            instance.SetDamageTaken(dmg, maxValue, detail, atkResist);
        }
    }
}
