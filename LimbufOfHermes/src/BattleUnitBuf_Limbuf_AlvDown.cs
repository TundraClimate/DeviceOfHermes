using LOR_DiceSystem;

namespace LimbufOfHermes;

/// <summary>A unit buf the AlvDown</summary>
public class BattleUnitBuf_Limbuf_AlvDown : LimbufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_AlvDown";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.AlvDown;

    /// <summary>Impl positiveType</summary>
    public override BufPositiveType positiveType => BufPositiveType.Negative;

    /// <summary>Impl BeforeRollDice</summary>
    public override void BeforeRollDice(BattleDiceBehavior behavior)
    {
        if (behavior.Type is BehaviourType.Atk)
        {
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                max = -(5.Min(this.stack / 5)),
            });
        }

        behavior.ApplyDiceStatBonus(new DiceStatBonus
        {
            dmgRate = -(50.Min(this.stack)),
            breakRate = -(50.Min(this.stack)),
        });
    }

    /// <summary>Impl OnRoundEnd</summary>
    public override void OnRoundEnd()
    {
        Destroy();
    }
}
