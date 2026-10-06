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

    protected void AddProcess(int timing, Action<AbilityInstance> procs)
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

    public void RunProcess(int timing, AbilityInstance instance)
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

    private Dictionary<int, Action<AbilityInstance>> _procs = new();
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
    public static CardAbilityContext Create(Token.DynamicAbility token)
    {
        if (_cache.TryGetValue(token, out var res))
        {
            return res;
        }

        var newInstance = new CardAbilityContext(token);

        _cache.Add(token, newInstance);

        return newInstance;
    }

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
            };

            Action<AbilityInstance> root = _ => { };

            foreach (var argument in fn.inner)
            {
                root += ContextCommand.ConvertFrom(argument);
            }

            AddProcess(timing, root);
        }
    }

    private void AddProcess(InvokeTimingCard timing, Action<AbilityInstance> procs)
    {
        AddProcess((int)timing, procs);
    }

    private static Dictionary<Token.DynamicAbility, CardAbilityContext> _cache = new();
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
    public static DiceAbilityContext Create(Token.DynamicAbility token)
    {
        if (_cache.TryGetValue(token, out var res))
        {
            return res;
        }

        var newInstance = new DiceAbilityContext(token);

        _cache.Add(token, newInstance);

        return newInstance;
    }

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

            Action<AbilityInstance> root = _ => { };

            foreach (var argument in fn.inner)
            {
                root += ContextCommand.ConvertFrom(argument);
            }

            AddProcess(timing, root);
        }
    }

    private void AddProcess(InvokeTimingDice timing, Action<AbilityInstance> procs)
    {
        AddProcess((int)timing, procs);
    }

    private static Dictionary<Token.DynamicAbility, DiceAbilityContext> _cache = new();
}

internal class AbilityInstance
{
    public BattleUnitModel? owner { get; private set; }

    public BattlePlayingCardDataInUnitModel? currentDiceAction { get; private set; }

    public BattleDiceBehavior? currentBehavior { get; set; }

    public AbilityInstance(DiceCardSelfAbilityBase ability)
    {
        owner = ability.owner;
        currentDiceAction = ability.card;
    }

    public AbilityInstance(DiceCardAbilityBase ability)
    {
        owner = ability.owner;
        currentDiceAction = ability.behavior?.card;
        currentBehavior = ability.behavior;
    }
}

internal static class ContextCommand
{
    public static Action<AbilityInstance> ConvertFrom(Token.Argument argument)
    {
        return argument.type switch
        {
            Token.ArgumentType.String => ConvertFrom((Token.String)argument.inner),
            Token.ArgumentType.Number => ConvertFrom((Token.Number)argument.inner),
            Token.ArgumentType.Fn => ConvertFrom((Token.Fn)argument.inner),
            _ => throw new InvalidCastException(),
        };
    }

    public static Action<AbilityInstance> ConvertFrom(Token.String str)
    {
        throw new NotImplementedException();
    }

    public static Action<AbilityInstance> ConvertFrom(Token.Number num)
    {
        throw new NotImplementedException();
    }

