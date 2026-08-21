namespace LimbufOfHermes;

/// <summary>An extension of LimbufOfHermes</summary>
public static class LimbufExtension
{
    /// <summary>Applies crit damage adder</summary>
    public static void ApplyCritDamageAdder(this BattleDiceBehavior beh, double value)
    {
        BattleUnitBuf_Limbuf_Poise.CritDamageAdder.GetValue(beh, _ => new(0)).value += value;
    }

    /// <summary>Applies crit damage adder</summary>
    public static void ApplyCritDamageAdder(this BattlePlayingCardDataInUnitModel card, double value)
    {
        foreach (var dice in card.cardBehaviorQueue)
        {
            BattleUnitBuf_Limbuf_Poise.CritDamageAdder.GetValue(dice, _ => new(0)).value += value;
        }
    }
}
