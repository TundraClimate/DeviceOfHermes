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
            AutoPatchAllMod();
        }
        catch (Exception e)
        {
            Mod.ModContentManager.Instance.AddErrorLog($"-Initializer Err-");
            Mod.ModContentManager.Instance.AddErrorLog($"{e}");
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
}
