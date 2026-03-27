using System.Reflection.Emit;
using HarmonyLib;

namespace DeviceOfHermes.AdvancedBase;

public class AdvancedUnitBuf : BattleUnitBuf
{
    static AdvancedUnitBuf()
    {
        var harmony = new Harmony("DeviceOfHermes.AdvancedBases.UnitBuf");

        harmony.CreateClassProcessor(typeof(UnitBufPatch.OriginalAdvInit)).Patch();
        harmony.CreateClassProcessor(typeof(UnitBufPatch.PatchAddBufInitializer)).Patch();
        harmony.CreateClassProcessor(typeof(UnitBufPatch.PatchAddBufWdInitializer)).Patch();
        harmony.CreateClassProcessor(typeof(UnitBufPatch.PatchObserver)).Patch();
        harmony.CreateClassProcessor(typeof(UnitBufPatch.PatchInstant)).Patch();
    }

    public override void Init(BattleUnitModel owner)
    {
        base.Init(owner);

        this.lastStack = this.DefaultStack;
        this.stack = this.DefaultStack;
    }

    public virtual int DefaultStack { get => 0; }

    public virtual bool IsInstant { get => false; }

    public virtual void OnInstant()
    {
    }

    public virtual void OnOtherInstant(AdvancedUnitBuf instant)
    {
    }

    public virtual void OnStackChange(int last)
    {
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

    [HarmonyPatch(typeof(StageController), "OnFixedUpdate")]
    internal static class PatchObserver
    {
        static void Postfix()
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
    }

    [HarmonyPatch(typeof(BattleUnitBufListDetail), "AddBuf")]
    internal static class PatchInstant
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
    }
}
