using System.Reflection;
using System.Reflection.Emit;
using LOR_DiceSystem;
using UI;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DeviceOfHermes.CustomDice;
using System.Runtime.CompilerServices;

namespace DeviceOfHermes.AdvancedBase;

internal static class AdvancedPatch
{
    private static Harmony harmony = new Harmony("DeviceOfHermes.AdvancedBase");

    public static void Init()
    {
        Patch(typeof(PatchTargetUI));
        Patch(typeof(PatchOnStartResolve));
        Patch(typeof(PatchOnStartAction));
        Patch(typeof(PatchOnDynamicParrying));
        Patch(typeof(PatchOnChangeTarget));
        Patch(typeof(PatchCanDiscard));
        Patch(typeof(PatchOnAddKeeps));
        Patch(typeof(PatchOnAddKeep));
        Patch(typeof(PatchParryingResult));
        Patch(typeof(PatchDiceResultValue));
        Patch(typeof(PatchDiceDamageValue));
        Patch(typeof(PatchOnRoundStartFirst));
        Patch(typeof(PatchOnBattleLast));
        Patch(typeof(PatchOnRoundStartLast));
        Patch(typeof(PatchCanDiscard));
        Patch(typeof(OriginalAdvInit));
        Patch(typeof(PatchAddBufInitializer));
        Patch(typeof(PatchAddBufWdInitializer));
        Patch(typeof(PatchUnusedRemove));
        Patch(typeof(PatchOnDrawCardPhase));
        Patch(typeof(PatchOnSetBuf));
        Patch(typeof(PatchOnUnitClick));
        Patch(typeof(PatchOnStartBattle));
        Patch(typeof(PatchOnAddKeywordBuf));
        Patch(typeof(PatchGetBreakDmgRedAll));
        Patch(typeof(PatchPlayForAuto));
        Patch(typeof(PatchOnChooseCard));
        Patch(typeof(PatchOnWaveStart));
        Patch(typeof(PatchStartOneSidePlay));
        Patch(typeof(PatchStartParrying));
        Patch(typeof(PatchDie));
        Patch(typeof(PatchExtinct));
        Patch(typeof(PatchOnRecoverHp));
        Patch(typeof(PatchOnRecoverPP));
        Patch(typeof(PatchGetCardBufIcon));
        Patch(typeof(PatchBeforeRollDice));
        Patch(typeof(PatchOnSucceedAttack));
        Patch(typeof(PatchOnAddNewKeywordBuf));
        Patch(typeof(PatchOnSucceedAreaAttack));
        Patch(typeof(PatchOnWaveStartInModel));
        Patch(typeof(PatchOnModifiedStack));
        Patch(typeof(PatchOnAddKeywordBufs));
        Patch(typeof(PatchOnEndParrying));
        Patch(typeof(PatchOnEndOneSideAction));
        Patch(typeof(PatchIsTargetableAllUnit));
        Patch(typeof(PatchOnCreatePlayingCard));
        Patch(typeof(PatchOnCreateSpeedDiceRule));
        Patch(typeof(PatchOnEndBattle));
        Patch(typeof(PatchOnRecoverHP));
        Patch(typeof(PatchOnUseCard));
        Patch(typeof(PatchOnEventDiceAbility));
        Patch(typeof(PatchUnitBeforeRollDice));
        Patch(typeof(PatchLambdaBeforeRollDice));
    }

    private static void Patch(Type type)
    {
        try
        {
            harmony.CreateClassProcessor(type).Patch();
        }
        catch (Exception e)
        {
            Hermes.Say($"DeviceOfHermes Patch failed: {e}");
        }
    }

    public static bool IsClashable(BattlePlayingCardDataInUnitModel cardA, BattlePlayingCardDataInUnitModel cardB)
    {
        bool isClashableA = !(cardA.cardAbility is AdvancedCardBase advAbiA && !advAbiA.IsClashable)
            && (cardA.owner?.passiveDetail?.PassiveList?.OfType<AdvancedPassiveBase>()
                .All(passive => passive.IsClashable(cardA) && passive.IsClashable(cardA, cardB)) ?? true);

        bool isClashableB = !(cardB.cardAbility is AdvancedCardBase advAbiB && !advAbiB.IsClashable)
            && (cardB.owner?.passiveDetail?.PassiveList?.OfType<AdvancedPassiveBase>()
                .All(passive => passive.IsClashable(cardB) && passive.IsClashable(cardB, cardA)) ?? true);

        var isClashableC = !(cardB.isKeepedCard && cardA.cardAbility is AdvancedCardBase advAbiC && !advAbiC.IsClashableWithStandby);

        var isClashableD = !(cardA.isKeepedCard && cardB.cardAbility is AdvancedCardBase advAbiD && !advAbiD.IsClashableWithStandby);

        return isClashableA && isClashableB && isClashableC && isClashableD;
    }

    [HarmonyPatch(typeof(BattleUnitTargetArrowManagerUI), "UpdateTargetListData")]
    static class PatchTargetUI
    {
        static void Postfix(List<BattleUnitTargetArrowData> ___TargetListData)
        {
            foreach (var arrow in ___TargetListData)
            {
                var cardA = arrow?.Dice?.CardInDice;
                var cardB = arrow?.TargetDice?.CardInDice;

                if (cardA is null || cardB is null)
                {
                    continue;
                }

                if (!IsClashable(cardA, cardB))
                {
                    arrow?.isPairing = false;
                }
            }
        }
    }

    [HarmonyPatch(typeof(StageController), "ArrangeCardsPhase")]
    class PatchOnStartResolve
    {
        static void Postfix(ref List<BattlePlayingCardDataInUnitModel> ____allCardList)
        {
            foreach (var card in ____allCardList)
            {
                var targetAry = card?.target?.cardSlotDetail?.cardAry;
                var targetSlotOrder = card?.targetSlotOrder ?? -1;

                if (targetAry is null || targetSlotOrder < 0)
                {
                    continue;
                }

                var target = targetAry[targetSlotOrder];

                if (card is null || target is null)
                {
                    continue;
                }

                if (!IsClashable(card, target))
                {
                    card?.targetSlotOrder = -1;
                }
            }
        }
    }

