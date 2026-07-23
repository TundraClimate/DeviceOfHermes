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

        return "";
    }
}
