using System.Reflection;
using UnityEngine;
using DeviceOfHermes.UI;
using DeviceOfHermes.CustomDice;
using DeviceOfHermes.Resource;
using DeviceOfHermes.Data;
using DeviceOfHermes.Localize;

namespace DeviceOfHermes.Boot;

internal class HermesBootStrap : DiceCardAbilityBase
{
    public static string Desc = OnBoot();

    private static string OnBoot()
    {
        try
        {
            Application.logMessageReceived += Hermes.CreateCleanLog("Output.hermes.log");

            HermesConfigLoader.Load();

            NoPatchMethod.Init();

            HermesPreloader.PreloadAssemblies();
            SaveModifier.Init();

            OnlyCardXmlLoader.Load();
            FormationXmlLoader.Load();
            EmotionCardXmlLoader.Load();

            TextModel.Init();
            Artwork.Init();
            AbnoCardArtworkLoader.Load();

            Localizer.Init();

            BattleUIBehaviour.Init();

            CompositePatch.Init();
            DynamicAbility.Init();
            UnitUIExtension.Init();

            CustomDicePatch.Init();
            RevengeDice.Init();
            UnbreakableDice.Init();
            SecondlyDice.Init();
            EqualDice.Init();
        }
        catch (Exception e)
        {
            Mod.ModContentManager.Instance.AddErrorLog($"-BootStrap Err-");
            Mod.ModContentManager.Instance.AddErrorLog($"{e}");
        }

        return "";
    }
}

internal class Initializer : ModInitializer
{
    public override void OnInitializeMod()
    {
        try
        {
            CheckHermesAssemblyIncludes();
            CheckHermesAssemblySubs();
            AutoPatchAllMod();
        }
        catch (Exception e)
        {
            Mod.ModContentManager.Instance.AddErrorLog($"-Initializer Err-");
            Mod.ModContentManager.Instance.AddErrorLog($"{e}");
        }
    }

    void CheckHermesAssemblyIncludes()
    {
        foreach (var mod in HermesPreloader.ActiveMods)
        {
            var assemblies = Path.Combine(mod.dirInfo.FullName, "Assemblies");
            var dohName = typeof(HermesBootStrap).Assembly.GetName();

            if (Directory.Exists(assemblies) && Walkdir.GetFilesRecursive(assemblies).Filter(file => Path.GetExtension(file) == ".dll").Any(file => AssemblyName.ReferenceMatchesDefinition(AssemblyName.GetAssemblyName(file), dohName)))
            {
                var pid = mod.invInfo.workshopInfo.uniqueId;

                if (pid != "DeviceOfHermes")
                {
                    Mod.ModContentManager.Instance.AddErrorLog($"Found invalid DeviceOfHermes Assembly in {pid}");
                    Mod.ModContentManager.Instance.AddErrorLog($"Found invalid DeviceOfHermes Assembly in {pid}");
                    Mod.ModContentManager.Instance.AddErrorLog($"Found invalid DeviceOfHermes Assembly in {pid}");
                    Mod.ModContentManager.Instance.AddErrorLog($"Found invalid DeviceOfHermes Assembly in {pid}");
                    Mod.ModContentManager.Instance.AddErrorLog($"Found invalid DeviceOfHermes Assembly in {pid}");
                    Mod.ModContentManager.Instance.AddErrorLog($"Found invalid DeviceOfHermes Assembly in {pid}");
                    Mod.ModContentManager.Instance.AddErrorLog($"Found invalid DeviceOfHermes Assembly in {pid}");
                    Mod.ModContentManager.Instance.AddErrorLog($"Found invalid DeviceOfHermes Assembly in {pid}");
                    Mod.ModContentManager.Instance.AddErrorLog($"Found invalid DeviceOfHermes Assembly in {pid}");
                    Mod.ModContentManager.Instance.AddErrorLog($"Found invalid DeviceOfHermes Assembly in {pid}");
                    Mod.ModContentManager.Instance.AddErrorLog($"Found invalid DeviceOfHermes Assembly in {pid}");
                }
            }
        }
    }

    void CheckHermesAssemblySubs()
    {
        if (Path.GetDirectoryName(Path.GetDirectoryName(typeof(HermesBootStrap).GetAsmDirectory())) is not "3689874580" and "workshop")
        {
            var logs = (List<string>)typeof(Mod.ModContentManager).GetField("_logs", HarmonyLib.AccessTools.all).GetValue(Mod.ModContentManager.Instance);

            logs.Insert(0, $"Game cannot <b>Start!!!</b>");
            logs.Insert(0, $"DeviceOfHermes only valids the workshop mod ID=3689874580");
            logs.Insert(0, $"Loaded DeviceOfHermes is the <b>NOT</b> valid");
            logs.Insert(0, $"<b>===FATAL FATAL FATAL FATAL FATAL ===</b>");

            new GameObject().AddComponent<DestroyApp>();
        }
    }

    void AutoPatchAllMod()
    {
        foreach (var (conf, mod) in HermesConfigLoader.ConfigDict.Values)
        {
            if (conf.AutoHarmonyPatch)
            {
                AutoPatcher.PatchAll(mod);
            }
        }
    }

    class DestroyApp : MonoBehaviour
    {
        void Awake()
        {
            StartCoroutine(DestroyAppRouine());
        }

        System.Collections.IEnumerator DestroyAppRouine()
        {
            yield return new WaitForSeconds(5);

            Application.Quit();
        }
    };

}
