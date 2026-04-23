using System.Reflection.Emit;
using LOR_DiceSystem;
using HarmonyLib;
using HarmonyExtension;

namespace DeviceOfHermes.AdvancedBase;

internal static class AdvancedPatch
{
    private static Harmony harmony = new Harmony("DeviceOfHermes.AdvancedBase");

    static AdvancedPatch()
    {
        Patch(typeof(PatchTargetUI));
        Patch(typeof(PatchOnStartResolve));
        Patch(typeof(PatchOnDynamicParrying));
        Patch(typeof(PatchOnChangeTarget));
        Patch(typeof(PatchCanDiscard));
        Patch(typeof(PatchOnAddKeeps1));
        Patch(typeof(PatchOnAddKeeps2));
        Patch(typeof(PatchOnAddKeep));
        Patch(typeof(PatchOnAddKeepForDef));
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
    }

    public static void Init()
    {
    }

    private static void Patch(Type type)
    {
        harmony.CreateClassProcessor(type).Patch();
    }

    public static bool IsClashable(BattlePlayingCardDataInUnitModel cardA, BattlePlayingCardDataInUnitModel cardB)
    {
        bool isClashableA = !(cardA.cardAbility is AdvancedCardBase advAbiA && !advAbiA.IsClashable)
            && (cardA.owner?.passiveDetail?.PassiveList?.OfType<AdvancedPassiveBase>()
                .All(passive => passive.IsClashable(cardA) && passive.IsClashable(cardA, cardB)) ?? true);

        bool isClashableB = !(cardB.cardAbility is AdvancedCardBase advAbiB && !advAbiB.IsClashable)
            && (cardB.owner?.passiveDetail?.PassiveList?.OfType<AdvancedPassiveBase>()
                .All(passive => passive.IsClashable(cardB) && passive.IsClashable(cardB, cardA)) ?? true);

        return isClashableA && isClashableB;
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

    [HarmonyPatch(typeof(StageController), "StartParrying")]
    class PatchOnDynamicParrying
    {
        static Action<StageController, BattlePlayingCardDataInUnitModel> startAction;

        static PatchOnDynamicParrying()
        {
            var act = AccessTools.Method(typeof(StageController), "StartAction");

            if (act is null)
            {
                throw new InvalidOperationException("The StageController::StartAction cannot access.");
            }

            startAction = (Action<StageController, BattlePlayingCardDataInUnitModel>)act.CreateDelegate(typeof(Action<StageController, BattlePlayingCardDataInUnitModel>));
        }

        static bool Prefix(StageController __instance, BattlePlayingCardDataInUnitModel cardA, BattlePlayingCardDataInUnitModel cardB)
        {
            if (cardA is null || cardB is null)
            {
                return true;
            }

            if (!IsClashable(cardA, cardB))
            {
                startAction(__instance, cardA);

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

    [HarmonyPatch
    (
        typeof(BattleKeepedCardDataInUnitModel),
        "AddBehaviours",
        new[] { typeof(DiceCardXmlInfo), typeof(List<BattleDiceBehavior>) }
    )]
    class PatchOnAddKeeps1
    {
        static void Prefix(List<BattleDiceBehavior> behaviourList)
        {
            var broke = AdvancedDiceBase.OnAddKeeped(behaviourList);

            behaviourList.RemoveAll(b => broke.Contains(b));
        }
    }

    [HarmonyPatch
    (
        typeof(BattleKeepedCardDataInUnitModel),
        "AddBehaviours",
        new[] { typeof(BattleDiceCardModel), typeof(List<BattleDiceBehavior>) }
    )]
    class PatchOnAddKeeps2
    {
        static void Prefix(List<BattleDiceBehavior> behaviourList)
        {
            var broke = AdvancedDiceBase.OnAddKeeped(behaviourList);

            behaviourList.RemoveAll(b => broke.Contains(b));
        }
    }

    [HarmonyPatch(typeof(BattleKeepedCardDataInUnitModel), "AddBehaviour")]
    class PatchOnAddKeep
    {
        static bool Prefix(BattleDiceBehavior behaviour)
        {
            return AdvancedDiceBase.OnAddKeeped(new() { behaviour }).Count != 1;
        }
    }

    [HarmonyPatch(typeof(BattleKeepedCardDataInUnitModel), "AddBehaviourForOnlyDefense")]
    class PatchOnAddKeepForDef
    {
        static bool Prefix(BattleDiceBehavior behaviour)
        {
            return AdvancedDiceBase.OnAddKeeped(new() { behaviour }).Count != 1;
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

    [HarmonyPatch(typeof(BattleDiceBehavior), "GiveDamage")]
    class PatchDiceDamageValue
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(
                CodeMatch.IsLdloc(),
                new CodeMatch(OpCodes.Ldc_I4_0),
                new CodeMatch(OpCodes.Ldarg_0),
                CodeMatch.Calls(typeof(BattleDiceBehavior).Method("get_owner")),
                new CodeMatch(OpCodes.Ldc_I4_0),
                CodeMatch.Calls(typeof(BattleUnitModel).Method("TakeDamage"))
            )
                .Advance(1)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, typeof(PatchDiceDamageValue).Method("ChangeDamage"))
                );

            matcher.MatchStartForward(
                CodeMatch.IsLdloc(),
                new CodeMatch(OpCodes.Ldc_I4_0),
                new CodeMatch(OpCodes.Ldarg_0),
                CodeMatch.Calls(typeof(BattleDiceBehavior).Method("get_owner")),
                CodeMatch.IsLdloc(),
                new CodeMatch(OpCodes.Ldc_I4_0),
                CodeMatch.Calls(typeof(BattleUnitModel).Method("TakeBreakDamage"))
            )
                .Advance(1)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, typeof(PatchDiceDamageValue).Method("ChangeBreakDamage"))
                );

            return matcher.Instructions();
        }

        static int ChangeDamage(int origin, BattleDiceBehavior instance)
        {
            var res = origin;

            foreach (var abi in instance.abilityList.OfType<AdvancedDiceBase>())
            {
                res = abi.GetFinalResultDamageValue(res);
            }

            return res;
        }

        static int ChangeBreakDamage(int origin, BattleDiceBehavior instance)
        {
            var res = origin;

            foreach (var abi in instance.abilityList.OfType<AdvancedDiceBase>())
            {
                res = abi.GetFinalResultBreakDamageValue(res);
            }

            return res;
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
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "OnRoundStart_after")]
    static class PatchOnRoundStartLast
    {
        static void Postfix(BattleUnitModel __instance)
        {
            foreach (var passive in __instance.passiveDetail.PassiveList.OfType<AdvancedPassiveBase>())
            {
                passive.OnRoundStartLast();
            }
        }
    }

    [HarmonyPatch(typeof(StageController), "SetCurrentDiceActionPhase")]
    static class PatchOnBattleLast
    {
        static void Postfix(ref StageController.StagePhase ____phase)
        {
            if (____phase == StageController.StagePhase.RoundEndPhase)
            {
                var all = BattleObjectManager.instance.GetAliveList(false);

                foreach (var unit in all)
                {
                    foreach (var passive in unit.passiveDetail.PassiveList.OfType<AdvancedPassiveBase>())
                    {
                        if (!passive.IsAllowRoundEnd())
                        {
                            ____phase = StageController.StagePhase.SetCurrentDiceAction;
                        }
                    }
                }
            }
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
}
