using LOR_DiceSystem;

namespace LimbufOfHermes;

/// <summary>A unit buf the DlvDown</summary>
public class BattleUnitBuf_Limbuf_DlvDown : LimbufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_DlvDown";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.DlvDown;

    /// <summary>Impl positiveType</summary>
    public override BufPositiveType positiveType => BufPositiveType.Negative;

    /// <summary>Impl BeforeRollDice</summary>
    public override void BeforeRollDice(BattleDiceBehavior behavior)
    {
        if (behavior.Type is BehaviourType.Def)
        {
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                max = -(5.Min(this.stack / 5)),
            });
        }
    }

    /// <summary>Impl DmgFactor</summary>
    public override float DmgFactor(int dmg, DamageType type, KeywordBuf keyword)
    {
        return 1f + (float)(this.stack.Min(50)) * 0.01f;
    }

    /// <summary>Impl BreakDmgFactor</summary>
    public override float BreakDmgFactor(int dmg, DamageType type, KeywordBuf keyword)
    {
        return 1f + (float)(this.stack.Min(50)) * 0.01f;
    }

    /// <summary>Impl OnRoundEnd</summary>
    public override void OnRoundEnd()
    {
        Destroy();
    }
}
