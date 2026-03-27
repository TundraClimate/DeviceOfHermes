using System.Reflection.Emit;

using HarmonyLib;

namespace DeviceOfHermes.AdvancedBase;

public class AdvancedCardBase : DiceCardSelfAbilityBase
{
    static AdvancedCardBase()
    {
        var harmony = new Harmony("DeviceOfHermes.AdvancedBases.Card");

        harmony.CreateClassProcessor(typeof(CardPatch.PatchTargetUI)).Patch();
        harmony.CreateClassProcessor(typeof(CardPatch.PatchOnStartResolve)).Patch();
        harmony.CreateClassProcessor(typeof(CardPatch.PatchOnDynamicParrying)).Patch();
        harmony.CreateClassProcessor(typeof(CardPatch.PatchOnChangeTarget)).Patch();
        harmony.CreateClassProcessor(typeof(CardPatch.PatchCanDiscard)).Patch();
    }

    public virtual bool IsClashable => true;

    public virtual bool IsIgnoreSpeedByMatch => false;

    public virtual bool CanDiscardByAbility(BattleDiceCardModel self)
    {
        return true;
    }
}

internal class CardPatch
{
    [HarmonyPatch(typeof(BattleUnitTargetArrowManagerUI), "UpdateTargetListData")]
    internal static class PatchTargetUI
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

                if (!CardPatch.IsClash(cardA, cardB))
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
                if (card?.cardAbility is AdvancedCardBase selfAdvAbi && !selfAdvAbi.IsClashable)
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

            if (!CardPatch.IsClash(cardA, cardB))
            {
                startAction(__instance, cardA);

                return false;
            }

            return true;
        }
    }

    internal static bool IsClash(BattlePlayingCardDataInUnitModel cardA, BattlePlayingCardDataInUnitModel cardB)
    {
        bool isClashableA = !(cardA.cardAbility is AdvancedCardBase advAbiA && !advAbiA.IsClashable);
        bool isClashableB = !(cardB.cardAbility is AdvancedCardBase advAbiB && !advAbiB.IsClashable);

        return isClashableA && isClashableB;
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
            if (speedWin)
            {
                return speedWin;
            }

            var selfAbi = self?.view?.speedDiceSetterUI?.GetSpeedDiceByIndex(myIndex)?.CardInDice?.cardAbility;
            var targetAbi = target?.view?.speedDiceSetterUI?.GetSpeedDiceByIndex(targetIndex)?.CardInDice?.cardAbility;

            if (selfAbi is AdvancedCardBase selfAdv && selfAdv.IsIgnoreSpeedByMatch)
            {
                return true;
            }

            if (targetAbi is AdvancedCardBase targetAdv && targetAdv.IsIgnoreSpeedByMatch)
            {
                return true;
            }

            return speedWin;
        }
    }

    [HarmonyPatch(typeof(BattleAllyCardDetail), "DiscardACardByAbility", [typeof(List<BattleDiceCardModel>)])]
    internal class PatchCanDiscard
    {
        static void Prefix(List<BattleDiceCardModel> cardList)
        {
            List<BattleDiceCardModel> cancel = new();

            foreach (var card in cardList)
            {
                if (card?.CreateDiceCardSelfAbilityScript() is AdvancedCardBase adv && !adv.CanDiscardByAbility(card))
                {
                    cancel.Add(card);
                }
            }

            foreach (var card in cancel)
            {
                cardList.Remove(card);
            }
        }
    }
}
