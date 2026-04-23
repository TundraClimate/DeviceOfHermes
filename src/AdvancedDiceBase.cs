namespace DeviceOfHermes.AdvancedBase;

/// <summary>An advanced <see cref="DiceCardAbilityBase"/></summary>
/// <remarks>
/// Can replaces <c>DiceCardAbilityBase</c> into this.
/// </remarks>
/// <example><code>
/// public class DiceCardAbility_Advance : AdvancedDiceBase
/// {
/// }
/// </code></example>
public class AdvancedDiceBase : DiceCardAbilityBase
{
    static AdvancedDiceBase() => AdvancedPatch.Init();

    /// <summary>Dice on move to keeped</summary>
    public virtual void OnAddToKeeped()
    {
    }

    /// <summary>Dice can move to keeps</summary>
    /// <returns>Is dice can move to keeps if true</returns>
    public virtual bool IsKeeps()
    {
        return true;
    }

    /// <summary>Returns dice parrying result</summary>
    /// <returns>A result of parrying with this dice</returns>
    public virtual ParryingResult GetParryingResult(ParryingResult origin)
    {
        return origin;
    }

    /// <summary>Returns dice final result value</summary>
    /// <returns>A result value of override</returns>
    public virtual int GetDiceFinalResultValue(int origin)
    {
        return origin;
    }

    /// <summary>Returns dice final damage value</summary>
    /// <returns>A damage value of override</returns>
    public virtual int GetFinalResultDamageValue(int origin)
    {
        return origin;
    }

    /// <summary>Returns dice final break damage value</summary>
    /// <returns>A break damage value of override</returns>
    public virtual int GetFinalResultBreakDamageValue(int origin)
    {
        return origin;
    }

    /// <summary>A result of parrying</summary>
    /// <remarks>That uses only the <see cref="GetParryingResult(ParryingResult)"/></remarks>
    public enum ParryingResult
    {
        /// <summary>Win</summary>
        Win,

        /// <summary>Draw</summary>
        Draw,

        /// <summary>Lose</summary>
        Lose,
    }

    internal static List<BattleDiceBehavior> OnAddKeeped(List<BattleDiceBehavior> behaviourList)
    {
        List<BattleDiceBehavior> broke = new();

        foreach (var beh in behaviourList)
        {
            foreach (var abi in beh.abilityList)
            {
                if (abi is AdvancedDiceBase)
                {
                    var advAbi = (AdvancedDiceBase)abi;

                    advAbi.OnAddToKeeped();

                    if (!advAbi.IsKeeps())
                    {
                        broke.Add(beh);
                    }
                }
            }
        }

        return broke;
    }

}
