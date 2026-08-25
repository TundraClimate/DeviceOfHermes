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
    public static GameObject CreateShinAura(BattleUnitModel owner, BattleUnitBuf_Limbuf_Shin? marker = null)
    {
        var go = bundle.LoadAsset<GameObject>("Shin");

        return UnityObject.Instantiate(go, owner.view.charAppearance.transform)
            .Also(aura => aura.AddComponent<ShinAura>().Init(owner, marker));
    }

    private GameObject? aura;

    class ShinAura : MonoBehaviour
    {
        public void Init(BattleUnitModel owner, BattleUnitBuf_Limbuf_Shin? shin)
        {
            this.owner = owner;
            this.shin = shin;
        }

        void FixedUpdate()
        {
            gameObject.GetComponentsInChildren<Transform>().Filter(i => i.gameObject != gameObject)
                .Foreach(i => i.gameObject.layer = LayerMask.NameToLayer(owner!.view.charAppearance.GetLayerName()));

            if (shin is not null)
            {
                if (shin.IsDestroyed() || shin.Hide || !shin._owner.bufListDetail.GetActivatedBufList().Contains(shin))
                {
                    UnityObject.Destroy(gameObject);
                }
            }
        }

        private BattleUnitModel? owner;

        private BattleUnitBuf_Limbuf_Shin? shin;
    }
}
