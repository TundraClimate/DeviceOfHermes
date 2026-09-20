namespace LimbufOfHermes;

/// <summary>A unit buf the ClashPowerUp</summary>
public class BattleUnitBuf_Limbuf_ClashPowerUp : LimbufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_ClashPowerUp";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.ClashPowerUp;

    /// <summary>Impl positiveType</summary>
    public override BufPositiveType positiveType => BufPositiveType.Positive;

    /// <summary>Impl BeforeRollDice</summary>
    public override void BeforeRollDice(BattleDiceBehavior behavior)
    {
        if (base._owner.IsImmune(this.bufType))
        {
            return;
        }

        if (behavior.TargetDice is not null)
        {
            behavior.ApplyDiceStatBonus(new() { power = this.stack, dmg = -this.stack });
        }
    }

    /// <summary>Impl OnRoundEnd</summary>
    public override void OnRoundEnd()
    {
        Destroy();
    }
}
