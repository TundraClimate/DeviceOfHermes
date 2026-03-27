using HarmonyLib;

namespace DeviceOfHermes;

public static class PassiveResolver
{
    static PassiveResolver()
    {
        var harmony = new Harmony("DeviceOfHermes.PassiveResolver");

        harmony.CreateClassProcessor(typeof(PatchPassiveResolveState)).Patch();
    }

    public static void AddRestrict(Func<List<PassiveXmlInfo>, PassiveXmlInfo, bool> restrict)
    {
        _restricts.Add(restrict);
    }

    public static Func<List<PassiveXmlInfo>, PassiveXmlInfo, bool> LockAlways(LorId targetId)
    {
        return (_, target) => target.id == targetId;
    }

    public static Func<List<PassiveXmlInfo>, PassiveXmlInfo, bool> LockIf(LorId targetId, Func<List<PassiveXmlInfo>, bool> ifCond)
    {
        return (passives, target) => target.id == targetId && ifCond(passives);
    }

    public static Func<List<PassiveXmlInfo>, PassiveXmlInfo, bool> LockIfExists(LorId targetId, params LorId[] excludes)
    {
        return LockIf(targetId, passives => passives.Exists(p => excludes.Contains(p.id)));
    }

    public static Func<List<PassiveXmlInfo>, PassiveXmlInfo, bool> Conflict(params LorId[] cand)
    {
        var excludeCand = cand.ToList();

        return (passives, target) => excludeCand.Contains(target.id) && passives.Exists(p => p.id != target.id && excludeCand.Contains(p.id));
    }

    private static List<Func<List<PassiveXmlInfo>, PassiveXmlInfo, bool>> _restricts = new();

    [HarmonyPatch(typeof(BookModel), "CanSuccessionPassive")]
    class PatchPassiveResolveState
    {
        static void Postfix(BookModel __instance, PassiveModel targetpassive, ref bool __result, ref GivePassiveState haspassiveState)
        {
            if (targetpassive is null)
            {
                return;
            }

            var models = __instance.GetPassiveModelList();
            var originals = models.Map(model => model.reservedData.currentpassive).Filter(passive => passive is not null).ToList() ?? new();

            foreach (var restrict in _restricts)
            {
                if (restrict(originals, targetpassive.originpassive))
                {
                    haspassiveState = GivePassiveState.Lock;
                    __result = false;
                }
            }
        }
    }
}
