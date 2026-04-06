using System.Text;
using LOR_XML;
using HarmonyLib;

internal class DynamicAbility
{
    public static void Init()
    {
        var harmony = new Harmony("DeviceOfHermes.DynamicAbility");

        harmony.CreateClassProcessor(typeof(PatchInstanceSelf)).Patch();
        harmony.CreateClassProcessor(typeof(PatchInstanceDice)).Patch();
        harmony.CreateClassProcessor(typeof(PatchDesc)).Patch();
    }

    public const string CardPrefix = "dohc_";

    public const string DicePrefix = "dohd_";

    public static ref Dictionary<string, BattleCardAbilityDesc> AbilityDescDict =>
        ref _abilityDescRef(BattleCardAbilityDescXmlList.Instance);

    private static readonly AccessTools.FieldRef<BattleCardAbilityDescXmlList, Dictionary<string, BattleCardAbilityDesc>> _abilityDescRef =
        typeof(BattleCardAbilityDescXmlList).FieldRefAccess<Dictionary<string, BattleCardAbilityDesc>>("_dictionary");

    [HarmonyPatch(typeof(AssemblyManager), "CreateInstance_DiceCardSelfAbility")]
    class PatchInstanceSelf
    {
        static Exception Finalizer(Exception __exception, string name, ref DiceCardSelfAbilityBase? __result)
        {
            if (name.ToLower().StartsWith(CardPrefix))
            {
                __result = DynamicAbilityParser.ParseToCardAbility(name);
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(AssemblyManager), "CreateInstance_DiceCardAbility")]
    class PatchInstanceDice
    {
        static Exception Finalizer(Exception __exception, string name, ref DiceCardAbilityBase? __result)
        {
            if (name.ToLower().StartsWith(DicePrefix))
            {
                __result = DynamicAbilityParser.ParseToDiceAbility(name);
            }

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleCardAbilityDescXmlList), "GetAbilityDesc", [typeof(string)])]
    class PatchDesc
    {
        static void Prefix(ref string id)
        {
            var lid = id.ToLower();

            if (lid.Length < 5)
            {
                return;
            }

            var prefix = lid.Substring(0, 5);

            if (prefix != CardPrefix && prefix != DicePrefix)
            {
                return;
            }

            var tmp = id.Split(['_'], 3);

            if (tmp.Length > 2)
            {
                DynamicAbilityParser.InitDesc(id);

                id = tmp[1];
            }
        }
    }
}

internal class DynamicCardAbility(Context ctx) : DiceCardSelfAbilityBase
{
    public override void OnUseCard()
    {
        foreach (var c in ctx.useCard)
        {
            switch (c.type)
            {
                case Context.CommandType.Light:
                    base.owner?.cardSlotDetail.RecoverPlayPoint(c.A0());
                    break;

                case Context.CommandType.Draw:
                    base.owner?.allyCardDetail.DrawCards(c.A0());
                    break;

                case Context.CommandType.Gain:
                    base.owner?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextGain:
                    base.owner?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextGain:
                    base.owner?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Inf:
                    base.card?.target?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Heal:
                    base.owner?.RecoverHP(c.A0());
                    break;

                case Context.CommandType.HealBreak:
                    base.owner?.breakDetail.RecoverBreak(c.A0());
                    break;
            }
        }
    }

    public override void OnStartBattle()
    {
        foreach (var c in ctx.startBattle)
        {
            switch (c.type)
            {
                case Context.CommandType.Light:
                    base.owner?.cardSlotDetail.RecoverPlayPoint(c.A0());
                    break;

                case Context.CommandType.Draw:
                    base.owner?.allyCardDetail.DrawCards(c.A0());
                    break;

                case Context.CommandType.Gain:
                    base.owner?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextGain:
                    base.owner?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextGain:
                    base.owner?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Inf:
                    base.card?.target?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Heal:
                    base.owner?.RecoverHP(c.A0());
                    break;

                case Context.CommandType.HealBreak:
                    base.owner?.breakDetail.RecoverBreak(c.A0());
                    break;
            }
        }
    }

