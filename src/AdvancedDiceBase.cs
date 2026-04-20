using System.Reflection.Emit;
using HarmonyLib;
using HarmonyExtension;
using LOR_DiceSystem;

namespace DeviceOfHermes.AdvancedBase;

/// <summary>An advanced <see cref="DiceCardAbilityBase"/></summary>
/// <remarks>
/// Can replaces <c>DiceCardAbilityBase</c> into this.
/// </remarks>
/// <example><code>
/// public class DiceCardAbility_Advance : AdvancedDiceBase
/// {
/// }
/// </code></example>
public class AdvancedDiceBase : DiceCardAbilityBase
{
    static AdvancedDiceBase()
    {
        var harmony = new Harmony("DeviceOfHermes.AdvancedBases.Dice");

        harmony.CreateClassProcessor(typeof(DicePatch.PatchOnAddKeeps1)).Patch();
        harmony.CreateClassProcessor(typeof(DicePatch.PatchOnAddKeeps2)).Patch();
        harmony.CreateClassProcessor(typeof(DicePatch.PatchOnAddKeep)).Patch();
        harmony.CreateClassProcessor(typeof(DicePatch.PatchOnAddKeepForDef)).Patch();
        harmony.CreateClassProcessor(typeof(DicePatch.PatchParryingResult)).Patch();
        harmony.CreateClassProcessor(typeof(DicePatch.PatchDiceResultValue)).Patch();
        harmony.CreateClassProcessor(typeof(DicePatch.PatchDiceDamageValue)).Patch();
    }

    /// <summary>Dice on move to keeped</summary>
    public virtual void OnAddToKeeped()
    {
    }

    /// <summary>Dice can move to keeps</summary>
    /// <returns>Is dice can move to keeps if true</returns>
    public virtual bool IsKeeps()
    {
        return true;
    }

    /// <summary>Returns dice parrying result</summary>
    /// <returns>A result of parrying with this dice</returns>
    public virtual ParryingResult GetParryingResult(ParryingResult origin)
    {
        return origin;
    }

    /// <summary>Returns dice final result value</summary>
    /// <returns>A result value of override</returns>
    public virtual int GetDiceFinalResultValue(int origin)
    {
        return origin;
    }

    /// <summary>Returns dice final damage value</summary>
    /// <returns>A damage value of override</returns>
    public virtual int GetFinalResultDamageValue(int origin)
    {
        return origin;
    }

    /// <summary>Returns dice final break damage value</summary>
    /// <returns>A break damage value of override</returns>
    public virtual int GetFinalResultBreakDamageValue(int origin)
    {
        return origin;
    }

    /// <summary>A result of parrying</summary>
    /// <remarks>That uses only the <see cref="GetParryingResult(ParryingResult)"/></remarks>
    public enum ParryingResult
    {
        /// <summary>Win</summary>
        Win,

        /// <summary>Draw</summary>
        Draw,

        /// <summary>Lose</summary>
        Lose,
    }
}

static class DicePatch
{
    static List<BattleDiceBehavior> OnAddKeeped(List<BattleDiceBehavior> behaviourList)
    {
        List<BattleDiceBehavior> broke = new();

        foreach (var beh in behaviourList)
        {
            foreach (var abi in beh.abilityList)
            {
                if (abi is AdvancedDiceBase)
                {
                    var advAbi = (AdvancedDiceBase)abi;

                    advAbi.OnAddToKeeped();

                    if (!advAbi.IsKeeps())
                    {
                        broke.Add(beh);
                    }
                }
            }
        }

        return broke;
    }

    [HarmonyPatch
    (
        typeof(BattleKeepedCardDataInUnitModel),
        "AddBehaviours",
        new[] { typeof(DiceCardXmlInfo), typeof(List<BattleDiceBehavior>) }
    )]
    internal class PatchOnAddKeeps1
    {
        static void Prefix(List<BattleDiceBehavior> behaviourList)
        {
            var broke = DicePatch.OnAddKeeped(behaviourList);

            behaviourList.RemoveAll(b => broke.Contains(b));
        }
    }

    [HarmonyPatch
    (
        typeof(BattleKeepedCardDataInUnitModel),
        "AddBehaviours",
        new[] { typeof(BattleDiceCardModel), typeof(List<BattleDiceBehavior>) }
    )]
    internal class PatchOnAddKeeps2
    {
        static void Prefix(List<BattleDiceBehavior> behaviourList)
        {
            var broke = DicePatch.OnAddKeeped(behaviourList);

            behaviourList.RemoveAll(b => broke.Contains(b));
        }
    }

    [HarmonyPatch(typeof(BattleKeepedCardDataInUnitModel), "AddBehaviour")]
    internal class PatchOnAddKeep
    {
        static bool Prefix(BattleDiceBehavior behaviour)
        {
            return DicePatch.OnAddKeeped(new() { behaviour }).Count != 1;
        }
    }

    [HarmonyPatch(typeof(BattleKeepedCardDataInUnitModel), "AddBehaviourForOnlyDefense")]
    internal class PatchOnAddKeepForDef
    {
        static bool Prefix(BattleDiceBehavior behaviour)
        {
            return DicePatch.OnAddKeeped(new() { behaviour }).Count != 1;
        }
    }

