using UnityEngine;

namespace LimbufOfHermes;

/// <summary>A unit buf base</summary>
public class LimbufBase : AdvancedUnitBuf
{
    /// <summary>Change buf stack</summary>
    public virtual void ChangeStack(Func<int, int> f)
    {
        var bef = this.stack;

        this.stack = f(bef);

        if (this.stack > bef)
        {
            this.OnAddBuf(this.stack - bef);
        }

        if (0 >= this.stack)
        {
            this.Destroy();
        }
    }

    /// <summary>On buf activate</summary>
    public virtual void OnActivate(int stack)
    {
    }

    internal static AssetBundle bundle = AssetBundle.LoadFromStream(typeof(LimbufBase).Assembly.GetManifestResourceStream("LimbufOfHermes.public.limbuf.assetbundle"));

    internal class EffectManage : MonoBehaviour
    {
        public void Init(BattleUnitView view, BattleUnitBuf? marker)
        {
            this.view = view;
            this.marker = marker;
        }

        void FixedUpdate()
        {
            if (view is not null)
            {
                gameObject.GetComponentsInChildren<Transform>().Filter(i => i.gameObject != gameObject)
                    .Foreach(i => i.gameObject.layer = LayerMask.NameToLayer(view.charAppearance.GetLayerName()));

                if (marker is not null)
                {
                    if (marker.IsDestroyed() || marker.Hide || !marker.Owner.bufListDetail.GetActivatedBufList().Contains(marker))
                    {
                        UnityObject.Destroy(gameObject);
                    }
                }
            }
        }

        private BattleUnitView? view;

        private BattleUnitBuf? marker;
    }
}