    public override void OnStartParrying()
    {
        foreach (var c in ctx.startParrying)
        {
            switch (c.type)
            {
                case Context.CommandType.Light:
                    base.owner?.cardSlotDetail.RecoverPlayPoint(c.A0());
                    break;

                case Context.CommandType.Draw:
                    base.owner?.allyCardDetail.DrawCards(c.A0());
                    break;

                case Context.CommandType.Gain:
                    base.owner?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextGain:
                    base.owner?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextGain:
                    base.owner?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Inf:
                    base.card?.target?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Heal:
                    base.owner?.RecoverHP(c.A0());
                    break;

                case Context.CommandType.HealBreak:
                    base.owner?.breakDetail.RecoverBreak(c.A0());
                    break;
            }
        }
    }

    public override void OnStartOneSideAction()
    {
        foreach (var c in ctx.startOneside)
        {
            switch (c.type)
            {
                case Context.CommandType.Light:
                    base.owner?.cardSlotDetail.RecoverPlayPoint(c.A0());
                    break;

                case Context.CommandType.Draw:
                    base.owner?.allyCardDetail.DrawCards(c.A0());
                    break;

                case Context.CommandType.Gain:
                    base.owner?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextGain:
                    base.owner?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextGain:
                    base.owner?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Inf:
                    base.card?.target?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Heal:
                    base.owner?.RecoverHP(c.A0());
                    break;

                case Context.CommandType.HealBreak:
                    base.owner?.breakDetail.RecoverBreak(c.A0());
                    break;
            }
        }
    }

    public override void OnWinParryingAtk()
    {
        foreach (var c in ctx.winParrying)
        {
            switch (c.type)
            {
                case Context.CommandType.Light:
                    base.owner?.cardSlotDetail.RecoverPlayPoint(c.A0());
                    break;

                case Context.CommandType.Draw:
                    base.owner?.allyCardDetail.DrawCards(c.A0());
                    break;

                case Context.CommandType.Gain:
                    base.owner?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextGain:
                    base.owner?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextGain:
                    base.owner?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Inf:
                    base.card?.target?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Heal:
                    base.owner?.RecoverHP(c.A0());
                    break;

                case Context.CommandType.HealBreak:
                    base.owner?.breakDetail.RecoverBreak(c.A0());
                    break;
            }
        }
    }

    public override void OnWinParryingDef()
    {
        foreach (var c in ctx.winParrying)
        {
            switch (c.type)
            {
                case Context.CommandType.Light:
                    base.owner?.cardSlotDetail.RecoverPlayPoint(c.A0());
                    break;

                case Context.CommandType.Draw:
                    base.owner?.allyCardDetail.DrawCards(c.A0());
                    break;

                case Context.CommandType.Gain:
                    base.owner?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextGain:
                    base.owner?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextGain:
                    base.owner?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Inf:
                    base.card?.target?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Heal:
                    base.owner?.RecoverHP(c.A0());
                    break;

                case Context.CommandType.HealBreak:
                    base.owner?.breakDetail.RecoverBreak(c.A0());
                    break;
            }
        }
    }

    public override void OnLoseParrying()
    {
        foreach (var c in ctx.loseParrying)
        {
            switch (c.type)
            {
                case Context.CommandType.Light:
                    base.owner?.cardSlotDetail.RecoverPlayPoint(c.A0());
                    break;

                case Context.CommandType.Draw:
                    base.owner?.allyCardDetail.DrawCards(c.A0());
                    break;

                case Context.CommandType.Gain:
                    base.owner?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextGain:
                    base.owner?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextGain:
                    base.owner?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Inf:
                    base.card?.target?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Heal:
                    base.owner?.RecoverHP(c.A0());
                    break;

                case Context.CommandType.HealBreak:
                    base.owner?.breakDetail.RecoverBreak(c.A0());
                    break;
            }
        }
    }