    [HarmonyPatch(typeof(BattleParryingManager), "GetDecisionResult")]
    internal class PatchParryingResult
    {
        static Exception Finalizer(
            Exception __exception,
            ref BattleParryingManager.ParryingDecisionResult __result,
            BattleParryingManager.ParryingTeam teamA,
            BattleParryingManager.ParryingTeam teamB
        )
        {
            var enemyAdvAbility = teamA?.playingCard?.currentBehavior?.abilityList?
                .Find(abi => abi is AdvancedDiceBase)?
                .Let(adv => (AdvancedDiceBase)adv);
            var librarianAdvAbility = teamB?.playingCard?.currentBehavior?.abilityList?
                .Find(abi => abi is AdvancedDiceBase)?
                .Let(adv => (AdvancedDiceBase)adv);

            var enemyOrigin = ParseTo(__result, Faction.Enemy);
            var librarianOrigin = ParseTo(__result, Faction.Player);
            var enemyResult = enemyAdvAbility?.GetParryingResult(enemyOrigin) ?? enemyOrigin;
            var librarianResult = librarianAdvAbility?.GetParryingResult(librarianOrigin) ?? librarianOrigin;

            if (enemyOrigin != enemyResult)
            {
                __result = ParseFrom(enemyResult, Faction.Enemy);
            }

            if (librarianOrigin != librarianResult)
            {
                __result = ParseFrom(librarianResult, Faction.Player);
            }

            return __exception;
        }

        static BattleParryingManager.ParryingDecisionResult ParseFrom(AdvancedDiceBase.ParryingResult adv, Faction f)
        {
            if (adv is AdvancedDiceBase.ParryingResult.Win)
            {
                if (f is Faction.Player)
                {
                    return BattleParryingManager.ParryingDecisionResult.WinLibrarian;
                }
                else
                {
                    return BattleParryingManager.ParryingDecisionResult.WinEnemy;
                }
            }
            else if (adv is AdvancedDiceBase.ParryingResult.Lose)
            {
                if (f is Faction.Player)
                {
                    return BattleParryingManager.ParryingDecisionResult.WinEnemy;
                }
                else
                {
                    return BattleParryingManager.ParryingDecisionResult.WinLibrarian;
                }
            }
            else
            {
                return BattleParryingManager.ParryingDecisionResult.Draw;
            }
        }

        static AdvancedDiceBase.ParryingResult ParseTo(BattleParryingManager.ParryingDecisionResult origin, Faction f)
        {
            if (origin is BattleParryingManager.ParryingDecisionResult.WinEnemy)
            {
                if (f is Faction.Player)
                {
                    return AdvancedDiceBase.ParryingResult.Lose;
                }
                else
                {
                    return AdvancedDiceBase.ParryingResult.Win;
                }
            }
            else if (origin is BattleParryingManager.ParryingDecisionResult.WinLibrarian)
            {
                if (f is Faction.Player)
                {
                    return AdvancedDiceBase.ParryingResult.Win;
                }
                else
                {
                    return AdvancedDiceBase.ParryingResult.Lose;
                }
            }
            else
            {
                return AdvancedDiceBase.ParryingResult.Draw;
            }
        }
    }

    [HarmonyPatch(typeof(BattleDiceBehavior), "UpdateDiceFinalValue")]
    internal class PatchDiceResultValue
    {
        static Exception Finalizer(Exception __exception, BattleDiceBehavior __instance, ref int ____diceFinalResultValue)
        {
            foreach (var abi in __instance.abilityList)
            {
                if (abi is AdvancedDiceBase adv)
                {
                    ____diceFinalResultValue = adv.GetDiceFinalResultValue(____diceFinalResultValue);
                }
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleDiceBehavior), "GiveDamage")]
    internal class PatchDiceDamageValue
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(
                CodeMatch.IsLdloc(),
                new CodeMatch(OpCodes.Ldc_I4_0),
                new CodeMatch(OpCodes.Ldarg_0),
                CodeMatch.Calls(typeof(BattleDiceBehavior).Method("get_owner")),
                new CodeMatch(OpCodes.Ldc_I4_0),
                CodeMatch.Calls(typeof(BattleUnitModel).Method("TakeDamage"))
            )
                .Advance(1)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, typeof(PatchDiceDamageValue).Method("ChangeDamage"))
                );

            matcher.MatchStartForward(
                CodeMatch.IsLdloc(),
                new CodeMatch(OpCodes.Ldc_I4_0),
                new CodeMatch(OpCodes.Ldarg_0),
                CodeMatch.Calls(typeof(BattleDiceBehavior).Method("get_owner")),
                CodeMatch.IsLdloc(),
                new CodeMatch(OpCodes.Ldc_I4_0),
                CodeMatch.Calls(typeof(BattleUnitModel).Method("TakeBreakDamage"))
            )
                .Advance(1)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, typeof(PatchDiceDamageValue).Method("ChangeBreakDamage"))
                );

            return matcher.Instructions();
        }

        static int ChangeDamage(int origin, BattleDiceBehavior instance)
        {
            var res = origin;

            foreach (var abi in instance.abilityList)
            {
                if (abi is AdvancedDiceBase adv)
                {
                    res = adv.GetFinalResultDamageValue(res);
                }
            }

            return res;
        }

        static int ChangeBreakDamage(int origin, BattleDiceBehavior instance)
        {
            var res = origin;

            foreach (var abi in instance.abilityList)
            {
                if (abi is AdvancedDiceBase adv)
                {
                    res = adv.GetFinalResultBreakDamageValue(res);
                }
            }

            return res;
        }
    }
}
