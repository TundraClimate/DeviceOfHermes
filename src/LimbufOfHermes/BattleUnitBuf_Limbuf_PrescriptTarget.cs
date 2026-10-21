using DeviceOfHermes.Effect;
using UnityEngine;

namespace LimbufOfHermes;

/// <summary>A unit buf the PrescriptTarget</summary>
public class BattleUnitBuf_Limbuf_PrescriptTarget : LimbufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_PrescriptTarget";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.PrescriptTarget;

    /// <summary>Impl Init</summary>
    public override void Init(BattleUnitModel owner)
    {
        if (effect is not null)
        {
            return;
        }

        if (owner.faction.AliveUnits.Exists(unit => unit.bufListDetail.HasBuf<BattleUnitBuf_Limbuf_PrescriptTarget>() && unit != owner))
        {
            Destroy();
        }
        else
        {
            effect = UnityObject.Instantiate(bundle.LoadAsset<GameObject>("IndexMark"), owner.view.characterRotationCenter)
                .Also(aura => aura.AddComponent<EffectManage>().Also(em =>
                {
                    em.layerUpdate = () => LayerMask.NameToLayer(owner.view.charAppearance.GetLayerName());
                    em.isDestroySelf = () => this.IsDestroyed() || this.Hide || !this.Owner.bufListDetail.GetActivatedBufList().Contains(this);
                }));
        }

        BattleObjectManager.instance.GetAliveList_random(Faction.Enemy, 1)[0].bufListDetail.AddKeywordBufThisRoundByEtc(LimKeywordBuf.PrescriptTarget, 1);
    }

    /// <summary>Impl OnAddBuf</summary>
    public override void OnAddBuf(int addedStack)
    {
        this.stack = 0;
    }

    /// <summary>Impl OnRoundEnd</summary>
    public override void OnRoundEnd()
    {
        Destroy();
    }

    private GameObject? effect;
}
