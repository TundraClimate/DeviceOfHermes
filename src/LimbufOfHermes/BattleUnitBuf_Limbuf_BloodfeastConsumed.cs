namespace LimbufOfHermes;

/// <summary>A stage buf the blood feast consumed</summary>
public class BattleUnitBuf_Limbuf_BloodfeastConsumed : LimbufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_BloodfeastConsumed";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.BloodfeastConsumed;

    /// <summary>Impl OnAddBuf</summary>
    public override void OnAddBuf(int addedStack)
    {
        this.stack = 999.Min(this.stack);
    }
}