    public static Action<AbilityInstance> ConvertFrom(Token.Fn fn)
    {
        var name = fn.name.inner;

        Action<AbilityInstance> res = name switch
        {
            "log" or "Log" => Log(fn.CastToStr(0)),
            "light" or "Light" => RecoverPlayPoint(fn.CastToNum(0)),
            "draw" or "Draw" => DrawCard(fn.CastToNum(0)),
            "card" or "GetCard" => GetCard(fn.CastToStr(0), fn.CastToNum(1)),
            "cardexhaust" or "GetCardExhaust" => GetCardExhaust(fn.CastToStr(0), fn.CastToNum(1)),
            "heal" or "Heal" => HealHP(fn.CastToNum(0)),
            "bheal" or "HealBreak" => HealBP(fn.CastToNum(0)),
            "statbonus" or "StatBonus" => fn.CastToStr(0) switch
            {
                "dmg" => ApplyDiceStat(new DiceStatBonus() { dmg = fn.CastToNum(1) }, true, fn.TryCastToNum(2) ?? -1),
                "breakDmg" => ApplyDiceStat(new DiceStatBonus() { breakDmg = fn.CastToNum(1) }, true, fn.TryCastToNum(2) ?? -1),
                "power" => ApplyDiceStat(new DiceStatBonus() { power = fn.CastToNum(1) }, true, fn.TryCastToNum(2) ?? -1),
                "face" => ApplyDiceStat(new DiceStatBonus() { face = fn.CastToNum(1) }, true, fn.TryCastToNum(2) ?? -1),
                "dmgRate" => ApplyDiceStat(new DiceStatBonus() { dmgRate = fn.CastToNum(1) }, true, fn.TryCastToNum(2) ?? -1),
                "breakRate" => ApplyDiceStat(new DiceStatBonus() { breakRate = fn.CastToNum(1) }, true, fn.TryCastToNum(2) ?? -1),
                "min" => ApplyDiceStat(new DiceStatBonus() { min = fn.CastToNum(1) }, true, fn.TryCastToNum(2) ?? -1),
                "max" => ApplyDiceStat(new DiceStatBonus() { max = fn.CastToNum(1) }, true, fn.TryCastToNum(2) ?? -1),
                "guardBreakAdder" => ApplyDiceStat(new DiceStatBonus() { guardBreakAdder = fn.CastToNum(1) }, true, fn.TryCastToNum(2) ?? -1),
                "guardBreakMultiplier" => ApplyDiceStat(new DiceStatBonus() { guardBreakMultiplier = fn.CastToNum(1) }, true, fn.TryCastToNum(2) ?? -1),
                _ => throw new InvalidOperationException($"The '{fn.CastToStr(0)}' is not supported on 'statbonus' Fn"),
            },
            "estatbonus" or "EnemyStatBonus" => fn.CastToStr(0) switch
            {
                "dmg" => ApplyDiceStat(new DiceStatBonus() { dmg = fn.CastToNum(1) }, true, fn.TryCastToNum(2) ?? -1),
                "breakDmg" => ApplyDiceStat(new DiceStatBonus() { breakDmg = fn.CastToNum(1) }, true, fn.TryCastToNum(2) ?? -1),
                "power" => ApplyDiceStat(new DiceStatBonus() { power = fn.CastToNum(1) }, true, fn.TryCastToNum(2) ?? -1),
                "face" => ApplyDiceStat(new DiceStatBonus() { face = fn.CastToNum(1) }, true, fn.TryCastToNum(2) ?? -1),
                "dmgRate" => ApplyDiceStat(new DiceStatBonus() { dmgRate = fn.CastToNum(1) }, true, fn.TryCastToNum(2) ?? -1),
                "breakRate" => ApplyDiceStat(new DiceStatBonus() { breakRate = fn.CastToNum(1) }, true, fn.TryCastToNum(2) ?? -1),
                "min" => ApplyDiceStat(new DiceStatBonus() { min = fn.CastToNum(1) }, true, fn.TryCastToNum(2) ?? -1),
                "max" => ApplyDiceStat(new DiceStatBonus() { max = fn.CastToNum(1) }, true, fn.TryCastToNum(2) ?? -1),
                "guardBreakAdder" => ApplyDiceStat(new DiceStatBonus() { guardBreakAdder = fn.CastToNum(1) }, true, fn.TryCastToNum(2) ?? -1),
                "guardBreakMultiplier" => ApplyDiceStat(new DiceStatBonus() { guardBreakMultiplier = fn.CastToNum(1) }, true, fn.TryCastToNum(2) ?? -1),
                _ => throw new InvalidOperationException($"The '{CastTo<Token.String>(fn, 0).inner}' is not supported on 'estatbonus' Fn"),
            },
            "gain" or "Gain" => GainBuf(fn.CastToStr(0), fn.CastToNum(1), 0),
            "gainr" or "GainReady" => GainBuf(fn.CastToStr(0), fn.CastToNum(1), 1),
            "gainrr" or "GainReadyReady" => GainBuf(fn.CastToStr(0), fn.CastToNum(1), 2),
            "inf" or "Inflict" => InflictBuf(fn.CastToStr(0), fn.CastToNum(1), 0),
            "infr" or "InflictReady" => InflictBuf(fn.CastToStr(0), fn.CastToNum(1), 1),
            "infrr" or "InflictReadyReady" => InflictBuf(fn.CastToStr(0), fn.CastToNum(1), 2),
            "allgain" or "AllGain" => AllGainBuf(fn.CastToStr(0), fn.CastToNum(1), 0),
            "allgainr" or "AllGainReady" => AllGainBuf(fn.CastToStr(0), fn.CastToNum(1), 1),
            "allgainrr" or "AllGainReadyReady" => AllGainBuf(fn.CastToStr(0), fn.CastToNum(1), 2),
            "allinf" or "AllInflict" => AllInflictBuf(fn.CastToStr(0), fn.CastToNum(1), 0),
            "allinfr" or "AllInflictReady" => AllInflictBuf(fn.CastToStr(0), fn.CastToNum(1), 1),
            "allinfrr" or "AllInflictReadyReady" => AllInflictBuf(fn.CastToStr(0), fn.CastToNum(1), 2),
            "takedmg" or "TakeDamage" => TakeDamage(true, fn.CastToNum(0)),
            "givedmg" or "GiveDamage" => TakeDamage(false, fn.CastToNum(0)),
            "takebdmg" or "TakeBreakDamage" => TakeBreakDamage(true, fn.CastToNum(0)),
            "givebdmg" or "GiveBreakDamage" => TakeBreakDamage(false, fn.CastToNum(0)),
            _ => throw new InvalidOperationException($"The '{name}' is not supported with Fn"),
        };

        return res;
    }

    private static string CastToStr(this Token.Fn token, int idx) => CastTo<Token.String>(token, idx).inner;

