namespace LimbufOfHermes;

/// <summary>A unit buf the TremorScorch</summary>
public class BattleUnitBuf_Limbuf_TremorScorch : BattleUnitBuf_Limbuf_Tremor
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_TremorScorch";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.TremorScorch;

    /// <summary>Impl OnTremorBurst</summary>
    public override void OnTremorBurst(int stack)
    {
        base.OnTremorBurst(stack);

        var burn = base._owner?.bufListDetail?.GetActivatedBuf(KeywordBuf.Burn);
        var dmg = ((burn?.stack ?? 0) + stack) / 2;

        base._owner?.StyledDamage(dmg, GetCurrentTremor().GetBufIcon(), DamageType.Buf, this.bufType);

        burn?.stack *= 4;
        burn?.stack /= 5;

        if (0 >= burn?.stack)
        {
            burn?.Destroy();
        }
    }
}