    public override void BeforeRollDice(BattleDiceBehavior behavior)
    {
        foreach (var c in ctx.beforeRollDice)
        {
            switch (c.type)
            {
                case Context.CommandType.Light:
                    base.owner?.cardSlotDetail.RecoverPlayPoint(c.A0());
                    break;

                case Context.CommandType.Draw:
                    base.owner?.allyCardDetail.DrawCards(c.A0());
                    break;

                case Context.CommandType.Gain:
                    base.owner?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextGain:
                    base.owner?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextGain:
                    base.owner?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Inf:
                    base.card?.target?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Heal:
                    base.owner?.RecoverHP(c.A0());
                    break;

                case Context.CommandType.HealBreak:
                    base.owner?.breakDetail.RecoverBreak(c.A0());
                    break;
            }
        }
    }

    public override void OnSucceedAttack(BattleDiceBehavior behavior)
    {
        foreach (var c in ctx.succeedAttack)
        {
            switch (c.type)
            {
                case Context.CommandType.Light:
                    base.owner?.cardSlotDetail.RecoverPlayPoint(c.A0());
                    break;

                case Context.CommandType.Draw:
                    base.owner?.allyCardDetail.DrawCards(c.A0());
                    break;

                case Context.CommandType.Gain:
                    base.owner?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextGain:
                    base.owner?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextGain:
                    base.owner?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Inf:
                    base.card?.target?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Heal:
                    base.owner?.RecoverHP(c.A0());
                    break;

                case Context.CommandType.HealBreak:
                    base.owner?.breakDetail.RecoverBreak(c.A0());
                    break;
            }
        }
    }

    public override void OnRollDice(BattleDiceBehavior behavior)
    {
        foreach (var c in ctx.rollDice)
        {
            switch (c.type)
            {
                case Context.CommandType.Light:
                    base.owner?.cardSlotDetail.RecoverPlayPoint(c.A0());
                    break;

                case Context.CommandType.Draw:
                    base.owner?.allyCardDetail.DrawCards(c.A0());
                    break;

                case Context.CommandType.Gain:
                    base.owner?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextGain:
                    base.owner?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextGain:
                    base.owner?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Inf:
                    base.card?.target?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Heal:
                    base.owner?.RecoverHP(c.A0());
                    break;

                case Context.CommandType.HealBreak:
                    base.owner?.breakDetail.RecoverBreak(c.A0());
                    break;
            }
        }
    }

    private Context ctx = ctx;
}

internal class DynamicDiceAbility(Context ctx) : DiceCardAbilityBase
{
    public override void OnWinParrying()
    {
        foreach (var c in ctx.winParrying)
        {
            switch (c.type)
            {
                case Context.CommandType.Light:
                    base.owner?.cardSlotDetail.RecoverPlayPoint(c.A0());
                    break;

                case Context.CommandType.Draw:
                    base.owner?.allyCardDetail.DrawCards(c.A0());
                    break;

                case Context.CommandType.Gain:
                    base.owner?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextGain:
                    base.owner?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextGain:
                    base.owner?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Inf:
                    base.card?.target?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Heal:
                    base.owner?.RecoverHP(c.A0());
                    break;

                case Context.CommandType.HealBreak:
                    base.owner?.breakDetail.RecoverBreak(c.A0());
                    break;
            }
        }
    }

    public override void OnLoseParrying()
    {
        foreach (var c in ctx.loseParrying)
        {
            switch (c.type)
            {
                case Context.CommandType.Light:
                    base.owner?.cardSlotDetail.RecoverPlayPoint(c.A0());
                    break;

                case Context.CommandType.Draw:
                    base.owner?.allyCardDetail.DrawCards(c.A0());
                    break;

                case Context.CommandType.Gain:
                    base.owner?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextGain:
                    base.owner?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextGain:
                    base.owner?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Inf:
                    base.card?.target?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Heal:
                    base.owner?.RecoverHP(c.A0());
                    break;

                case Context.CommandType.HealBreak:
                    base.owner?.breakDetail.RecoverBreak(c.A0());
                    break;
            }
        }
    }

