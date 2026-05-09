using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Concurrent;
using HarmonyLib;

namespace DeviceOfHermes;

internal class DynamicAbilityParser
{
    public static Token.DynamicAbility Parse(string script)
    {
        if (_cache.TryGetValue(script, out var cached))
        {
            return cached;
        }

        var stream = new ParseStream(script);
        var res = stream.Parse<Token.DynamicAbility>();

        if (res is null || stream.Error.IsError)
        {
            var builder = new StringBuilder();

            if (stream.Error.IsError)
            {
                builder.AppendLine(stream.Error.message);
                builder.AppendLine(script);
                builder.Append(string.Format("{0}^", new string(' ', stream.Error.pos!.Value)));
            }
            else
            {
                builder.AppendLine("Script tokenize was failed");
                builder.Append(script);
            }

            throw new InvalidOperationException(builder.ToString());
        }

        _cache.Add(script, res);

        return res;
    }

    public static bool TryParse(string script, [NotNullWhen(true)] out Token.DynamicAbility? res)
    {
        try
        {
            res = Parse(script);

            return true;
        }
        catch
        {
            res = null;

            return false;
        }
    }

    private static Dictionary<string, Token.DynamicAbility> _cache = new();
}

internal class Token
{
    public class DynamicAbility(Prefix prefix, Ident name, Fn[] fn) : Token
    {
        public Prefix prefix = prefix;

        public Ident name = name;

        public Fn[] fn = fn;

        Token? Parse(ParseStream stream)
        {
            if (!stream.TryParse<Prefix>(out var prefix))
                return null;

            if (!stream.TryParse<Separator>(out var _))
                return null;

            if (!stream.TryParse<Ident>(out var name))
                return null;

            List<Fn> fns = new();

            while (stream.TryParse<Separator>(out var _) && stream.TryParse<Fn>(out var fn))
            {
                fns.Add(fn);
            }

            if (stream.Remaining > 0)
            {
                return null;
            }

            return new DynamicAbility(prefix, name, fns.ToArray());
        }
    }

    public class Separator : Token
    {
        Token? Parse(ParseStream stream) => stream.Parse('-')?.Let(_ => new Separator());
    }

    public enum PrefixType
    {
        Card,
        Dice,
    }

    public class Prefix(PrefixType inner) : Token
    {
        public PrefixType inner = inner;

        Token? Parse(ParseStream stream)
        {
            if (stream.Parse("DOHC") is not null || stream.Parse("dohc") is not null || stream.Parse("Card") is not null)
            {
                return new Prefix(PrefixType.Card);
            }
            else if (stream.Parse("DOHD") is not null || stream.Parse("dohd") is not null || stream.Parse("Dice") is not null)
            {
                return new Prefix(PrefixType.Dice);
            }

            return null;
        }
    }

    public class Ident(string inner) : Token
    {
        public string inner = inner;

        Token? Parse(ParseStream stream)
        {
            var builder = new StringBuilder();

            while (stream.Remaining > 0)
            {
                var peeked = stream.Peek();

                if (peeked is '-' or '(' or ')' or '[' or ']' or '{' or '}' or ' ' or '\'')
                {
                    break;
                }

                builder.Append(stream.Parse(peeked!.Value));
            }

            if (builder.Length == 0)
            {
                return null;
            }

            return new Ident(builder.ToString());
        }
    }

    public class String(string inner) : Token
    {
        public string inner = inner;

        Token? Parse(ParseStream stream)
        {
            if (stream.Parse('\'') is null)
            {
                return null;
            }

            var builder = new StringBuilder();
            var closed = false;

            while (stream.Remaining > 0)
            {
                var parsed = stream.Parse(stream.Peek()!.Value);

                if (parsed == '\'')
                {
                    closed = true;

                    break;
                }

                builder.Append(parsed);
            }

            if (!closed)
            {
                stream.SetError("String not closed");

                return null;
            }

            return new String(builder.ToString());
        }
    }

    public class Number(int inner) : Token
    {
        public int inner = inner;

        Token? Parse(ParseStream stream)
        {
            var sign = stream.Peek();
            var builder = new StringBuilder();

            if (sign is '+' or '-')
            {
                if (stream.Peek(2) is >= '0' and <= '9')
                {
                    builder.Append(stream.Parse(sign.Value));
                }
                else
                {
                    return null;
                }
            }

            while (stream.Remaining > 0)
            {
                var peeked = stream.Peek();

                if (peeked is >= '0' and <= '9')
                {
                    builder.Append(stream.Parse(peeked.Value));
                }
                else
                {
                    break;
                }
            }

            if (builder.Length == 0)
            {
                return null;
            }

            if (!int.TryParse(builder.ToString(), out var parsed))
            {
                stream.SetError("Parser expected number");

                return null;
            }

            return new Number(parsed);
        }
    }

