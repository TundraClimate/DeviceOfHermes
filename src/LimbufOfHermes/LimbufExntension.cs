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
        beh.ApplyDiceStatBonus(new AdvancedDiceStatBonus { customFields = new() { ["loh_critDmgRate"] = value } });
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
        beh.ApplyDiceStatBonus(new AdvancedDiceStatBonus { customFields = new() { ["loh_tremorBreakRate"] = value } });
    }

    /// <summary>Applies tremor break rate adder</summary>
    public static void ApplyTremorBreakRateAdder(this BattlePlayingCardDataInUnitModel card, int value)
    {
        foreach (var dice in card.cardBehaviorQueue)
        {
            dice.ApplyTremorBreakRateAdder(value);
        }
    }

    /// <summary>Returns card is assist defense card</summary>
    public static bool IsAssistDefenseCard(this BattlePlayingCardDataInUnitModel card)
    {
        return card.card.GetBufList().Exists(buf => buf is AssistDefenseMarker);
    }

    /// <summary>Init ammo buf with max</summary>
    public static void InitAmmo<T>(this BattleUnitModel owner, T ammo, int max)
        where T : BattleUnitBuf_Limbuf_Ammo
    {
        if (owner.GetBuf<T>() is BattleUnitBuf a)
        {
            a.Destroy();
        }

        ammo.MaxValue = max;

        owner.bufListDetail?.AddBuf(ammo);
    }

    /// <summary>Consumes ammo</summary>
    public static bool ConsumeAmmo(this BattleUnitModel owner, BattleDiceBehavior dice, int num) => ConsumeAmmo<BattleUnitBuf_Limbuf_Ammo>(owner, dice, num);

    /// <summary>Consumes ammo</summary>
    public static bool ConsumeAmmo<T>(this BattleUnitModel owner, BattleDiceBehavior dice, int num)
        where T : BattleUnitBuf_Limbuf_Ammo
    {
        return owner.GetBuf<T>()?.Consume(dice, num) == true;
    }

    /// <summary>Is ammo consumed by dice</summary>
    public static bool IsConsumedAmmo(this BattleDiceBehavior dice) => IsConsumedAmmo<BattleUnitBuf_Limbuf_Ammo>(dice);

    /// <summary>Is ammo consumed by dice</summary>
    public static bool IsConsumedAmmo<T>(this BattleDiceBehavior dice)
        where T : BattleUnitBuf_Limbuf_Ammo
    {
        return dice.StatBonus is AdvancedDiceStatBonus adv && adv.customFields.ContainsKey($"LimbufOfHermes_Ammo_{typeof(T).FullName}");
    }

    /// <summary>Reloads ammo</summary>
    public static void ReloadAmmo(this BattleUnitModel owner) => ReloadAmmo<BattleUnitBuf_Limbuf_Ammo>(owner);

    /// <summary>Reloads ammo</summary>
    public static void ReloadAmmo<T>(this BattleUnitModel owner) where T : BattleUnitBuf_Limbuf_Ammo => owner.GetBuf<T>()?.Reload();
}
