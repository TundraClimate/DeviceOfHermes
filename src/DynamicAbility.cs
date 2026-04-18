using System.Text;
using LOR_XML;
using HarmonyLib;
using HarmonyExtension;

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
        Context.Command.RunCard(this, ctx.useCard);
    }

    public override void OnStartBattle()
    {
        Context.Command.RunCard(this, ctx.startBattle);
    }

    public override void OnStartParrying()
    {
        Context.Command.RunCard(this, ctx.startParrying);
    }

    public override void OnStartOneSideAction()
    {
        Context.Command.RunCard(this, ctx.startOneside);
    }

    public override void OnWinParryingAtk()
    {
        Context.Command.RunCard(this, ctx.winParrying);
    }

    public override void OnWinParryingDef()
    {
        Context.Command.RunCard(this, ctx.winParrying);
    }

    public override void OnLoseParrying()
    {
        Context.Command.RunCard(this, ctx.loseParrying);
    }

    public override void BeforeRollDice(BattleDiceBehavior behavior)
    {
        Context.Command.RunCard(this, ctx.beforeRollDice);
    }

    public override void OnSucceedAttack(BattleDiceBehavior behavior)
    {
        Context.Command.RunCard(this, ctx.succeedAttack);
    }

    public override void OnRollDice(BattleDiceBehavior behavior)
    {
        Context.Command.RunCard(this, ctx.rollDice);
    }

    private Context ctx = ctx;
}

internal class DynamicDiceAbility(Context ctx) : DiceCardAbilityBase
{
    public override void OnWinParrying()
    {
        Context.Command.RunDice(this, ctx.winParrying);
    }

    public override void OnLoseParrying()
    {
        Context.Command.RunDice(this, ctx.loseParrying);
    }

    public override void BeforeRollDice()
    {
        Context.Command.RunDice(this, ctx.beforeRollDice);
    }

    public override void OnSucceedAttack(BattleUnitModel target)
    {
        Context.Command.RunDice(this, ctx.succeedAttack);
    }

    public override void OnRollDice()
    {
        Context.Command.RunDice(this, ctx.rollDice);
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

        public static void RunCard(DiceCardSelfAbilityBase ability, List<Context.Command> commands)
        {
            foreach (var c in commands)
            {
                switch (c.type)
                {
                    case Context.CommandType.Light:
                        ability.owner?.cardSlotDetail.RecoverPlayPoint(c.A0());
                        break;

                    case Context.CommandType.Draw:
                        ability.owner?.allyCardDetail.DrawCards(c.A0());
                        break;

                    case Context.CommandType.Gain:
                        ability.owner?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], ability.owner);
                        break;

                    case Context.CommandType.NextGain:
                        ability.owner?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], ability.owner);
                        break;

