namespace DeviceOfHermes.AdvancedBase;

/// <summary>An advanced <see cref="DiceCardSelfAbilityBase"/></summary>
/// <remarks>
/// Can replaces <c>DiceCardSelfAbilityBase</c> into this.
/// </remarks>
/// <example><code>
/// public class DiceCardSelfAbility_Advance : AdvancedCardBase
/// {
/// }
/// </code></example>
public class AdvancedCardBase : DiceCardSelfAbilityBase
{
    /// <summary>Property is card clashable</summary>
    public virtual bool IsClashable => true;

    /// <summary>Property is card clashable with standby</summary>
    public virtual bool IsClashableWithStandby => true;

    /// <summary>Property is card ignore speed by match</summary>
    public virtual bool IsIgnoreSpeedByMatch => false;

    /// <summary>Property is card take ammo effect</summary>
    public virtual bool IsTakeAmmoEffectNotFar => false;

    /// <summary>Is card draws on wave start</summary>
    public virtual bool DrawsOnWaveStart => false;

    /// <summary>Buf list of auto sets on card ability generated</summary>
    public virtual Type[] AutoSetCardBufs => [];

    /// <summary>Can discard this card by ability</summary>
    /// <returns>Is card can discard</returns>
    public virtual bool CanDiscardByAbility(BattleDiceCardModel self)
    {
        return true;
    }

    /// <summary>Returns special priority</summary>
    public virtual int SpecialPriorityAdder(int slot, int speed)
    {
        return 0;
    }

    /// <summary>Returns dice final damage value</summary>
    /// <returns>A damage value of override</returns>
    public virtual int GetFinalResultDamageValue(int origin)
    {
        return origin;
    }

    /// <summary>On card use before</summary>
    public virtual void BeforeUseCard(ref BattlePlayingCardDataInUnitModel card)
    {
    }

    /// <summary>Card owner on adds keep dice</summary>
    public virtual bool OnAddKeepDice(BattleDiceBehavior behavior)
    {
        return true;
    }

    /// <summary>Unit before roll dice with first</summary>
    public virtual void BeforeRollDiceFirst(BattleDiceBehavior behavior)
    {
    }

    /// <summary>Unit before roll dice with before</summary>
    public virtual void BeforeRollDiceBefore(BattleDiceBehavior behavior)
    {
    }

    /// <summary>Unit before roll dice with after</summary>
    public virtual void BeforeRollDiceAfter(BattleDiceBehavior behavior)
    {
    }

    /// <summary>Unit before roll dice with last</summary>
    public virtual void BeforeRollDiceLast(BattleDiceBehavior behavior)
    {
    }
}
