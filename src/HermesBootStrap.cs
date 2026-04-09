using System.Reflection;
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

        Load1FrameworkLoaders();

        DynamicAbility.Init();

        return "";
    }

    private static void LoadNotExists(string depsPath)
    {
        var path = Path.Combine(DepsPath, depsPath);

        if (!File.Exists(path))
        {
            Hermes.Say($"A file '{path}' is not exists");

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

    private static void Load1FrameworkLoaders()
    {
        try
        {
            var contents = Mod.ModContentManager.Instance.GetAllMods();

            foreach (var content in contents)
            {
                var modDir = content.dirInfo.FullName;
                var flDir = Path.Combine(modDir, "Assemblies", "1FrameworkAssemblies");

                if (Directory.Exists(flDir))
                {
                    var files = Directory.GetFiles(flDir);

                    Hermes.Say($"Find the preload files: In {flDir}");

                    foreach (var file in files)
                    {
                        if (!File.Exists(file))
                        {
                            continue;
                        }

                        if (Path.GetExtension(file) != ".dll")
                        {
                            Hermes.Say($"Non-assembly skip: {file}");

                            continue;
                        }

                        var addsAsm = AssemblyName.GetAssemblyName(file);

                        var find = AppDomain.CurrentDomain.GetAssemblies()
                            .Any(asm => AssemblyName.ReferenceMatchesDefinition(asm.GetName(), addsAsm));

                        if (!find)
                        {
                            Hermes.Say($"Load by DeviceOfHermes: {file}");
                            Assembly.LoadFrom(file);
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Hermes.Say($"Error by loadings 1FrameworkLoader files: {e.Message}", MessageLevel.Error);
            Hermes.Say($"Stacktrace: {e.StackTrace}");
        }
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
