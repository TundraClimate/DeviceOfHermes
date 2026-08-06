namespace DeviceOfHermes.AdvancedBase;

/// <summary>An advanced DiceCardBuf</summary>
public class AdvancedCardBuf : BattleDiceCardBuf
{
    /// <summary>An own unit on start battle</summary>
    public virtual void OnStartBattle()
    {
    }

    /// <summary>An own unit on start battle in hand</summary>
    public virtual void OnStartBattle_inHand()
    {
    }

    /// <summary>An own unit on start battle in deck</summary>
    public virtual void OnStartBattle_inDeck()
    {
    }

    /// <summary>An own unit before roll dice</summary>
    public virtual void BeforeRollDice(BattleDiceBehavior behavior)
    {
    }

    /// <summary>An own unit on success attack</summary>
    public virtual void OnSuccessAttack(BattleDiceBehavior behavior)
    {
    }

    /// <summary>An own unit on success attack in hand</summary>
    public virtual void OnSuccessAttack_inHand(BattleDiceBehavior behavior)
    {
    }

    /// <summary>An own unit on success attack in deck</summary>
    public virtual void OnSuccessAttack_inDeck(BattleDiceBehavior behavior)
    {
    }
}
