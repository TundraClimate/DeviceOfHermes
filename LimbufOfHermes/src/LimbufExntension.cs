namespace LimbufOfHermes;

/// <summary>An extension of LimbufOfHermes</summary>
public static class LimbufExtension
{
    /// <summary>Applies crit damage adder</summary>
    public static void ApplyCritDamageAdder(this BattleDiceBehavior beh, double value)
    {
        beh.ApplyDiceStatBonus(new AdvancedDiceStatBonus { customFields = new() { ["loh_critDmgRate"] = (int)(value * 100) } });
    }

    /// <summary>Applies crit damage adder</summary>
    public static void ApplyCritDamageAdder(this BattlePlayingCardDataInUnitModel card, double value)
    {
        foreach (var dice in card.cardBehaviorQueue)
        {
            dice.ApplyCritDamageAdder(value);
        }
    }

    /// <summary>Applies crit damage rate adder</summary>
    public static void ApplyCritDamageRateAdder(this BattleDiceBehavior beh, int value)
    {
        beh.ApplyDiceStatBonus(new AdvancedDiceStatBonus { customFields = new() { ["loh_critDmgRate"] = (int)value } });
    }

    /// <summary>Applies crit damage rate adder</summary>
    public static void ApplyCritDamageRateAdder(this BattlePlayingCardDataInUnitModel card, int value)
    {
        foreach (var dice in card.cardBehaviorQueue)
        {
            dice.ApplyCritDamageRateAdder(value);
        }
    }

    /// <summary>Applies tremor break rate adder</summary>
    public static void ApplyTremorBreakRateAdder(this BattleDiceBehavior beh, int value)
    {
        beh.ApplyDiceStatBonus(new AdvancedDiceStatBonus { customFields = new() { ["loh_tremorBreakRate"] = (int)value } });
    }

    /// <summary>Applies tremor break rate adder</summary>
    public static void ApplyTremorBreakRateAdder(this BattlePlayingCardDataInUnitModel card, int value)
    {
        foreach (var dice in card.cardBehaviorQueue)
        {
            dice.ApplyTremorBreakRateAdder(value);
        }
    }
}