    public override void BeforeRollDice()
    {
        foreach (var c in ctx.beforeRollDice)
        {
            switch (c.type)
            {
                case Context.CommandType.Light:
                    base.owner?.cardSlotDetail.RecoverPlayPoint(c.A0());
                    break;

                case Context.CommandType.Draw:
                    base.owner?.allyCardDetail.DrawCards(c.A0());
                    break;

                case Context.CommandType.Gain:
                    base.owner?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextGain:
                    base.owner?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextGain:
                    base.owner?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Inf:
                    base.card?.target?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Heal:
                    base.owner?.RecoverHP(c.A0());
                    break;

                case Context.CommandType.HealBreak:
                    base.owner?.breakDetail.RecoverBreak(c.A0());
                    break;
            }
        }
    }

    public override void OnSucceedAttack(BattleUnitModel target)
    {
        foreach (var c in ctx.succeedAttack)
        {
            switch (c.type)
            {
                case Context.CommandType.Light:
                    base.owner?.cardSlotDetail.RecoverPlayPoint(c.A0());
                    break;

                case Context.CommandType.Draw:
                    base.owner?.allyCardDetail.DrawCards(c.A0());
                    break;

                case Context.CommandType.Gain:
                    base.owner?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextGain:
                    base.owner?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextGain:
                    base.owner?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Inf:
                    base.card?.target?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Heal:
                    base.owner?.RecoverHP(c.A0());
                    break;

                case Context.CommandType.HealBreak:
                    base.owner?.breakDetail.RecoverBreak(c.A0());
                    break;
            }
        }
    }

    public override void OnRollDice()
    {
        foreach (var c in ctx.rollDice)
        {
            switch (c.type)
            {
                case Context.CommandType.Light:
                    base.owner?.cardSlotDetail.RecoverPlayPoint(c.A0());
                    break;

                case Context.CommandType.Draw:
                    base.owner?.allyCardDetail.DrawCards(c.A0());
                    break;

                case Context.CommandType.Gain:
                    base.owner?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextGain:
                    base.owner?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextGain:
                    base.owner?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Inf:
                    base.card?.target?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.NextNextInf:
                    base.card?.target?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], base.owner);
                    break;

                case Context.CommandType.Heal:
                    base.owner?.RecoverHP(c.A0());
                    break;

                case Context.CommandType.HealBreak:
                    base.owner?.breakDetail.RecoverBreak(c.A0());
                    break;
            }
        }
    }

    private Context ctx = ctx;
}

internal class Context
{
    public enum ContextType
    {
        Card,
        Dice,
    }

    public class Command
    {
        public static Command New(CommandType type, object[]? args = null)
        {
            return new Command()
            {
                type = type,
                args = args ?? [],
            };
        }

        public int A0()
        {
            return (int)args[0];
        }

        public CommandType type;

        public object[] args = [];
    }

    public enum CommandType
    {
        Light,
        Draw,
        Gain,
        Inf,
        NextGain,
        NextNextGain,
        NextInf,
        NextNextInf,
        Heal,
        HealBreak,
    }

    public ContextType ctxType = ContextType.Card;

    public string name = "";

    public List<Command> useCard = new();

    public List<Command> startBattle = new();

    public List<Command> startParrying = new();

    public List<Command> startOneside = new();

    public List<Command> winParrying = new();

    public List<Command> loseParrying = new();

    public List<Command> beforeRollDice = new();

    public List<Command> succeedAttack = new();

    public List<Command> rollDice = new();

    public string desc = "";
}

internal class DynamicAbilityParser
{
    public static DiceCardSelfAbilityBase? ParseToCardAbility(string script)
    {
        if (ContextCache.TryGetValue(script, out var cachedCtx))
        {
            return cachedCtx is null ? new DiceCardSelfAbilityBase() : new DynamicCardAbility(cachedCtx);
        }
        else
        {
            var ctx = ParseToContext(script);

            ContextCache.Add(script, ctx);

            if (ctx is null)
            {
                return null;
            }

            return new DynamicCardAbility(ctx);
        }
    }

