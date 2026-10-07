namespace DeviceOfHermes.Derive;

/// <summary>A template for Derive</summary>
[DeriveTemplate(typeof(DiceCardSelfAbilityBase))]
public static class Draws1CardOnUseCard
{
    /// <summary>auto implement</summary>
    [DeriveUsage(typeof(DiceCardSelfAbilityBase), "OnUseCard")]
    public static void OnUseCard(DiceCardSelfAbilityBase instance)
    {
        instance.card?.owner?.allyCardDetail?.DrawCards(1);
    }
}

/// <summary>A template for Derive</summary>
[DeriveTemplate(typeof(DiceCardSelfAbilityBase))]
public static class Recover1CostOnUseCard
{
    /// <summary>auto implement</summary>
    [DeriveUsage(typeof(DiceCardSelfAbilityBase), "OnUseCard")]
    public static void OnUseCard(DiceCardSelfAbilityBase instance)
    {
        instance.card?.owner?.cardSlotDetail?.RecoverPlayPoint(1);
    }
}

/// <summary>A template for Derive</summary>
[DeriveTemplate(typeof(DiceCardAbilityBase))]
public static class DestroyOppoAllDiceOnWinParrying
{
    /// <summary>auto implement</summary>
    [DeriveUsage(typeof(DiceCardAbilityBase), "OnWinParrying")]
    public static void OnWinParrying(DiceCardAbilityBase instance)
    {
        instance.behavior?.card?.target?.currentDiceAction?.DestroyDice(DiceMatch.AllDice);
    }
}

/// <summary>A template for Derive</summary>
[DeriveTemplate(typeof(DiceCardAbilityBase))]
public static class DestroyOppoNextDiceOnWinParrying
{
    /// <summary>auto implement</summary>
    [DeriveUsage(typeof(DiceCardAbilityBase), "OnWinParrying")]
    public static void OnWinParrying(DiceCardAbilityBase instance)
    {
        instance.behavior?.card?.target?.currentDiceAction?.DestroyDice(DiceMatch.AllDice);
    }
}

/// <summary>A template for Derive</summary>
[DeriveTemplate(typeof(DiceCardAbilityBase))]
public static class DestroySelfAllDiceOnLoseParrying
{
    /// <summary>auto implement</summary>
    [DeriveUsage(typeof(DiceCardAbilityBase), "OnLoseParrying")]
    public static void OnLoseParrying(DiceCardAbilityBase instance)
    {
        instance.behavior?.card?.owner?.currentDiceAction?.DestroyDice(DiceMatch.AllDice, DiceUITiming.AttackAfter);
    }
}

/// <summary>A template for Derive</summary>
[DeriveTemplate(typeof(DiceCardAbilityBase))]
public static class DestroySelfAllDiceOnDrawParrying
{
    /// <summary>auto implement</summary>
    [DeriveUsage(typeof(DiceCardAbilityBase), "OnDrawParrying")]
    public static void OnDrawParrying(DiceCardAbilityBase instance)
    {
        instance.behavior?.card?.owner?.currentDiceAction?.DestroyDice(DiceMatch.AllDice, DiceUITiming.AttackAfter);
    }
}

/// <summary>A template for Derive</summary>
[DeriveTemplate(typeof(BattleUnitBuf))]
public static class DestroyOnRoundEnd
{
    /// <summary>auto implement</summary>
    [DerivePriority(-1000)]
    [DeriveUsage(typeof(BattleUnitBuf), "OnRoundEnd")]
    public static void OnRoundEnd(BattleUnitBuf instance)
    {
        instance.Destroy();
    }
}

/// <summary>A template for Derive</summary>
[DeriveTemplate(typeof(BattleUnitBuf))]
public static class DestroyIfZeroOnRoundEnd
{
    /// <summary>auto implement</summary>
    [DerivePriority(-1000)]
    [DeriveUsage(typeof(BattleUnitBuf), "OnRoundEnd")]
    public static void OnRoundEnd(BattleUnitBuf instance)
    {
        if (instance.stack <= 0)
        {
            instance.Destroy();
        }
    }
}

/// <summary>A template for Derive</summary>
[DeriveTemplate(typeof(BattleUnitBuf))]
public static class Decrease1StackOnRoundEnd
{
    /// <summary>auto implement</summary>
    [DeriveUsage(typeof(BattleUnitBuf), "OnRoundEnd")]
    public static void OnRoundEnd(BattleUnitBuf instance)
    {
        instance.stack -= 1;
    }
}

