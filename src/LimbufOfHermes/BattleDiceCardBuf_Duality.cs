using HarmonyLib;

namespace LimbufOfHermes;

/// <summary>A card buf the Duality</summary>
public class BattleDiceCardBuf_Limbuf_Duality : LimbufCardBuf
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_Duality";

    /// <summary>Impl IsTargetableAllUnit</summary>
    public override bool IsTargetableAllUnit => true;

    /// <summary>Impl OnEndEncounter</summary>
    public override void OnEndEncounter(BattleUnitModel? target, bool isParrying, bool wasAttacker)
    {
        if (base._card?.owner?.currentDiceAction is null)
        {
            base._card?.owner?.currentDiceAction = StageController.Instance.GetAllCards().Find(c => c.owner == base._card?.owner && c.owner.faction == c.target.faction && c.card.GetBufList().Exists(b => b is BattleDiceCardBuf_Limbuf_Duality));
        }
    }

    /// <summary>Impl OnEndEncounter</summary>
    public override void BeforeRollDice(BattleDiceBehavior behavior)
    {
        if (behavior.card.target.faction == base._card?.owner?.faction)
        {
            behavior.ApplyDiceStatBonus(new AdvancedDiceStatBonus { noHit = true });
        }
    }

    [HarmonyPatch(typeof(StageController), "ActivateStartBattleEffectPhase")]
    class PatchOnStartBattle
    {
        static Exception Finalizer(Exception __exception)
        {
            var allCards = StageController.Instance.GetAllCards();

            var f = allCards.Filter(c => c.target.faction == c.owner.faction && c.card?.GetBufList()?.Any(b => b is BattleDiceCardBuf_Limbuf_Duality) == true).ToList();
            var s = allCards.Filter(c => !(c.target.faction == c.owner.faction && c.card?.GetBufList()?.Any(b => b is BattleDiceCardBuf_Limbuf_Duality) == true)).ToList();

            f.ForEach(c => c.speedDiceResultValue = 9999);

            allCards.Clear();

            allCards.AddRange(f);
            allCards.AddRange(s);

            return __exception;
        }
    }
}