    public static DiceCardAbilityBase? ParseToDiceAbility(string script)
    {
        if (ContextCache.TryGetValue(script, out var cachedCtx))
        {
            return cachedCtx is null ? new DiceCardAbilityBase() : new DynamicDiceAbility(cachedCtx);
        }
        else
        {
            var ctx = ParseToContext(script);

            ContextCache.Add(script, ctx);

            if (ctx is null)
            {
                return null;
            }

            return new DynamicDiceAbility(ctx);
        }
    }

    private static Context? ParseToContext(string script)
    {
        try
        {
            var texts = SplitScript(script);

            if (texts.Count < 3)
            {
                throw new InvalidOperationException($"Specified the '{script}' is command not includes.");
            }

            var contextType = ParseContextType(texts[0]);
            var name = texts[1];

            var ctx = new Context()
            {
                ctxType = contextType,
                name = name,
            };

            foreach (var text in texts.Skip(2))
            {
                ParseCommands(ctx, text);
            }

            if (!string.IsNullOrEmpty(ctx.desc))
            {
                ref var dict = ref DynamicAbility.AbilityDescDict;

                if (dict.ContainsKey(name))
                {
                    dict[name] = new BattleCardAbilityDesc()
                    {
                        id = name,
                        desc = [ctx.desc],
                    };
                }
                else
                {
                    dict.Add(name, new BattleCardAbilityDesc()
                    {
                        id = name,
                        desc = [ctx.desc],
                    });
                }
            }

            return ctx;
        }
        catch (InvalidOperationException e)
        {
            var msg = e.Message;

            Hermes.Say($"Ability not allowed: {script}", MessageLevel.Warn);
            Hermes.Say($"Cause: {msg}", MessageLevel.Warn);

            return null;
        }
    }

    private static List<string> SplitScript(string script)
    {
        var builder = new StringBuilder();
        List<string> texts = new();

        var inBrace = false;

        foreach (var c in script)
        {
            if (c == '_' && !inBrace)
            {
                texts.Add(builder.ToString());
                builder.Clear();

                continue;
            }

            if (c == '(' && !inBrace)
            {
                inBrace = true;
            }

            if (c == ')' && inBrace)
            {
                inBrace = false;
            }

            builder.Append(c);
        }

        if (inBrace)
        {
            throw new InvalidOperationException($"Unclosed brace in the '{script} found.'");
        }

        if (builder.Length > 0)
        {
            texts.Add(builder.ToString());
        }

        return texts;
    }

    private static Context.ContextType ParseContextType(string input)
    {
        var lower = input.ToLower();

        if (lower == "dohc")
        {
            return Context.ContextType.Card;
        }
        else if (lower == "dohd")
        {
            return Context.ContextType.Dice;
        }
        else
        {
            throw new InvalidOperationException($"A input the '{input}' is not supported type syntax.");
        }
    }

    private static void ParseCommands(Context ctx, string input)
    {
        var lower = input.ToLower();

        if (lower.StartsWith("uc"))
        {
            ctx.useCard = ParseCommand(ParseBrace(input.Substring(2)));
        }
        else if (lower.StartsWith("sb"))
        {
            ctx.startBattle = ParseCommand(ParseBrace(input.Substring(2)));
        }
        else if (lower.StartsWith("sp"))
        {
            ctx.startParrying = ParseCommand(ParseBrace(input.Substring(2)));
        }
        else if (lower.StartsWith("so"))
        {
            ctx.startOneside = ParseCommand(ParseBrace(input.Substring(2)));
        }
        else if (lower.StartsWith("wp"))
        {
            ctx.winParrying = ParseCommand(ParseBrace(input.Substring(2)));
        }
        else if (lower.StartsWith("lp"))
        {
            ctx.loseParrying = ParseCommand(ParseBrace(input.Substring(2)));
        }
        else if (lower.StartsWith("brd"))
        {
            ctx.beforeRollDice = ParseCommand(ParseBrace(input.Substring(3)));
        }
        else if (lower.StartsWith("sa"))
        {
            ctx.succeedAttack = ParseCommand(ParseBrace(input.Substring(2)));
        }
        else if (lower.StartsWith("rd"))
        {
            ctx.rollDice = ParseCommand(ParseBrace(input.Substring(2)));
        }
        else if (lower.StartsWith("desc"))
        {
            ctx.desc = ParseBrace(input.Substring(4));
        }
        else
        {
            throw new InvalidOperationException($"No matches command type the '{input}' that input.");
        }
    }

