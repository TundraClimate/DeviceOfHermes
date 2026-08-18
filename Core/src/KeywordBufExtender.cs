using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using HarmonyExtension;

namespace DeviceOfHermes;

/// <summary>An attribute the KeywordBufExtender uses</summary>
[AttributeUsage(AttributeTargets.Property)]
public class KeywordBufAttribute(Type target) : Attribute
{
    internal Type _target = target;
}

/// <summary>An attribute the KeywordBufExtender uses</summary>
[AttributeUsage(AttributeTargets.Class)]
public class KeywordBufExtendAttribute : Attribute
{
}

/// <summary>An extender of KeywordBuf</summary>
public static class KeywordBufExtender
{
    static KeywordBufExtender()
    {
        _harmony.CreateClassProcessor(typeof(PatchOnGameStart)).Patch();
    }

    internal static void ExtendAll(Assembly asm)
    {
        foreach (var ty in asm.GetTypes().Filter(ty => ty.GetTypeInfo()?.GetCustomAttribute<KeywordBufExtendAttribute>() is not null))
        {
            ExtendProps(ty);
        }
    }

    /// <summary>Extends inner properties that returns KeywordBuf</summary>
    public static void ExtendProps(Type target)
    {
        var props = target.GetProperties(AccessTools.all);

        foreach (var kwd in props)
        {
            if (kwd.GetCustomAttribute<KeywordBufAttribute>() is KeywordBufAttribute attr)
            {
                _mapping[kwd] = attr._target;
            }
        }
    }

    private static Harmony _harmony = new("DeviceOfHermes.KeywordBufExtender");

    private static Dictionary<PropertyInfo, Type> _mapping = new();

    private static Dictionary<KeywordBuf, Type> _dict = new();

    [HarmonyPatch(typeof(GameSceneManager), "Start")]
    class PatchOnGameStart
    {
        static Exception Finalizer(Exception __exception)
        {
            var next = 94;
            var detail = new BattleUnitBufListDetail(new(0));

            foreach (var (prop, ty) in _mapping)
            {
            BEFORE: try
                {
                    if (next > 999)
                    {
                        Hermes.Say($"KeywordBufExtender timeout: TOO long");

                        break;
                    }

                    var res = addNewKeywordBufInList(detail, BufReadyType.NextRound, (KeywordBuf)next++);

                    if (res is null)
                    {
                        var getter = prop.GetGetMethod(true);

                        if (getter is null)
                        {
                            Hermes.Say($"KeywordBufExtender skip one: {prop.DeclaringType.Assembly.GetName().Name}.{prop.DeclaringType.FullName}.{prop.Name} has not getter", MessageLevel.Warn);

                            continue;
                        }

                        if (prop.GetSetMethod(true) is null)
                        {
                            Hermes.Say($"KeywordBufExtender skip one: {prop.DeclaringType.Assembly.GetName().Name}.{prop.DeclaringType.FullName}.{prop.Name} has not setter", MessageLevel.Warn);

                            continue;
                        }

                        if (!getter.IsStatic)
                        {
                            Hermes.Say($"KeywordBufExtender skip one: {prop.DeclaringType.Assembly.GetName().Name}.{prop.DeclaringType.FullName}.{prop.Name} is not static", MessageLevel.Warn);

                            continue;
                        }

                        if (getter.ReturnType != typeof(KeywordBuf))
                        {
                            Hermes.Say($"KeywordBufExtender skip one: {prop.DeclaringType.Assembly.GetName().Name}.{prop.DeclaringType.FullName}.{prop.Name} is not returns KeywordBuf", MessageLevel.Warn);

                            continue;
                        }

                        prop.SetValue(null, (KeywordBuf)next - 1, AccessTools.all, null, null, null);
                        _dict[(KeywordBuf)next - 1] = ty;

                        continue;
                    }

                    goto BEFORE;
                }
                catch (Exception)
                {
                    goto BEFORE;
                }
            }

            _harmony.CreateClassProcessor(typeof(PatchOnAddKeywordBuf)).Patch();

            return __exception;
        }

        static Func<BattleUnitBufListDetail, BufReadyType, KeywordBuf, BattleUnitBuf> addNewKeywordBufInList
            = (Func<BattleUnitBufListDetail, BufReadyType, KeywordBuf, BattleUnitBuf>)typeof(BattleUnitBufListDetail).Method("AddNewKeywordBufInList")
                .CreateDelegate(typeof(Func<BattleUnitBufListDetail, BufReadyType, KeywordBuf, BattleUnitBuf>));
    }

    [HarmonyPatch(typeof(BattleUnitBufListDetail), "AddNewKeywordBufInList")]
    class PatchOnAddKeywordBuf
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(CodeMatch.IsOpCode(OpCodes.Switch))
                .Advance(1)
                .Insert(
                    new CodeInstruction(OpCodes.Ldloca, 2),
                    CodeInstruction.Local(3),
                    CodeInstruction.Call(typeof(PatchOnAddKeywordBuf).Method("InjectMethod"))
                );

            return matcher.Instructions();
        }

        static void InjectMethod(ref BattleUnitBuf? buf, KeywordBuf bufType)
        {
            if (buf is not null)
            {
                return;
            }

            if (_dict.TryGetValue(bufType, out var res))
            {
                buf = Activator.CreateInstance(res) as BattleUnitBuf;
            }
        }
    }
}
