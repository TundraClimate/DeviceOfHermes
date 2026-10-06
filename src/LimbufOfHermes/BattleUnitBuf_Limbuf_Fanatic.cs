namespace LimbufOfHermes;

/// <summary>A unit buf the Fanatic</summary>
public class BattleUnitBuf_Limbuf_Fanatic : LimbufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_Fanatic";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.Fanatic;

    /// <summary>Impl positiveType</summary>
    public override BufPositiveType positiveType => BufPositiveType.Positive;

    /// <summary>Impl BeforeRollDice</summary>
    public override void BeforeRollDice(BattleDiceBehavior behavior)
    {
        if (base._owner.IsImmune(this.bufType))
        {
            return;
        }

        if (3 > (int)behavior.Detail && behavior.card.target?.bufListDetail?.GetActivatedBuf(LimKeywordBuf.Nails) is not null)
        {
            behavior.ApplyDiceStatBonus(new DiceStatBonus { power = this.stack });
        }
    }

    /// <summary>Impl OnRoundEnd</summary>
    public override void OnRoundEnd()
    {
        Destroy();
    }
}
