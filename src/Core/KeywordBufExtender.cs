using System.Reflection;
using HarmonyLib;
using HarmonyExtension;
using EnumExtenderV2;
using AutoKeywordUtil;

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

    [HarmonyPatch(typeof(GameSceneManager), "Start")]
    class PatchOnGameStart
    {
        static Exception Finalizer(Exception __exception)
        {
            foreach (var (prop, target) in _mapping)
            {
                var isValid = prop.GetGetMethod(true)?.Let(getter => getter.IsStatic && getter.ReturnType == typeof(KeywordBuf)) == true
                    && prop.GetGetMethod(true) is not null;

                if (!isValid)
                {
                    continue;
                }

                var key = $"TundraClimate_DeviceOfHermes_KeywordBufExtender_{prop.DeclaringType.Assembly.GetName().Name}_{prop.DeclaringType.Name}_{prop.Name}__{target.Name}";

                if (EnumExtender.TryGetValueOf(key, out KeywordBuf res)
                    || EnumExtender.TryFindUnnamedValue((KeywordBuf)94, null, false, out res)
                    && EnumExtender.TryAddName(key, res, false)
                )
                {
                    prop.SetValue(null, res, AccessTools.all, null, null, null);

                    typeof(AutoKeywordUtils).Method(nameof(AutoKeywordUtils.RegisterKeywordBuf))
                        .MakeGenericMethod(target).Invoke(null, null);
                }
            }

            return __exception;
        }
    }
}
