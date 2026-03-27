using HarmonyLib;
using LOR_DiceSystem;

namespace DeviceOfHermes.AdvancedBase;

public class AdvancedDiceBase : DiceCardAbilityBase
{
    static AdvancedDiceBase()
    {
        var harmony = new Harmony("DeviceOfHermes.AdvancedBases.Dice");

        harmony.CreateClassProcessor(typeof(DicePatch.PatchOnAddKeeps1)).Patch();
        harmony.CreateClassProcessor(typeof(DicePatch.PatchOnAddKeeps2)).Patch();
        harmony.CreateClassProcessor(typeof(DicePatch.PatchOnAddKeep)).Patch();
        harmony.CreateClassProcessor(typeof(DicePatch.PatchOnAddKeepForDef)).Patch();
    }

    public virtual void OnAddToKeeped()
    {
    }

    public virtual bool IsKeeps()
    {
        return true;
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
}
