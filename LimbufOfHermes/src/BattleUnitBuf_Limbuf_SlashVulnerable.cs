using LOR_DiceSystem;

namespace LimbufOfHermes;

/// <summary>A unit buf the Slash vulnerable</summary>
public class BattleUnitBuf_Limbuf_SlashVulnerable : LimbufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_SlashVulnerable";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.SlashVulnerable;

    /// <summary>Impl positiveType</summary>
    public override BufPositiveType positiveType => BufPositiveType.Negative;

    /// <summary>Impl GetDamageReduction</summary>
    public override int GetDamageReduction(BattleDiceBehavior behavior)
    {
        if (base._owner.IsImmune(this.bufType) || behavior.Detail is not BehaviourDetail.Slash)
        {
            return base.GetDamageReduction(behavior);
        }

        return -this.stack;
    }

    /// <summary>Impl OnRoundEnd</summary>
    public override void OnRoundEnd()
    {
        Destroy();
    }
}
