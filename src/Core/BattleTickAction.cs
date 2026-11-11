using HarmonyLib;

namespace DeviceOfHermes;

/// <summary>Manage tick actions in battle</summary>
public class BattleTickAction
{
    static BattleTickAction()
    {
        var harmony = new Harmony("DeviceOfHermes.TickAction");

        harmony.CreateClassProcessor(typeof(PatchOnFrame)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnTick)).Patch();
        harmony.CreateClassProcessor(typeof(PatchBeforeSetBehaviour)).Patch();
        harmony.CreateClassProcessor(typeof(PatchOnSetBehaviour)).Patch();
    }

    /// <summary>An event that invokes every ticks</summary>
    public static event Action OnTick = () => { };

    /// <summary>An event that invokes every frames</summary>
    public static event Action OnFrame = () => { };

    [HarmonyPatch(typeof(StageController), "OnUpdate")]
    private class PatchOnFrame
    {
        static Exception Finalizer(Exception __exception)
        {
            OnFrame.Invoke();

            return __exception;
        }
    }

    [HarmonyPatch(typeof(StageController), "OnFixedUpdate")]
    private class PatchOnTick
    {
        static Exception Finalizer(Exception __exception)
        {
            OnTick.Invoke();

            return __exception;
        }
    }

    [HarmonyPatch(typeof(BattleCardTotalResult), "SetCurrentBuf")]
    internal class PatchBeforeSetBehaviour
    {
        static void Prefix()
        {
            OnFrame.Invoke();
            OnTick.Invoke();
        }
    }

    [HarmonyPatch(typeof(BattleCardTotalResult), "SetBehaviourDiceResultUI")]
    internal class PatchOnSetBehaviour
    {
        static void Prefix()
        {
            StageController.Instance.dontUseUILog = true;
            OnFrame.Invoke();
            OnTick.Invoke();
            StageController.Instance.dontUseUILog = false;
        }
    }
}