    [HarmonyPatch(typeof(StageController), "StartAction")]
    class PatchOnStartAction
    {
        static bool Prefix(StageController __instance, BattlePlayingCardDataInUnitModel card)
        {
            if (card?.owner is not BattleUnitModel attacker || card?.target is not BattleUnitModel victim)
            {
                return true;
            }

            BattlePlayingCardDataInUnitModel? useCard = null;

            victim.passiveDetail?.PassiveList?.OfType<AdvancedPassiveBase>()?.Foreach(adv => useCard ??= adv.BeforeTakeOneSideAction(attacker, card));
            victim.bufListDetail?.GetActivatedBufList()?.OfType<AdvancedUnitBuf>()?.Foreach(adv => useCard ??= adv.BeforeTakeOneSideAction(attacker));

            victim.faction.AliveUnits.Filter(u => u != victim).Foreach(ally =>
            {
                ally.passiveDetail?.PassiveList?.OfType<AdvancedPassiveBase>()?.Foreach(adv => adv.BeforeAlliesTakeOneSideAction(victim, attacker, card, ref useCard));
                ally.bufListDetail?.GetActivatedBufList()?.OfType<AdvancedUnitBuf>()?.Foreach(adv => adv.BeforeAlliesTakeOneSideAction(victim, attacker, card, ref useCard));
            });

            if (useCard is not null)
            {
                if (!attacker.IsTargetable(victim) || !IsClashable(card, useCard) || card.cardBehaviorQueue.Any(q => q.abilityList.Exists(abi => abi is RevengeDice)))
                {
                    return true;
                }

                useCard.target = attacker;
                card.target = useCard.owner;

                if (useCard.owner.faction == Faction.Player)
                {
                    __instance.StartParryingNoPatch(card, useCard);
                }
                else
                {
                    __instance.StartParryingNoPatch(useCard, card);
                }

                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(StageController), "StartParrying")]
    class PatchOnDynamicParrying
    {
        static bool Prefix(StageController __instance, BattlePlayingCardDataInUnitModel cardA, BattlePlayingCardDataInUnitModel cardB)
        {
            if (cardA is null || cardB is null)
            {
                return true;
            }

            if (!IsClashable(cardA, cardB))
            {
                __instance.StartActionNoPatch(cardA);

                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "CanChangeAttackTarget")]
    class PatchOnChangeTarget
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var inject = AccessTools.Method(typeof(PatchOnChangeTarget), "InjectMethod");

            var matcher = new CodeMatcher(instructions);

            matcher.End()
                .MatchStartBackwards(new CodeMatch(OpCodes.Ret))
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Ldarg_3),
                    new CodeInstruction(OpCodes.Call, inject)
                );

