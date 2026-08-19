using UnityEngine;

namespace LimbufOfHermes;

/// <summary>A unit buf the panic</summary>
public class BattleUnitBuf_Limbuf_Panic : LimbufBase
{
    /// <summary>Impl keywordId</summary>
    protected override string keywordId => "LimbufOfHermes_Panic";

    /// <summary>Impl bufType</summary>
    public override KeywordBuf bufType => LimKeywordBuf.Panic;

    /// <summary>Impl DefaultStack</summary>
    public override int DefaultStack => 0;

    /// <summary>Impl Init</summary>
    public override void Init(BattleUnitModel owner)
    {
        if (aura is not null)
        {
            return;
        }

        var go = bundle.LoadAsset<GameObject>("PanicAura");

        aura = UnityObject.Instantiate(go, base._owner.view.charAppearance.transform);
    }

    /// <summary>Impl OnAddBufAll</summary>
    public override void OnAddBufAll(BattleUnitBuf buf, int addedStack)
    {
        if (buf is BattleUnitBuf_Limbuf_Sinking)
        {
            buf.Destroy();
        }
    }

    /// <summary>Impl BeforeRollDice</summary>
    public override void BeforeRollDice(BattleDiceBehavior behavior)
    {
        if (base._owner.IsImmune(this.bufType))
        {
            return;
        }

        behavior.ApplyDiceStatBonus(new AdvancedDiceStatBonus { highrollGlobalWeight = -99 });
    }

    /// <summary>Impl OnRoundEnd</summary>
    public override void OnRoundEnd()
    {
        Destroy();

        UnityObject.Destroy(aura);
    }

    private GameObject? aura;
}
