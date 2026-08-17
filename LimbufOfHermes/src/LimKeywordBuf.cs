global using DeviceOfHermes;
global using DeviceOfHermes.AdvancedBase;

namespace LimbufOfHermes;

/// <summary>A Keyword list of Limbus buff</summary>
[KeywordBufExtend]
public class LimKeywordBuf
{
    /// <summary>Rupture KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_Rupture))]
    public static KeywordBuf Rupture { get; private set; }

    /// <summary>Tremor KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_Tremor))]
    public static KeywordBuf Tremor { get; private set; }

    /// <summary>TremorBurst KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_TremorBurst))]
    public static KeywordBuf TremorBurst { get; private set; }

    /// <summary>ConsumeTremor KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_ConsumeTremor))]
    public static KeywordBuf ConsumeTremor { get; private set; }

    /// <summary>Sinking KeywordBuf</summary>
    [KeywordBuf(typeof(BattleUnitBuf_Limbuf_Sinking))]
    public static KeywordBuf Sinking { get; private set; }

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
}
