using System.Reflection.Emit;
using LOR_DiceSystem;
using HarmonyLib;
using HarmonyExtension;

namespace DeviceOfHermes.CustomDice;

internal static class CustomDicePatch
{
    static Harmony harmony = new Harmony("DeviceOfHermes.CustomDice");

    static CustomDicePatch()
    {
        Patch(typeof(PatchOnUseCard));
        Patch(typeof(PatchOnEndBattle));
        Patch(typeof(PatchOnLoseParrying));
        Patch(typeof(PatchStandbyResolve));
        Patch(typeof(PatchOnTakeDamage));
        Patch(typeof(PatchClearDices));
        Patch(typeof(PatchStartParrying));
        Patch(typeof(PatchStartAction));
        Patch(typeof(PatchDecision));
        Patch(typeof(PatchOnParryingResultDecided));
        Patch(typeof(PatchOnEndAction));
        Patch(typeof(PatchSetBehaviourResult));
        Patch(typeof(PatchOnDiceRollen));
        Patch(typeof(PatchOnDiceInit));
        Patch(typeof(PatchOnDiceTick));
    }

    public static void Init()
    {
    }

    private static void Patch(Type ty)
    {
        harmony.CreateClassProcessor(ty).Patch();
    }

    [HarmonyPatch(typeof(BattleUnitModel), "OnUseCard")]
    class PatchOnUseCard
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

