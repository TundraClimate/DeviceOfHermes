using Nett;

namespace DeviceOfHermes.Boot;

internal class HermesConfig
{
    [TomlMember(Key = "auto-patch")]
    public bool AutoHarmonyPatch { get; set; } = false;
}
