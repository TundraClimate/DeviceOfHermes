namespace LimbufOfHermes;

/// <summary>A unit buf the DmgDown</summary>
public class BattleUnitBuf_Limbuf_DmgDown : LimbufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_DmgDown";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.DmgDown;

    /// <summary>Impl positiveType</summary>
    public override BufPositiveType positiveType => BufPositiveType.Negative;

    /// <summary>Impl BeforeRollDice</summary>
    public override void BeforeRollDice(BattleDiceBehavior behavior)
    {
        if (!_owner.IsImmune(bufType))
        {
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                dmg = -this.stack
            });
        }
    }

    /// <summary>Impl OnRoundEnd</summary>
    public override void OnRoundEnd()
    {
        Destroy();
    }
}
