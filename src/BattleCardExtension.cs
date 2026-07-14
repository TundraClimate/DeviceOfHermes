using LOR_DiceSystem;

namespace DeviceOfHermes;

/// <summary>A extension of BattleCard</summary>
public static class BattleCardExtension
{
    extension(BattleUnitModel owner)
    {
        /// <summary>Creates new <see cref="BattlePlayingCardDataInUnitModel"/></summary>
        public BattlePlayingCardDataInUnitModel CreatePlayingCard(
            DiceCardXmlInfo cardInfo,
            BattleUnitModel? target = null,
            int targetSlotOrder = -1,
            int speedDiceResultValue = 0,
            List<BattleUnitModel>? subTargets = null
        )
        {
            var card = BattleDiceCardModel.CreatePlayingCard(cardInfo);

            card.owner = owner;

            var playcard = new BattlePlayingCardDataInUnitModel()
            {
                owner = owner,
                card = card,
                target = target,
                targetSlotOrder = targetSlotOrder,
                earlyTarget = target,
                earlyTargetOrder = targetSlotOrder,
                cardAbility = card.CreateDiceCardSelfAbilityScript(),
                cardBehaviorQueue = new(),
                speedDiceResultValue = speedDiceResultValue,
                subTargets = subTargets?
                    .Filter(unit => unit != owner)
                    .Map(unit => new BattlePlayingCardDataInUnitModel.SubTarget()
                    {
                        target = unit,
                        targetSlotOrder = RandomUtil.Range(0, unit.speedDiceResult.Count)
                    })
                    .Collect()
            };

            playcard.cardAbility?.card = playcard;

            foreach (var (i, beh) in cardInfo.DiceBehaviourList.Enumerate())
            {
                playcard.cardBehaviorQueue.Enqueue(playcard.CreateBattleDiceBehavior(beh, i));
            }

            return playcard;
        }
    }

    extension(BattlePlayingCardDataInUnitModel playcard)
    {
        /// <summary>Creates new <see cref="BattleDiceBehavior"/></summary>
        public BattleDiceBehavior CreateBattleDiceBehavior(
            DiceBehaviour behaviour,
            int idx = -1
        )
        {
            var beh = new BattleDiceBehavior()
            {
                card = playcard,
                behaviourInCard = behaviour,
                abilityList = string.IsNullOrEmpty(behaviour.Script) ?
                    new() : [AssemblyManager.Instance.CreateInstance_DiceCardAbility(behaviour.Script)],
            };

            foreach (var abi in beh.abilityList)
            {
                abi.behavior = beh;
            }

            beh.SetIndex(0 > idx ? playcard?.cardBehaviorQueue?.Last()?.Index ?? 0 : idx);

            return beh;
        }

        /// <summary>Applies dice ability</summary>
        public void ApplyDiceAbility<T>(Predicate<DiceMatch> match)
            where T : DiceCardAbilityBase, new()
        {
            playcard.ForeachQueue(match, dice => dice.AddAbility(new T()));
        }
    }
}