                    case Context.CommandType.NextNextGain:
                        ability.owner?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], ability.owner);
                        break;

                    case Context.CommandType.Inf:
                        ability.card?.target?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], ability.owner);
                        break;

                    case Context.CommandType.NextInf:
                        ability.card?.target?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], ability.owner);
                        break;

                    case Context.CommandType.NextNextInf:
                        ability.card?.target?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], ability.owner);
                        break;

                    case Context.CommandType.Heal:
                        ability.owner?.RecoverHP(c.A0());
                        break;

                    case Context.CommandType.HealBreak:
                        ability.owner?.breakDetail.RecoverBreak(c.A0());
                        break;

                    case Context.CommandType.ApplyDmg:
                        ability.card?.ApplyDiceStatBonus(_ => true, new DiceStatBonus() { dmg = c.A0() });

                        break;

                    case Context.CommandType.ApplyBreakDmg:
                        ability.card?.ApplyDiceStatBonus(_ => true, new DiceStatBonus() { breakDmg = c.A0() });

                        break;

                    case Context.CommandType.ApplyPower:
                        ability.card?.ApplyDiceStatBonus(_ => true, new DiceStatBonus() { power = c.A0() });

                        break;

                    case Context.CommandType.ApplyDmgRate:
                        ability.card?.ApplyDiceStatBonus(_ => true, new DiceStatBonus() { dmgRate = c.A0() });

                        break;

                    case Context.CommandType.ApplyBreakDmgRate:
                        ability.card?.ApplyDiceStatBonus(_ => true, new DiceStatBonus() { breakRate = c.A0() });

                        break;

                    case Context.CommandType.ApplyMin:
                        ability.card?.ApplyDiceStatBonus(_ => true, new DiceStatBonus() { min = c.A0() });

                        break;

                    case Context.CommandType.ApplyMax:
                        ability.card?.ApplyDiceStatBonus(_ => true, new DiceStatBonus() { max = c.A0() });

                        break;
                }
            }
        }

        public static void RunDice(DiceCardAbilityBase ability, List<Context.Command> commands)
        {
            foreach (var c in commands)
            {
                switch (c.type)
                {
                    case Context.CommandType.Light:
                        ability.owner?.cardSlotDetail.RecoverPlayPoint(c.A0());
                        break;

                    case Context.CommandType.Draw:
                        ability.owner?.allyCardDetail.DrawCards(c.A0());
                        break;

                    case Context.CommandType.Gain:
                        ability.owner?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], ability.owner);
                        break;

                    case Context.CommandType.NextGain:
                        ability.owner?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], ability.owner);
                        break;

                    case Context.CommandType.NextNextGain:
                        ability.owner?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], ability.owner);
                        break;

                    case Context.CommandType.Inf:
                        ability.card?.target?.bufListDetail?.AddKeywordBufThisRoundByCard((KeywordBuf)c.args[0], (int)c.args[1], ability.owner);
                        break;

                    case Context.CommandType.NextInf:
                        ability.card?.target?.bufListDetail?.AddKeywordBufByCard((KeywordBuf)c.args[0], (int)c.args[1], ability.owner);
                        break;

                    case Context.CommandType.NextNextInf:
                        ability.card?.target?.bufListDetail?.AddKeywordBufNextNextByCard((KeywordBuf)c.args[0], (int)c.args[1], ability.owner);
                        break;

                    case Context.CommandType.Heal:
                        ability.owner?.RecoverHP(c.A0());
                        break;

                    case Context.CommandType.HealBreak:
                        ability.owner?.breakDetail.RecoverBreak(c.A0());
                        break;

                    case Context.CommandType.ApplyDmg:
                        ability.behavior?.ApplyDiceStatBonus(new DiceStatBonus() { dmg = c.A0() });

                        break;

                    case Context.CommandType.ApplyBreakDmg:
                        ability.behavior?.ApplyDiceStatBonus(new DiceStatBonus() { breakDmg = c.A0() });

                        break;

                    case Context.CommandType.ApplyPower:
                        ability.behavior?.ApplyDiceStatBonus(new DiceStatBonus() { power = c.A0() });

                        break;

                    case Context.CommandType.ApplyDmgRate:
                        ability.behavior?.ApplyDiceStatBonus(new DiceStatBonus() { dmgRate = c.A0() });

                        break;

                    case Context.CommandType.ApplyBreakDmgRate:
                        ability.behavior?.ApplyDiceStatBonus(new DiceStatBonus() { breakRate = c.A0() });

                        break;

                    case Context.CommandType.ApplyMin:
                        ability.behavior?.ApplyDiceStatBonus(new DiceStatBonus() { min = c.A0() });

                        break;

                    case Context.CommandType.ApplyMax:
                        ability.behavior?.ApplyDiceStatBonus(new DiceStatBonus() { max = c.A0() });

                        break;
                }
            }
        }
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
        ApplyDmg,
        ApplyBreakDmg,
        ApplyPower,
        ApplyDmgRate,
        ApplyBreakDmgRate,
        ApplyMin,
        ApplyMax,
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
                "admg" => Context.Command.New(Context.CommandType.ApplyDmg, [ParseNumber(commands, ref i)]),
                "abdmg" => Context.Command.New(Context.CommandType.ApplyBreakDmg, [ParseNumber(commands, ref i)]),
                "apow" => Context.Command.New(Context.CommandType.ApplyPower, [ParseNumber(commands, ref i)]),
                "adr" => Context.Command.New(Context.CommandType.ApplyDmgRate, [ParseNumber(commands, ref i)]),
                "abdr" => Context.Command.New(Context.CommandType.ApplyBreakDmgRate, [ParseNumber(commands, ref i)]),
                "amin" => Context.Command.New(Context.CommandType.ApplyMin, [ParseNumber(commands, ref i)]),
                "amax" => Context.Command.New(Context.CommandType.ApplyMax, [ParseNumber(commands, ref i)]),
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

        if (commands.Length > i && (commands[i] is '+' or '-'))
        {
            buffer.Append(commands[i]);

            i++;
        }

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
