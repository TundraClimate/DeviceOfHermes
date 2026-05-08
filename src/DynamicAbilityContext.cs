using LOR_XML;
using DeviceOfHermes.Resource;

namespace DeviceOfHermes;

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

    private Dictionary<int, Action<Instance>> _procs = new();
}

internal enum InvokeTimingCard
{
    UseCard,
    StartBattle,
    StartParrying,
    StartOneSide,
    WinParrying,
    LoseParrying,
    BeforeRollDice,
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

            var timing = fn.name.inner switch
            {
                "uc" or "UseCard" => InvokeTimingCard.UseCard,
                "sb" or "StartBattle" => InvokeTimingCard.StartBattle,
                "sp" or "StartParrying" => InvokeTimingCard.StartParrying,
                "so" or "StartOneSide" => InvokeTimingCard.StartOneSide,
                "wp" or "WinParrying" => InvokeTimingCard.WinParrying,
                "lp" or "LoseParrying" => InvokeTimingCard.LoseParrying,
                "brd" or "BeforeRollDice" => InvokeTimingCard.BeforeRollDice,
                "sa" or "SucceedAttack" => InvokeTimingCard.SucceedAttack,
                "rd" or "RollDice" => InvokeTimingCard.RollDice,
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

    private void AddProcess(InvokeTimingCard timing, Action<Instance> procs)
    {
        AddProcess((int)timing, procs);
    }
}

internal enum InvokeTimingDice
{
    WinParrying,
    LoseParrying,
    BeforeRollDice,
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

            var timing = fn.name.inner switch
            {
                "wp" or "WinParrying" => InvokeTimingDice.WinParrying,
                "lp" or "LoseParrying" => InvokeTimingDice.LoseParrying,
                "brd" or "BeforeRollDice" => InvokeTimingDice.BeforeRollDice,
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
    public BattleUnitModel owner { get; private set; }

    public Instance(DiceCardSelfAbilityBase ability)
    {
        owner = ability.owner;
    }

    public Instance(DiceCardAbilityBase ability)
    {
        owner = ability.owner;
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
            Token.ArgumentType.KeyValue => ConvertFrom((Token.KeyValue)argument.inner),
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

    public static Action<Instance> ConvertFrom(Token.KeyValue kv)
    {
        var name = kv.key.inner;

        Action<Instance> res = name switch
        {
            "light" => RecoverPlayPoint(CastTo<Token.Number>(kv).inner),
            _ => throw new InvalidOperationException($"The '{name}' is not supported with KeyValue"),
        };

        return res;
    }

    public static T CastTo<T>(Token.KeyValue token)
        where T : Token
    {
        var expected = token.type;

        if (token.value is T match)
        {
            return match;
        }

        throw new InvalidOperationException($"Value '{token.key.inner}' expected {expected}, found the '{token.value.GetType().Name}'");
    }

    public static Action<Instance> RecoverPlayPoint(int num) => self => self.owner.cardSlotDetail.RecoverPlayPointByCard(num);
}
