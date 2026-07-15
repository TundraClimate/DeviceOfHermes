using HarmonyLib;
using HarmonyExtension;

namespace DeviceOfHermes;

/// <summary>An advanced dice stat bonus</summary>
public class AdvancedDiceStatBonus : DiceStatBonus
{
    static AdvancedDiceStatBonus()
    {
        var harmony = new Harmony("DeviceOfHermes.AdvancedDiceStatBonus");

        harmony.CreateClassProcessor(typeof(PatchApplyDiceStat)).Patch();
        harmony.CreateClassProcessor(typeof(PatchUpdateDiceFinalValue)).Patch();
        harmony.CreateClassProcessor(typeof(PatchAddNewKeywordBuf)).Patch();
    }

    private AdvancedDiceStatBonus CopyFrom(DiceStatBonus origin)
    {
        base.dmg += origin.dmg;
        base.breakDmg += origin.breakDmg;
        base.power += origin.power;
        base.face += origin.face;
        base.min += origin.min;
        base.max += origin.max;
        base.dmgRate += origin.dmgRate;
        base.breakRate += origin.breakRate;
        base.guardBreakAdder += origin.guardBreakAdder;
        base.guardBreakMultiplier *= origin.guardBreakMultiplier;

        if (origin.ignorePower)
        {
            this.ignorePower = true;
        }

        return this;
    }

    private void ApplyAdvancedBonus(AdvancedDiceStatBonus bonus)
    {
        this.powerRate += bonus.powerRate;
        this.powerMultiplier *= bonus.powerMultiplier;
        this.kwdBufModifier += bonus.kwdBufModifier;
    }

    /// <summary>Power rate</summary>
    public int powerRate = 0;

    /// <summary>Power multiplier</summary>
    public float powerMultiplier = 1f;

    /// <summary>A keyword buf modifier</summary>
    public KwdBufModifier kwdBufModifier = (_, ref _, _) => { };

    /// <summary>A delegate of KwdBufModifier</summary>
    public delegate void KwdBufModifier(BattleUnitBuf origin, ref BattleUnitBuf? result, BattleUnitModel target);

    [HarmonyPatch(typeof(BattleDiceBehavior), "ApplyDiceStatBonus")]
    class PatchApplyDiceStat
    {
        static Exception Finalizer(Exception __exception, ref DiceStatBonus ____statBonus, DiceStatBonus bonus)
        {
            if (bonus is AdvancedDiceStatBonus adv)
            {
                if (____statBonus is not AdvancedDiceStatBonus)
                {
                    ____statBonus = new AdvancedDiceStatBonus().CopyFrom(____statBonus);
                }

                if (____statBonus is AdvancedDiceStatBonus _adv)
                {
                    _adv.ApplyAdvancedBonus(adv);
                }
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleDiceBehavior), "UpdateDiceFinalValue")]
    class PatchUpdateDiceFinalValue
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);
            var statBonus = typeof(BattleDiceBehavior).Field("_statBonus");

            matcher.MatchEndForward(
                CodeMatch.IsLdarg(0),
                CodeMatch.IsLdfld(statBonus),
                CodeMatch.IsLdfld(typeof(DiceStatBonus).Field("power")),
                CodeMatch.IsStloc()
            )
                .Advance(1)
                .Insert(
                    CodeInstruction.Local(1),
                    CodeInstruction.Instance,
                    CodeInstruction.Field(statBonus),
                    CodeInstruction.Call(typeof(PatchUpdateDiceFinalValue).Method("InjectMethod")),
                    CodeInstruction.SetLocal(1)
                );

            return matcher.Instructions();
        }

        static int InjectMethod(int loc1, DiceStatBonus bonus)
        {
            if (bonus is AdvancedDiceStatBonus adv)
            {
                var mul = adv.powerMultiplier;

                mul += (adv.powerRate * 1f) / 100;

                loc1 = (int)(loc1 * mul);
            }

            return loc1;
        }

        static Exception Finalizer(
            Exception __exception,
            DiceStatBonus ____statBonus,
            int ____diceResultValue,
            ref int ____diceFinalResultValue
        )
        {
            if (____statBonus is AdvancedDiceStatBonus adv && adv.ignorePower)
            {
                ____diceFinalResultValue = 1.Max(____diceResultValue);
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleUnitBufListDetail), "AddNewKeywordBufInList")]
    class PatchAddNewKeywordBuf
    {
        static Exception Finalizer(
            Exception __exception,
            BufReadyType readyType,
            BattleUnitModel ____self,
            List<BattleUnitBuf> ____bufList,
            List<BattleUnitBuf> ____readyBufList,
            List<BattleUnitBuf> ____readyReadyBufList,
            ref BattleUnitBuf? __result
        )
        {
            var beh = StageController.Instance.GetAllCards()
                .Find(card => card == card.owner?.currentDiceAction && card.target == ____self)?.currentBehavior;

            if (beh is BattleDiceBehavior b && _statBonusRef(b) is AdvancedDiceStatBonus bonus)
            {
                var list = readyType switch
                {
                    BufReadyType.NextRound => ____readyBufList,
                    BufReadyType.NextNextRound => ____readyReadyBufList,
                    _ => ____bufList,
                };

                foreach (ref var buf in list.AsRef())
                {
                    if (buf == __result)
                    {
                        bonus.kwdBufModifier(__result, ref buf, beh.card.target);

                        __result = buf;

                        break;
                    }
                }

                list.RemoveAll(e => e is null);
            }

            return __exception;
        }

        static AccessTools.FieldRef<BattleDiceBehavior, DiceStatBonus> _statBonusRef
            = typeof(BattleDiceBehavior).FieldRefAccess<DiceStatBonus>("_statBonus");
    }
}
