using System.Reflection;
using HarmonyLib;
using HarmonyExtension;
using UnityEngine;
using GameSave;
using DeviceOfHermes;
using DeviceOfHermes.UI;
using DeviceOfHermes.CustomDice;

internal class HermesBootStrap : DiceCardAbilityBase
{
    public static string Desc = OnBoot();

    private static string OnBoot()
    {
        var harmony = new Harmony("DeviceOfHermes.Boot");

        harmony.CreateClassProcessor(typeof(SaveModifierPatch.PatchOnStart)).Patch();
        harmony.CreateClassProcessor(typeof(SaveModifierPatch.PatchGetErr)).Patch();

        Application.logMessageReceived += Hermes.CreateCleanLog("Output.hermes.log");

        LoadPreloadAssemblies();

        UnitUIExtension.Init();
        DynamicAbility.Init();
        BattleUIBehaviour.Init();

        new RevengeDice();
        new UnbreakableDice();
        new SecondlyDice();

        return "";
    }

    private static void LoadPreloadAssemblies()
    {
        try
        {
            var popupInstance = UnityEngine.Object.FindObjectOfType<EntryScene>().modPopup;
            var contents = (List<UI.ModSlotData>)typeof(UI.UIModPopup).Field("dataList").GetValue(popupInstance);

            contents.RemoveAll(mod => mod is null || !mod.IsActivated);

            foreach (var content in contents)
            {
                var modDir = content.ModInfo.dirInfo.FullName;
                var flDir = Path.Combine(modDir, "Assemblies", "DoHAssemblies");

                if (Directory.Exists(flDir))
                {
                    var files = Directory.GetFiles(flDir);

                    foreach (var file in files)
                    {
                        LoadNotExists(file);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Hermes.Say($"Error by loadings DoHAssemblies files: {e.Message}", MessageLevel.Error);
            Hermes.Say($"Stacktrace: {e.StackTrace}");
        }
    }

    private static void LoadNotExistsFromDeps(string depsPath)
    {
        var path = Path.Combine(DepsPath, depsPath);

        LoadNotExists(path);
    }

    private static void LoadNotExists(string path)
    {
        if (!File.Exists(path))
        {
            Hermes.Say($"A file '{path}' is not exists");

            return;
        }

        if (Path.GetExtension(path) != ".dll")
        {
            Hermes.Say($"A file '{path}' is not dll");

            return;
        }

        var addsAsm = AssemblyName.GetAssemblyName(path);

        var find = AppDomain.CurrentDomain.GetAssemblies()
            .Any(asm => AssemblyName.ReferenceMatchesDefinition(asm.GetName(), addsAsm));

        if (!find)
        {
            Hermes.Say($"Load by DeviceOfHermes: {path}");
            Assembly.LoadFrom(path);
        }
    }

    private static string DepsPath => Path.Combine(typeof(HermesBootStrap).GetAsmDirectory(), "dependencies");

    private class SaveModifierPatch
    {
        private static string SavePath => Path.Combine(SaveManager.savePath, "ModSetting.save");

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
                if (__result is not null)
                {
                    __result.RemoveAll(txt => txt.Contains("1FrameworkPriorityLoader"));
                }

                return __exception;
            }
        }
    }
}
