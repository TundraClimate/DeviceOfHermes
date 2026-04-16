using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;

namespace DeviceOfHermes.AdvancedBase;

/// <summary>An advanced <see cref="PassiveAbilityBase"/></summary>
/// <remarks>
/// Can replaces <c>PassiveAbilityBase</c> into this.
/// </remarks>
/// <example><code>
/// public class PassiveAbility_Advance : AdvancedPassiveBase
/// {
/// }
/// </code></example>
public class AdvancedPassiveBase : PassiveAbilityBase
{
    static AdvancedPassiveBase()
    {
        var harmony = new Harmony("DeviceOfHermes.AdvancedBases.Passive");

        harmony.CreateClassProcessor(typeof(PassivePatch.PatchOnBattleLast)).Patch();
        harmony.CreateClassProcessor(typeof(PassivePatch.PatchOnRoundStartFirst)).Patch();
        harmony.CreateClassProcessor(typeof(PassivePatch.PatchOnRoundStartLast)).Patch();
        harmony.CreateClassProcessor(typeof(PassivePatch.PatchTargetUI)).Patch();
        harmony.CreateClassProcessor(typeof(PassivePatch.PatchOnStartResolve)).Patch();
        harmony.CreateClassProcessor(typeof(PassivePatch.PatchOnDynamicParrying)).Patch();
        harmony.CreateClassProcessor(typeof(PassivePatch.PatchCanDiscard)).Patch();
        harmony.CreateClassProcessor(typeof(PassivePatch.PatchOnChangeTarget)).Patch();

        BattleTickAction.OnTick += OnTick;
    }

    /// <summary>Unit on round start before <see cref="PassiveAbilityBase.OnRoundStart"/></summary>
    public virtual void OnRoundStartFirst()
    {
    }

    /// <summary>Unit on round start after <see cref="PassiveAbilityBase.OnRoundStartAfter"/></summary>
    public virtual void OnRoundStartLast()
    {
    }

    /// <summary>Unit on activate bufs</summary>
    public virtual void OnActivatedBuf(BattleUnitBuf activate)
    {
    }

    /// <summary>Unit on change buf stack</summary>
    public virtual void OnChangeBufStack(BattleUnitBuf changed, int last)
    {
    }

    /// <summary>Is <paramref name="card"/> is clashable</summary>
    /// <param name="card">A card of set slot</param>
    /// <returns>Is <paramref name="card"/> is clashable if true</returns>
    public virtual bool IsClashable(BattlePlayingCardDataInUnitModel card)
    {
        return true;
    }

    /// <summary>Is <paramref name="self"/> is clashable with <paramref name="target"/></summary>
    /// <param name="self">A card of set slot</param>
    /// <param name="target">A card of targeted</param>
    /// <returns>Is <paramref name="self"/> is clashable with <paramref name="target"/> if true</returns>
    public virtual bool IsClashable(BattlePlayingCardDataInUnitModel self, BattlePlayingCardDataInUnitModel target)
    {
        return true;
    }

    /// <summary>Is <paramref name="self"/> is ignore speed by match with <paramref name="target"/></summary>
    /// <param name="self">A card of set slot</param>
    /// <param name="target">A card of targeted</param>
    /// <returns>Is <paramref name="self"/> is ignore speed by match with <paramref name="target"/> if true</returns>
    public virtual bool IsIgnoreSpeedByMatch(BattlePlayingCardDataInUnitModel self, BattlePlayingCardDataInUnitModel target)
    {
        return false;
    }

    /// <summary>Is allows round end</summary>
    /// <returns>Is allows if true</returns>
    public virtual bool IsAllowRoundEnd()
    {
        return true;
    }

    /// <summary>Can discard this <paramref name="card"/> by ability</summary>
    /// <param name="card">A card of discard</param>
    /// <returns>Is card can discard</returns>
    public virtual bool CanDiscardByAbility(BattleDiceCardModel card)
    {
        return true;
    }

    static void OnTick()
    {
        var alives = BattleObjectManager.instance.GetAliveList();

        foreach (var unit in alives)
        {
            var passives = unit.passiveDetail.PassiveList.FilterMap(p => p is AdvancedPassiveBase ap ? ap : null).ToList();

            if (passives.Count == 0)
            {
                continue;
            }

            List<BattleUnitBuf> activatedBufs = new();

            var lastBufs = _lastBufList.GetValue(unit, _ => new());
            var actives = unit.bufListDetail.GetActivatedBufList();

            foreach (var buf in actives)
            {
                if (!lastBufs.Contains(buf))
                {
                    activatedBufs.Add(buf);
                }

                var lastStack = _lastBufStack.GetValue(buf, _ => new());

                if (buf.stack != lastStack.value)
                {
                    foreach (var p in passives)
                    {
                        p?.OnChangeBufStack(buf, lastStack.value);
                    }

                    lastStack.value = buf.stack;
                }
            }

            foreach (var p in passives)
            {
                foreach (var buf in activatedBufs)
                {
                    p?.OnActivatedBuf(buf);
                }
            }

            if (activatedBufs.Count != 0)
            {
                lastBufs.Clear();
                lastBufs.AddRange(actives);
            }
        }
    }

    private static ConditionalWeakTable<BattleUnitModel, List<BattleUnitBuf>> _lastBufList = new();

    private static ConditionalWeakTable<BattleUnitBuf, BufStack> _lastBufStack = new();

