using HarmonyLib;
using LOR_XML;

namespace DeviceOfHermes;

internal static class CustomDialogueScript
{
    internal static void Init()
    {
        var harmony = new Harmony("DeviceOfHermes.CustomDialogueScript");

        harmony.CreateClassProcessor(typeof(PatchOnInit)).Patch();
    }

    public static void SetDialogScript(LorId id, string text)
    {
        dialogueScripts[id] = text;
    }

    public static Type? FindType(string script)
    {
        if (cache.TryGetValue(script, out var cached))
        {
            return cached;
        }

        var res = allType.Value.FirstOrDefault(t => t.Name == $"BattleDialogueModel_{script}");

        if (res is not null)
        {
            cache[script] = res;
        }

        return res;
    }

    public static BattleDialogueModel? CreateInstance(LorId id, BattleDialogCharacter dialog)
    {
        if (!dialogueScripts.TryGetValue(id, out var text) || FindType(text) is not Type ty)
        {
            return null;
        }

        return Activator.CreateInstance(ty, [dialog]) as BattleDialogueModel;
    }

    static Lazy<List<Type>> allType = new Lazy<List<Type>>(() => AppDomain.CurrentDomain.GetAssemblies().FlatMap(asm => asm.GetTypes()).Filter(t => t.Name.StartsWith("BattleDialogueModel_")).ToList());

    static Dictionary<string, Type> cache = new();

    static Dictionary<LorId, string> dialogueScripts = new();

    [HarmonyPatch(typeof(UnitDataModel), "InitBattleDialogByDefaultBook")]
    class PatchOnInit
    {
        static Exception Finalizer(Exception __exception, UnitDataModel __instance, LorId lorId)
        {
            var character = BattleDialogXmlList.Instance.GetCharacterData_Mod(lorId.packageId, lorId.id);

            if (lorId.IsWorkshop() && CreateInstance(lorId, character) is BattleDialogueModel model)
            {
                __instance.battleDialogModel = model;
            }

            return __exception;
        }
    }
}
