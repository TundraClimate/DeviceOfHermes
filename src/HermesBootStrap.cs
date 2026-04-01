using HarmonyLib;
using UnityEngine;
using GameSave;

internal class HermesBootStrap : DiceCardAbilityBase
{
    public static string Desc = OnBoot();

    private static string OnBoot()
    {
        var harmony = new Harmony("DeviceOfHermes.Boot");

        harmony.CreateClassProcessor(typeof(SaveModifierPatch.PatchOnStart)).Patch();

        Application.logMessageReceived += Hermes.CreateCleanLog("Output.hermes.log");

        return "";
    }

    private static string SavePath => Path.Combine(SaveManager.savePath, "ModSetting.save");
    private static string DepsPath => Path.Combine(typeof(HermesBootStrap).GetAsmDirectory(), "dependencies");

    private class SaveModifierPatch
    {
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
    }
}
