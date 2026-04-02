using System.Reflection.Emit;
using LOR_DiceSystem;
using HarmonyLib;
using UnityEngine;
using DeviceOfHermes.AdvancedBase;

namespace DeviceOfHermes.CustomDice;

/// <summary>The custom dice of Limbus standby</summary>
/// <remarks>
/// How to apply revenge dice:<br/>
/// - Exntends this class to your dice ability. <br/>
/// - That ability adds for <c>Standby</c> dice. <br/>
/// <para/>
/// Dice specify <br/>
/// - When take damage by attack, use revenge dices in a card.<br/>
/// - The revenge card not clashable.<br/>
/// </remarks>
/// <example><code>
/// public class DiceCardAbility_Revenge : RevengeDice
/// {
/// }
/// </code></example>
public class RevengeDice : AdvancedDiceBase
{
    static RevengeDice()
    {
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Type is BehaviourType.Standby &&
                beh.Detail is BehaviourDetail.Slash &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is RevengeDice,
            HermesConstants.RevengeDiceSlash,
            new Color(255, 0, 200, 200)
        );
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Type is BehaviourType.Standby &&
                beh.Detail is BehaviourDetail.Penetrate &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is RevengeDice,
            HermesConstants.RevengeDicePenetrate,
            new Color(255, 0, 200, 200)
        );
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Type is BehaviourType.Standby &&
                beh.Detail is BehaviourDetail.Hit &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is RevengeDice,
            HermesConstants.RevengeDiceHit,
            new Color(255, 0, 200, 200)
        );

        var harmony = new Harmony("DeviceOfHermes.CustomDice.Revenge");

        harmony.CreateClassProcessor(typeof(PatchRevengeDice.PatchStandbyResolve)).Patch();
        harmony.CreateClassProcessor(typeof(PatchRevengeDice.PatchOnEndBattle)).Patch();
        harmony.CreateClassProcessor(typeof(PatchRevengeDice.PatchClearDices)).Patch();
        harmony.CreateClassProcessor(typeof(PatchRevengeDice.PatchOnTakeDamage)).Patch();
        harmony.CreateClassProcessor(typeof(PatchRevengeDice.PatchStartParrying)).Patch();
        harmony.CreateClassProcessor(typeof(PatchRevengeDice.PatchStartAction)).Patch();
        harmony.CreateClassProcessor(typeof(PatchRevengeDice.PatchOnUseCard)).Patch();

        Cards = new();
        CurrentRevenge = new();
    }

    /// <summary>A unit when revenge decided</summary>
    /// <param name="card">A revenge dicecard</param>
    /// <param name="revengeBy">A dice of revenge decided</param>
    public virtual void OnBeforeRevenge(BattlePlayingCardDataInUnitModel card, BattleDiceBehavior revengeBy)
    {
    }

    /// <summary>A unit when use revenge</summary>
    /// <param name="card">A revenge dicecard</param>
    public virtual void OnRevenge(BattlePlayingCardDataInUnitModel card)
    {
    }

    /// <summary>Adds revenge card to unit</summary>
    /// <param name="unit">A unit for adds card</param>
    /// <param name="display">The card that display in battle</param>
    /// <param name="behaviors">Override behavior if not null</param>
    public static void AddRevengeCard(BattleUnitModel unit, DiceCardXmlInfo display, List<BattleDiceBehavior>? behaviors = null)
    {
        var playcard = unit.CreatePlayingCard(display);

        behaviors?.Let(it =>
        {
            playcard.cardBehaviorQueue = new();

            foreach (var beh in it)
            {
                beh.card = playcard;

                foreach (var abi in beh.abilityList)
                {
                    abi.behavior = beh;
                }

                playcard.cardBehaviorQueue.Enqueue(beh);
            }
        });

        if (!RevengeDice.Cards.ContainsKey(unit))
        {
            RevengeDice.Cards.Add(unit, new());
        }

        RevengeDice.Cards[unit].Enqueue(playcard);
    }

    /// <summary>Adds revenge card to unit</summary>
    /// <param name="unit">A unit for adds card</param>
    /// <param name="card">A card to adds</param>
    public static void AddRevengeCard(BattleUnitModel unit, BattlePlayingCardDataInUnitModel card)
    {
        if (!RevengeDice.Cards.ContainsKey(unit))
        {
            RevengeDice.Cards.Add(unit, new());
        }

        RevengeDice.Cards[unit].Enqueue(card);
    }

    internal static Dictionary<BattleUnitModel, Queue<BattlePlayingCardDataInUnitModel>> Cards
    {
        get;
        set => field = value;
    }

    internal static Dictionary<BattleUnitModel, BattlePlayingCardDataInUnitModel> CurrentRevenge
    {
        get;
        set => field = value;
    }
}

