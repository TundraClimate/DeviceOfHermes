using UnityEngine;

namespace LimbufOfHermes;

/// <summary>A unit buf the shin</summary>
public class BattleUnitBuf_Limbuf_Shin : LimbufBase
{
    /// <summary>Impl keywordIconId</summary>
    protected override string keywordIconId => "LimbufOfHermes_Shin";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => KeywordBuf.None;

    /// <summary>Impl DefaultStack</summary>
    public override int DefaultStack => 0;

    /// <summary>Impl Init</summary>
    public override void Init(BattleUnitModel owner)
    {
        if (aura is not null)
        {
            return;
        }

        aura = CreateShinAura(base._owner, this);
    }

    /// <summary>Creates new Shin</summary>
    public static GameObject CreateShinAura(BattleUnitModel owner, BattleUnitBuf? marker = null)
    {
        var go = bundle.LoadAsset<GameObject>("Shin");

        return UnityObject.Instantiate(go, owner.view.charAppearance.transform)
            .Also(aura => aura.AddComponent<EffectManage>().Init(owner.view, marker));
    }

    private GameObject? aura;
}
