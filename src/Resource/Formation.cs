using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using HarmonyExtension;

namespace DeviceOfHermes.Resource;

/// <summary>A formation additional data</summary>
public static class Formation
{
    static Formation()
    {
        var harmony = new Harmony("DeviceOfHermes.Resource.Formation");

        harmony.CreateClassProcessor(typeof(PatchStageWaveInit)).Patch();
    }

    /// <summary>Add new info</summary>
    public static void Add(string pid, FormationXmlInfo info)
    {
        var dict = _data.GetValue(FormationXmlList.Instance, _ => new());

        if (!dict.ContainsKey(pid))
        {
            dict[pid] = new();
        }

        dict[pid].Add(info);
    }

    /// <summary>Get info matches id</summary>
    public static FormationXmlInfo? Get(string pid, int id)
    {
        var dict = _data.GetValue(FormationXmlList.Instance, _ => new());

        if (dict.TryGetValue(pid, out var list) && list.Find(i => i.id == id) is FormationXmlInfo info)
        {
            return info;
        }

        return null;
    }

    [HarmonyPatch(typeof(StageWaveModel), "Init")]
    class PatchStageWaveInit
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(
                CodeMatch.Calls(typeof(FormationXmlList).Method("GetData")),
                new CodeMatch(i => i.opcode == OpCodes.Newobj && i.OperandIs(typeof(FormationModel).Ctor([typeof(FormationXmlInfo)]))),
                CodeMatch.IsStfld()
            )
                .Advance(1)
                .Insert(
                    CodeInstruction.Arg(1),
                    CodeInstruction.Arg(2),
                    CodeInstruction.Call(typeof(PatchStageWaveInit).Method("InjectMethod"))
                );

            return matcher.Instructions();
        }

        static FormationXmlInfo InjectMethod(FormationXmlInfo origin, StageModel stage, StageWaveInfo wave)
        {
            if (Get(stage.ClassInfo.workshopID, wave.formationId) is FormationXmlInfo info)
            {
                return info;
            }

            return origin;
        }
    }

    private static ConditionalWeakTable<FormationXmlList, Dictionary<string, List<FormationXmlInfo>>> _data = new();
}
