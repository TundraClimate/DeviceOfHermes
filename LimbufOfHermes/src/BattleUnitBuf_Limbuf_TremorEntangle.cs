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
        effect.txt_resist.color = new Color32(170, 60, 60, 255);

        effect.txt_resist.text = TextDataModel.GetText("LimbufOfHermes_TremorEntangle");
        effect.txt_resist.transform.localPosition -= new Vector3(0, 30, 0);

        AttackEffectManager.Instance.SetEffectSizeByCamZoom(effect);
        AttackEffectManager.Instance.SetEffectSizeByUnitHeight(owner, effect);
    }
}
