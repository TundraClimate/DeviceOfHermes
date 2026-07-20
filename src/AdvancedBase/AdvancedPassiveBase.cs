namespace DeviceOfHermes.AdvancedBase;

/// <summary>An advanced <see cref="PassiveAbilityBase"/></summary>
/// <remarks>
/// Can replaces <c>PassiveAbilityBase</c> into this.
/// </remarks>
/// <example><code>
/// public class PassiveAbility_Advance : AdvancedPassiveBase
/// {
/// }
/// </code></example>
public class AdvancedPassiveBase : PassiveAbilityBase
{
    static AdvancedPassiveBase()
    {
        AdvancedPatch.Init();
        BattleBufTracker.Init();
    }

    /// <summary>A stopper of health limit</summary>
    public virtual int? HealthStopperLine => null;

    /// <summary>Addr number of draw cards</summary>
    public virtual int DrawCardAddr => 0;

    /// <summary>Unit on wave start before</summary>
    public virtual void OnWaveStartBefore()
    {
    }

    /// <summary>Unit on wave start after</summary>
    public virtual void OnWaveStartAfter()
    {
    }

    /// <summary>Unit on round start before <see cref="PassiveAbilityBase.OnRoundStart"/></summary>
    public virtual void OnRoundStartFirst()
    {
    }

    /// <summary>Unit on round start after <see cref="PassiveAbilityBase.OnRoundStartAfter"/></summary>
    public virtual void OnRoundStartLast()
    {
    }

    /// <summary>Unit on activate bufs</summary>
    public virtual void OnActivatedBuf(BattleUnitBuf activate)
    {
    }

    /// <summary>Unit on change buf stack</summary>
    public virtual void OnChangeBufStack(BattleUnitBuf changed, int last)
    {
    }

    /// <summary>Is <paramref name="card"/> is clashable</summary>
    /// <param name="card">A card of set slot</param>
    /// <returns>Is <paramref name="card"/> is clashable if true</returns>
    public virtual bool IsClashable(BattlePlayingCardDataInUnitModel card)
    {
        return true;
    }

    /// <summary>Is <paramref name="self"/> is clashable with <paramref name="target"/></summary>
    /// <param name="self">A card of set slot</param>
    /// <param name="target">A card of targeted</param>
    /// <returns>Is <paramref name="self"/> is clashable with <paramref name="target"/> if true</returns>
    public virtual bool IsClashable(BattlePlayingCardDataInUnitModel self, BattlePlayingCardDataInUnitModel target)
    {
        return true;
    }

    /// <summary>Is <paramref name="self"/> is ignore speed by match with <paramref name="target"/></summary>
    /// <param name="self">A card of set slot</param>
    /// <param name="target">A card of targeted</param>
    /// <returns>Is <paramref name="self"/> is ignore speed by match with <paramref name="target"/> if true</returns>
    public virtual bool IsIgnoreSpeedByMatch(BattlePlayingCardDataInUnitModel self, BattlePlayingCardDataInUnitModel target)
    {
        return false;
    }

    /// <summary>On Round end before</summary>
    public virtual void OnPreRoundEnd()
    {
    }

    /// <summary>Is allows round end</summary>
    /// <returns>Is allows if true</returns>
    public virtual bool IsAllowRoundEnd()
    {
        return true;
    }

    /// <summary>Can discard this <paramref name="card"/> by ability</summary>
    /// <param name="card">A card of discard</param>
    /// <returns>Is card can discard</returns>
    public virtual bool CanDiscardByAbility(BattleDiceCardModel card)
    {
        return true;
    }

    /// <summary>On dropped card</summary>
    /// <param name="playcard">A card of dropped</param>
    public virtual void OnDropCard(BattlePlayingCardDataInUnitModel playcard)
    {
    }

    /// <summary>On unit clicked</summary>
    public virtual void OnClickUnit(ClickType ty)
    {
    }

    /// <summary>On unit added buf</summary>
    public virtual void OnAddBuf(BattleUnitBuf buf, int addedStack)
    {
    }

    /// <summary>On unit choose card</summary>
    public virtual bool OnChooseCard(BattleDiceCardModel card)
    {
        return true;
    }

    /// <summary>On unit take oneside before</summary>
    public virtual BattlePlayingCardDataInUnitModel? BeforeTakeOneSideAction(
        BattleUnitModel attacker, BattlePlayingCardDataInUnitModel attckCard
    )
    {
        return null;
    }

    /// <summary>The type of OnClick</summary>
    public enum ClickType
    {
        /// <summary>Left</summary>
        Left,

        /// <summary>Right</summary>
        Right,

        /// <summary>Middle</summary>
        Middle,
    }
}
