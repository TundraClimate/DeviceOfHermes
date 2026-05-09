using HarmonyLib;
using DeviceOfHermes;

internal class DynamicAbility
{
    public static void Init()
    {
        var harmony = new Harmony("DeviceOfHermes.DynamicAbility");

        harmony.CreateClassProcessor(typeof(PatchInstanceSelf)).Patch();
        harmony.CreateClassProcessor(typeof(PatchInstanceDice)).Patch();
        harmony.CreateClassProcessor(typeof(PatchDesc)).Patch();
    }

    [HarmonyPatch(typeof(AssemblyManager), "CreateInstance_DiceCardSelfAbility")]
    class PatchInstanceSelf
    {
        static Exception Finalizer(Exception __exception, string name, ref DiceCardSelfAbilityBase? __result)
        {
            if (name.Length <= 4)
            {
                return __exception;
            }

            if (!(name.Substring(0, 4) is "DOHC" or "dohc" or "Card"))
            {
                return __exception;
            }

            try
            {
                var res = new CardAbilityContext(DynamicAbilityParser.Parse(name));

                __result = new DynamicCardAbility(res);
            }
            catch (Exception e)
            {
                Hermes.Say($"{e.Message}");
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(AssemblyManager), "CreateInstance_DiceCardAbility")]
    class PatchInstanceDice
    {
        static Exception Finalizer(Exception __exception, string name, ref DiceCardAbilityBase? __result)
        {
            if (name.Length <= 4)
            {
                return __exception;
            }

            if (!(name.Substring(0, 4) is "DOHD" or "dohd" or "Dice"))
            {
                return __exception;
            }

            try
            {
                var res = new DiceAbilityContext(DynamicAbilityParser.Parse(name));

                __result = new DynamicDiceAbility(res);
            }
            catch (Exception e)
            {
                Hermes.Say($"{e.Message}");
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleCardAbilityDescXmlList), "GetAbilityDesc", [typeof(string)])]
    class PatchDesc
    {
        static void Prefix(ref string id)
        {
            if (DynamicAbilityParser.TryParse(id, out var res))
            {
                id = new DynamicAbilityContext(res).Name;
            }
        }
    }
}

internal class DynamicCardAbility(CardAbilityContext ctx) : DiceCardSelfAbilityBase
{
    public override void OnUseCard()
        => ctx.RunProcess((int)InvokeTimingCard.UseCard, new Instance(this));

    public override void OnStartBattle()
        => ctx.RunProcess((int)InvokeTimingCard.StartBattle, new Instance(this));

    public override void OnStartParrying()
        => ctx.RunProcess((int)InvokeTimingCard.StartParrying, new Instance(this));

    public override void OnStartOneSideAction()
        => ctx.RunProcess((int)InvokeTimingCard.StartOneSide, new Instance(this));

    public override void OnWinParryingAtk()
        => ctx.RunProcess((int)InvokeTimingCard.WinParrying, new Instance(this));

    public override void OnWinParryingDef()
        => ctx.RunProcess((int)InvokeTimingCard.WinParrying, new Instance(this));

    public override void OnLoseParrying()
        => ctx.RunProcess((int)InvokeTimingCard.LoseParrying, new Instance(this));

    public override void BeforeRollDice(BattleDiceBehavior behavior)
        => ctx.RunProcess((int)InvokeTimingCard.BeforeRollDice, new Instance(this) { currentBehavior = behavior });

    public override void OnSucceedAttack(BattleDiceBehavior behavior)
        => ctx.RunProcess((int)InvokeTimingCard.SucceedAttack, new Instance(this) { currentBehavior = behavior });

    public override void OnRollDice(BattleDiceBehavior behavior)
        => ctx.RunProcess((int)InvokeTimingCard.RollDice, new Instance(this) { currentBehavior = behavior });

    private CardAbilityContext ctx = ctx;
}

internal class DynamicDiceAbility(DiceAbilityContext ctx) : DiceCardAbilityBase
{
    public override void OnWinParrying()
        => ctx.RunProcess((int)InvokeTimingDice.WinParrying, new Instance(this));

    public override void OnLoseParrying()
        => ctx.RunProcess((int)InvokeTimingDice.LoseParrying, new Instance(this));

    public override void BeforeRollDice()
        => ctx.RunProcess((int)InvokeTimingDice.BeforeRollDice, new Instance(this));

    public override void OnSucceedAttack(BattleUnitModel target)
        => ctx.RunProcess((int)InvokeTimingDice.SucceedAttack, new Instance(this));

    public override void OnRollDice()
        => ctx.RunProcess((int)InvokeTimingDice.RollDice, new Instance(this));

    private DiceAbilityContext ctx = ctx;
}
