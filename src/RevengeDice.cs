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

        harmony.CreateClassProcessor(typeof(PatchRevengeDice.PatchBehaviourToKeeps)).Patch();
        harmony.CreateClassProcessor(typeof(PatchRevengeDice.PatchClearDices)).Patch();
        harmony.CreateClassProcessor(typeof(PatchRevengeDice.PatchOnTakeDamage)).Patch();
        harmony.CreateClassProcessor(typeof(PatchRevengeDice.PatchStartParrying)).Patch();
        harmony.CreateClassProcessor(typeof(PatchRevengeDice.PatchStartAction)).Patch();
        harmony.CreateClassProcessor(typeof(PatchRevengeDice.PatchOnUseCard)).Patch();

        _dices = new();
    }

    internal static void UpdateDices(BattleUnitModel owner, BattleDiceCardModel card, BattleDiceBehavior dice)
    {
        if (!_dices.ContainsKey(owner))
        {
            _dices.Add(owner, new());
        }

        var newBeh = dice.behaviourInCard.Copy();

        newBeh.Type = BehaviourType.Atk;

        dice.behaviourInCard = newBeh;

        if (_dices[owner].Any(pc => pc.card == card))
        {
            _dices[owner].Foreach(pc =>
            {
                if (pc.card == card)
                {
                    dice.card = pc;
                    pc.cardBehaviorQueue.Enqueue(dice);
                }
            });
        }
        else
        {
            var playCard = new BattlePlayingCardDataInUnitModel()
            {
                card = card,
                cardBehaviorQueue = new(),
            };

            dice.card = playCard;

            playCard.cardBehaviorQueue.Enqueue(dice);
            playCard.owner = owner;

            _dices[owner].Enqueue(playCard);
        }
    }

    /// <summary>A unit when revenge decided</summary>
    /// <param name="card">A revenge dicecard</param>
    /// <param name="revengeBy">A dice of revenge decided</param>
    public virtual void OnRevenge(BattlePlayingCardDataInUnitModel card, BattleDiceBehavior revengeBy)
    {
    }

    /// <summary>A unit when use revenge</summary>
    /// <param name="card">A revenge dicecard</param>
    public virtual void OnUseRevenge(BattlePlayingCardDataInUnitModel card)
    {
    }

    internal static Dictionary<BattleUnitModel, Queue<BattlePlayingCardDataInUnitModel>> Dices => _dices;

    private static Dictionary<BattleUnitModel, Queue<BattlePlayingCardDataInUnitModel>> _dices;

    internal static BattlePlayingCardDataInUnitModel? currentRevenge;
}

internal class PatchRevengeDice
{
    [HarmonyPatch(typeof(BattleKeepedCardDataInUnitModel), "AddBehaviours", [typeof(BattleDiceCardModel), typeof(List<BattleDiceBehavior>)])]
    public class PatchBehaviourToKeeps
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var target = AccessTools.Method(typeof(Queue<BattleDiceBehavior>), "Enqueue");
            var inject = AccessTools.Method(typeof(PatchBehaviourToKeeps), "InjectMethod");

            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(CodeMatch.Calls(target))
                .SetInstruction(new CodeInstruction(OpCodes.Call, inject))
                .Insert(new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldarg_1));

            return matcher.Instructions();
        }

        static void InjectMethod(Queue<BattleDiceBehavior> cardBehaviorQueue, BattleDiceBehavior beh, BattleKeepedCardDataInUnitModel keeps, BattleDiceCardModel card)
        {
            if (beh.abilityList.Exists(abi => abi is RevengeDice))
            {
                RevengeDice.UpdateDices(keeps.owner, card, beh);
            }
            else
            {
                cardBehaviorQueue.Enqueue(beh);
            }
        }
    }

    [HarmonyPatch(typeof(StageController), "ArrangeCardsPhase")]
    public class PatchClearDices
    {
        static void Prefix()
        {
            RevengeDice.Dices.Clear();
            RevengeDice.currentRevenge = null;
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

    [HarmonyPatch(typeof(BattleUnitModel), "OnTakeDamageByAttack")]
    public class PatchOnTakeDamage
    {
        static void Prefix(BattleUnitModel __instance, BattleDiceBehavior atkDice)
        {
            if (atkDice.abilityList.Exists(abi => abi is RevengeDice))
            {
                RevengeDice.currentRevenge = null;

                return;
            }


            if (RevengeDice.Dices.ContainsKey(__instance) && RevengeDice.Dices[__instance].Count > 0)
            {
                if (RevengeDice.currentRevenge is null)
                {
                    var card = RevengeDice.Dices[__instance].Dequeue();

                    card.target = atkDice.owner;
                    card.targetSlotOrder = -1;
                    card.speedDiceResultValue = 99;

                    card.cardBehaviorQueue.Foreach(beh => beh.abilityList.Filter(abi => abi is RevengeDice).Foreach(abi => ((RevengeDice)abi).OnRevenge(card, atkDice)));

                    RevengeDice.currentRevenge = card;
                    StageController.Instance.GetAllCards().Add(card);
                }
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
                    rev.OnUseRevenge(card);
                }
            }
        }
    }
}
