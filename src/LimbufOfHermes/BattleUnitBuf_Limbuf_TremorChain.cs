namespace LimbufOfHermes;

/// <summary>A unit buf the TremorChain</summary>
public class BattleUnitBuf_Limbuf_TremorChain : BattleUnitBuf_Limbuf_Tremor
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_TremorChain";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.TremorChain;

    /// <summary>Impl BeforeRollDice</summary>
    public override void BeforeRollDice(BattleDiceBehavior behavior)
    {
        var tremor = TremorStack;

        if (behavior.TargetDice is not null && tremor >= 10)
        {
            var num = 3.Min(tremor / 10);

            behavior.ApplyDiceStatBonus(new() { power = -num });
        }
    }
}
