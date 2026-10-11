using LOR_DiceSystem;

namespace LimbufOfHermes;

internal class AssistDefenseMarker : BattleDiceCardBuf
{
}

/// <summary>A unit buf the AssistDefense</summary>
public class BattleUnitBuf_Limbuf_AssistDefense : LimbufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_AssistDefense";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.AssistDefense;

    /// <summary>Impl positiveType</summary>
    public override BufPositiveType positiveType => BufPositiveType.None;

    /// <summary>Impl BeforeAlliesTakeOneSideAction</summary>
    public override void BeforeAlliesTakeOneSideAction(BattleUnitModel victim, BattleUnitModel attacker, BattlePlayingCardDataInUnitModel attckCard, ref BattlePlayingCardDataInUnitModel? altCard)
    {
        if (0 >= this.stack || altCard is not null || attckCard.target != victim || base._owner.faction.AliveUnits.Filter(u => u != victim && u.bufListDetail.HasBuf<BattleUnitBuf_Limbuf_AssistDefenseCard>()).MaxBy(u => u.GetBufStack<BattleUnitBuf_Limbuf_AssistDefense>() ?? 0) != base._owner)
        {
            return;
        }

        if (base._owner.GetBuf<BattleUnitBuf_Limbuf_AssistDefenseCard>()?.cardId is not LorId cardId || ItemXmlDataList.instance.GetCardItem(cardId) is not DiceCardXmlInfo card)
        {
            Hermes.Say($"[LoH] AssistDefense activation failed", MessageLevel.Warn);

            return;
        }

        ChangeStack(s => s - 1);

        attckCard.target = base._owner;

        base._owner.EachPassiveOf<ILimbuf.OnAssistDefense>(i => i.OnAssistDefense(victim));
        base._owner.EachUnitBufOf<ILimbuf.OnAssistDefense>(i => i.OnAssistDefense(victim));

        victim.EachPassiveOf<ILimbuf.OnDefensedByAssist>(i => i.OnDefensedByAssist(base._owner));
        victim.EachUnitBufOf<ILimbuf.OnDefensedByAssist>(i => i.OnDefensedByAssist(base._owner));

        if (!base._owner.IsBreakLifeZero() && !base._owner.IsDead() && base._owner.IsTargetable(attacker))
        {
            var playcard = base._owner.CreatePlayingCard(card, attacker, speedDiceResultValue: attckCard.speedDiceResultValue);

            playcard.card.AddBuf(new AssistDefenseMarker());

            altCard = playcard;

            base._owner.EachPassiveOf<ILimbuf.OnSuccessAssistDefense>(i => i.OnSuccessAssistDefense(victim, playcard));
            base._owner.EachUnitBufOf<ILimbuf.OnSuccessAssistDefense>(i => i.OnSuccessAssistDefense(victim, playcard));
        }
    }

    /// <summary>Impl OnRoundEnd</summary>
    public override void OnRoundEnd()
    {
        Destroy();
    }
}