    private class BufStack
    {
        public int value = -1;
    }
}

static class PassivePatch
{
    [HarmonyPatch(typeof(BattleUnitModel), "OnRoundStart_ignoreDead")]
    internal static class PatchOnRoundStartFirst
    {
        static void Prefix(BattleUnitModel __instance)
        {
            foreach (var passive in __instance.passiveDetail.PassiveList)
            {
                if (passive is AdvancedPassiveBase)
                {
                    ((AdvancedPassiveBase)passive).OnRoundStartFirst();
                }
            }
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "OnRoundStart_after")]
    internal static class PatchOnRoundStartLast
    {
        static void Postfix(BattleUnitModel __instance)
        {
            foreach (var passive in __instance.passiveDetail.PassiveList)
            {
                if (passive is AdvancedPassiveBase)
                {
                    ((AdvancedPassiveBase)passive).OnRoundStartLast();
                }
            }
        }
    }

    [HarmonyPatch(typeof(StageController), "SetCurrentDiceActionPhase")]
    internal static class PatchOnBattleLast
    {
        static void Postfix(ref StageController.StagePhase ____phase)
        {
            if (____phase == StageController.StagePhase.RoundEndPhase)
            {
                var all = BattleObjectManager.instance.GetAliveList(false);

                foreach (var unit in all)
                {
                    foreach (var passive in unit.passiveDetail.PassiveList)
                    {
                        if (passive is AdvancedPassiveBase && !((AdvancedPassiveBase)passive).IsAllowRoundEnd())
                        {
                            ____phase = StageController.StagePhase.SetCurrentDiceAction;
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(BattleUnitTargetArrowManagerUI), "UpdateTargetListData")]
    internal static class PatchTargetUI
    {
        static void Postfix(List<BattleUnitTargetArrowData> ___TargetListData)
        {
            foreach (var arrow in ___TargetListData)
            {
                var selfCard = arrow?.Dice?.CardInDice;
                var targetCard = arrow?.TargetDice?.CardInDice;

                if (selfCard is null || targetCard is null)
                {
                    continue;
                }

                if (!PassivePatch.IsClash(selfCard, targetCard))
                {
                    arrow?.isPairing = false;
                }
            }
        }
    }

    [HarmonyPatch(typeof(StageController), "ArrangeCardsPhase")]
    internal class PatchOnStartResolve
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

                if (!PassivePatch.IsClash(card, target))
                {
                    card?.targetSlotOrder = -1;
                }
            }
        }
    }

    [HarmonyPatch(typeof(StageController), "StartParrying")]
    internal class PatchOnDynamicParrying
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

            if (!PassivePatch.IsClash(cardA, cardB))
            {
                startAction(__instance, cardA);

                return false;
            }

            return true;
        }
    }

    internal static bool IsClash(BattlePlayingCardDataInUnitModel cardA, BattlePlayingCardDataInUnitModel cardB)
    {
        bool isClashableA = true;
        bool isClashableB = true;

        {
            var passiveList = cardA.owner?.passiveDetail.PassiveList ?? new();

            foreach (var passive in passiveList)
            {
                if (passive is AdvancedPassiveBase adv)
                {
                    if (!adv.IsClashable(cardA) || !adv.IsClashable(cardA, cardB))
                    {
                        isClashableA = false;
                    }
                }
            }
        }

        {
            var passiveList = cardB.owner?.passiveDetail.PassiveList ?? new();

            foreach (var passive in passiveList)
            {
                if (passive is AdvancedPassiveBase adv)
                {
                    if (!adv.IsClashable(cardB) || !adv.IsClashable(cardB, cardA))
                    {
                        isClashableB = false;
                    }
                }
            }
        }

        return isClashableA && isClashableB;
    }

    [HarmonyPatch(typeof(BattleAllyCardDetail), "DiscardACardByAbility", [typeof(List<BattleDiceCardModel>)])]
    internal class PatchCanDiscard
    {
        static void Prefix(BattleUnitModel ____self, List<BattleDiceCardModel> cardList)
        {
            List<BattleDiceCardModel> cancel = new();

            foreach (var passive in ____self.passiveDetail.PassiveList)
            {
                foreach (var card in cardList)
                {
                    if (card is null || cancel.Contains(card))
                    {
                        continue;
                    }

                    if (passive is AdvancedPassiveBase adv && !adv.CanDiscardByAbility(card))
                    {
                        cancel.Add(card);
                    }
                }
            }

            foreach (var card in cancel)
            {
                cardList.Remove(card);
            }
        }
    }

    [HarmonyPatch(typeof(BattleUnitModel), "CanChangeAttackTarget")]
    internal class PatchOnChangeTarget
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

            var selfPassives = self?.passiveDetail?.PassiveList?.Filter(passive => passive is AdvancedPassiveBase);
            var targetPassives = target?.passiveDetail?.PassiveList?.Filter(passive => passive is AdvancedPassiveBase);

            if (selfPassives?.Any(p => ((AdvancedPassiveBase)p).IsIgnoreSpeedByMatch(selfCard, targetCard)) == true)
            {
                return true;
            }

            if (targetPassives?.Any(p => ((AdvancedPassiveBase)p).IsIgnoreSpeedByMatch(targetCard, selfCard)) == true)
            {
                return true;
            }

            return speedWin;
        }
    }
}
