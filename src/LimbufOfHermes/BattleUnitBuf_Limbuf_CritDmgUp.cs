namespace LimbufOfHermes;

/// <summary>A unit buf the CritDmgUp</summary>
public class BattleUnitBuf_Limbuf_CritDmgUp : LimbufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_CritDmgUp";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.CritDmgUp;

    /// <summary>Impl positiveType</summary>
    public override BufPositiveType positiveType => BufPositiveType.Positive;

    /// <summary>Impl paramInBufDesc</summary>
    public override int paramInBufDesc => this.stack * 10;

    /// <summary>Impl BeforeRollDice</summary>
    public override void BeforeRollDice(BattleDiceBehavior behavior)
    {
        behavior.ApplyCritDamageAdder(0.1);
    }

    /// <summary>Impl OnRoundEnd</summary>
    public override void OnRoundEnd()
    {
        Destroy();
    }
}
