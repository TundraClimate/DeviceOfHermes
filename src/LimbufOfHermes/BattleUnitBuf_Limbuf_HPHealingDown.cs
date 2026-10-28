namespace LimbufOfHermes;

/// <summary>A unit buf the HPHealingDown</summary>
public class BattleUnitBuf_Limbuf_HPHealingDown : LimbufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_HPHealingDown";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.HPHealingDown;

    /// <summary>Impl positiveType</summary>
    public override BufPositiveType positiveType => BufPositiveType.Negative;

    /// <summary>Impl GetHealingAdder</summary>
    public override int GetHealingAdder(int origin)
    {
        return -this.stack;
    }

    /// <summary>Impl OnRoundEnd</summary>
    public override void OnRoundEnd()
    {
        Destroy();
    }
}
