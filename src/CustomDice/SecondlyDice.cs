using System.Runtime.CompilerServices;
using UnityEngine;
using LOR_DiceSystem;
using DeviceOfHermes.AdvancedBase;

namespace DeviceOfHermes.CustomDice;

/// <summary>A dice of secondly in clash</summary>
public class SecondlyDice : AdvancedDiceBase
{
    static SecondlyDice()
    {
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Type is BehaviourType.Standby &&
                beh.Detail is BehaviourDetail.Slash &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is SecondlyDice,
            HermesConstants.SecondlySlash,
            new Color(255, 255, 200, 200)
        );
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Type is BehaviourType.Standby &&
                beh.Detail is BehaviourDetail.Penetrate &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is SecondlyDice,
            HermesConstants.SecondlyPenetrate,
            new Color(255, 255, 200, 200)
        );
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Type is BehaviourType.Standby &&
                beh.Detail is BehaviourDetail.Hit &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is SecondlyDice,
            HermesConstants.SecondlyHit,
            new Color(255, 255, 200, 200)
        );
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Type is BehaviourType.Standby &&
                beh.Detail is BehaviourDetail.Guard &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is SecondlyDice,
            HermesConstants.SecondlyGuard,
            new Color(255, 255, 200, 200)
        );
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Type is BehaviourType.Standby &&
                beh.Detail is BehaviourDetail.Evasion &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is SecondlyDice,
            HermesConstants.SecondlyEvasion,
            new Color(255, 255, 200, 200)
        );

        CustomDicePatch.Init();
    }

    /// <summary>Add secondly dice</summary>
    public static void AddSecondlyDice(BattlePlayingCardDataInUnitModel playCard, params List<BattleDiceBehavior> behs)
    {
        _table.GetValue(playCard, _ => new CardModelAdditFields() { _secondlyDiceQueue = new(behs) });
    }

    internal static ConditionalWeakTable<BattlePlayingCardDataInUnitModel, CardModelAdditFields> _table = new();

    internal class CardModelAdditFields
    {
        public BattleDiceBehavior? _tempBehavior;

        public BattleDiceBehavior? _tempSecondly;

        public Dictionary<int, BattleDiceBehavior> _uiBehaviorDict = new();

        public Queue<BattleDiceBehavior> _secondlyDiceQueue = new();

        public bool IsBonusDice()
        {
            return _secondlyDiceQueue.TryPeek(out var peeked)
                && (peeked.isBonusEvasion || peeked.isBonusAttack)
                && !peeked.forbiddenBonusDice;
        }

        public void EndAction(BattlePlayingCardDataInUnitModel playingCard)
        {
            if (_tempBehavior is not null)
            {
                playingCard.currentBehavior = Mem.Take(ref _tempBehavior);
            }

            if (IsBonusDice())
            {
                var dice = _secondlyDiceQueue.Peek();

                _uiBehaviorDict.TryAdd(dice.Index, dice);
                _tempSecondly = dice;
            }
            else if (_secondlyDiceQueue.Count != 0)
            {
                var dice = _secondlyDiceQueue.Dequeue();

                _uiBehaviorDict.TryAdd(dice.Index, dice);
                _tempSecondly = dice;
            }
        }
    }
}
