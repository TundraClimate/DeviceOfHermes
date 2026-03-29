using HarmonyLib;

namespace DeviceOfHermes;

/// <summary>Resolver the passive attribute succession</summary>
/// <remarks>
/// Restricts passive attribution with cond func.<br/>
/// <see cref="AddRestrict"/> is restrict adds for Game in running.
/// </remarks>
/// <example><code>
/// PassiveResolver.AddRestrict(PassiveResolver.LockAlways(new LorId(1)));
/// PassiveResolver.AddRestrict(PassiveResolver.Conflict(new LorId(2), new LorId(3)));
/// </code></example>
public static class PassiveResolver
{
    static PassiveResolver()
    {
        var harmony = new Harmony("DeviceOfHermes.PassiveResolver");

        harmony.CreateClassProcessor(typeof(PatchPassiveResolveState)).Patch();
    }

    /// <summary>Add restrict for Resolver</summary>
    /// <param name="restrict">If returns true, locks that passive</param>
    /// <remarks>
    /// The <paramref name="restrict"/> arg0: passive attributes<br/>
    /// The <paramref name="restrict"/> arg1: restricts target passive
    /// If <paramref name="restrict"/> returns true, that passive is locking.
    /// </remarks>
    /// <example><code>
    /// </code></example>
    public static void AddRestrict(Func<List<PassiveXmlInfo>, PassiveXmlInfo, bool> restrict)
    {
        _restricts.Add(restrict);
    }

    /// <summary>A helper of lock always</summary>
    /// <param name="targetId">Lock passive ID</param>
    public static Func<List<PassiveXmlInfo>, PassiveXmlInfo, bool> LockAlways(LorId targetId)
    {
        return (_, target) => target.id == targetId;
    }

    /// <summary>A helper of lock if</summary>
    /// <param name="targetId">Lock passive ID</param>
    /// <param name="ifCond">Locking cond</param>
    public static Func<List<PassiveXmlInfo>, PassiveXmlInfo, bool> LockIf(LorId targetId, Func<List<PassiveXmlInfo>, bool> ifCond)
    {
        return (passives, target) => target.id == targetId && ifCond(passives);
    }

    /// <summary>A helper of lock if exists</summary>
    /// <param name="targetId">Lock passive ID</param>
    /// <param name="excludes">Exclude ids</param>
    public static Func<List<PassiveXmlInfo>, PassiveXmlInfo, bool> LockIfExists(LorId targetId, params LorId[] excludes)
    {
        return LockIf(targetId, passives => passives.Exists(p => excludes.Contains(p.id)));
    }

    /// <summary>A helper of confilict ids</summary>
    /// <param name="cand">Conflict passive candidates</param>
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
