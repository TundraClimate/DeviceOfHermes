using System.Text;
using LOR_XML;
using HarmonyLib;
using DeviceOfHermes.Resource;

namespace DeviceOfHermes;

/// <summary>The config of DynamicAbility</summary>
public static class DynamicAbilityCfg
{
    /// <summary>Add the buf routing</summary>
    public static void AddBattleUnitBufRoute<T>(string key)
        where T : BattleUnitBuf
    {
        if (!BattleUnitBufCompats.TryAdd(key, typeof(T)))
        {
            BattleUnitBufCompats[key] = typeof(T);
        }
    }

    internal static Dictionary<string, Type> BattleUnitBufCompats = new();
}

internal class DynamicAbilityContext
{
    public string Name { get; protected set; }

    public DynamicAbilityContext(Token.DynamicAbility token)
    {
        Name = token.name.inner;
    }

    protected void AddProcess(int timing, Action<Instance> procs)
    {
        if (_procs.ContainsKey(timing))
        {
            _procs[timing] += procs;
        }
        else
        {
            _procs.Add(timing, procs);
        }
    }

    public void RunProcess(int timing, Instance instance)
    {
        if (_procs.TryGetValue(timing, out var proc))
        {
            proc(instance);
        }
    }

    protected void ApplyDesc(Token.Argument[] args)
    {
        if (args.Length == 0)
        {
            throw new InvalidOperationException("Desc needs one input that displayed string");
        }

        var inner = args[0];

        if (inner.type != Token.ArgumentType.String || inner.inner is not Token.String)
        {
            throw new InvalidOperationException("Desc needs string input");
        }

        var desc = ((Token.String)inner.inner).inner;

        TextModel.SetBattleCardAbilityDesc(new BattleCardAbilityDesc() { id = Name, desc = [desc] }, true);

    }

    protected void DebugMode(Token.DynamicAbility token)
    {
        var builder = new StringBuilder();

        builder.AppendLine($"ContextType: {token.prefix.inner}");
        builder.AppendLine($"ContextName: {token.name.inner}");

        foreach (var fn in token.fn)
        {
            builder.AppendLine($"Func:");
            builder.AppendLine($"  Name: {fn.name.inner}");

            if (fn.inner.Length != 0)
            {
                builder.AppendLine($"  Argument:");

                foreach (var argument in fn.inner)
                {
                    builder.AppendLine($"    Type: {argument.type}");

                    switch (argument.type)
                    {
                        case Token.ArgumentType.String:
                            builder.AppendLine($"    Value: {((Token.String)argument.inner).inner}");

                            break;
                        case Token.ArgumentType.Number:
                            builder.AppendLine($"    Value: {((Token.Number)argument.inner).inner}");

                            break;
                        case Token.ArgumentType.Fn:
                            var func = (Token.Fn)argument.inner;

                            builder.AppendLine($"    Name: {func.name.inner}");

                            foreach (var arg in func.inner)
                            {
                                builder.AppendLine($"      Type: {arg.type}");

                                switch (arg.type)
                                {
                                    case Token.ArgumentType.String:
                                        builder.AppendLine($"      Value: {((Token.String)arg.inner).inner}");

                                        break;
                                    case Token.ArgumentType.Number:
                                        builder.AppendLine($"      Value: {((Token.Number)arg.inner).inner}");

                                        break;
                                }
                            }

                            break;
                    }
                }
            }
        }

        Hermes.Say(builder.ToString());
    }

    private Dictionary<int, Action<Instance>> _procs = new();
}

internal enum InvokeTimingCard
{
    UseCard,
    StartBattle,
    StartParrying,
    StartOneSide,
    EndBattle,
    WinParrying,
    LoseParrying,
    DrawParrying,
    BeforeRollDice,
    BeforeGiveDamage,
    SucceedAttack,
    RollDice,
}

internal class CardAbilityContext : DynamicAbilityContext
{
    public CardAbilityContext(Token.DynamicAbility token) : base(token)
    {
        foreach (var fn in token.fn)
        {
            if (fn.name.inner is "desc" or "Desc")
            {
                ApplyDesc(fn.inner);

                continue;
            }

            if (fn.name.inner is "debug" or "Debug")
            {
                DebugMode(token);

                continue;
            }

            var timing = fn.name.inner switch
            {
                "uc" or "UseCard" => InvokeTimingCard.UseCard,
                "sb" or "StartBattle" => InvokeTimingCard.StartBattle,
                "sp" or "StartParrying" => InvokeTimingCard.StartParrying,
                "so" or "StartOneSide" => InvokeTimingCard.StartOneSide,
                "eb" or "EndBattle" => InvokeTimingCard.EndBattle,
                "wp" or "WinParrying" => InvokeTimingCard.WinParrying,
                "lp" or "LoseParrying" => InvokeTimingCard.LoseParrying,
                "dp" or "DrawParrying" => InvokeTimingCard.DrawParrying,
                "brd" or "BeforeRollDice" => InvokeTimingCard.BeforeRollDice,
                "bgd" or "BeforeGiveDamage" => InvokeTimingCard.BeforeGiveDamage,
                "sa" or "SucceedAttack" => InvokeTimingCard.SucceedAttack,
                "rd" or "RollDice" => InvokeTimingCard.RollDice,
                _ => throw new InvalidOperationException($"A function '{fn.name.inner}' is not supported"),
            }
        ;

            Action<Instance> root = _ => { };

            foreach (var argument in fn.inner)
            {
                root += Command.ConvertFrom(argument);
            }

            AddProcess(timing, root);
        }
    }

