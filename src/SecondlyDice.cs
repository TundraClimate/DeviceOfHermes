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
        harmony.CreateClassProcessor(typeof(PatchSetBehaviourResult)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnDiceRollen)).Patch();
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

        public BattleDiceBehavior? _tempSecondly;

        public Dictionary<int, BattleDiceBehavior> _uiBehaviorDict = new();

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

            if (IsBonusDice())
            {
                var dice = _secondlyDiceQueue.Peek();

                _uiBehaviorDict.TryAdd(dice.Index, dice);
                _tempSecondly = dice;
            }
            else if (_secondlyDiceQueue.Count != 0)
            {
                var dice = _secondlyDiceQueue.Dequeue();

                _uiBehaviorDict.TryAdd(dice.Index, dice);
                _tempSecondly = dice;
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

    [HarmonyPatch(typeof(BattleParryingManager), "SetBehaviourResultData")]
    class PatchSetBehaviourResult
    {
        static void Postfix(BattleParryingManager __instance, BattleParryingManager.ParryingTeam team, BattleParryingManager.ParryingTeam opponentTeam, BattleCardTotalResult result)
        {
            if (team.playingCard is null)
            {
                return;
            }

            if (!_table.TryGetValue(team.playingCard, out var additFields))
            {
                return;
            }

            var beh = Mem.Take(ref additFields._tempSecondly);

            if (beh is null)
            {
                return;
            }

            var diceBehaviourResultData = default(DiceBehaviourResultData);
            var parryingDiceType = beh.Detail switch
            {
                BehaviourDetail.Slash or BehaviourDetail.Penetrate or BehaviourDetail.Hit => BattleParryingManager.ParryingDiceType.Attack,
                _ => BattleParryingManager.ParryingDiceType.Defense,
            };

            if (parryingDiceType != BattleParryingManager.ParryingDiceType.Attack)
            {
                if (parryingDiceType == BattleParryingManager.ParryingDiceType.Defense)
                {
                    diceBehaviourResultData.actionType = ActionType.Def;
                }
            }
            else
            {
                diceBehaviourResultData.actionType = ActionType.Atk;
            }
            if (diceBehaviourResultData.actionType == ActionType.Def && !opponentTeam.DiceExists())
            {
                diceBehaviourResultData.skip = true;
                result.SetSkip(DiceUITiming.Start);
            }
            else
            {
                diceBehaviourResultData.skip = false;
            }

            diceBehaviourResultData.passingEvasion = false;
            diceBehaviourResultData.BreakState = false;

            diceBehaviourResultData.passingEvasion = beh.passingEvasion;
            diceBehaviourResultData.BreakState = beh.breakState;

            diceBehaviourResultData.behaviourDetail = beh.Detail;

            if (beh.card.card.GetSpec().Ranged == CardRange.Far)
            {
                diceBehaviourResultData.range = CardRange.Far;
            }
            else
            {
                diceBehaviourResultData.range = CardRange.Near;
            }

            diceBehaviourResultData.actionDetail = MotionConverter.MotionToAction(beh.behaviourInCard.MotionDetail);

            if (beh.behaviourInCard.MotionDetailDefault != MotionDetail.N && !beh.owner.customBook.ContainsCategory(beh.card.card.GetCategory()))
            {
                diceBehaviourResultData.actionDetail = MotionConverter.MotionToAction(beh.behaviourInCard.MotionDetail);
            }

            diceBehaviourResultData.actionStartDir = ActionDirection.Front;

            var decidedResult = _decidedResultRef(__instance);

            if (decidedResult == BattleParryingManager.ParryingDecisionResult.Draw)
            {
                diceBehaviourResultData.result = Result.Draw;
            }
            else if (decidedResult == BattleParryingManager.ParryingDecisionResult.WinEnemy)
            {
                if (beh.owner.faction is Faction.Enemy)
                {
                    diceBehaviourResultData.result = Result.Win;
                }
                else
                {
                    diceBehaviourResultData.result = Result.Lose;
                }
            }
            else if (beh.owner.faction is Faction.Player)
            {
                diceBehaviourResultData.result = Result.Win;
            }
            else
            {
                diceBehaviourResultData.result = Result.Lose;
            }

            if (diceBehaviourResultData.result == Result.Lose && opponentTeam.GetBehaviourType() == BehaviourType.Atk && diceBehaviourResultData.range != CardRange.Far)
            {
                diceBehaviourResultData.actionDetail = ActionDetail.Damaged;
            }

            result.SetBehaviourResult(diceBehaviourResultData);
        }
    }

    [HarmonyPatch(typeof(RencounterManager), "PrintActivatedAbility")]
    class PatchOnDiceRollen
    {
        static void Prefix(
            BattleCardBehaviourResult ____currentEnemyBehaviourResult,
            BattleCardBehaviourResult ____currentLibrarianBehaviourResult,
            BattleUnitView ____enemy,
            BattleUnitView ____librarian
        )
        {
            PrepareDice(____currentEnemyBehaviourResult, ____enemy);
            PrepareDice(____currentLibrarianBehaviourResult, ____librarian);
        }

        static void PrepareDice(BattleCardBehaviourResult result, BattleUnitView view)
        {
            var playCard = result.playingCard;

            if (playCard is null)
            {
                return;
            }

            if (!_table.TryGetValue(playCard, out var additFields))
            {
                return;
            }

            if (additFields._uiBehaviorDict.TryGetValue(result.behaviourIdx, out var beh))
            {
                view.diceActionUI.currentDice.PrepareDice(new BattleDiceBehaviourUI(beh));
                view.diceActionUI.currentDice.SetDiceFace(beh.DiceResultValue);
                view.diceActionUI.currentDice.SetDiceValue(true, beh.DiceResultValue);

                additFields._uiBehaviorDict.Remove(result.behaviourIdx);
            }
        }
    }

    private static AccessTools.FieldRef<BattleParryingManager, BattleParryingManager.ParryingDecisionResult> _decidedResultRef
        = typeof(BattleParryingManager).FieldRefAccess<BattleParryingManager.ParryingDecisionResult>("_decisionResult");

    private static AccessTools.FieldRef<BattleParryingManager, BattleParryingManager.ParryingTeam> _loserTeamRef
        = typeof(BattleParryingManager).FieldRefAccess<BattleParryingManager.ParryingTeam>("_currentLoserTeam");
}