internal class PatchRevengeDice
{
    [HarmonyPatch(typeof(StageController), "ActivateStartBattleEffectPhase")]
    public class PatchStandbyResolve
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var target = typeof(BattleKeepedCardDataInUnitModel).Method("AddBehaviours", [typeof(BattleDiceCardModel), typeof(List<BattleDiceBehavior>)]);
            var inject = typeof(PatchStandbyResolve).Method("InjectMethod");

            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(CodeMatch.Calls(target))
                .Insert(new CodeInstruction(OpCodes.Call, inject));

            return matcher.Instructions();
        }

        static List<BattleDiceBehavior> InjectMethod(List<BattleDiceBehavior> standbyDices)
        {
            List<BattleDiceBehavior> solved = new();
            List<BattleDiceBehavior> revenges = new();
            BattleUnitModel? owner = null;

            foreach (var dice in standbyDices)
            {
                if (dice.abilityList.Exists(abi => abi is RevengeDice))
                {
                    owner = dice.owner;

                    revenges.Add(dice);
                }
                else
                {
                    solved.Add(dice);
                }
            }

            if (owner is not null)
            {
                var xmlInfo = revenges[0].card.card.XmlData;

                RevengeDice.AddRevengeCard(owner, xmlInfo, revenges);
            }

            return solved;
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "OnTakeDamageByAttack")]
    public class PatchOnTakeDamage
    {
        static void Prefix(BattleUnitModel __instance, BattleDiceBehavior atkDice)
        {
            if (!atkDice.owner.IsTargetable(__instance))
            {
                return;
            }

            if (!RevengeDice.CurrentRevenge.ContainsKey(__instance) && RevengeDice.Cards.TryGetValue(__instance, out var res))
            {
                if (res.Count < 1)
                {
                    return;
                }

                var playcard = res.Dequeue();

                var speed = atkDice.card.speedDiceResultValue + 1;
                var target = atkDice.owner;

                playcard.Let(it =>
                {
                    it.target = target;
                    it.earlyTarget = target;
                    it.speedDiceResultValue = speed;
                });

                foreach (var beh in playcard.cardBehaviorQueue)
                {
                    if (beh.Type == BehaviourType.Standby)
                    {
                        var newBeh = beh.behaviourInCard.Copy();

                        newBeh.Type = BehaviourType.Atk;

                        beh.behaviourInCard = newBeh;
                    }

                }

                foreach (var abi in playcard.cardBehaviorQueue.SelectMany(beh => beh.abilityList))
                {
                    if (abi is RevengeDice rev)
                    {
                        rev.OnBeforeRevenge(playcard, atkDice);
                    }
                }

                RevengeDice.CurrentRevenge.Add(__instance, playcard);
                StageController.Instance.GetAllCards().Insert(0, playcard);
            }
        }
    }

    [HarmonyPatch(typeof(BattlePlayingCardDataInUnitModel), "OnEndBattle")]
    public class PatchOnEndBattle
    {
        static void Prefix(BattlePlayingCardDataInUnitModel __instance)
        {
            if (RevengeDice.CurrentRevenge.ContainsValue(__instance))
            {
                RevengeDice.CurrentRevenge.Remove(__instance.owner);
            }
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "OnUseCard")]
    public class PatchOnUseCard
    {
        static void Prefix(BattlePlayingCardDataInUnitModel card)
        {
            foreach (var abi in card.GetDiceBehaviorList().Map(beh => beh.abilityList).Flatten())
            {
                if (abi is RevengeDice rev)
                {
                    rev.OnRevenge(card);
                }
            }
        }
    }

    [HarmonyPatch(typeof(StageController), "ArrangeCardsPhase")]
    public class PatchClearDices
    {
        static void Prefix()
        {
            RevengeDice.Cards.Clear();
            RevengeDice.CurrentRevenge.Clear();
        }
    }

    [HarmonyPatch(typeof(StageController), "StartParrying")]
    public class PatchStartParrying
    {
        static bool Prefix(BattlePlayingCardDataInUnitModel cardA)
        {
            if (cardA.cardBehaviorQueue.All(beh => beh.abilityList.Exists(abi => abi is RevengeDice)))
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
