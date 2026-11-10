using System.Reflection.Emit;
using HarmonyLib;

namespace DeviceOfHermes;

internal static class EmotionAbilityFactory
{
    public static void Init()
    {
        var harmony = new Harmony("DeviceOfHermes.EmotionAbilityFactory");

        harmony.CreateClassProcessor(typeof(PatchOnLoadTypes)).Patch();
        harmony.CreateClassProcessor(typeof(PatchBattleEmotionCardModelCtor)).Patch();
    }

    static Dictionary<string, Type> abilities = new();

    [HarmonyPatch(typeof(AssemblyManager), "LoadTypesFromAssembly")]
    class PatchOnLoadTypes
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchEndForward(CodeMatch.IsLdloc(0), CodeMatch.IsLdloc(1), CodeMatch.IsLdelem(), CodeMatch.IsStloc(2))
                .Advance(1)
                .Insert(CodeInstruction.Ldloc(2), CodeInstruction.Call(typeof(PatchOnLoadTypes).Method("InjectMethod")));

            return matcher.Instructions();
        }

        static void InjectMethod(Type type)
        {
            var name = type.Name;

            if (type.IsSubclassOf(typeof(EmotionCardAbilityBase)) && name.StartsWith("EmotionCardAbility_"))
            {
                abilities[name["EmotionCardAbility_".Length..]] = type;
            }
        }
    }

    [HarmonyPatch(typeof(BattleEmotionCardModel), MethodType.Constructor, [typeof(EmotionCardXmlInfo), typeof(BattleUnitModel)])]
    class PatchBattleEmotionCardModelCtor
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(CodeMatch.Calls(typeof(Activator).Method("CreateInstance", [typeof(Type)])), CodeMatch.IsOpCode(OpCodes.Isinst), CodeMatch.IsStloc(2))
                .Insert(CodeInstruction.Ldloc(1), CodeInstruction.Call(typeof(PatchBattleEmotionCardModelCtor).Method("InjectMethod")));

            return matcher.Instructions();
        }

        static Type? InjectMethod(Type? type, string script)
        {
            if (abilities.TryGetValue(script, out var res))
            {
                type ??= res;
            }

            return type;
        }
    }
}