    public class FnSeparator : Token
    {
        Token? Parse(ParseStream stream) => stream.Parse(',')?.Let(_ => new FnSeparator());
    }

    public class Space : Token
    {
        Token? Parse(ParseStream stream)
        {
            while (stream.Remaining > 0)
            {
                var peeked = stream.Peek();

                if (peeked == ' ')
                {
                    stream.Parse(peeked.Value);

                    continue;
                }

                break;
            }

            return new Space();
        }
    }

    public class Fn(Ident name, Argument[] args) : Token
    {
        public Ident name = name;

        public Argument[] inner = args;

        Token? Parse(ParseStream stream)
        {
            if (!stream.TryParse<Ident>(out var name))
                return null;

            if (stream.Peek() != '(')
            {
                stream.SetError("Parentheses not found");

                return null;
            }

            var paren = stream.Parenthesized();

            if (paren is null)
            {
                return null;
            }

            List<Argument> args = new();

            if (paren.TryParse<Argument>(out var a))
            {
                args.Add(a);

                while (paren.TryParse<FnSeparator>(out var _) && paren.TryParse<Space>(out var _) && paren.TryParse<Argument>(out var argument))
                {
                    args.Add(argument);
                }
            }

            if (paren.Remaining > 0)
            {
                return null;
            }

            return new Fn(name, args.ToArray());
        }
    }

    public enum ArgumentType
    {
        String,
        Number,
        Fn,
    }

    public class Argument(ArgumentType type, Token inner) : Token
    {
        public ArgumentType type = type;

        // String || Number || Fn
        public Token inner = inner;

        Token? Parse(ParseStream stream)
        {
            ArgumentType type;
            Token? inner;

            var peeked = stream.Peek();

            if (peeked == '\'')
            {
                type = ArgumentType.String;
                inner = stream.Parse<String>();
            }
            else if (peeked is '+' or '-' or (>= '0' and <= '9'))
            {
                type = ArgumentType.Number;
                inner = stream.Parse<Number>();
            }
            else
            {
                type = ArgumentType.Fn;
                inner = stream.Parse<Fn>();
            }

            if (inner is null)
            {
                return null;
            }

            return new Argument(type, inner);
        }
    }
}

internal class ParseStream(string script)
{
    public ParseError Error = new ParseError();

    public ParseStream Fork(string script)
    {
        return new ParseStream(script) { Error = Error, _mergin = _cursor - script.Length };
    }

    public ParseStream Fork()
    {
        return Fork(_script.Substring(_cursor + 1));
    }

    public void SetError(string msg)
    {
        Error.message = msg;
        Error.pos = _cursor + 1 + _mergin;
    }

    public int Remaining => _script.Length - (_cursor + 1);

    public T? Parse<T>()
        where T : Token
    {
        var res = (Func<ParseStream, Token?>)_cache.GetOrAdd(typeof(T), _ =>
        {
            var method = typeof(T).GetMethod("Parse", AccessTools.all);

            if (method is null)
            {
                throw new NotImplementedException($"Not implemented 'Parse' in {typeof(T).Name}");
            }

            return method.CreateDelegate(typeof(Func<ParseStream, Token?>), null);
        });

        return res(this) as T;
    }

    public bool TryParse<T>([NotNullWhen(true)] out T? res)
        where T : Token
    {
        res = Parse<T>();

        return res is not null;
    }

    public char? Peek(int advance = 1)
    {
        if (advance > Remaining)
        {
            return null;
        }

        return _script[_cursor + advance];
    }

    public char? Parse(char match)
    {
        var peeked = Peek();

        if (peeked != match)
        {
            return null;
        }

        _cursor++;

        return peeked;
    }

    public string? Parse(string match)
    {
        if (match.Length > Remaining)
        {
            return null;
        }

        var start = _cursor + 1;
        var end = start + match.Length;

        var strip = _script.Substring(start, end);

        if (strip != match)
        {
            return null;
        }

        _cursor += match.Length;

        return strip;
    }

    public ParseStream? Parenthesized()
    {
        if (Parse('(') is null)
        {
            return null;
        }

        var depth = 0;
        var builder = new StringBuilder();
        var closed = false;

        while (Remaining > 0)
        {
            var parsed = Parse(Peek()!.Value);

            if (parsed == '(')
            {
                depth++;
            }
            else if (parsed == ')')
            {
                if (depth > 0)
                {
                    depth--;
                }
                else
                {
                    closed = true;

                    break;
                }
            }

            builder.Append(parsed);
        }

        if (!closed)
        {
            SetError("Parentheses not closed");

            return null;
        }

        return Fork(builder.ToString());
    }

    private string _script = script;

    private int _cursor = -1;

    private int _mergin = 0;

    private static ConcurrentDictionary<Type, Delegate> _cache = new();
}

internal class ParseError
{
    public bool IsError => message is not null;

    public string? message;

    public int? pos;
}
