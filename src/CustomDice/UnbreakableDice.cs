using LOR_DiceSystem;
using UnityEngine;
using DeviceOfHermes.AdvancedBase;

namespace DeviceOfHermes.CustomDice;

/// <summary>The custom dice of Limbus unbreakable</summary>
/// <remarks>
/// How to apply unbreakable dice:<br/>
/// - Exntends this class to your dice ability. <br/>
/// - That ability adds for <c>Atk</c> dice. <br/>
/// <para/>
/// Dice specify <br/>
/// - Dice when lose clash, use losed dices in a card.<br/>
/// - The unbreakable card not clashable.<br/>
/// </remarks>
/// <example><code>
/// public class DiceCardAbility_Unbreakable : Unbreakable
/// {
/// }
/// </code></example>
public class UnbreakableDice : AdvancedDiceBase
{
    internal static void Init()
    {
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Type is BehaviourType.Atk &&
                beh.Detail is BehaviourDetail.Slash &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is UnbreakableDice,
            HermesConstants.UnbreakableSlash,
            new Color(180, 0, 0, 200)
        );
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Type is BehaviourType.Atk &&
                beh.Detail is BehaviourDetail.Penetrate &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is UnbreakableDice,
            HermesConstants.UnbreakablePenetrate,
            new Color(180, 0, 0, 200)
        );
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Type is BehaviourType.Atk &&
                beh.Detail is BehaviourDetail.Hit &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is UnbreakableDice,
            HermesConstants.UnbreakableHit,
            new Color(180, 0, 0, 200)
        );
    }

    /// <summary>Returns true if dice is unbreakable</summary>
    public static bool IsUnbreakableDice(BattleDiceBehavior beh)
    {
        return beh.abilityList.Any(abi => abi is UnbreakableDice);
    }

    /// <summary>Returns true if dice is breaked unbreakable</summary>
    public static bool IsBreakedDice(BattleDiceBehavior beh)
    {
        return beh.abilityList.Any(abi => abi is UnbreakableDice unb && unb.IsBreaked);
    }

    /// <summary>A unit when use unbreakable dices</summary>
    /// <param name="card">A unbreakable dices</param>
    public virtual void OnUseBreaked(BattlePlayingCardDataInUnitModel card)
    {
    }

    /// <summary>A dice is breaked</summary>
    public bool IsBreaked => _isBreaked;

    internal bool _isBreaked = false;

    internal static Dictionary<BattleUnitModel, Queue<BattleDiceBehavior>> Stash { get; set; } = new();
}
