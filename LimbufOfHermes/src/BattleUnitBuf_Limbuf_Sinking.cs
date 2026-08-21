namespace LimbufOfHermes;

/// <summary>A unit buf the Sinking</summary>
public class BattleUnitBuf_Limbuf_Sinking : LimbufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_Sinking";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.Sinking;

    /// <summary>Impl positiveType</summary>
    public override BufPositiveType positiveType => BufPositiveType.Negative;

    /// <summary>Impl OnAddBuf</summary>
    public override void OnAddBuf(int addedStack)
    {
        if (base._owner.IsImmune(this.bufType) || base._owner.bufListDetail.HasBuf<BattleUnitBuf_Limbuf_Panic>())
        {
            Destroy();
        }

        this.stack = 99.Min(this.stack);
    }

    /// <summary>Impl BeforeRollDice</summary>
    public override void BeforeRollDice(BattleDiceBehavior behavior)
    {
        if (base._owner.IsImmune(this.bufType))
        {
            return;
        }

        behavior.ApplyDiceStatBonus(new AdvancedDiceStatBonus { highrollGlobalWeight = -this.stack });
    }

    /// <summary>Impl OnRoundEndTheLast</summary>
    public override void OnRoundEndTheLast()
    {
        if (this.stack == 99)
        {
            Destroy();

            base._owner.bufListDetail.AddKeywordBufByEtc(LimKeywordBuf.Panic, 1);

            base._owner.EachPassiveOf<ILimbuf.OnPanic>(i => i.OnPanic());
            base._owner.EachUnitBufOf<ILimbuf.OnPanic>(i => i.OnPanic());
        }
    }
}
