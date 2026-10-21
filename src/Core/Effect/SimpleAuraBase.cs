using ScrapOfHermes;
using UnityEngine;

namespace DeviceOfHermes.Effect;

/// <summary>A base class the SimpleAura</summary>
public abstract class SimpleAuraBase
{
    internal void Apply(SimpleAura aura)
    {
        _aura = aura;

        Init();
    }

    /// <summary>Init</summary>
    public abstract void Init();

    /// <summary>Set material</summary>
    protected void SetMaterial(AuraLayer layer, AuraSpec spec) => _aura?.SetMaterial(GetTarget(_aura, layer), (int)spec);

    /// <summary>Set color</summary>
    protected void SetColor(AuraLayer layer, Color color) => _aura?.SetColor(GetTarget(_aura, layer), color);

    GameObject? GetTarget(SimpleAura aura, AuraLayer layer) => layer switch
    {
        AuraLayer.Aura1 => aura.Aura1,
        AuraLayer.Aura2 => aura.Aura2,
        AuraLayer.Aura3 => aura.Aura3,
        AuraLayer.AuraN3 => aura.AuraN3,
        AuraLayer.AuraN2 => aura.AuraN2,
        AuraLayer.AuraN1 => aura.AuraN1,

        _ => null,
    };

    /// <summary>Layer enum</summary>
    protected enum AuraLayer
    {
        /// <summary>Aura1</summary>
        Aura1,

        /// <summary>Aura2</summary>
        Aura2,

        /// <summary>Aura3</summary>
        Aura3,

        /// <summary>AuraN3</summary>
        AuraN3,

        /// <summary>AuraN2</summary>
        AuraN2,

        /// <summary>AuraN1</summary>
        AuraN1,
    }

    /// <summary>Layer materials</summary>
    protected enum AuraSpec
    {
        /// <summary>Mat1</summary>
        Mat1,

        /// <summary>Mat2</summary>
        Mat2,

        /// <summary>Mat3</summary>
        Mat3,
    }

    SimpleAura? _aura;
}
