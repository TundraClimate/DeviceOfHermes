using System.Reflection;
using HarmonyExtension;
using UnityEngine;
using DeviceOfHermes;
using DeviceOfHermes.UI;
using DeviceOfHermes.CustomDice;

internal class HermesBootStrap : DiceCardAbilityBase
{
    public static string Desc = OnBoot();

    private static string OnBoot()
    {
        Application.logMessageReceived += Hermes.CreateCleanLog("Output.hermes.log");

        LoadPreloadAssemblies();

        SaveModifier.Init();
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
}
