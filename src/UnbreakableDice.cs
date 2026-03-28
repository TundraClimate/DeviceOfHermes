using LOR_DiceSystem;
using HarmonyLib;
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
        harmony.CreateClassProcessor(typeof(PatchUnbreakableDice.PatchStartParrying)).Patch();
        harmony.CreateClassProcessor(typeof(PatchUnbreakableDice.PatchStartAction)).Patch();

        _stash = new();
    }

    /// <summary>A unit when use unbreakable dices</summary>
    /// <param name="card">A unbreakable dices</param>
    public virtual void OnUseBreaked(BattlePlayingCardDataInUnitModel card)
    {
    }

    internal bool IsBreaked = false;

    private static Dictionary<BattleUnitModel, Queue<BattleDiceBehavior>> _stash;

    internal static Dictionary<BattleUnitModel, Queue<BattleDiceBehavior>> Stash => _stash;
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
                    if (unb.IsBreaked)
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

            if (UnbreakableDice.Stash.ContainsKey(owner) && UnbreakableDice.Stash[owner].Count > 0)
            {
                var queue = UnbreakableDice.Stash[owner];

                var playCard = new BattlePlayingCardDataInUnitModel()
                {
                    cardBehaviorQueue = new(),
                    target = __instance.target,
                    targetSlotOrder = -1,
                    speedDiceResultValue = 99,
                };

                while (queue.Count != 0)
                {
                    var dice = queue.Dequeue();

                    if (playCard.card is null)
                    {
                        playCard.owner = owner;
                        playCard.card = dice.card.card;
                        playCard.speedDiceResultValue = dice.card.speedDiceResultValue;
                    }

                    dice.card = playCard;

                    var behInCard = dice.behaviourInCard.Copy();

                    behInCard.Dice = behInCard.Min;

                    dice.behaviourInCard = behInCard;

                    playCard.cardBehaviorQueue.Enqueue(dice);
                }

                StageController.Instance.GetAllCards().Add(playCard);
            }
        }
    }

    [HarmonyPatch(typeof(BattlePlayingCardDataInUnitModel), "OnLoseParrying")]
    public class PatchOnLoseParrying
    {
        static void Prefix(BattlePlayingCardDataInUnitModel __instance)
        {
            var beh = __instance.currentBehavior;
            var owner = __instance.owner;

            if (beh is not null && owner is not null)
            {
                if (beh.abilityList.Exists(abi => abi is UnbreakableDice adv && !adv.IsBreaked))
                {
                    if (!UnbreakableDice.Stash.ContainsKey(owner))
                    {
                        UnbreakableDice.Stash.Add(owner, new());
                    }

                    beh.abilityList.Filter(abi => abi is UnbreakableDice).Foreach(abi => ((UnbreakableDice)abi).IsBreaked = true);

                    UnbreakableDice.Stash[owner].Enqueue(beh);
                }
            }
        }
    }

    [HarmonyPatch(typeof(StageController), "StartParrying")]
    public class PatchStartParrying
    {
        static bool Prefix(BattlePlayingCardDataInUnitModel cardA)
        {
            if (cardA.cardBehaviorQueue.All(beh => beh.abilityList.Exists(abi => abi is UnbreakableDice adv && adv.IsBreaked)))
            {
                PatchStartAction.StartAction(StageController.Instance, cardA);

                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(StageController), "StartAction")]
    public class PatchStartAction
    {
        [HarmonyReversePatch]
        public static void StartAction(StageController instance, BattlePlayingCardDataInUnitModel card) =>
            throw new NotImplementedException();
    }
}