    private void AddProcess(InvokeTimingCard timing, Action<Instance> procs)
    {
        AddProcess((int)timing, procs);
    }
}

internal enum InvokeTimingDice
{
    WinParrying,
    LoseParrying,
    DrawParrying,
    BeforeRollDice,
    BeforeGiveDamage,
    SucceedAttack,
    RollDice,
}

internal class DiceAbilityContext : DynamicAbilityContext
{
    public DiceAbilityContext(Token.DynamicAbility token) : base(token)
    {
        foreach (var fn in token.fn)
        {
            if (fn.name.inner is "desc" or "Desc")
            {
                ApplyDesc(fn.inner);

                continue;
            }

            if (fn.name.inner is "debug" or "Debug")
            {
                DebugMode(token);

                continue;
            }

            var timing = fn.name.inner switch
            {
                "wp" or "WinParrying" => InvokeTimingDice.WinParrying,
                "lp" or "LoseParrying" => InvokeTimingDice.LoseParrying,
                "dp" or "DrawParrying" => InvokeTimingDice.DrawParrying,
                "brd" or "BeforeRollDice" => InvokeTimingDice.BeforeRollDice,
                "bgd" or "BeforeGiveDamage" => InvokeTimingDice.BeforeGiveDamage,
                "sa" or "SucceedAttack" => InvokeTimingDice.SucceedAttack,
                "rd" or "RollDice" => InvokeTimingDice.RollDice,
                _ => throw new InvalidOperationException($"A function '{fn.name.inner}' is not supported"),
            };

            Action<Instance> root = _ => { };

            foreach (var argument in fn.inner)
            {
                root += Command.ConvertFrom(argument);
            }

            AddProcess(timing, root);
        }
    }

    private void AddProcess(InvokeTimingDice timing, Action<Instance> procs)
    {
        AddProcess((int)timing, procs);
    }
}

internal class Instance
{
    public BattleUnitModel? owner { get; private set; }

    public BattlePlayingCardDataInUnitModel? currentDiceAction { get; set; }

    public BattleDiceBehavior? currentBehavior { get; set; }

    public Instance(DiceCardSelfAbilityBase ability)
    {
        owner = ability.owner;
        currentDiceAction = ability.card;
    }

    public Instance(DiceCardAbilityBase ability)
    {
        owner = ability.owner;
        currentDiceAction = ability.behavior?.card;
        currentBehavior = ability.behavior;
    }
}

internal static class Command
{
    public static Action<Instance> ConvertFrom(Token.Argument argument)
    {
        return argument.type switch
        {
            Token.ArgumentType.String => ConvertFrom((Token.String)argument.inner),
            Token.ArgumentType.Number => ConvertFrom((Token.Number)argument.inner),
            Token.ArgumentType.Fn => ConvertFrom((Token.Fn)argument.inner),
            _ => throw new InvalidCastException(),
        };
    }

    public static Action<Instance> ConvertFrom(Token.String str)
    {
        throw new NotImplementedException();
    }

    public static Action<Instance> ConvertFrom(Token.Number num)
    {
        throw new NotImplementedException();
    }

