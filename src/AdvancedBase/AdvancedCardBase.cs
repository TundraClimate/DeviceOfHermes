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
    static AdvancedCardBase() => AdvancedPatch.Init();

    /// <summary>Property is card clashable</summary>
    public virtual bool IsClashable => true;

    /// <summary>Property is card ignore speed by match</summary>
    public virtual bool IsIgnoreSpeedByMatch => false;

    /// <summary>Can discard this card by ability</summary>
    /// <returns>Is card can discard</returns>
    public virtual bool CanDiscardByAbility(BattleDiceCardModel self)
    {
        return true;
    }
}
