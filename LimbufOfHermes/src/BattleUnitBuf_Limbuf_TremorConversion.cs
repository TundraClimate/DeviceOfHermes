using UnityEngine;
using Sound;

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
        var owner = base._owner;
        var effect = UnityObject.Instantiate<DamageTextEffect>(AttackEffectManager.Instance.damagedTextPrefab, owner.view.damageTextEffectRoot);

        effect.maxEffect = false;
        effect.isAtk = true;

        var color = AttackEffectManager.Instance.damageRwbpTextColor[2];

        effect.img_resistIcon.sprite = this.GetBufIcon();
        effect.img_resistIcon.color = new Color32(200, 200, 0, 255);
        effect.img_resistIconBg.color = new Color(0, 0, 0, 0);
        effect.img_resistIconFg.color = new Color(0, 0, 0, 0);
        effect.txt_resist.fontMaterial.SetColor("_UnderlayColor", color);
        effect.txt_resist.color = new Color32(60, 170, 170, 255);

        effect.txt_resist.text = TextDataModel.GetText("LimbufOfHermes_TremorConversion");
        effect.txt_resist.transform.localPosition -= new Vector3(0, 30, 0);

        AttackEffectManager.Instance.SetEffectSizeByCamZoom(effect);
        AttackEffectManager.Instance.SetEffectSizeByUnitHeight(owner, effect);

        SoundEffectManager.Instance.PlayClip("creature/quitegirl_hit", false, 10f, null).source.pitch = 3.2f;
    }
}
