using HarmonyLib;

namespace DeviceOfHermes;

/// <summary>Manage tick actions in battle</summary>
public class BattleTickAction
{
    static BattleTickAction()
    {
        var harmony = new Harmony("DeviceOfHermes.TickAction");

        harmony.CreateClassProcessor(typeof(PatchOnTick)).Patch();
    }

    /// <summary>An event that invokes every ticks</summary>
    public static event Action OnTick = () => { };

    [HarmonyPatch(typeof(StageController), "OnFixedUpdate")]
    private class PatchOnTick
    {
        static Exception Finalizer(Exception __exception)
        {
            OnTick.Invoke();

            return __exception;
        }
    }
}