    public static Action<Instance> ConvertFrom(Token.Fn fn)
    {
        var name = fn.name.inner;

        Action<Instance> res = name switch
        {
            "log" or "Log" => Log(CastTo<Token.String>(fn, 0).inner),
            "light" or "Light" => RecoverPlayPoint(CastTo<Token.Number>(fn, 0).inner),
            "draw" or "Draw" => DrawCard(CastTo<Token.Number>(fn, 0).inner),
            "heal" or "Heal" => HealHP(CastTo<Token.Number>(fn, 0).inner),
            "bheal" or "HealBreak" => HealBP(CastTo<Token.Number>(fn, 0).inner),
            "statbonus" or "StatBonus" => CastTo<Token.String>(fn, 0).inner switch
            {
                "dmg" => ApplyDiceStat(new DiceStatBonus() { dmg = CastTo<Token.Number>(fn, 1).inner }),
                "breakDmg" => ApplyDiceStat(new DiceStatBonus() { breakDmg = CastTo<Token.Number>(fn, 1).inner }),
                "power" => ApplyDiceStat(new DiceStatBonus() { power = CastTo<Token.Number>(fn, 1).inner }),
                "face" => ApplyDiceStat(new DiceStatBonus() { face = CastTo<Token.Number>(fn, 1).inner }),
                "dmgRate" => ApplyDiceStat(new DiceStatBonus() { dmgRate = CastTo<Token.Number>(fn, 1).inner }),
                "breakRate" => ApplyDiceStat(new DiceStatBonus() { breakRate = CastTo<Token.Number>(fn, 1).inner }),
                "min" => ApplyDiceStat(new DiceStatBonus() { min = CastTo<Token.Number>(fn, 1).inner }),
                "max" => ApplyDiceStat(new DiceStatBonus() { max = CastTo<Token.Number>(fn, 1).inner }),
                "guardBreakAdder" => ApplyDiceStat(new DiceStatBonus() { guardBreakAdder = CastTo<Token.Number>(fn, 1).inner }),
                "guardBreakMultiplier" => ApplyDiceStat(new DiceStatBonus() { guardBreakMultiplier = CastTo<Token.Number>(fn, 1).inner }),
                _ => throw new InvalidOperationException($"The '{CastTo<Token.String>(fn, 0).inner}' is not supported on 'stat' Fn"),
            },
            "gain" or "Gain" => GainBuf(CastTo<Token.String>(fn, 0).inner, CastTo<Token.Number>(fn, 1).inner, 0),
            "gainr" or "GainReady" => GainBuf(CastTo<Token.String>(fn, 0).inner, CastTo<Token.Number>(fn, 1).inner, 1),
            "gainrr" or "GainReadyReady" => GainBuf(CastTo<Token.String>(fn, 0).inner, CastTo<Token.Number>(fn, 1).inner, 2),
            "inf" or "Inflict" => InflictBuf(CastTo<Token.String>(fn, 0).inner, CastTo<Token.Number>(fn, 1).inner, 0),
            "infr" or "InflictReady" => InflictBuf(CastTo<Token.String>(fn, 0).inner, CastTo<Token.Number>(fn, 1).inner, 1),
            "infrr" or "InflictReadyReady" => InflictBuf(CastTo<Token.String>(fn, 0).inner, CastTo<Token.Number>(fn, 1).inner, 2),
            _ => throw new InvalidOperationException($"The '{name}' is not supported with Fn"),
        };

        return res;
    }

    public static T CastTo<T>(Token.Fn token, int idx)
        where T : Token
    {
        if (idx >= token.inner.Length)
        {
            throw new InvalidOperationException($"Argument not enough on '{token.name}', required idx:{idx} access");
        }

        var expected = token.inner[idx].type;

        if (token.inner[idx].inner is T match)
        {
            return match;
        }

        throw new InvalidOperationException($"Value '{token.name.inner}[{idx}]' expected {expected}, found the '{token.inner[idx].inner.GetType().Name}'");
    }

    public static Action<Instance> Log(string txt) => _ => Hermes.Say(txt);

    public static Action<Instance> RecoverPlayPoint(int num) => self => self.owner?.cardSlotDetail?.RecoverPlayPointByCard(num);

    public static Action<Instance> DrawCard(int num) => self => self.owner?.allyCardDetail?.DrawCards(num);

    public static Action<Instance> HealHP(int num) => self => self.owner?.RecoverHP(num);

    public static Action<Instance> HealBP(int num) => self => self.owner?.breakDetail?.RecoverBreak(num);

    public static Action<Instance> ApplyDiceStat(DiceStatBonus stat)
    {
        return self =>
        {
            if (self.currentBehavior is not null)
            {
                self.currentBehavior.ApplyDiceStatBonus(stat);
            }
            else
            {
                self.currentDiceAction?.ApplyDiceStatBonus(_ => true, stat);
            }
        };
    }

    public static Action<Instance> GainBuf(string bufName, int stack, int turn)
    {
        return AddBuf(true, bufName, stack, turn);
    }

    public static Action<Instance> InflictBuf(string bufName, int stack, int turn)
    {
        return AddBuf(false, bufName, stack, turn);
    }

    public static Action<Instance> AddBuf(bool isSelf, string bufName, int stack, int turn)
    {
        return self =>
        {
            var target = isSelf ? self.owner : self.currentDiceAction?.target;

            if (target is null)
            {
                return;
            }

            if (bufName.StartsWith("KeywordBuf_") && Enum.TryParse<KeywordBuf>(bufName.StripPrefix("KeywordBuf_"), out var keyword))
            {
                if (turn == 1)
                {
                    target.bufListDetail?.AddKeywordBufByCard(keyword, stack, self.owner);
                }
                else if (turn == 2)
                {
                    target.bufListDetail?.AddKeywordBufNextNextByCard(keyword, stack, self.owner);
                }
                else
                {
                    target.bufListDetail?.AddKeywordBufThisRoundByCard(keyword, stack, self.owner);
                }
            }
            else if (DynamicAbilityCfg.BattleUnitBufCompats.TryGetValue(bufName, out var resType))
            {
                var buf = target.bufListDetail?.GetActivatedBufList()?.Find(buf => buf.GetType() == resType && !buf.IsDestroyed());

                if (buf is null)
                {
                    buf = Activator.CreateInstance(resType, AccessTools.all, null, [], null) as BattleUnitBuf
                        ?? throw new InvalidOperationException($"UnitBuf the '{resType.Name}' has not empty constructor");

                    target.bufListDetail?.AddBuf(buf);
                }

                buf.stack += stack;
            }
            else
            {
                throw new InvalidOperationException($"Keyword the '{bufName}' is not compatible");
            }
        };
    }
}
