namespace LimbufOfHermes;

/// <summary>A unit buf the TremorReverb</summary>
public class BattleUnitBuf_Limbuf_TremorReverb : BattleUnitBuf_Limbuf_Tremor
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_TremorReverb";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.TremorReverb;

    /// <summary>Impl OnTremorBurst</summary>
    public override void OnTremorBurst(int stack)
    {
        base.OnTremorBurst(stack);

        var dmg = stack;

        base._owner?.StyledDamage(dmg, GetCurrentTremor().GetBufIcon(), DamageType.Buf, this.bufType);
    }
}
