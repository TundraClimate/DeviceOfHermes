namespace LimbufOfHermes;

/// <summary>A unit buf the HPHealingUp</summary>
public class BattleUnitBuf_Limbuf_HPHealingUp : LimbufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_HPHealingUp";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.HPHealingUp;

    /// <summary>Impl positiveType</summary>
    public override BufPositiveType positiveType => BufPositiveType.Positive;

    /// <summary>Impl GetHealingAdder</summary>
    public override int GetHealingAdder(int origin)
    {
        return this.stack;
    }

    /// <summary>Impl OnRoundEnd</summary>
    public override void OnRoundEnd()
    {
        Destroy();
    }
}
