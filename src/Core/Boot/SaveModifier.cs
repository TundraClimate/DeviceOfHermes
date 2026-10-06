using GameSave;
using HarmonyLib;

namespace DeviceOfHermes.Boot;

internal static class SaveModifier
{
    public static void Init()
    {
        var harmony = new Harmony("DeviceOfHermes.Boot.SaveModifier");

        harmony.CreateClassProcessor(typeof(PatchOnStart)).Patch();
        harmony.CreateClassProcessor(typeof(PatchGetErr)).Patch();
    }

    [HarmonyPatch(typeof(GameSceneManager), "Start")]
    public class PatchOnStart
    {
        static void Postfix()
        {
            var saveData = SaveManager.Instance.LoadData(SavePath);

            if (saveData is null)
            {
                Hermes.Say("Mods data save is null");

                return;
            }

            var orders = saveData.GetData("orders");
            var actives = saveData.GetData("lastActivated");

            if (orders is null)
            {
                Hermes.Say("Mod orders is null");

                return;
            }

            List<string> newOrderList = new();

            foreach (var order in orders)
            {
                newOrderList.Add(order.GetStringSelf());
            }

            if (newOrderList.Contains("DeviceOfHermes"))
            {
                newOrderList.Remove("DeviceOfHermes");
                newOrderList.Insert(0, "DeviceOfHermes");
            }

            var newOrders = new SaveData(SaveDataType.List);

            foreach (var order in newOrderList)
            {
                newOrders.AddToList(new SaveData(order));
            }

            var newModSave = new SaveData();

            newModSave.AddData("orders", newOrders);
            newModSave.AddData("lastActivated", actives);

            SaveManager.Instance.SaveData(SavePath, newModSave);
        }
    }

    [HarmonyPatch(typeof(Mod.ModContentManager), "GetErrorLogs")]
    public class PatchGetErr
    {
        static Exception Finalizer(Exception __exception, List<string> __result)
        {
            __result?.RemoveAll(txt => txt.Contains("1FrameworkPriorityLoader"));

            return __exception;
        }
    }

    private static string SavePath => Path.Combine(SaveManager.savePath, "ModSetting.save");
}
