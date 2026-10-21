using ScrapOfHermes;
using UnityEngine;

namespace DeviceOfHermes.Effect;

/// <summary>Factory the SimpleAura</summary>
public static class SimpleAuraFactory
{
    /// <summary>Attach</summary>
    public static GameObject Attach(BattleUnitModel owner, SimpleAuraBase data)
    {
        var aura = Scrap.Load<SimpleAura>(owner.view.charAppearance.transform);

        data.Apply(aura);

        aura.gameObject.AddComponent<EffectManage>().Also(em =>
        {
            em.layerUpdate = () => LayerMask.NameToLayer(owner.view.charAppearance.GetLayerName());
        });

        return aura.gameObject;
    }
}
