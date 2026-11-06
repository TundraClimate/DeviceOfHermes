using HarmonyLib;

namespace DeviceOfHermes;

/// <summary>A set of Stage events</summary>
public static class StageEvents
{
    static StageEvents()
    {
        var harmony = new Harmony("DeviceOfHermes.StageEvents");

        harmony.CreateClassProcessor(typeof(PatchOnClearResource)).Patch();
    }

    /// <summary>An event on cleanup</summary>
    public static event Action OnCleanup = () => { };

    [HarmonyPatch(typeof(StageController), "ClearResources")]
    class PatchOnClearResource
    {
        static Exception Finalizer(Exception __exception)
        {
            OnCleanup.Invoke();

            return __exception;
        }
    }
}
