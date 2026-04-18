using LOR_DiceSystem;
using HarmonyLib;
using HarmonyExtension;
using UnityEngine;
using DeviceOfHermes.AdvancedBase;

namespace DeviceOfHermes.CustomDice;

/// <summary>The custom dice of Limbus unbreakable</summary>
/// <remarks>
/// How to apply unbreakable dice:<br/>
/// - Exntends this class to your dice ability. <br/>
/// - That ability adds for <c>Atk</c> dice. <br/>
/// <para/>
/// Dice specify <br/>
/// - Dice when lose clash, use losed dices in a card.<br/>
/// - The unbreakable card not clashable.<br/>
/// </remarks>
/// <example><code>
/// public class DiceCardAbility_Unbreakable : Unbreakable
/// {
/// }
/// </code></example>
public class UnbreakableDice : AdvancedDiceBase
{
    static UnbreakableDice()
    {
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Type is BehaviourType.Atk &&
                beh.Detail is BehaviourDetail.Slash &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is UnbreakableDice,
            HermesConstants.UnbreakableSlash,
            new Color(180, 0, 0, 200)
        );
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Type is BehaviourType.Atk &&
                beh.Detail is BehaviourDetail.Penetrate &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is UnbreakableDice,
            HermesConstants.UnbreakablePenetrate,
            new Color(180, 0, 0, 200)
        );
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Type is BehaviourType.Atk &&
                beh.Detail is BehaviourDetail.Hit &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is UnbreakableDice,
            HermesConstants.UnbreakableHit,
            new Color(180, 0, 0, 200)
        );

        var harmony = new Harmony("DeviceOfHermes.CustomDice.Unbreakable");

        harmony.CreateClassProcessor(typeof(PatchUnbreakableDice.PatchOnUseCard)).Patch();
        harmony.CreateClassProcessor(typeof(PatchUnbreakableDice.PatchOnEndBattle)).Patch();
        harmony.CreateClassProcessor(typeof(PatchUnbreakableDice.PatchOnLoseParrying)).Patch();

        Stash = new();
    }

    /// <summary>A unit when use unbreakable dices</summary>
    /// <param name="card">A unbreakable dices</param>
    public virtual void OnUseBreaked(BattlePlayingCardDataInUnitModel card)
    {
    }

    /// <summary>A dice is breaked</summary>
    public bool IsBreaked => _isBreaked;

    internal bool _isBreaked = false;

    internal static Dictionary<BattleUnitModel, Queue<BattleDiceBehavior>> Stash
    {
        get;
        set => field = value;
    }
}

internal class PatchUnbreakableDice
{
    [HarmonyPatch(typeof(BattleUnitModel), "OnUseCard")]
    public class PatchOnUseCard
    {
        static void Prefix(BattlePlayingCardDataInUnitModel card)
        {
            foreach (var abi in card.GetDiceBehaviorList().Map(beh => beh.abilityList).Flatten())
            {
                if (abi is UnbreakableDice unb)
                {
                    if (unb._isBreaked)
                    {
                        unb.OnUseBreaked(card);
                    }
                }
            }

            if (card.owner is not null && UnbreakableDice.Stash.ContainsKey(card.owner))
            {
                UnbreakableDice.Stash.Remove(card.owner);
            }
        }
    }

    [HarmonyPatch(typeof(BattlePlayingCardDataInUnitModel), "OnEndBattle")]
    public class PatchOnEndBattle
    {
        static void Prefix(BattlePlayingCardDataInUnitModel __instance)
        {
            var owner = __instance.owner;

            if (owner is null)
            {
                return;
            }

            if (UnbreakableDice.Stash.TryGetValue(owner, out var queue))
            {
                if (queue.Count < 1)
                {
                    return;
                }

                var xmlInfo = queue.Peek().card.card.XmlData;
                var target = queue.Peek().card.target;
                var speed = queue.Peek().card.speedDiceResultValue;

                var playcard = owner.CreatePlayingCard(xmlInfo, target, speedDiceResultValue: speed)
                    .Also(it =>
                    {
                        it.cardBehaviorQueue = new();
                        it.cardAbility = null;
                    });

                while (queue.Count > 0)
                {
                    var beh = queue.Dequeue();

                    var behInCard = beh.behaviourInCard.Copy();

                    behInCard.Dice = behInCard.Min;

                    var newBeh = new BattleDiceBehavior()
                    {
                        card = playcard,
                        behaviourInCard = behInCard,
                        abilityList = beh.abilityList,
                    };

                    foreach (var abi in newBeh.abilityList)
                    {
                        abi.behavior = newBeh;
                    }

                    var stat = _diceStatBonus(beh);

                    ref var newStat = ref _diceStatBonus(newBeh);

                    newStat = stat;

                    playcard.cardBehaviorQueue.Enqueue(newBeh);
                }

                StageController.Instance.GetAllCards().Insert(0, playcard);
            }
        }

        private static AccessTools.FieldRef<BattleDiceBehavior, DiceStatBonus> _diceStatBonus =
            typeof(BattleDiceBehavior).FieldRefAccess<DiceStatBonus>("_statBonus");
    }

    [HarmonyPatch(typeof(BattlePlayingCardDataInUnitModel), "OnLoseParrying")]
    public class PatchOnLoseParrying
    {
        static void Postfix(BattlePlayingCardDataInUnitModel __instance)
        {
            var beh = __instance.currentBehavior;
            var owner = __instance.owner;

            if (beh is not null && owner is not null)
            {
                if (beh.abilityList.Exists(abi => abi is UnbreakableDice adv && !adv._isBreaked))
                {
                    if (!UnbreakableDice.Stash.ContainsKey(owner))
                    {
                        UnbreakableDice.Stash.Add(owner, new());
                    }

                    foreach (var abi in beh.abilityList)
                    {
                        if (abi is UnbreakableDice unb)
                        {
                            unb._isBreaked = true;
                        }
                    }

                    UnbreakableDice.Stash[owner].Enqueue(beh);
                }
            }
        }
    }
}
