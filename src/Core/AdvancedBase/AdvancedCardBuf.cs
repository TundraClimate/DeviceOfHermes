namespace DeviceOfHermes.AdvancedBase;

/// <summary>An advanced DiceCardBuf</summary>
public class AdvancedCardBuf : BattleDiceCardBuf
{
    /// <summary>An own card is targetable all unit</summary>
    public virtual bool IsTargetableAllUnit => false;

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

    /// <summary>An own unit on win parrying</summary>
    public virtual void OnWinParrying(BattleDiceBehavior behavior, BattleDiceBehavior targetDice, bool isAtkDice)
    {
    }

    /// <summary>An own unit on lose parrying</summary>
    public virtual void OnLoseParrying(BattleDiceBehavior behavior, BattleDiceBehavior targetDice)
    {
    }

    /// <summary>An own unit on draw parrying</summary>
    public virtual void OnDrawParrying(BattleDiceBehavior behavior, BattleDiceBehavior targetDice)
    {
    }

    /// <summary>An own unit on win parrying in hand</summary>
    public virtual void OnWinParrying_inHand(BattleDiceBehavior behavior, BattleDiceBehavior targetDice, bool isAtkDice)
    {
    }

    /// <summary>An own unit on win parrying in deck</summary>
    public virtual void OnWinParrying_inDeck(BattleDiceBehavior behavior, BattleDiceBehavior targetDice, bool isAtkDice)
    {
    }

    /// <summary>An own unit on lose parrying in hand</summary>
    public virtual void OnLoseParrying_inHand(BattleDiceBehavior behavior, BattleDiceBehavior targetDice)
    {
    }

    /// <summary>An own unit on lose parrying in deck</summary>
    public virtual void OnLoseParrying_inDeck(BattleDiceBehavior behavior, BattleDiceBehavior targetDice)
    {
    }

    /// <summary>An own unit on draw parrying in hand</summary>
    public virtual void OnDrawParrying_inHand(BattleDiceBehavior behavior, BattleDiceBehavior targetDice)
    {
    }

    /// <summary>An own unit on draw parrying in deck</summary>
    public virtual void OnDrawParrying_inDeck(BattleDiceBehavior behavior, BattleDiceBehavior targetDice)
    {
    }

    /// <summary>On own unit ends encounter that card</summary>
    public virtual void OnEndEncounter(BattleUnitModel? target, bool isParrying, bool wasAttacker)
    {
    }
}