/// <summary>A template for Derive</summary>
[DeriveTemplate(typeof(BattleUnitBuf))]
public static class DecreaseHalfStackOnRoundEnd
{
    /// <summary>auto implement</summary>
    [DeriveUsage(typeof(BattleUnitBuf), "OnRoundEnd")]
    public static void OnRoundEnd(BattleUnitBuf instance)
    {
        instance.stack /= 2;
    }
}

/// <summary>A template for Derive</summary>
[DeriveTemplate(typeof(BattleUnitBuf))]
public static class Into1s3StackOnRoundEnd
{
    /// <summary>auto implement</summary>
    [DeriveUsage(typeof(BattleUnitBuf), "OnRoundEnd")]
    public static void OnRoundEnd(BattleUnitBuf instance)
    {
        instance.stack /= 3;
    }
}


/// <summary>A template for Derive</summary>
[DeriveTemplate(typeof(BattleUnitBuf))]
public static class Into2s3StackOnRoundEnd
{
    /// <summary>auto implement</summary>
    [DeriveUsage(typeof(BattleUnitBuf), "OnRoundEnd")]
    public static void OnRoundEnd(BattleUnitBuf instance)
    {
        instance.stack *= 2;
        instance.stack /= 3;
    }
}

/// <summary>A template for Derive</summary>
[DeriveTemplate(typeof(BattleUnitBuf))]
public static class Into4s5StackOnRoundEnd
{
    /// <summary>auto implement</summary>
    [DeriveUsage(typeof(BattleUnitBuf), "OnRoundEnd")]
    public static void OnRoundEnd(BattleUnitBuf instance)
    {
        instance.stack *= 4;
        instance.stack /= 5;
    }
}

/// <summary>A template for Derive</summary>
[DeriveTemplate(typeof(BattleUnitBuf))]
public static class Maximum1StackOnAddbuf
{
    /// <summary>auto implement</summary>
    [DerivePriority(1)]
    [DeriveUsage(typeof(BattleUnitBuf), "OnAddBuf", [typeof(int)])]
    public static void OnAddBuf(BattleUnitBuf instance, int added)
    {
        instance.stack = 1.Min(instance.stack);
    }
}

/// <summary>A template for Derive</summary>
[DeriveTemplate(typeof(BattleUnitBuf))]
public static class Maximum10StackOnAddbuf
{
    /// <summary>auto implement</summary>
    [DerivePriority(10)]
    [DeriveUsage(typeof(BattleUnitBuf), "OnAddBuf", [typeof(int)])]
    public static void OnAddBuf(BattleUnitBuf instance, int added)
    {
        instance.stack = 10.Min(instance.stack);
    }
}

/// <summary>A template for Derive</summary>
[DeriveTemplate(typeof(BattleUnitBuf))]
public static class Maximum20StackOnAddbuf
{
    /// <summary>auto implement</summary>
    [DerivePriority(20)]
    [DeriveUsage(typeof(BattleUnitBuf), "OnAddBuf", [typeof(int)])]
    public static void OnAddBuf(BattleUnitBuf instance, int added)
    {
        instance.stack = 20.Min(instance.stack);
    }
}

/// <summary>A template for Derive</summary>
[DeriveTemplate(typeof(BattleUnitBuf))]
public static class Maximum50StackOnAddbuf
{
    /// <summary>auto implement</summary>
    [DerivePriority(50)]
    [DeriveUsage(typeof(BattleUnitBuf), "OnAddBuf", [typeof(int)])]
    public static void OnAddBuf(BattleUnitBuf instance, int added)
    {
        instance.stack = 50.Min(instance.stack);
    }
}

/// <summary>A template for Derive</summary>
[DeriveTemplate(typeof(BattleUnitBuf))]
public static class Maximum100StackOnAddbuf
{
    /// <summary>auto implement</summary>
    [DerivePriority(100)]
    [DeriveUsage(typeof(BattleUnitBuf), "OnAddBuf", [typeof(int)])]
    public static void OnAddBuf(BattleUnitBuf instance, int added)
    {
        instance.stack = 100.Min(instance.stack);
    }
}
