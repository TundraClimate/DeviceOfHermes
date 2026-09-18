namespace LimbufOfHermes;

/// <summary>A unit buf the TremorHemorrhage</summary>
public class BattleUnitBuf_Limbuf_TremorHemorrhage : BattleUnitBuf_Limbuf_Tremor
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_TremorHemorrhage";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.TremorHemorrhage;

    /// <summary>Impl OnTremorBurst</summary>
    public override void OnTremorBurst(int stack)
    {
        base.OnTremorBurst(stack);

        var bleed = base._owner?.bufListDetail?.GetActivatedBuf(KeywordBuf.Bleeding);
        var dmg = ((bleed?.stack ?? 0) + stack) / 2;

        base._owner?.StyledDamage(dmg, this.GetBufIcon(), DamageType.Buf, this.bufType);

        if (base._owner?.bufListDetail?.GetActivatedBuf(KeywordBuf.BloodStackBlock) is null)
        {
            bleed?.stack *= 4;
            bleed?.stack /= 5;
        }

        if (0 >= bleed?.stack)
        {
            bleed?.Destroy();
        }
    }
}
