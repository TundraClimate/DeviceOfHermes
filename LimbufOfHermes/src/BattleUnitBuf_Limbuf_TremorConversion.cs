using UnityEngine;

namespace LimbufOfHermes;

/// <summary>A unit buf the Tremor conversion</summary>
public sealed class BattleUnitBuf_Limbuf_TremorConversion : LimbufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_TremorConversion";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.TremorConversion;

    /// <summary>Impl IsInstant</summary>
    public override bool IsInstant => true;

    /// <summary>Impl OnInstant</summary>
    public override void OnInstant()
    {
        if (base._owner.bufListDetail.GetActivatedBuf(LimKeywordBuf.Tremor) is BattleUnitBuf_Limbuf_Tremor tremor && !tremor.IsEntangled())
        {
            var bufs = base._owner.bufListDetail.GetActivatedBufList();

            foreach (var all in bufs.OfType<BattleUnitBuf_Limbuf_Tremor>())
            {
                all.active = false;
            }

            bufs.FirstOrDefault(buf => buf.GetType() != typeof(BattleUnitBuf_Limbuf_Tremor) && buf is BattleUnitBuf_Limbuf_Tremor)?
                .Let(buf =>
                {
                    ((BattleUnitBuf_Limbuf_Tremor)buf).active = true;

                    bufs.RemoveAll(all => all is BattleUnitBuf_Limbuf_Tremor && all != tremor && all != buf);
                });

            base._owner.EachPassiveOf<ILimbuf.OnTremorConversion>(i => i.OnTremorConversion());
            base._owner.EachUnitBufOf<ILimbuf.OnTremorConversion>(i => i.OnTremorConversion());
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
            TextDataModel.GetText("LimbufOfHermes_TremorConversion"),
            this.GetBufIcon(),
            new Color32(60, 170, 170, 255),
            new Color32(200, 200, 0, 255)
        );
    }
}
