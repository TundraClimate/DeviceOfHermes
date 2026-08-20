using UnityEngine;

namespace LimbufOfHermes;

/// <summary>A unit buf the SinkingDeluge</summary>
public class BattleUnitBuf_Limbuf_SinkingDeluge : LimbufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_SinkingDeluge";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.SinkingDeluge;

    /// <summary>Impl positiveType</summary>
    public override BufPositiveType positiveType => BufPositiveType.Negative;

    /// <summary>Impl IsInstant</summary>
    public override bool IsInstant => true;

    /// <summary>Impl OnInstant</summary>
    public override void OnInstant()
    {
        var sinking = base._owner.bufListDetail.GetActivatedBuf(LimKeywordBuf.Sinking);

        if (base._owner.IsImmune(this.bufType) || sinking is null)
        {
            return;
        }

        var value = sinking.stack * 2;
        var bg = base._owner.breakDetail.breakGauge;

        base._owner.breakDetail.TakeBreakDamage(value, DamageType.Buf, keyword: this.bufType);

        if (base._owner.IsBreakLifeZero() || base._owner.breakDetail.breakGauge <= 0)
        {
            var num = value - bg;

            if (num > 0)
            {
                base._owner.TakeDamage(num, DamageType.Buf, keyword: this.bufType);
            }
        }

        sinking.Destroy();

        if (StageController.Instance.IsLogState())
        {
            base._owner.AddRencounterEvent(RencounterEvent.PrintEffect, () =>
            {
                OnActivate(this.stack);

                base._owner.view.unitBottomStatUI.UpdateStatUI(base._owner.hp, base._owner.breakDetail.breakGauge, null);
            });
        }
        else
        {
            OnActivate(this.stack);

            base._owner.view.unitBottomStatUI.UpdateStatUI(base._owner.hp, base._owner.breakDetail.breakGauge, null);
        }
    }

    /// <summary>Impl OnActivate</summary>
    public override void OnActivate(int stack)
    {
        var owner = base._owner;
        var effect = UnityObject.Instantiate<DamageTextEffect>(AttackEffectManager.Instance.damagedTextPrefab, owner.view.damageTextEffectRoot);

        effect.maxEffect = false;
        effect.isAtk = true;

        effect.img_resistIcon.sprite = this.GetBufIcon();
        effect.img_resistIcon.color = new Color32(0, 0, 200, 255);
        effect.img_resistIconBg.color = new Color(0, 0, 0, 0);
        effect.img_resistIconFg.color = new Color(0, 0, 0, 0);
        effect.txt_resist.fontMaterial.SetColor("_UnderlayColor", new Color32(60, 60, 170, 255));
        effect.txt_resist.color = new Color32(60, 60, 170, 255);

        effect.txt_resist.text = TextDataModel.GetText("LimbufOfHermes_SinkingDeluge");
        effect.txt_resist.transform.localPosition -= new Vector3(0, 30, 0);

        AttackEffectManager.Instance.SetEffectSizeByCamZoom(effect);
        AttackEffectManager.Instance.SetEffectSizeByUnitHeight(owner, effect);
    }
}