                if (abi is RevengeDice rev)
                {
                    rev.OnRevenge(card);
                }
            }

            if (card.owner is not null && UnbreakableDice.Stash.ContainsKey(card.owner))
            {
                UnbreakableDice.Stash.Remove(card.owner);
            }
        }
    }

    [HarmonyPatch(typeof(BattlePlayingCardDataInUnitModel), "OnEndBattle")]
    class PatchOnEndBattle
    {
        static void Prefix(BattlePlayingCardDataInUnitModel __instance)
        {
            var owner = __instance.owner;

            if (owner is null)
            {
                return;
            }

            if (RevengeDice.CurrentRevenge.ContainsValue(__instance))
            {
                RevengeDice.CurrentRevenge.Remove(__instance.owner);
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

                var usedSlot = queue.Peek().card.slotOrder;
                var earlyTarget = queue.Peek().card.targetSlotOrder;
                var targetSlot = StageController.Instance.GetAllCards()
                    .Find(c => c.owner == target && c.targetSlotOrder == usedSlot && c.slotOrder != earlyTarget)?
                    .Let(c => c.slotOrder) ?? -1;

                var playcard = owner.CreatePlayingCard(xmlInfo, target, targetSlotOrder: targetSlot, speedDiceResultValue: speed)
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
    class PatchOnLoseParrying
    {
        static void Postfix(BattlePlayingCardDataInUnitModel __instance)
        {
            var beh = __instance.currentBehavior;
            var owner = __instance.owner;

            if (beh is not null && owner is not null)
            {
                if (UnbreakableDice.IsUnbreakableDice(beh) && !UnbreakableDice.IsBreakedDice(beh))
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

                    if (beh.TargetDice?.card.speedDiceResultValue is int targetNum)
                    {
                        if (targetNum >= beh.card.speedDiceResultValue)
                        {
                            beh.card.speedDiceResultValue = targetNum + 1;
                        }
                    }

                    UnbreakableDice.Stash[owner].Enqueue(beh);
                }
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

                List<BattleDiceBehavior> revenges = new();
                List<BattleDiceBehavior> secondlies = new();
                BattleUnitModel? owner = null;

                foreach (var dice in behs)
                {
                    if (dice.Type is BehaviourType.Standby && dice.Detail is BehaviourDetail.Slash or BehaviourDetail.Penetrate or BehaviourDetail.Hit && dice.abilityList.Exists(abi => abi is RevengeDice))
                    {
                        dice.card = card;
                        owner = dice.owner;

                        revenges.Add(dice);
                    }

                    if (dice.Type is BehaviourType.Standby && dice.abilityList.Exists(abi => abi is SecondlyDice))
                    {
                        dice.card = card;

                        secondlies.Add(dice);
                    }
                }

                if (owner is not null)
                {
                    var xmlInfo = revenges[0].card.card.XmlData;

                    RevengeDice.AddRevengeCard(owner, xmlInfo, revenges);
                }

                SecondlyDice.AddSecondlyDice(card, secondlies);
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
                    if (!dice.abilityList.Exists(abi => abi is RevengeDice or SecondlyDice))
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

    [HarmonyPatch(typeof(BattleUnitModel), "OnTakeDamageByAttack")]
    class PatchOnTakeDamage
    {
        static void Prefix(BattleUnitModel __instance, BattleDiceBehavior atkDice)
        {
            if (!atkDice.owner.IsTargetable(__instance))
            {
                return;
            }

            if (RevengeDice.IsRevengeNow(atkDice.owner))
            {
                return;
            }

            if (!RevengeDice.IsRevengeNow(__instance) && RevengeDice.Cards.TryGetValue(__instance, out var res))
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

    [HarmonyPatch(typeof(StageController), "ArrangeCardsPhase")]
    class PatchClearDices
    {
        static void Prefix()
        {
            RevengeDice.Cards.Clear();
            RevengeDice.CurrentRevenge.Clear();
        }
    }

    [HarmonyPatch(typeof(StageController), "StartParrying")]
    class PatchStartParrying
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
    class PatchStartAction
    {
        [HarmonyReversePatch]
        public static void StartAction(StageController instance, BattlePlayingCardDataInUnitModel card) =>
            throw new NotImplementedException();
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

            if (!SecondlyDice._table.TryGetValue(loserUsing, out var additFields))
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
            if (SecondlyDice._table.TryGetValue(____teamEnemy.playingCard, out var enemyAdditFields))
            {
                enemyAdditFields.EndAction(____teamEnemy.playingCard);
            }

            if (SecondlyDice._table.TryGetValue(____teamEnemy.playingCard, out var librarianAdditFields))
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

            if (!SecondlyDice._table.TryGetValue(team.playingCard, out var additFields))
            {
                return;
            }

            var beh = Mem.Take(ref additFields._tempSecondly);

            if (beh is null)
            {
                return;
            }

            if (!opponentTeam.DiceExists())
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

            diceBehaviourResultData.skip = false;
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

            if (!SecondlyDice._table.TryGetValue(playCard, out var additFields))
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

    [HarmonyPatch(typeof(BattleSimpleActionUI_Dice), "PrepareDice", [typeof(List<BattleCardBehaviourResult>)])]
    class PatchOnDiceInit
    {
        static void Postfix(BattleSimpleActionUI_Dice __instance, List<BattleCardBehaviourResult> battleDiceBehaviorResults)
        {
            if (battleDiceBehaviorResults.Count <= 0)
            {
                return;
            }

            ChangeDiceUI(__instance, battleDiceBehaviorResults[0].behaviourRawData);
        }
    }

    [HarmonyPatch(typeof(BattleSimpleActionUI_Dice), "PrepareDice", [typeof(BattleDiceBehaviourUI)])]
    class PatchOnDiceTick
    {
        static void Postfix(BattleSimpleActionUI_Dice __instance, BattleDiceBehaviourUI b)
        {
            ChangeDiceUI(__instance, b.behaviourInCard);
        }
    }

    static void ChangeDiceUI(BattleSimpleActionUI_Dice __instance, DiceBehaviour beh)
    {
        var ability = AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script);

        if (ability is UnbreakableDice)
        {
            switch (beh.Detail)
            {
                case BehaviourDetail.Slash:
                    __instance.imgDetailIcon_Center.sprite = HermesConstants.UnbreakableSlashBeh;

                    break;
                case BehaviourDetail.Penetrate:
                    __instance.imgDetailIcon_Center.sprite = HermesConstants.UnbreakablePenetrateBeh;

                    break;
                case BehaviourDetail.Hit:
                    __instance.imgDetailIcon_Center.sprite = HermesConstants.UnbreakableHitBeh;

                    break;

                default:
                    return;
            }

            var dhsv = __instance.img_diceFace.gameObject.GetComponent<_2dxFX_HSV>();

            if (dhsv is null)
            {
                dhsv = __instance.img_diceFace.gameObject.AddComponent<_2dxFX_HSV>();
            }

            dhsv._HueShift = 0f;
            dhsv._Saturation = 1.55f;
            dhsv._ValueBrightness = 0.60f;

            var dc = __instance.imgDetailIcon_Center.gameObject.GetComponent<_2dxFX_HSV>();

            if (dc is not null)
            {
                UnityEngine.Object.Destroy(dc);
            }
        }
        else if (ability is RevengeDice)
        {
            var dhsv = __instance.img_diceFace.gameObject.GetComponent<_2dxFX_HSV>();

            if (dhsv is null)
            {
                dhsv = __instance.img_diceFace.gameObject.AddComponent<_2dxFX_HSV>();
            }

            dhsv._HueShift = 90f;
            dhsv._Saturation = 1.5f;
            dhsv._ValueBrightness = 1f;

            var chsv = __instance.imgDetailIcon_Center.gameObject.GetComponent<_2dxFX_HSV>();

            if (chsv is null)
            {
                chsv = __instance.imgDetailIcon_Center.gameObject.AddComponent<_2dxFX_HSV>();
            }

            chsv._HueShift = 90f;
            chsv._Saturation = 1.5f;
            chsv._ValueBrightness = 1f;
        }
        else
        {
            var df = __instance.img_diceFace.gameObject.GetComponent<_2dxFX_HSV>();

            if (df is not null)
            {
                UnityEngine.Object.Destroy(df);
            }

            var dc = __instance.imgDetailIcon_Center.gameObject.GetComponent<_2dxFX_HSV>();

            if (dc is not null)
            {
                UnityEngine.Object.Destroy(dc);
            }
        }
    }

    private static AccessTools.FieldRef<BattleParryingManager, BattleParryingManager.ParryingDecisionResult> _decidedResultRef
        = typeof(BattleParryingManager).FieldRefAccess<BattleParryingManager.ParryingDecisionResult>("_decisionResult");

    private static AccessTools.FieldRef<BattleParryingManager, BattleParryingManager.ParryingTeam> _loserTeamRef
        = typeof(BattleParryingManager).FieldRefAccess<BattleParryingManager.ParryingTeam>("_currentLoserTeam");
}
