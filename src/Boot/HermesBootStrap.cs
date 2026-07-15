using UnityEngine;
using DeviceOfHermes.UI;
using DeviceOfHermes.CustomDice;
using DeviceOfHermes.Data;

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

        SaveModifier.Init();
        UnitUIExtension.Init();
        DynamicAbility.Init();
        BattleUIBehaviour.Init();

        CustomDicePatch.Init();
        RevengeDice.Init();
        UnbreakableDice.Init();
        SecondlyDice.Init();
        EqualDice.Init();

        return "";
    }
}
