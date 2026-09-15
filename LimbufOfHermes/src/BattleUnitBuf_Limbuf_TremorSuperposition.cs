namespace LimbufOfHermes;

/// <summary>A unit buf the Tremor</summary>
public sealed class BattleUnitBuf_Limbuf_TremorSuperposition : BattleUnitBuf_Limbuf_Tremor
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_TremorSuperposition";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.TremorSuperposition;

    /// <summary>Impl bufActivatedText</summary>
    public override string bufActivatedText
    {
        get
        {
            var list = string.Join(Environment.NewLine, GetConversionTremor().Map(tremor => "- " + BattleEffectTextsXmlList.Instance.GetEffectTextName(tremor.KeywordId)));

            return BattleEffectTextsXmlList.Instance.GetEffectTextDesc(this.keywordId) + list;
        }
    }

    /// <summary>Impl OnRoundEnd</summary>
    public override void OnRoundEnd()
    {
        base._owner.bufListDetail.GetActivatedBufList()
            .OfType<BattleUnitBuf_Limbuf_Tremor>()
            .Filter(buf => buf.GetType() != typeof(BattleUnitBuf_Limbuf_Tremor))
            .Foreach(buf =>
            {
                buf.active = false;
                buf.Destroy();
            });

        (base._owner.bufListDetail.GetActivatedBuf(LimKeywordBuf.Tremor) as BattleUnitBuf_Limbuf_Tremor)?.active = true;

        base._owner.UpdateBufIcons();
    }

    /// <summary>Impl OnTremorBurst</summary>
    public override void OnTremorBurst(int stack)
    {
    }
}
