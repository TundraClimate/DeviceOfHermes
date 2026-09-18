global using DeviceOfHermes;
global using DeviceOfHermes.AdvancedBase;

namespace LimbufOfHermes;

/// <summary>A Keyword list of Limbus buff</summary>
[KeywordBufExtend]
public class LimKeywordBuf
{
    /// <summary>Barrier KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_Barrier))]
    public static KeywordBuf Barrier { get; private set; }

    /// <summary>Rupture KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_Rupture))]
    public static KeywordBuf Rupture { get; private set; }

    /// <summary>Tremor KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_Tremor))]
    public static KeywordBuf Tremor { get; private set; }

    /// <summary>TremorScorch KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_TremorScorch))]
    public static KeywordBuf TremorScorch { get; private set; }

    /// <summary>TremorHemorrhage KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_TremorHemorrhage))]
    public static KeywordBuf TremorHemorrhage { get; private set; }

    /// <summary>TremorReverb KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_TremorReverb))]
    public static KeywordBuf TremorReverb { get; private set; }

    /// <summary>TremorSuperposition KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_TremorSuperposition))]
    public static KeywordBuf TremorSuperposition { get; private set; }

    /// <summary>TremorBurst KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_TremorBurst))]
    public static KeywordBuf TremorBurst { get; private set; }

    /// <summary>TremorConversion KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_TremorConversion))]
    public static KeywordBuf TremorConversion { get; private set; }

    /// <summary>TremorEntangle KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_TremorEntangle))]
    public static KeywordBuf TremorEntangle { get; private set; }

    /// <summary>ConsumeTremor KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_ConsumeTremor))]
    public static KeywordBuf ConsumeTremor { get; private set; }

    /// <summary>Sinking KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_Sinking))]
    public static KeywordBuf Sinking { get; private set; }

    /// <summary>SinkingDeluge KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_SinkingDeluge))]
    public static KeywordBuf SinkingDeluge { get; private set; }

    /// <summary>Panic KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_Panic))]
    public static KeywordBuf Panic { get; private set; }

    /// <summary>Poise KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_Poise))]
    public static KeywordBuf Poise { get; private set; }

    /// <summary>AlvUp KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_AlvUp))]
    public static KeywordBuf AlvUp { get; private set; }

    /// <summary>AlvDown KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_AlvDown))]
    public static KeywordBuf AlvDown { get; private set; }

    /// <summary>DlvUp KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_DlvUp))]
    public static KeywordBuf DlvUp { get; private set; }

    /// <summary>DlvDown KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_DlvDown))]
    public static KeywordBuf DlvDown { get; private set; }

    /// <summary>SlashVulnerable KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_SlashVulnerable))]
    public static KeywordBuf SlashVulnerable { get; private set; }

    /// <summary>PenetrateVulnerable KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_PenetrateVulnerable))]
    public static KeywordBuf PenetrateVulnerable { get; private set; }

    /// <summary>HitVulnerable KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_HitVulnerable))]
    public static KeywordBuf HitVulnerable { get; private set; }

    /// <summary>BloodfeastConsumed KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_BloodfeastConsumed))]
    public static KeywordBuf BloodfeastConsumed { get; private set; }

    /// <summary>PrescriptTarget KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_PrescriptTarget))]
    public static KeywordBuf PrescriptTarget { get; private set; }

    /// <summary>CritDmgUp KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_CritDmgUp))]
    public static KeywordBuf CritDmgUp { get; private set; }
}
