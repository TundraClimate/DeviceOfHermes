using UnityEngine;

namespace LimbufOfHermes;

/// <summary>A buf the Reload</summary>
public class BattleUnitBuf_Limbuf_Reload<T> : LimbufBase
    where T : BattleUnitBuf_Limbuf_Ammo
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_Reload";

    /// <summary>Impl positiveType</summary>
    public override BufPositiveType positiveType => BufPositiveType.None;

    /// <summary>Impl IsInstant</summary>
    public override bool IsInstant => true;

    /// <summary>Impl OnInstant</summary>
    public override void OnInstant()
    {
        BeforeReload();

        if (appliesEffect)
        {
            if (StageController.Instance.IsLogState())
            {
                base._owner.AddRencounterEvent(RencounterEvent.AfterAction, () => OnActivate(this.stack));
            }
            else
            {
                OnActivate(this.stack);
            }
        }

        base._owner.ReloadAmmo<T>();
    }

    /// <summary>Before reload</summary>
    public virtual void BeforeReload()
    {
    }

    /// <summary>Impl OnActivate</summary>
    public override void OnActivate(int stack)
    {
        this._owner.CreateTextEffect(
            TextDataModel.GetText("LimbufOfHermes_Reload"),
            GetBufIcon(),
            AttackEffectManager.Instance.damageRwbpTextColor[2],
            new Color32(200, 200, 0, 255)
        );
    }

    /// <summary>Is effect applies</summary>
    public bool appliesEffect;
}