            return matcher.Instructions();
        }

        static bool InjectMethod(bool speedWin, BattleUnitModel? self, BattleUnitModel? target, int myIndex, int targetIndex)
        {
            var selfCard = self?.view?.speedDiceSetterUI?.GetSpeedDiceByIndex(myIndex)?.CardInDice;
            var targetCard = target?.view?.speedDiceSetterUI?.GetSpeedDiceByIndex(targetIndex)?.CardInDice;

            if (speedWin || selfCard is null || targetCard is null)
            {
                return speedWin;
            }

            var selfAbi = selfCard.cardAbility;
            var targetAbi = targetCard.cardAbility;

            if (selfAbi is AdvancedCardBase selfAdv && selfAdv.IsIgnoreSpeedByMatch)
            {
                return true;
            }

            if (targetAbi is AdvancedCardBase targetAdv && targetAdv.IsIgnoreSpeedByMatch)
            {
                return true;
            }

            var selfPassives = self?.passiveDetail?.PassiveList?.OfType<AdvancedPassiveBase>();
            var targetPassives = target?.passiveDetail?.PassiveList?.OfType<AdvancedPassiveBase>();

            if (selfPassives?.Any(p => p.IsIgnoreSpeedByMatch(selfCard, targetCard)) == true)
            {
                return true;
            }

            if (targetPassives?.Any(p => p.IsIgnoreSpeedByMatch(targetCard, selfCard)) == true)
            {
                return true;
            }

            return speedWin;
        }
    }

    [HarmonyPatch(typeof(BattleAllyCardDetail), "DiscardACardByAbility", [typeof(List<BattleDiceCardModel>)])]
    class PatchCanDiscard
    {
        static void Prefix(BattleUnitModel ____self, List<BattleDiceCardModel> cardList)
        {
            List<BattleDiceCardModel> cancel = new();

            foreach (var card in cardList)
            {
                if (card is null)
                {
                    continue;
                }

                if (card.CreateDiceCardSelfAbilityScript() is AdvancedCardBase adv && !adv.CanDiscardByAbility(card))
                {
                    cancel.Add(card);
                }

                foreach (var passive in ____self.passiveDetail.PassiveList.OfType<AdvancedPassiveBase>())
                {
                    if (!passive.CanDiscardByAbility(card))
                    {
                        if (!cancel.Contains(card))
                        {
                            cancel.Add(card);
                        }
                    }
                }
            }

            foreach (var card in cancel)
            {
                cardList.Remove(card);
            }
        }
    }

    static bool IsKeepBehavior(BattleDiceBehavior behavior)
    {
        var abis = behavior.abilityList?.OfType<AdvancedDiceBase>();

        abis?.Foreach(abi => abi.OnAddToKeeped());

        bool F<T>(IEnumerable<T> list, Predicate<T> pred)
        {
            var res = true;

            foreach (var elem in list)
            {
                if (!pred(elem))
                {
                    res = false;
                }
            }

            return res;
        }

        var self = behavior.card?.owner;
        var isKeepsDice = !(abis is null || !abis.All(abi => abi.IsKeeps()));
        var isKeepsCard = !(behavior.card?.cardAbility is AdvancedCardBase card && !card.OnAddKeepDice(behavior));
        var isKeepsPassive = F(self?.passiveDetail?.PassiveList?.OfType<AdvancedPassiveBase>() ?? [], p => p.OnAddKeepDice(behavior));
        var isKeepsUnitBuf = F(self?.bufListDetail?.GetActivatedBufList()?.OfType<AdvancedUnitBuf>() ?? [], p => p.OnAddKeepDice(behavior));

        return isKeepsDice && isKeepsCard && isKeepsPassive && isKeepsUnitBuf;
    }

    [HarmonyPatch]
    class PatchOnAddKeeps
    {
        static IEnumerable<MethodInfo> TargetMethods()
        {
            yield return typeof(BattleKeepedCardDataInUnitModel).Method("AddBehaviours", [typeof(DiceCardXmlInfo), typeof(List<BattleDiceBehavior>)]);
            yield return typeof(BattleKeepedCardDataInUnitModel).Method("AddBehaviours", [typeof(BattleDiceCardModel), typeof(List<BattleDiceBehavior>)]);
        }

        static void Prefix(List<BattleDiceBehavior> behaviourList)
        {
            behaviourList.RemoveAll(b => !IsKeepBehavior(b));
        }
    }

    [HarmonyPatch]
    class PatchOnAddKeep
    {
        static IEnumerable<MethodInfo> TargetMethods()
        {
            yield return typeof(BattleKeepedCardDataInUnitModel).Method("AddBehaviour");
            yield return typeof(BattleKeepedCardDataInUnitModel).Method("AddBehaviourForOnlyDefense");
        }

        static bool Prefix(BattleDiceBehavior behaviour)
        {
            return IsKeepBehavior(behaviour);
        }
    }

    [HarmonyPatch(typeof(BattleParryingManager), "GetDecisionResult")]
    class PatchParryingResult
    {
        static Exception Finalizer(
            Exception __exception,
            ref BattleParryingManager.ParryingDecisionResult __result,
            BattleParryingManager.ParryingTeam teamA,
            BattleParryingManager.ParryingTeam teamB
        )
        {
            var enemyAdvAbility = teamA?.playingCard?.currentBehavior?.abilityList?.OfType<AdvancedDiceBase>();
            var librarianAdvAbility = teamB?.playingCard?.currentBehavior?.abilityList?.OfType<AdvancedDiceBase>();

            var enemyOrigin = ParseTo(__result, Faction.Enemy);
            var librarianOrigin = ParseTo(__result, Faction.Player);
            var enemyResult = enemyOrigin;
            var librarianResult = librarianOrigin;

            if (enemyAdvAbility is not null)
            {
                foreach (var eaa in enemyAdvAbility)
                {
                    enemyResult = eaa.GetParryingResult(enemyOrigin);
                }
            }

            if (librarianAdvAbility is not null)
            {
                foreach (var laa in librarianAdvAbility)
                {
                    librarianResult = laa.GetParryingResult(librarianOrigin);
                }
            }

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
    class PatchDiceResultValue
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

    [HarmonyPatch(typeof(BattleUnitModel), "TakeDamage")]
    class PatchDiceDamageValue
    {
        static void Prefix(BattleUnitModel __instance, ref int v, DamageType type, BattleUnitModel attacker)
        {
            if (attacker is not null && type is DamageType.Attack)
            {
                if (attacker.currentDiceAction?.cardAbility is AdvancedCardBase advCard)
                {
                    v = advCard.GetFinalResultDamageValue(v);
                }

                var abilities = attacker.currentDiceAction?.currentBehavior?.abilityList?.OfType<AdvancedDiceBase>();

                if (abilities is not null)
                {
                    var res = v;

                    foreach (var abi in abilities)
                    {
                        res = abi.GetFinalResultDamageValue(res);
                    }

                    v = res;
                }
            }

            var line = __instance.passiveDetail?.PassiveList?.OfType<AdvancedPassiveBase>()?.Max(p => p.HealthStopperLine);

            if (line is not null && line >= __instance.hp - v)
            {
                var redu = line - (int)(__instance.hp - v);

                v -= redu.Value;
            }
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchEndForward(CodeMatch.IsLdarg(0), CodeMatch.IsLdloc(), CodeMatch.Calls(typeof(BattleUnitModel).Method("OnLoseHp")))
                .Advance(1)
                .Insert(CodeInstruction.Instance, CodeInstruction.Ldloc(1), CodeInstruction.Ldarg(2), CodeInstruction.Ldarg(4), CodeInstruction.Call(typeof(PatchDiceDamageValue).Method("InjectMethod")));

            return matcher.Instructions();
        }

        static void InjectMethod(BattleUnitModel __instance, int dmg, DamageType dty, KeywordBuf buf)
        {
            if (dty is DamageType.Buf && buf is not KeywordBuf.None)
            {
                __instance.passiveDetail?.PassiveList?.OfType<AdvancedPassiveBase>()?.Foreach(i => i.OnLoseHpByKeyword(dmg, buf));
                __instance.bufListDetail?.GetActivatedBufList()?.OfType<AdvancedUnitBuf>()?.Foreach(i => i.OnLoseHpByKeyword(dmg, buf));
            }
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "OnRoundStart_ignoreDead")]
    static class PatchOnRoundStartFirst
    {
        static void Prefix(BattleUnitModel __instance)
        {
            foreach (var passive in __instance.passiveDetail.PassiveList.OfType<AdvancedPassiveBase>())
            {
                passive.OnRoundStartFirst();
            }

            foreach (var buf in __instance.bufListDetail.GetActivatedBufList().OfType<AdvancedUnitBuf>())
            {
                buf.OnRoundStartFirst();
            }
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "OnRoundStart_after")]
    static class PatchOnRoundStartLast
    {
        static Exception Finalizer(Exception __exception, BattleUnitModel __instance)
        {
            foreach (var passive in __instance.passiveDetail.PassiveList.OfType<AdvancedPassiveBase>())
            {
                passive.OnRoundStartLast();
            }

            foreach (var buf in __instance.bufListDetail.GetActivatedBufList().OfType<AdvancedUnitBuf>())
            {
                buf.OnRoundStartLast();
            }

            return __exception;
        }
    }

    [HarmonyPatch]
    static class PatchOnBattleLast
    {
        static IEnumerable<MethodInfo> TargetMethods()
        {
            yield return typeof(StageController).Method("SetCurrentDiceActionPhase");
            yield return typeof(StageController).Method("MoveUnitPhase");
            yield return typeof(StageController).Method("CheckFarAreaPlayPhase");
            yield return typeof(StageController).Method("WaitUnitArrivePhase");
            yield return typeof(StageController).Method("ActivateStartBattleEffectPhase");
        }

        static Exception Finalizer(Exception __exception, ref StageController.StagePhase ____phase)
        {
            if (____phase == StageController.StagePhase.RoundEndPhase)
            {
                var all = BattleObjectManager.instance.GetAliveList(false)
                    .Map(unit => unit.passiveDetail.PassiveList.OfType<AdvancedPassiveBase>()).Flatten();

                foreach (var passive in all)
                {
                    passive.OnPreRoundEnd();
                }

                if (StageController.Instance.GetAllCards().Count > 0)
                {
                    ____phase = StageController.StagePhase.SetCurrentDiceAction;
                }
                else
                {
                    foreach (var passive in all)
                    {
                        if (!passive.IsAllowRoundEnd())
                        {
                            ____phase = StageController.StagePhase.SetCurrentDiceAction;
                        }
                    }
                }
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(AdvancedUnitBuf), "Init")]
    static class OriginalAdvInit
    {
        [HarmonyReversePatch]
        internal static void Init(AdvancedUnitBuf instance, BattleUnitModel owner) =>
            throw new NotImplementedException();
    }

    [HarmonyPatch(typeof(BattleUnitBufListDetail), "AddBuf")]
    static class PatchAddBufInitializer
    {
        static bool Prefix(BattleUnitBufListDetail __instance, BattleUnitBuf buf, BattleUnitModel ____self)
        {
            if (!__instance.CanAddBuf(buf))
            {
                return false;
            }

            if (buf is AdvancedUnitBuf adv && adv.IsInstant)
            {
                adv.Init(____self);
                adv.OnInstant();

                foreach (var unit in BattleObjectManager.instance.GetAliveList())
                {
                    var otherBufs = unit?.bufListDetail?.GetActivatedBufList()?.OfType<AdvancedUnitBuf>();

                    if (otherBufs is null)
                    {
                        continue;
                    }

                    foreach (var otherBuf in otherBufs)
                    {
                        otherBuf.OnOtherInstant(adv);
                    }
                }

                return false;
            }

            return true;
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var inject = AccessTools.Method(typeof(PatchAddBufInitializer), "Init");
            var target = AccessTools.Method(typeof(BattleUnitBuf), "Init");

            var _self = AccessTools.Field(typeof(BattleUnitBufListDetail), "_self");

            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(CodeMatch.Calls(target))
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, _self),
                    new CodeInstruction(OpCodes.Call, inject)
                );

            return matcher.Instructions();
        }

        static void Init(BattleUnitBuf instance, BattleUnitModel owner)
        {
            if (instance is AdvancedUnitBuf advInstance)
            {
                OriginalAdvInit.Init(advInstance, owner);
            }
        }
    }

    [HarmonyPatch(typeof(BattleUnitBufListDetail), "AddBufWithoutDuplication")]
    static class PatchAddBufWdInitializer
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var inject = AccessTools.Method(typeof(PatchAddBufWdInitializer), "Init");
            var target = AccessTools.Method(typeof(BattleUnitBuf), "Init");

            var _self = AccessTools.Field(typeof(BattleUnitBufListDetail), "_self");

            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(CodeMatch.Calls(target))
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, _self),
                    new CodeInstruction(OpCodes.Call, inject)
                );

            return matcher.Instructions();
        }

        static void Init(BattleUnitBuf instance, BattleUnitModel owner)
        {
            if (instance is AdvancedUnitBuf advInstance)
            {
                OriginalAdvInit.Init(advInstance, owner);
            }
        }
    }

    [HarmonyPatch(typeof(StageController), "RemoveUnusedCards")]
    class PatchUnusedRemove
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var target = typeof(List<BattlePlayingCardDataInUnitModel>).Method("Remove");
            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(
                CodeMatch.IsLdarg(),
                CodeMatch.IsLdfld(),
                CodeMatch.IsLdloc(),
                CodeMatch.Calls(target),
                new CodeMatch(i => i.opcode == OpCodes.Pop)
            )
                .Advance(1)
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Call, typeof(PatchUnusedRemove).Method("InjectMethod"))
                );

            return matcher.Instructions();
        }

        static void InjectMethod(BattlePlayingCardDataInUnitModel playcard)
        {
            var passives = playcard.owner?.passiveDetail?.PassiveList?.OfType<AdvancedPassiveBase>();

            if (passives is not null)
            {
                foreach (var p in passives)
                {
                    p.OnDropCard(playcard);
                }
            }
        }
    }

    [HarmonyPatch(typeof(StageController), "DrawCardPhase")]
    class PatchOnDrawCardPhase
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var target = typeof(BattleUnitEmotionDetail).Method("DrawCardAdder");
            var matcher = new CodeMatcher(instructions);

            matcher.MatchEndForward(
                CodeMatch.Calls(target),
                new CodeMatch(i => i.opcode == OpCodes.Add),
                CodeMatch.IsStloc()
            )
                .Advance(1)
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(OpCodes.Call, typeof(PatchOnDrawCardPhase).Method("InjectMethod")),
                    new CodeInstruction(OpCodes.Add),
                    new CodeInstruction(OpCodes.Stloc_3)
                );

            return matcher.Instructions();
        }

        static int InjectMethod(BattleUnitModel owner)
        {
            var res = 0;
            var passives = owner.passiveDetail.PassiveList.OfType<AdvancedPassiveBase>();

            foreach (var p in passives)
            {
                res += p.DrawCardAddr;
            }

            return res;
        }
    }

    class Clickable : MonoBehaviour, IPointerClickHandler
    {
        public void SetBuf(AdvancedUnitBuf buf)
        {
            _buf = buf;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    _buf?.OnClick(AdvancedUnitBuf.ClickType.Left);

                    break;
                case PointerEventData.InputButton.Right:
                    _buf?.OnClick(AdvancedUnitBuf.ClickType.Right);

                    break;
                case PointerEventData.InputButton.Middle:
                    _buf?.OnClick(AdvancedUnitBuf.ClickType.Middle);

                    break;
            }
        }

        private AdvancedUnitBuf? _buf;
    }

    [HarmonyPatch(typeof(BattleUnitBottomStatUI), "SetBufs")]
    class PatchOnSetBuf
    {
        static void Postfix(BattleBufUIDataList bufDataList, Image[] ___bufIcons)
        {
            var bufs = bufDataList.bufActivatedList.Concat(bufDataList.bufReadyList);

            foreach (var (i, bufIcon) in ___bufIcons.Enumerate())
            {
                var clickable = bufIcon.gameObject.GetComponent<Clickable>();

                clickable ??= bufIcon.gameObject.AddComponent<Clickable>();

                if (bufs.ElementAtOrDefault(i) is BattleBufUIData data && data.buf is AdvancedUnitBuf advBuf)
                {
                    clickable.SetBuf(advBuf);
                }
            }
        }
    }

    [HarmonyPatch(typeof(BattleUnitInteractor), "OnPointerClick")]
    class PatchOnUnitClick
    {
        static void Prefix(BaseEventData data, BattleUnitView ____view)
        {
            var passives = ____view?.model?.passiveDetail?.PassiveList?.OfType<AdvancedPassiveBase>();

            switch (UIControlManager.GetInpuTypeOf(data))
            {
                case InputType.LeftClick:
                    passives?.Foreach(p => p.OnClickUnit(AdvancedPassiveBase.ClickType.Left));

                    break;
                case InputType.RightClick:
                    passives?.Foreach(p => p.OnClickUnit(AdvancedPassiveBase.ClickType.Right));

                    break;
                case InputType.WheelClick:
                    passives?.Foreach(p => p.OnClickUnit(AdvancedPassiveBase.ClickType.Middle));

                    break;
            }
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "OnStartBattle")]
    class PatchOnStartBattle
    {
        static Exception Finalizer(BattleUnitModel __instance, Exception __exception)
        {
            __instance?.bufListDetail?.GetActivatedBufList()?.OfType<AdvancedUnitBuf>()?.Foreach(buf =>
            {
                buf.OnStartBattle();
            });

            StageController.Instance.GetAllCards()
                .Filter(card => card.owner == __instance)?
                .FlatMap(card => card.card.GetBufList())?
                .OfType<AdvancedCardBuf>()?
                .Foreach(buf =>
                {
                    buf.OnStartBattle();
                });

            __instance?.allyCardDetail?.Hand()?.FlatMap(card => card.GetBufList())?.OfType<AdvancedCardBuf>()?.Foreach(buf =>
            {
                buf.OnStartBattle_inHand();
            });

            __instance?.allyCardDetail?.GetAllDeck()?.FlatMap(card => card.GetBufList())?.OfType<AdvancedCardBuf>()?.Foreach(buf =>
            {
                buf.OnStartBattle_inDeck();
            });

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "OnAddKeywordBufByCard")]
    class PatchOnAddKeywordBuf
    {
        static Exception Finalizer(Exception __exception, BattleUnitModel __instance, ref int __result, BattleUnitBuf buf, int stack)
        {
            var num = __instance?.bufListDetail?.GetActivatedBufList()?.OfType<AdvancedUnitBuf>()?.Sum(b => b.OnAddKeywordBufByCard(buf, stack)) ?? 0;

            __result += num;

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "GetBreakDamageReductionAll")]
    class PatchGetBreakDmgRedAll
    {
        static Exception Finalizer(Exception __exception, BattleUnitModel __instance, ref int __result, int dmg, DamageType dmgType, BattleUnitModel attacker)
        {
            var num = __instance?.bufListDetail?.GetActivatedBufList()?.OfType<AdvancedUnitBuf>()?.Sum(b => b.GetBreakDamageReductionAll(dmg, dmgType, attacker)) ?? 0;

            __result += num;

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleAllyCardDetail), "PlayTurnAutoForEnemy")]
    class PatchPlayForAuto
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchEndForward(CodeMatch.IsLdarg(0), CodeMatch.IsLdfld(), new CodeMatch(i => i.opcode == OpCodes.Newobj))
                .Advance(1)
                .Insert(
                    CodeInstruction.Ldarg(1),
                    CodeInstruction.Ldarg(2),
                    CodeInstruction.Call(typeof(PatchPlayForAuto).Method("InjectMethod"))
                );

            return matcher.Instructions();
        }

        static List<BattleDiceCardModel> InjectMethod(List<BattleDiceCardModel> list, int slot, int speed)
        {
            foreach (var card in list)
            {
                if (card.CreateDiceCardSelfAbilityScript() is AdvancedCardBase adv)
                {
                    var adder = adv.SpecialPriorityAdder(slot, speed);

                    card.SetPriorityAdder(card.GetPriorityAdder() + adder);

                    changed.Add(card);
                }
            }

            return list;
        }

        static Exception Finalizer(Exception __exception, int currentDiceSlotIdx, int speed)
        {
            foreach (var card in changed)
            {
                if (card.CreateDiceCardSelfAbilityScript() is AdvancedCardBase adv)
                {
                    var adder = adv.SpecialPriorityAdder(currentDiceSlotIdx, speed);

                    card.SetPriorityAdder(card.GetPriorityAdder() - adder);
                }
            }

            changed.Clear();

            return __exception;
        }

        static List<BattleDiceCardModel> changed = new();
    }

    [HarmonyPatch(typeof(BattleUnitModel), "IsCardChoosable")]
    class PatchOnChooseCard
    {
        static Exception Finalizer(Exception __exception, BattleUnitModel __instance, ref bool __result, BattleDiceCardModel card)
        {
            foreach (var advPassive in __instance.passiveDetail.PassiveList.OfType<AdvancedPassiveBase>())
            {
                if (!advPassive.OnChooseCard(card))
                {
                    __result = false;
                }
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleUnitPassiveDetail), "OnWaveStart")]
    class PatchOnWaveStart
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);
            var passiveList = typeof(BattleUnitPassiveDetail).Field("_passiveList");

            matcher.MatchStartForward(CodeMatch.IsLdarg(0), CodeMatch.IsLdfld(passiveList))
                .Insert(
                    CodeInstruction.Instance,
                    CodeInstruction.Ldfld(passiveList),
                    CodeInstruction.Call(typeof(PatchOnWaveStart).Method("InjectBefore"))
                )
                .MatchStartForward(CodeMatch.IsOpCode(OpCodes.Leave))
                .MatchStartForward(CodeMatch.IsOpCode(OpCodes.Leave))
                .Insert(
                    CodeInstruction.Instance.MoveLabelsFrom(matcher.Instruction),
                    CodeInstruction.Ldfld(passiveList),
                    CodeInstruction.Call(typeof(PatchOnWaveStart).Method("InjectAfter"))
                );

            return matcher.Instructions();
        }

        static void InjectBefore(List<PassiveAbilityBase> _passiveList)
        {
            foreach (var p in _passiveList.OfType<AdvancedPassiveBase>())
            {
                if (p.isActiavted)
                {
                    p.OnWaveStartBefore();
                }
            }
        }

        static void InjectAfter(List<PassiveAbilityBase> _passiveList)
        {
            foreach (var p in _passiveList.OfType<AdvancedPassiveBase>())
            {
                if (p.isActiavted)
                {
                    p.OnWaveStartAfter();
                }
            }
        }
    }

    [HarmonyPatch(typeof(BattleOneSidePlayManager), "StartOneSidePlay")]
    class PatchStartOneSidePlay
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.Start()
                .Insert(
                    new CodeInstruction(OpCodes.Ldarga, 1),
                    CodeInstruction.Call(typeof(PatchStartOneSidePlay).Method("InjectMethod"))
                );

            return matcher.Instructions();
        }

        static void InjectMethod(ref BattlePlayingCardDataInUnitModel card)
        {
            var origin = card;

            if (card?.cardAbility is AdvancedCardBase adv)
            {
                adv.BeforeUseCard(ref card);

                if (card != origin)
                {
                    StageController.Instance.GetAllCards().RemoveAll(c => c == origin);
                }
            }
        }
    }

    [HarmonyPatch(typeof(BattleParryingManager), "StartParrying")]
    class PatchStartParrying
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.Start()
                .Insert(
                    new CodeInstruction(OpCodes.Ldarga, 1),
                    new CodeInstruction(OpCodes.Ldarga, 2),
                    CodeInstruction.Call(typeof(PatchStartParrying).Method("InjectMethod"))
                );

            return matcher.Instructions();
        }

        static void InjectMethod(ref BattlePlayingCardDataInUnitModel cardA, ref BattlePlayingCardDataInUnitModel cardB)
        {
            var origin = cardA;

            if (cardA?.cardAbility is AdvancedCardBase advA)
            {
                advA.BeforeUseCard(ref cardA);

                if (cardA != origin)
                {
                    StageController.Instance.GetAllCards().RemoveAll(c => c == origin);
                }
            }

            origin = cardB;

            if (cardB?.cardAbility is AdvancedCardBase advB)
            {
                advB.BeforeUseCard(ref cardB);

                if (cardB != origin)
                {
                    StageController.Instance.GetAllCards().RemoveAll(c => c == origin);
                }
            }
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "Die")]
    class PatchDie
    {
        static bool Prefix(BattleUnitModel __instance)
        {
            var insteadOfExtinct = false;

            foreach (var p in __instance.passiveDetail.PassiveList.OfType<AdvancedPassiveBase>())
            {
                if (p.IsDieInsteadOfExtinction && !insteadOfExtinct)
                {
                    insteadOfExtinct = true;
                }
            }

            if (insteadOfExtinct)
            {
                __instance.Extinct(true);

                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "Extinct")]
    class PatchExtinct
    {
        static Exception Finalizer(Exception __exception, BattleUnitModel __instance, bool b)
        {
            if (b)
            {
                __instance.passiveDetail.PassiveList.OfType<AdvancedPassiveBase>().Foreach(p => p.OnExtinct());
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "OnRecoverHp")]
    class PatchOnRecoverHp
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchEndForward(
                CodeMatch.IsLdarg(0),
                CodeMatch.IsLdfld(typeof(BattleUnitModel).Field("passiveDetail")),
                CodeMatch.IsLdarg(1),
                CodeMatch.Calls(typeof(BattleUnitPassiveDetail).Method("OnRecoverHp"))
            )
                .Advance(1)
                .Insert(CodeInstruction.Instance, CodeInstruction.Ldarg(1), CodeInstruction.Call(typeof(PatchOnRecoverHp).Method("InjectMethod")));

            return matcher.Instructions();
        }

        static void InjectMethod(BattleUnitModel __instance, int amount)
        {
            foreach (var buf in __instance.bufListDetail.GetActivatedBufList().OfType<AdvancedUnitBuf>())
            {
                buf.OnRecoverHp(amount);
            }
        }
    }

    [HarmonyPatch]
    class PatchOnRecoverPP
    {
        static IEnumerable<MethodInfo> TargetMethods()
        {
            yield return typeof(BattlePlayingCardSlotDetail).Method("RecoverPlayPoint");
            yield return typeof(BattlePlayingCardSlotDetail).Method("RecoverPlayPointByCard");
        }

        static Exception Finalizer(Exception __exception, BattlePlayingCardSlotDetail __instance, int value)
        {
            _selfRef(__instance)?.passiveDetail?.PassiveList?.OfType<AdvancedPassiveBase>()?.Foreach(p => p.OnRecoverPP(value));
            _selfRef(__instance)?.bufListDetail?.GetActivatedBufList()?.OfType<AdvancedUnitBuf>()?.Foreach(b => b.OnRecoverPP(value));

            return __exception;
        }

        static AccessTools.FieldRef<BattlePlayingCardSlotDetail, BattleUnitModel> _selfRef
            = typeof(BattlePlayingCardSlotDetail).FieldRefAccess<BattleUnitModel>("_self");
    }

    [HarmonyPatch(typeof(BattleDiceCardBuf), "GetBufIcon")]
    class PatchGetCardBufIcon
    {
        static Exception Finalizer(Exception __exception, ref Sprite __result, BattleDiceCardBuf __instance)
        {
            if (__result is null && __instance is AdvancedCardBuf)
            {
                new BattleUnitBuf().GetBufIcon();

                if (BattleUnitBuf._bufIconDictionary is Dictionary<string, Sprite> dict)
                {
                    dict.TryGetValue((string)typeof(BattleDiceCardBuf).Property("keywordIconId").GetValue(__instance), out __result);
                }
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattlePlayingCardDataInUnitModel), "BeforeRollDice")]
    class PatchBeforeRollDice
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(CodeMatch.IsOpCode(OpCodes.Leave))
                .Insert(
                    CodeInstruction.Instance,
                    CodeInstruction.Ldarg(1),
                    CodeInstruction.Call(typeof(PatchBeforeRollDice).Method("InjectMethod"))
                );

            return matcher.Instructions();
        }

        static void InjectMethod(BattlePlayingCardDataInUnitModel __instance, BattleDiceBehavior behavior)
        {
            __instance?.card?.GetBufList()?.OfType<AdvancedCardBuf>()?.Foreach(buf => buf.BeforeRollDice(behavior));
        }
    }

    [HarmonyPatch(typeof(BattlePlayingCardDataInUnitModel), "OnSucceedAttack")]
    class PatchOnSucceedAttack
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(CodeMatch.IsOpCode(OpCodes.Leave))
                .Insert(
                    CodeInstruction.Instance,
                    CodeInstruction.Ldarg(1),
                    CodeInstruction.Call(typeof(PatchOnSucceedAttack).Method("InjectMethod"))
                );

            return matcher.Instructions();
        }

        static void InjectMethod(BattlePlayingCardDataInUnitModel __instance, BattleDiceBehavior behavior)
        {
            __instance?.card?.GetBufList()?.OfType<AdvancedCardBuf>()?.Foreach(buf => buf.OnSuccessAttack(behavior));

            __instance?.owner?.allyCardDetail?.Hand()?.FlatMap(card => card.GetBufList().OfType<AdvancedCardBuf>())?.Foreach(buf => buf.OnSuccessAttack_inHand(behavior));

            __instance?.owner?.allyCardDetail?.GetAllDeck()?.FlatMap(card => card.GetBufList().OfType<AdvancedCardBuf>())?.Foreach(buf => buf.OnSuccessAttack_inDeck(behavior));
        }
    }

    [HarmonyPatch(typeof(BattleUnitBufListDetail), "AddNewKeywordBufInList")]
    class PatchOnAddNewKeywordBuf
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.End()
                .MatchEndBackwards(CodeMatch.Calls(typeof(BattleUnitBuf).Method("Init")))
                .Advance(1)
                .Insert(
                    CodeInstruction.Ldloc(2),
                    CodeInstruction.Instance,
                    CodeInstruction.Ldfld(typeof(BattleUnitBufListDetail).Field("_self")),
                    CodeInstruction.Call(typeof(PatchOnAddNewKeywordBuf).Method("InjectMethod"))
                );

            return matcher.Instructions();
        }

        static void InjectMethod(BattleUnitBuf target, BattleUnitModel self)
        {
            if (target is AdvancedUnitBuf adv)
            {
                OriginalAdvInit.Init(adv, self);
            }
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "OnSucceedAreaAttack")]
    class PatchOnSucceedAreaAttack
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(CodeMatch.IsOpCode(OpCodes.Leave))
                .Insert(
                    CodeInstruction.Instance,
                    CodeInstruction.Ldarg(1),
                    CodeInstruction.Ldarg(2),
                    CodeInstruction.Call(typeof(PatchOnSucceedAreaAttack).Method("InjectMethod"))
                );

            return matcher.Instructions();
        }

        static void InjectMethod(BattleUnitModel __instance, BattleDiceBehavior behavior, BattleUnitModel target)
        {
            __instance.bufListDetail.GetActivatedBufList().OfType<AdvancedUnitBuf>().Filter(buf => !buf.IsDestroyed())
                .Foreach(buf => buf.OnSuccessAreaAttack(behavior, target));
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "OnWaveStart")]
    class PatchOnWaveStartInModel
    {
        static void Prefix(BattleUnitModel __instance)
        {
            var dataModel = __instance.UnitData;

            if (dataModel is not null)
            {
                var savedPassives = _saveOnReceptionPassive.GetValue(dataModel, _ => new());
                var passives = __instance.passiveDetail?.PassiveList ?? [];

                foreach (ref var passive in passives.AsRef())
                {
                    var id = passive.id;
                    var dup = savedPassives.Find(p => id == p.id && id is not null);

                    if (dup is not null)
                    {
                        passive = dup;
                    }
                }

                var savedBufs = _saveOnReceptionUnitbuf.GetValue(dataModel, _ => new());

                __instance.bufListDetail?.GetActivatedBufList()?.AddRange(savedBufs);
            }

            foreach (var deckCard in __instance.allyCardDetail.Deck().AsDefer(out var self))
            {
                if (deckCard.CreateDiceCardSelfAbilityScript() is AdvancedCardBase adv && adv.DrawsOnWaveStart)
                {
                    __instance.allyCardDetail.Hand().Add(deckCard);
                    self.Remove(deckCard);
                }
            }
        }
    }

    [HarmonyPatch(typeof(BattleUnitBufListDetail), "ModifyStack")]
    class PatchOnModifiedStack
    {
        static Exception Finalizer(Exception __exception, BattleUnitModel ____self, BattleUnitBuf buf, int stack, ref int __result)
        {
            if (stack > 0)
            {
                var num = 0;

                num += ____self.passiveDetail?.PassiveList?.OfType<AdvancedPassiveBase>()?.Sum(p => p.OnAddKeywordBuf(buf, stack)) ?? 0;

                num += ____self.bufListDetail?.GetActivatedBufList()?
                    .Filter(buf => !buf.IsDestroyed())
                    .OfType<AdvancedUnitBuf>()?
                    .Sum(p => p.OnAddKeywordBuf(buf, stack)) ?? 0;

                __result += num;
            }

            return __exception;
        }
    }

    [HarmonyPatch]
    class PatchOnAddKeywordBufs
    {
        static IEnumerable<MethodInfo> TargetMethods()
        {
            yield return typeof(BattleUnitBufListDetail).Method("AddKeywordBufThisRoundByEtc");
            yield return typeof(BattleUnitBufListDetail).Method("AddKeywordBufByEtc");
            yield return typeof(BattleUnitBufListDetail).Method("AddKeywordBufThisRoundByCard");
            yield return typeof(BattleUnitBufListDetail).Method("AddKeywordBufByCard");
            yield return typeof(BattleUnitBufListDetail).Method("AddKeywordBufNextNextByCard");
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchEndForward(CodeMatch.Calls(typeof(BattleUnitBufListDetail).Method("AddNewKeywordBufInList")), CodeMatch.IsStloc())
                .Advance(1)
                .Insert(
                    CodeInstruction.Instance,
                    CodeInstruction.Instance,
                    CodeInstruction.Ldfld(typeof(BattleUnitBufListDetail).Field("_self")),
                    new CodeInstruction(OpCodes.Ldloca, 0),
                    CodeInstruction.Ldarg(3),
                    CodeInstruction.Call(typeof(PatchOnAddKeywordBufs).Method("InjectMethod"))
                );

            return matcher.Instructions();
        }

        static void InjectMethod(BattleUnitBufListDetail __instance, BattleUnitModel _self, ref BattleUnitBuf? added, BattleUnitModel actor)
        {
            if (added is AdvancedUnitBuf adv)
            {
                if (actor is not null && actor != _self)
                {
                    adv.LastInflictBy = actor;
                }

                if (adv.IsInstant)
                {
                    adv.OnInstant();

                    foreach (var unit in BattleObjectManager.instance.GetAliveList())
                    {
                        var otherBufs = unit?.bufListDetail?.GetActivatedBufList()?.OfType<AdvancedUnitBuf>();

                        if (otherBufs is null)
                        {
                            continue;
                        }

                        foreach (var otherBuf in otherBufs)
                        {
                            otherBuf.OnOtherInstant(adv);
                        }
                    }

                    __instance.GetActivatedBufList().Remove(added);
                    __instance.GetReadyBufList().Remove(added);
                    __instance.GetReadyReadyBufList().Remove(added);

                    added = null;
                }
            }
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "OnEndParrying")]
    class PatchOnEndParrying
    {
        static void Prefix(BattleUnitModel __instance, out BattlePlayingCardDataInUnitModel? __state)
        {
            __state = __instance.currentDiceAction;
        }

        static Exception Finalizer(Exception __exception, BattleUnitModel __instance, BattlePlayingCardDataInUnitModel? __state)
        {
            var target = __state?.target;

            __instance.passiveDetail?.PassiveList?.OfType<AdvancedPassiveBase>()?.Foreach(i => i.OnEndEncounter(target, true, true));
            __instance.bufListDetail?.GetActivatedBufList()?.OfType<AdvancedUnitBuf>()?.Foreach(i => i.OnEndEncounter(target, true, true));

            __state?.card?.GetBufList()?.OfType<AdvancedCardBuf>()?.Foreach(i => i.OnEndEncounter(target, true, true));

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "OnEndOneSideAction")]
    class PatchOnEndOneSideAction
    {
        static void Prefix(BattleUnitModel __instance, out BattlePlayingCardDataInUnitModel? __state)
        {
            __state = __instance.currentDiceAction;
        }

        static Exception Finalizer(Exception __exception, BattleUnitModel __instance, bool wasAttacker, BattlePlayingCardDataInUnitModel? __state)
        {
            var target = __state?.target;

            __instance.passiveDetail?.PassiveList?.OfType<AdvancedPassiveBase>()?.Foreach(i => i.OnEndEncounter(target, false, wasAttacker));
            __instance.bufListDetail?.GetActivatedBufList()?.OfType<AdvancedUnitBuf>()?.Foreach(i => i.OnEndEncounter(target, false, wasAttacker));

            __state?.card?.GetBufList()?.OfType<AdvancedCardBuf>()?.Foreach(i => i.OnEndEncounter(target, false, wasAttacker));

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleDiceCardModel), "IsTargetableAllUnit")]
    class PatchIsTargetableAllUnit
    {
        static Exception Finalizer(Exception __exception, BattleDiceCardModel __instance, ref bool __result)
        {
            __result = __result || __instance.GetBufList()?.OfType<AdvancedCardBuf>()?.Any(b => b.IsTargetableAllUnit) == true;

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleDiceCardModel), "CreatePlayingCard")]
    class PatchOnCreatePlayingCard
    {
        static Exception Finalizer(Exception __exception, BattleDiceCardModel __result)
        {
            var script = __result.CreateDiceCardSelfAbilityScript();
            var bufList = __result.GetBufList();

            if (script is AdvancedCardBase adv)
            {
                foreach (var buf in adv.AutoSetCardBufs)
                {
                    if (bufList.Exists(b => b.GetType() == buf.GetType()) || Activator.CreateInstance(buf) is not BattleDiceCardBuf cb)
                    {
                        continue;
                    }

                    __result.AddBuf(cb);
                }
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BookModel), "GetSpeedDiceRule")]
    class PatchOnCreateSpeedDiceRule
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchEndForward(CodeMatch.IsLdarg(0), CodeMatch.Calls(typeof(BookModel).Method("get_SpeedMin")))
                .Advance(1)
                .Insert(CodeInstruction.Ldarg(1), CodeInstruction.Call(typeof(PatchOnCreateSpeedDiceRule).Method("InjectMin")));

            matcher.MatchEndForward(CodeMatch.IsLdarg(0), CodeMatch.Calls(typeof(BookModel).Method("get_SpeedMax")))
                .Advance(1)
                .Insert(CodeInstruction.Ldarg(1), CodeInstruction.Call(typeof(PatchOnCreateSpeedDiceRule).Method("InjectMax")));

            return matcher.Instructions();
        }

        static int InjectMin(int origin, BattleUnitModel unit)
        {
            var num = 0;

            foreach (var p in unit.passiveDetail?.PassiveList?.OfType<AdvancedPassiveBase>() ?? [])
            {
                num += p.GetSpeedDiceMinAdder();
            }

            foreach (var p in unit.bufListDetail?.GetActivatedBufList()?.OfType<AdvancedUnitBuf>() ?? [])
            {
                num += p.GetSpeedDiceMinAdder();
            }

            return 1.Max(origin + num);
        }

        static int InjectMax(int origin, BattleUnitModel unit)
        {
            var num = 0;

            foreach (var p in unit.passiveDetail?.PassiveList?.OfType<AdvancedPassiveBase>() ?? [])
            {
                num += p.GetSpeedDiceMaxAdder();
            }

            foreach (var p in unit.bufListDetail?.GetActivatedBufList()?.OfType<AdvancedUnitBuf>() ?? [])
            {
                num += p.GetSpeedDiceMaxAdder();
            }

            return origin + num;
        }
    }

    [HarmonyPatch(typeof(StageController), "EndBattle")]
    class PatchOnEndBattle
    {
        static void Prefix()
        {
            var list = BattleObjectManager.instance.GetList();

            foreach (var unit in list)
            {
                var dataModel = unit.UnitData;

                var passives = unit.passiveDetail.PassiveList.OfType<AdvancedPassiveBase>().Filter(i => i.IsSaveWhileReception).Collect();
                var bufs = unit.bufListDetail.GetActivatedBufList().OfType<AdvancedUnitBuf>().Filter(i => i.IsSaveWhileReception).Collect();

                _saveOnReceptionPassive.GetValue(dataModel, _ => new()).Also(list =>
                {
                    list.Clear();
                    list.AddRange(passives);
                });

                _saveOnReceptionUnitbuf.GetValue(dataModel, _ => new()).Also(list =>
                {
                    list.Clear();
                    list.AddRange(bufs);
                });
            }
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "RecoverHP")]
    class PatchOnRecoverHP
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(
                CodeMatch.IsLdarg(0),
                CodeMatch.IsLdarg(1),
                CodeMatch.Calls(typeof(BattleUnitModel).Method("CanRecoverHp", [typeof(int)]))
            )
                .Insert(CodeInstruction.Instance.MoveLabelsFrom(matcher.Instruction), CodeInstruction.Ldarga(1), CodeInstruction.Call(typeof(PatchOnRecoverHP).Method("InjectMethod")));

            return matcher.Instructions();
        }

        static void InjectMethod(BattleUnitModel __instance, ref int v)
        {
            var origin = v;
            var adder = 0;
            var passives = __instance.passiveDetail?.PassiveList?.OfType<AdvancedPassiveBase>() ?? [];
            var bufs = __instance.bufListDetail?.GetActivatedBufList()?.OfType<AdvancedUnitBuf>() ?? [];

            adder += passives.Sum(i => i.GetHealingAdder(origin));
            adder += bufs.Sum(i => i.GetHealingAdder(origin));

            var isNullify = false;

            foreach (var p in passives)
            {
                if (p.IsNullifyHealing(v))
                {
                    isNullify = true;
                }
            }

            foreach (var b in bufs)
            {
                if (b.IsNullifyHealing(v))
                {
                    isNullify = true;
                }
            }

            if (isNullify)
            {
                v = 0;
            }
        }
    }

    [HarmonyPatch(typeof(BattlePlayingCardDataInUnitModel), "OnUseCard")]
    class PatchOnUseCard
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(
                CodeMatch.IsLdarg(0),
                CodeMatch.IsLdfld(typeof(BattlePlayingCardDataInUnitModel).Field("owner")),
                CodeMatch.IsLdfld(typeof(BattleUnitModel).Field("battleCardResultLog"))
            )
                .Insert(CodeInstruction.Instance.MoveLabelsFrom(matcher.Instruction), CodeInstruction.Call(typeof(PatchOnUseCard).Method("InjectMethod")));

            return matcher.Instructions();
        }

        static void InjectMethod(BattlePlayingCardDataInUnitModel __instance)
        {
            __instance?.cardBehaviorQueue?.FlatMap(d => d.abilityList.OfType<AdvancedDiceBase>())?.Foreach(abi => abi.OnUseCard(__instance));
        }
    }

    [HarmonyPatch(typeof(BattleDiceBehavior), "OnEventDiceAbility")]
    class PatchOnEventDiceAbility
    {
        static void Prefix(BattleDiceBehavior __instance, DiceCardAbilityBase.DiceCardPassiveType dicePassiveType, ref CardRange? __state)
        {
            if (__instance.card?.cardAbility is AdvancedCardBase adv && adv.IsTakeAmmoEffectNotFar)
            {
                __state = __instance.card.card?.GetSpec()?.Ranged;

                if (__state is not null)
                {
                    __instance.card.card?.GetSpec().Ranged = CardRange.Far;
                }
            }

            if (dicePassiveType is not DiceCardAbilityBase.DiceCardPassiveType.BeforeRollDice)
            {
                return;
            }

            __instance.abilityList?.OfType<AdvancedDiceBase>()?.Foreach(abi => abi.BeforeRollDiceFirst());

            if (__instance.card?.cardAbility is AdvancedCardBase abi)
            {
                abi.BeforeRollDiceFirst(__instance);
            }
        }

        static Exception? Finalizer(Exception __exception, BattleDiceBehavior __instance, DiceCardAbilityBase.DiceCardPassiveType dicePassiveType, CardRange? __state)
        {
            try
            {
                if (__exception is not null)
                {
                    throw __exception;
                }

                if (dicePassiveType is DiceCardAbilityBase.DiceCardPassiveType.BeforeRollDice)
                {
                    __instance?.abilityList?.OfType<AdvancedDiceBase>()?.Foreach(abi => abi.BeforeRollDiceLast());

                    if (__instance?.card?.cardAbility is AdvancedCardBase abi)
                    {
                        abi.BeforeRollDiceLast(__instance);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("card internal error");
                Debug.LogError(ex);
            }
            finally
            {
                if (__state is not null)
                {
                    __instance.card?.card?.XmlData?.Spec?.Ranged = __state.Value;
                }
            }

            return null;
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "BeforeRollDice")]
    class PatchUnitBeforeRollDice
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(CodeMatch.IsLdarg(1), CodeMatch.IsLdfld(typeof(BattleDiceBehavior).Field("card")), CodeMatch.IsDup())
                .Insert(CodeInstruction.Ldarg(1), CodeInstruction.Call(typeof(PatchUnitBeforeRollDice).Method("InjectBefore")));

            matcher.MatchEndForward(CodeMatch.IsLdarg(1), CodeMatch.Calls(typeof(BattlePlayingCardDataInUnitModel).Method("BeforeRollDice")))
                .Advance(1)
                .Insert(CodeInstruction.Ldarg(1), CodeInstruction.Call(typeof(PatchUnitBeforeRollDice).Method("InjectAfter")));

            return matcher.Instructions();
        }

        static void InjectBefore(BattleDiceBehavior behavior)
        {
            if (behavior?.card?.cardAbility is AdvancedCardBase adv)
            {
                adv.BeforeRollDiceBefore(behavior);
            }
        }

        static void InjectAfter(BattleDiceBehavior behavior)
        {
            if (behavior?.card?.cardAbility is AdvancedCardBase adv)
            {
                adv.BeforeRollDiceAfter(behavior);
            }
        }
    }

    [HarmonyPatch]
    class PatchLambdaBeforeRollDice
    {
        static MethodBase TargetMethod()
        {
            return typeof(BattleDiceBehavior)
                .GetNestedTypes(AccessTools.all)
                .First(t => t.Name.Contains("<>c"))
                .Method("<OnEventDiceAbility>b__85_0");
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(CodeMatch.IsLdarg(1), CodeMatch.Calls(typeof(DiceCardAbilityBase).Method("BeforeRollDice")))
                .Insert(CodeInstruction.Ldarg(1), CodeInstruction.Call(typeof(PatchLambdaBeforeRollDice).Method("InjectBefore")));

            matcher.MatchEndForward(CodeMatch.IsLdarg(1), CodeMatch.Calls(typeof(DiceCardAbilityBase).Method("BeforeRollDice")))
                .Advance(1)
                .Insert(CodeInstruction.Ldarg(1), CodeInstruction.Call(typeof(PatchLambdaBeforeRollDice).Method("InjectAfter")));

            return matcher.Instructions();
        }

        static void InjectBefore(DiceCardAbilityBase ability)
        {
            if (ability is AdvancedDiceBase adv)
            {
                adv.BeforeRollDiceBefore();
            }
        }

        static void InjectAfter(DiceCardAbilityBase ability)
        {
            if (ability is AdvancedDiceBase adv)
            {
                adv.BeforeRollDiceAfter();
            }
        }
    }

    static ConditionalWeakTable<UnitBattleDataModel, List<AdvancedPassiveBase>> _saveOnReceptionPassive = new();

    static ConditionalWeakTable<UnitBattleDataModel, List<AdvancedUnitBuf>> _saveOnReceptionUnitbuf = new();
}
