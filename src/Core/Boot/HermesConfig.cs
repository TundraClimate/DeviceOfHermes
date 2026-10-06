using Nett;

namespace DeviceOfHermes.Boot;

internal class HermesConfig
{
    [TomlMember(Key = "auto-patch")]
    public bool AutoHarmonyPatch { get; set; } = false;

    public DataConfig data { get; set; } = new();

    public LocalizeConfig localize { get; set; } = new();

    public ResourceConfig resource { get; set; } = new();
}

internal class DataConfig
{
    [TomlMember(Key = "onlycard")]
    public string OnlyCard { get; set; } = "";

    [TomlMember(Key = "formation")]
    public string Formation { get; set; } = "";

    [TomlMember(Key = "emotioncard")]
    public string EmotionCard { get; set; } = "";
}

internal class LocalizeConfig
{
    [TomlMember(Key = "abnocard")]
    public string AbnoCard { get; set; } = "";
}

internal class ResourceConfig
{
    [TomlMember(Key = "abnocard-artwork")]
    public string AbnoCardArtwork { get; set; } = "";
}
