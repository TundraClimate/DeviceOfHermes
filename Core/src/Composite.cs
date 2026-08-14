using System.Reflection;
using HarmonyLib;

namespace DeviceOfHermes;

/// <summary>Composite marker</summary>
public interface IComposite
{
    /// <summary>Children ids</summary>
    public string[] children { get; }
}

/// <summary>Interface router</summary>
[AttributeUsage(AttributeTargets.Class)]
public class RouteInterfacesAttribute(Type[] interfaces) : Attribute
{
    internal Type[] _interfaces = interfaces;
}

internal static class CompositePatch
{
    internal static void Init()
    {
        var harmony = new Harmony("DeviceOfHermes.Composite");

        harmony.CreateClassProcessor(typeof(PatchInstanceSelf)).Patch();
    }

    [HarmonyPatch(typeof(AssemblyManager), "CreateInstance_DiceCardSelfAbility")]
    class PatchInstanceSelf
    {
        static Exception Finalizer(Exception __exception, ref DiceCardSelfAbilityBase? __result)
        {
            if (__result is IComposite c)
            {
                var children = c.children.Map(id => AssemblyManager.Instance.CreateInstance_DiceCardSelfAbility(id));
                var interfaces = __result.GetType()?.GetTypeInfo()?.GetCustomAttribute<RouteInterfacesAttribute>()?._interfaces ?? [];

                __result = Combiner.CreateClass(__result, children, interfaces) as DiceCardSelfAbilityBase;
            }

            return __exception;
        }
    }
}
