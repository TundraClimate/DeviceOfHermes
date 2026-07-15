using LOR_DiceSystem;
using UnityEngine;
using DeviceOfHermes.AdvancedBase;

namespace DeviceOfHermes.CustomDice;

/// <summary>A dice of equal</summary>
public class EqualDice : AdvancedDiceBase
{
    internal static void Init()
    {
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Detail is BehaviourDetail.Slash &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is EqualDice,
            HermesConstants.EqualSlash,
            new Color(0, 0, 200, 250)
        );
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Detail is BehaviourDetail.Penetrate &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is EqualDice,
            HermesConstants.EqualPenetrate,
            new Color(0, 0, 200, 250)
        );
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Detail is BehaviourDetail.Hit &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is EqualDice,
            HermesConstants.EqualHit,
            new Color(0, 0, 200, 250)
        );
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Detail is BehaviourDetail.Guard &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is EqualDice,
            HermesConstants.EqualGuard,
            new Color(0, 0, 200, 250)
        );
        CustomDiceSprite.AddSequence(
            beh =>
                beh.Detail is BehaviourDetail.Evasion &&
                AssemblyManager.Instance.CreateInstance_DiceCardAbility(beh.Script) is EqualDice,
            HermesConstants.EqualEvasion,
            new Color(0, 0, 200, 250)
        );
    }
}
