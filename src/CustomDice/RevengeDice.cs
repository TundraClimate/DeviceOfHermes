using LOR_DiceSystem;
using UnityEngine;
using DeviceOfHermes.AdvancedBase;

namespace DeviceOfHermes.CustomDice;

/// <summary>The custom dice of Limbus standby</summary>
/// <remarks>
/// How to apply revenge dice:<br/>
/// - Exntends this class to your dice ability. <br/>
/// - That ability adds for <c>Standby</c> dice. <br/>
/// <para/>
/// Dice specify <br/>
/// - When take damage by attack, use revenge dices in a card.<br/>
/// - The revenge card not clashable.<br/>
/// </remarks>
/// <example><code>
/// public class DiceCardAbility_Revenge : RevengeDice
/// {
/// }
/// </code></example>
public class RevengeDice : AdvancedDiceBase
{
    static RevengeDice()
    {
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Type is BehaviourType.Standby &&
                beh.Detail is BehaviourDetail.Slash &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is RevengeDice,
            HermesConstants.RevengeDiceSlash,
            new Color(255, 0, 200, 200)
        );
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Type is BehaviourType.Standby &&
                beh.Detail is BehaviourDetail.Penetrate &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is RevengeDice,
            HermesConstants.RevengeDicePenetrate,
            new Color(255, 0, 200, 200)
        );
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Type is BehaviourType.Standby &&
                beh.Detail is BehaviourDetail.Hit &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is RevengeDice,
            HermesConstants.RevengeDiceHit,
            new Color(255, 0, 200, 200)
        );

        CustomDicePatch.Init();

        Cards = new();
        CurrentRevenge = new();
    }

    /// <summary>Is revenge now, or before revenge</summary>
    public static bool IsRevengeNow(BattleUnitModel unit)
    {
        return CurrentRevenge.ContainsKey(unit);
    }

    /// <summary>Returns remaining revenges</summary>
    public static int RemainingRevenges(BattleUnitModel unit)
    {
        if (!Cards.TryGetValue(unit, out var res))
        {
            return 0;
        }

        return res.Count;
    }

    /// <summary>A unit when revenge decided</summary>
    /// <param name="card">A revenge dicecard</param>
    /// <param name="revengeBy">A dice of revenge decided</param>
    public virtual void OnBeforeRevenge(BattlePlayingCardDataInUnitModel card, BattleDiceBehavior revengeBy)
    {
    }

    /// <summary>A unit when use revenge</summary>
    /// <param name="card">A revenge dicecard</param>
    public virtual void OnRevenge(BattlePlayingCardDataInUnitModel card)
    {
    }

    /// <summary>Adds revenge card to unit</summary>
    /// <param name="unit">A unit for adds card</param>
    /// <param name="display">The card that display in battle</param>
    /// <param name="behaviors">Override behavior if not null</param>
    public static void AddRevengeCard(BattleUnitModel unit, DiceCardXmlInfo display, List<BattleDiceBehavior>? behaviors = null)
    {
        var playcard = unit.CreatePlayingCard(display);

        behaviors?.Let(it =>
        {
            playcard.cardBehaviorQueue = new();

            foreach (var beh in it)
            {
                beh.card = playcard;

                foreach (var abi in beh.abilityList)
                {
                    abi.behavior = beh;
                }

                playcard.cardBehaviorQueue.Enqueue(beh);
            }
        });

        if (!RevengeDice.Cards.ContainsKey(unit))
        {
            RevengeDice.Cards.Add(unit, new());
        }

        RevengeDice.Cards[unit].Enqueue(playcard);
    }

    /// <summary>Adds revenge card to unit</summary>
    /// <param name="unit">A unit for adds card</param>
    /// <param name="card">A card to adds</param>
    public static void AddRevengeCard(BattleUnitModel unit, BattlePlayingCardDataInUnitModel card)
    {
        if (!RevengeDice.Cards.ContainsKey(unit))
        {
            RevengeDice.Cards.Add(unit, new());
        }

        RevengeDice.Cards[unit].Enqueue(card);
    }

    internal static Dictionary<BattleUnitModel, Queue<BattlePlayingCardDataInUnitModel>> Cards { get; set; }

    internal static Dictionary<BattleUnitModel, BattlePlayingCardDataInUnitModel> CurrentRevenge { get; set; }
}
