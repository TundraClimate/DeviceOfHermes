using HarmonyLib;
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
}