    private static int CastToNum(this Token.Fn token, int idx) => CastTo<Token.Number>(token, idx).inner;

    private static string? TryCastToStr(this Token.Fn token, int idx) => TryCastTo<Token.String>(token, idx)?.inner;

    private static int? TryCastToNum(this Token.Fn token, int idx) => TryCastTo<Token.Number>(token, idx)?.inner;

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

    public static T? TryCastTo<T>(Token.Fn token, int idx)
        where T : Token
    {
        try
        {
            return CastTo<T>(token, idx);
        }
        catch
        {
            return null;
        }
    }

    public static Action<AbilityInstance> Log(string txt) => _ => Hermes.Say(txt);

    public static Action<AbilityInstance> RecoverPlayPoint(int num) => self => self.owner?.cardSlotDetail?.RecoverPlayPointByCard(num);

    public static Action<AbilityInstance> DrawCard(int num) => self => self.owner?.allyCardDetail?.DrawCards(num);

    public static Action<AbilityInstance> HealHP(int num) => self => self.owner?.RecoverHP(num);

    public static Action<AbilityInstance> HealBP(int num) => self => self.owner?.breakDetail?.RecoverBreak(num);

    public static Action<AbilityInstance> GetCard(string pid, int id) => self => self.owner?.allyCardDetail?.AddNewCard(new LorId(pid, id));

    public static Action<AbilityInstance> GetCardExhaust(string pid, int id)
        => self => self.owner?.allyCardDetail?.AddNewCard(new LorId(pid, id)).exhaust = true;

    public static Action<AbilityInstance> ApplyDiceStat(DiceStatBonus stat, bool isSelf, int idx) => self =>
    {
        if (self.currentBehavior is not null && idx == -1)
        {
            if (isSelf)
            {
                self.currentBehavior.ApplyDiceStatBonus(stat);
            }
            else
            {
                self.currentBehavior.TargetDice?.ApplyDiceStatBonus(stat);
            }
        }
        else
        {
            var matcher = idx == -1 ? _ => true : DiceMatch.DiceByIdx(idx);

            if (isSelf)
            {
                self.currentDiceAction?.ApplyDiceStatBonus(matcher, stat);
            }
            else
            {
                self.currentDiceAction?.target?.currentDiceAction?.ApplyDiceStatBonus(matcher, stat);
            }
        }
    };

    public static Action<AbilityInstance> GainBuf(string bufName, int stack, int turn) => AddBuf(true, bufName, stack, turn);

    public static Action<AbilityInstance> InflictBuf(string bufName, int stack, int turn) => AddBuf(false, bufName, stack, turn);

    public static Action<AbilityInstance> AllGainBuf(string bufName, int stack, int turn) => AddBufAll(true, bufName, stack, turn);

    public static Action<AbilityInstance> AllInflictBuf(string bufName, int stack, int turn) => AddBufAll(false, bufName, stack, turn);

    private static void AddBufImpl(BattleUnitModel target, string bufName, int stack, int turn, BattleUnitModel? owner)
    {
        if (bufName.StartsWith("KeywordBuf_") && Enum.TryParse<KeywordBuf>(bufName.StripPrefix("KeywordBuf_"), out var keyword))
        {
            if (turn == 1)
            {
                target.bufListDetail?.AddKeywordBufByCard(keyword, stack, owner);
            }
            else if (turn == 2)
            {
                target.bufListDetail?.AddKeywordBufNextNextByCard(keyword, stack, owner);
            }
            else
            {
                target.bufListDetail?.AddKeywordBufThisRoundByCard(keyword, stack, owner);
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
    }

    public static Action<AbilityInstance> AddBuf(bool isSelf, string bufName, int stack, int turn) => self =>
    {
        var target = isSelf ? self.owner : self.currentDiceAction?.target;

        if (target is null)
        {
            return;
        }

        AddBufImpl(target, bufName, stack, turn, self.owner);
    };

    public static Action<AbilityInstance> AddBufAll(bool isSelf, string bufName, int stack, int turn) => self =>
    {
        if (self.owner is null)
        {
            return;
        }

        var targets = (isSelf ? self.owner.faction : self.owner.faction.FaceTo()).GetAlives();

        foreach (var target in targets)
        {
            AddBufImpl(target, bufName, stack, turn, self.owner);
        }
    };

    public static Action<AbilityInstance> TakeDamage(bool isSelf, int dmg) => self =>
    {
        var target = isSelf ? self.owner : self.currentDiceAction?.target;

        if (target is null)
        {
            return;
        }

        target.TakeDamage(dmg, attacker: self.owner);
    };

    public static Action<AbilityInstance> TakeBreakDamage(bool isSelf, int dmg) => self =>
    {
        var target = isSelf ? self.owner : self.currentDiceAction?.target;

        if (target is null)
        {
            return;
        }

        target.TakeBreakDamage(dmg, attacker: self.owner);
    };
}
