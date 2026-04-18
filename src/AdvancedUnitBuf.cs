using System.Reflection.Emit;
using HarmonyLib;
using HarmonyExtension;

namespace DeviceOfHermes.AdvancedBase;

/// <summary>An advanced <see cref="BattleUnitBuf"/></summary>
/// <remarks>
/// Can replaces <c>BattleUnitBuf</c> into this.
/// </remarks>
/// <example><code>
/// public class BattleUnitBuf_Advance : AdvancedUnitBuf
/// {
/// }
/// </code></example>
public class AdvancedUnitBuf : BattleUnitBuf
{
    static AdvancedUnitBuf()
    {
        var harmony = new Harmony("DeviceOfHermes.AdvancedBases.UnitBuf");

        harmony.CreateClassProcessor(typeof(UnitBufPatch.OriginalAdvInit)).Patch();
        harmony.CreateClassProcessor(typeof(UnitBufPatch.PatchAddBufInitializer)).Patch();
        harmony.CreateClassProcessor(typeof(UnitBufPatch.PatchAddBufWdInitializer)).Patch();

        BattleTickAction.OnTick += OnTick;
    }

    /// <summary>Initialize UnitBuf</summary>
    /// <remarks>
    /// AdvancedUnitBuf is implemented force initializer.<br/>
    /// Dont calls <c>base.Init(onwer)</c>.
    /// </remarks>
    public override void Init(BattleUnitModel owner)
    {
        base.Init(owner);

        this.lastStack = this.DefaultStack;
        this.stack = this.DefaultStack;
    }

    /// <summary>The stack of Unitbuf on inflicted</summary>
    /// <returns>Returns default stack</returns>
    public virtual int DefaultStack { get => 0; }

    /// <summary>Is Unitbuf is instant</summary>
    /// <returns>Is instant buf if true</returns>
    public virtual bool IsInstant { get => false; }

    /// <summary>The instant buf on inflicted</summary>
    /// <remarks>
    /// Only activates <see cref="IsInstant"/> is true.
    /// </remarks>
    public virtual void OnInstant()
    {
    }

    /// <summary>Other instants on inflicted</summary>
    /// <param name="instant">inflicted other instant</param>
    public virtual void OnOtherInstant(AdvancedUnitBuf instant)
    {
    }

    /// <summary>Unitbuf stack on changed</summary>
    public virtual void OnStackChange(int last)
    {
    }

    internal static void OnTick()
    {
        var alives = BattleObjectManager.instance.GetAliveList();

        foreach (var unit in alives)
        {
            foreach (var buf in unit.bufListDetail?.GetActivatedBufList() ?? new())
            {
                if (buf is AdvancedUnitBuf advBuf && advBuf.stack != advBuf.lastStack)
                {
                    advBuf.OnStackChange(advBuf.lastStack);

                    advBuf.lastStack = advBuf.stack;
                }
            }
        }
    }

    internal int lastStack;
}

internal class UnitBufPatch
{
    [HarmonyPatch(typeof(AdvancedUnitBuf), "Init")]
    internal static class OriginalAdvInit
    {
        [HarmonyReversePatch]
        internal static void Init(AdvancedUnitBuf instance, BattleUnitModel owner) =>
            throw new NotImplementedException();
    }

    [HarmonyPatch(typeof(BattleUnitBufListDetail), "AddBuf")]
    internal static class PatchAddBufInitializer
    {
        static bool Prefix(BattleUnitBufListDetail __instance, BattleUnitBuf buf, BattleUnitModel ____self)
        {
            if (!__instance.CanAddBuf(buf))
            {
                return false;
            }

            if (buf is AdvancedUnitBuf adv && adv.IsInstant)
            {
                adv.Init(____self);
                adv.OnInstant();

                foreach (var unit in BattleObjectManager.instance.GetAliveList())
                {
                    foreach (var otherBuf in unit?.bufListDetail?.GetActivatedBufList() ?? new())
                    {
                        if (otherBuf is AdvancedUnitBuf otherAdv)
                        {
                            otherAdv.OnOtherInstant(adv);
                        }
                    }
                }

                return false;
            }

            return true;
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var inject = AccessTools.Method(typeof(PatchAddBufInitializer), "Init");
            var target = AccessTools.Method(typeof(BattleUnitBuf), "Init");

            var _self = AccessTools.Field(typeof(BattleUnitBufListDetail), "_self");

            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(CodeMatch.Calls(target))
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, _self),
                    new CodeInstruction(OpCodes.Call, inject)
                );

            return matcher.Instructions();
        }

        static void Init(BattleUnitBuf instance, BattleUnitModel owner)
        {
            if (instance is AdvancedUnitBuf advInstance)
            {
                OriginalAdvInit.Init(advInstance, owner);
            }
        }
    }

    [HarmonyPatch(typeof(BattleUnitBufListDetail), "AddBufWithoutDuplication")]
    internal static class PatchAddBufWdInitializer
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var inject = AccessTools.Method(typeof(PatchAddBufWdInitializer), "Init");
            var target = AccessTools.Method(typeof(BattleUnitBuf), "Init");

            var _self = AccessTools.Field(typeof(BattleUnitBufListDetail), "_self");

            var matcher = new CodeMatcher(instructions);

            matcher.MatchStartForward(CodeMatch.Calls(target))
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, _self),
                    new CodeInstruction(OpCodes.Call, inject)
                );

            return matcher.Instructions();
        }

        static void Init(BattleUnitBuf instance, BattleUnitModel owner)
        {
            if (instance is AdvancedUnitBuf advInstance)
            {
                OriginalAdvInit.Init(advInstance, owner);
            }
        }
    }
}