    private static List<Context.Command> ParseCommand(string commands)
    {
        var buffer = new StringBuilder();
        List<Context.Command> cmds = new();

        var i = 0;

        while (commands.Length > i)
        {
            buffer.Append(commands[i]);

            i++;

            var bufStr = buffer.ToString();

            var cmd = bufStr.ToLower() switch
            {
                "light" => Context.Command.New(Context.CommandType.Light, [ParseNumber(commands, ref i)]),
                "draw" => Context.Command.New(Context.CommandType.Draw, [ParseNumber(commands, ref i)]),
                "gain" => Context.Command.New(Context.CommandType.Gain, [ParseKeywordBuf(commands, ref i), ParseNumber(commands, ref i)]),
                "inf" => Context.Command.New(Context.CommandType.Inf, [ParseKeywordBuf(commands, ref i), ParseNumber(commands, ref i)]),
                "ngain" => Context.Command.New(Context.CommandType.NextGain, [ParseKeywordBuf(commands, ref i), ParseNumber(commands, ref i)]),
                "nngain" => Context.Command.New(Context.CommandType.NextNextGain, [ParseKeywordBuf(commands, ref i), ParseNumber(commands, ref i)]),
                "ninf" => Context.Command.New(Context.CommandType.NextInf, [ParseKeywordBuf(commands, ref i), ParseNumber(commands, ref i)]),
                "nninf" => Context.Command.New(Context.CommandType.NextNextInf, [ParseKeywordBuf(commands, ref i), ParseNumber(commands, ref i)]),
                "heal" => Context.Command.New(Context.CommandType.Heal, [ParseNumber(commands, ref i)]),
                "bheal" => Context.Command.New(Context.CommandType.HealBreak, [ParseNumber(commands, ref i)]),
                _ => null,
            };

            if (cmd is not null)
            {
                cmds.Add(cmd);
                buffer.Clear();
            }
        }

        if (buffer.Length > 0)
        {
            throw new InvalidOperationException($"The command '{commands}' is not supported.");
        }

        return cmds;
    }

    private static int ParseNumber(string commands, ref int i)
    {
        var buffer = new StringBuilder();

        while (commands.Length > i && char.IsNumber(commands[i]))
        {
            buffer.Append(commands[i]);

            i++;
        }

        try
        {
            return int.Parse(buffer.ToString());
        }
        catch
        {
            throw new InvalidOperationException($"The argument number '{buffer.ToString()}' is invalid.");
        }
    }

    private static KeywordBuf ParseKeywordBuf(string commands, ref int i)
    {
        var buffer = new StringBuilder();

        while (commands.Length > i)
        {
            buffer.Append(commands[i]);

            i++;

            if (Enum.TryParse<KeywordBuf>(buffer.ToString(), ignoreCase: true, out var keywordBuf))
            {
                return keywordBuf;
            }
        }

        throw new InvalidOperationException("The Specified input cannot parse to KeywordBuf.");
    }

    private static string ParseBrace(string commands)
    {
        if (commands.Length < 3)
        {
            throw new InvalidOperationException("The empty brace found.");
        }

        if (commands[0] != '(' || commands[commands.Length - 1] != ')')
        {
            throw new InvalidOperationException($"The argument '{commands}' is not brace.");
        }

        return commands.Substring(1, commands.Length - 2);
    }

    public static void InitDesc(string script)
    {
        if (script.Length < 5)
        {
            return;
        }

        if (ContextCache.ContainsKey(script))
        {
            return;
        }

        var prefix = script.Substring(0, 5).ToLower();

        if (prefix == DynamicAbility.CardPrefix)
        {
            ParseToCardAbility(script);
        }
        else if (prefix == DynamicAbility.DicePrefix)
        {
            ParseToDiceAbility(script);
        }
    }

    public static Dictionary<string, Context?> ContextCache = new();
}
