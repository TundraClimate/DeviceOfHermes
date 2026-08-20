using UnityEngine;

namespace LimbufOfHermes;

/// <summary>A unit buf the Tremor entangle</summary>
public sealed class BattleUnitBuf_Limbuf_TremorEntangle : LimbufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_TremorEntangle";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.TremorEntangle;

    /// <summary>Impl IsInstant</summary>
    public override bool IsInstant => true;

    /// <summary>Impl OnInstant</summary>
    public override void OnInstant()
    {
        if (base._owner.bufListDetail.GetActivatedBuf(LimKeywordBuf.Tremor) is BattleUnitBuf_Limbuf_Tremor tremor && !tremor.IsEntangled())
        {
            base._owner.bufListDetail.AddKeywordBufThisRoundByEtc(LimKeywordBuf.TremorSuperposition, 1);
            (base._owner.bufListDetail.GetActivatedBuf(LimKeywordBuf.Tremor) as BattleUnitBuf_Limbuf_Tremor)?.active = false;
        }

        if (StageController.Instance.IsLogState())
        {
            base._owner.AddRencounterEvent(RencounterEvent.PrintEffect, () => OnActivate(this.stack));
        }
        else
        {
            OnActivate(this.stack);
        }

        base._owner.UpdateBufIcons();
    }

    /// <summary>Impl OnActivate</summary>
    public override void OnActivate(int stack)
    {
        base._owner.CreateTextEffect(
            TextDataModel.GetText("LimbufOfHermes_TremorEntangle"),
            this.GetBufIcon(),
            new Color32(170, 170, 60, 255),
            new Color32(200, 200, 0, 255)
        );
    }
}
