using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using HarmonyExtension;
using UnityEngine;
using LOR_DiceSystem;
using DeviceOfHermes.AdvancedBase;

namespace DeviceOfHermes.CustomDice;

/// <summary>A dice of secondly in clash</summary>
public class SecondlyDice : AdvancedDiceBase
{
    static SecondlyDice()
    {
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Type is BehaviourType.Standby &&
                beh.Detail is BehaviourDetail.Slash &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is SecondlyDice,
            HermesConstants.SecondlySlash,
            new Color(255, 255, 200, 200)
        );
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Type is BehaviourType.Standby &&
                beh.Detail is BehaviourDetail.Penetrate &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is SecondlyDice,
            HermesConstants.SecondlyPenetrate,
            new Color(255, 255, 200, 200)
        );
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Type is BehaviourType.Standby &&
                beh.Detail is BehaviourDetail.Hit &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is SecondlyDice,
            HermesConstants.SecondlyHit,
            new Color(255, 255, 200, 200)
        );
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Type is BehaviourType.Standby &&
                beh.Detail is BehaviourDetail.Guard &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is SecondlyDice,
            HermesConstants.SecondlyGuard,
            new Color(255, 255, 200, 200)
        );
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Type is BehaviourType.Standby &&
                beh.Detail is BehaviourDetail.Evasion &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is SecondlyDice,
            HermesConstants.SecondlyEvasion,
            new Color(255, 255, 200, 200)
        );

        var harmony = new Harmony("DeviceOfHermes.CustomDice.SecondlyDice");

        harmony.CreateClassProcessor(typeof(PatchStandbyResolve)).Patch();
        harmony.CreateClassProcessor(typeof(PatchDecision)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnParryingResultDecided)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnEndAction)).Patch();
    }

    /// <summary>Add secondly dice</summary>
    public static void AddSecondlyDice(BattlePlayingCardDataInUnitModel playCard, params List<BattleDiceBehavior> behs)
    {
        _table.GetValue(playCard, _ => new CardModelAdditFields() { _secondlyDiceQueue = new(behs) });
    }

    private static ConditionalWeakTable<BattlePlayingCardDataInUnitModel, CardModelAdditFields> _table = new();

    private class CardModelAdditFields
    {
        public BattleDiceBehavior? _tempBehavior;

        public Queue<BattleDiceBehavior> _secondlyDiceQueue = new();

        public bool IsBonusDice()
        {
            return _secondlyDiceQueue.TryPeek(out var peeked)
                && (peeked.isBonusEvasion || peeked.isBonusAttack)
                && !peeked.forbiddenBonusDice;
        }

        public void EndAction(BattlePlayingCardDataInUnitModel playingCard)
        {
            if (_tempBehavior is not null)
            {
                playingCard.currentBehavior = Mem.Take(ref _tempBehavior);
            }

            if (!IsBonusDice() && _secondlyDiceQueue.Count != 0)
            {
                _secondlyDiceQueue.Dequeue();
            }
        }
    }

    [HarmonyPatch(typeof(StageController), "ActivateStartBattleEffectPhase")]
    class PatchStandbyResolve
    {
        static Exception Finalizer(Exception __exception, List<BattlePlayingCardDataInUnitModel> ____allCardList)
        {
            foreach (var card in ____allCardList)
            {
                var behs = card.card.CreateDiceCardBehaviorList();

                List<BattleDiceBehavior> dices = new();

                foreach (var dice in behs)
                {
                    if (dice.Type is BehaviourType.Standby && dice.abilityList.Exists(abi => abi is SecondlyDice))
                    {
                        dice.card = card;

                        dices.Add(dice);
                    }
                }

                AddSecondlyDice(card, dices);
            }

            foreach (var unit in BattleObjectManager.instance.GetAliveList())
            {
                if (unit?.cardSlotDetail?.keepCard?.cardBehaviorQueue is null)
                {
                    continue;
                }

                List<BattleDiceBehavior> behs = new();

                foreach (var dice in unit.cardSlotDetail.keepCard.cardBehaviorQueue)
                {
                    if (!dice.abilityList.Exists(abi => abi is SecondlyDice))
                    {
                        behs.Add(dice);
                    }
                }

                unit.cardSlotDetail.keepCard.cardBehaviorQueue.Clear();

                foreach (var dice in behs)
                {
                    unit.cardSlotDetail.keepCard.cardBehaviorQueue.Enqueue(dice);
                }
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleParryingManager), "Decision")]
    class PatchDecision
    {
        [HarmonyReversePatch]
        public static void Decision(BattleParryingManager __instance)
        {
            IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var target = typeof(BattleParryingManager).Method("ActionPhase");

                var matcher = new CodeMatcher(instructions);

                matcher.MatchStartForward(CodeMatch.IsLdarg(), CodeMatch.Calls(target))
                    .SetOpcodeAndAdvance(OpCodes.Nop)
                    .SetOpcodeAndAdvance(OpCodes.Nop);

                return matcher.Instructions();
            }

            List<CodeInstruction> dummy = new();

            _ = Transpiler(dummy);
        }
    }

    [HarmonyPatch(typeof(BattleParryingManager), "Decision")]
    class PatchOnParryingResultDecided
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var target = typeof(BattleParryingManager).Method("ActionPhase");
            var inject = typeof(PatchOnParryingResultDecided).Method("InjectMethod");

            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(CodeMatch.IsLdarg(), CodeMatch.Calls(target))
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(matcher.Instruction),
                    new CodeInstruction(OpCodes.Call, inject)
                );

            return matcher.Instructions();
        }

        static void InjectMethod(BattleParryingManager __instance)
        {
            var pRes = _decidedResultRef(__instance);

            Hermes.Say("======");
            Hermes.Say($"ParringResult: {pRes}");

            if (!RerollDice(__instance) || !RerollDice(__instance))
            {
                Hermes.Say($"End parrying");

                return;
            }
        }

        static bool RerollDice(BattleParryingManager __instance)
        {
            var loserTeam = _loserTeamRef(__instance);

            if (loserTeam is null)
            {
                Hermes.Say($"Result is draw");

                return false;
            }

            var loserUsing = loserTeam.playingCard;

            Hermes.Say($"Loser using: {loserUsing.card.XmlData.Name}:{loserUsing.currentBehaviorUI?.Index}");

            if (!_table.TryGetValue(loserUsing, out var additFields))
            {
                Hermes.Say($"Loser hasen't SecondlyDice");

                return false;
            }

            if ((loserUsing.currentBehavior?.abilityList?.OfType<SecondlyDice>()?.Count() ?? 0) != 0)
            {
                Hermes.Say("Dice was already rerolled");

                return false;
            }

            if (!additFields._secondlyDiceQueue.TryPeek(out var secondly))
            {
                Hermes.Say("secondly is not remaining");

                return false;
            }

            if (loserUsing.currentBehavior is not null)
            {
                Hermes.Say("Hook OnLoseParrying before Reroll");

                loserUsing.OnLoseParrying();

                additFields._tempBehavior = loserUsing.currentBehavior;
            }

            Hermes.Say($"Reroll with new SecondlyDice");

            loserUsing.currentBehavior = secondly.Also(dice =>
            {
                dice.SetIndex(loserUsing.currentBehavior?.Index ?? loserUsing.cardBehaviorQueue.Count);
            });

            PatchDecision.Decision(__instance);

            var newRes = _decidedResultRef(__instance);

            Hermes.Say($"Created new Result: {newRes}");

            return true;
        }
    }

    [HarmonyPatch(typeof(BattleParryingManager), "EndAction")]
    class PatchOnEndAction
    {
        static void Prefix(BattleParryingManager.ParryingTeam ____teamEnemy, BattleParryingManager.ParryingTeam ____teamLibrarian)
        {
            if (_table.TryGetValue(____teamEnemy.playingCard, out var enemyAdditFields))
            {
                enemyAdditFields.EndAction(____teamEnemy.playingCard);
            }

            if (_table.TryGetValue(____teamEnemy.playingCard, out var librarianAdditFields))
            {
                librarianAdditFields.EndAction(____teamLibrarian.playingCard);
            }
        }
    }

    private static AccessTools.FieldRef<BattleParryingManager, BattleParryingManager.ParryingDecisionResult> _decidedResultRef
        = typeof(BattleParryingManager).FieldRefAccess<BattleParryingManager.ParryingDecisionResult>("_decisionResult");

    private static AccessTools.FieldRef<BattleParryingManager, BattleParryingManager.ParryingTeam> _loserTeamRef
        = typeof(BattleParryingManager).FieldRefAccess<BattleParryingManager.ParryingTeam>("_currentLoserTeam");
}
