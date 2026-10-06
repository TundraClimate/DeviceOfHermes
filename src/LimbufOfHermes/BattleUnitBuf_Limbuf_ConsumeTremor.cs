namespace LimbufOfHermes;

/// <summary>A unit buf the consumes tremor</summary>
public sealed class BattleUnitBuf_Limbuf_ConsumeTremor : LimbufBase
{
    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.ConsumeTremor;

    /// <summary>Impl OnAddBuf</summary>
    public override void OnAddBuf(int addedStack)
    {
        (base._owner.bufListDetail?.GetActivatedBuf(LimKeywordBuf.Tremor) as BattleUnitBuf_Limbuf_Tremor)?.Consume(addedStack);

        if (!StageController.Instance.IsLogState())
        {
            base._owner.view.unitBottomStatUI.UpdateStatUI(base._owner.hp, base._owner.breakDetail.breakGauge, null);
        }

        Destroy();
    }
}
