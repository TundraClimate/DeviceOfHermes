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
        Application.logMessageReceived += Hermes.CreateCleanLog("Output.hermes.log");

        NoPatchMethod.Init();

        HermesPreloader.PreloadAssemblies();

        OnlyCardXmlLoader.Load();
        FormationXmlLoader.Load();
        EmotionCardXmlLoader.Load();

        TextModel.Init();
        Localizer.Init();

        SaveModifier.Init();
        UnitUIExtension.Init();
        DynamicAbility.Init();
        BattleUIBehaviour.Init();

        CompositePatch.Init();
        CustomDicePatch.Init();
        RevengeDice.Init();
        UnbreakableDice.Init();
        SecondlyDice.Init();
        EqualDice.Init();

        return "";
    }
}
