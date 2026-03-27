using System.Reflection;
using UnityEngine;

namespace DeviceOfHermes;

public static class HermesConstants
{
    static HermesConstants()
    {
        RevengeDiceSlash = Hermes.CreateSprite(LoadBytes("revenge_slash.png"))!;
        RevengeDicePenetrate = Hermes.CreateSprite(LoadBytes("revenge_penetrate.png"))!;
        RevengeDiceHit = Hermes.CreateSprite(LoadBytes("revenge_hit.png"))!;
        UnbreakableSlash = Hermes.CreateSprite(LoadBytes("unbreakable_slash.png"), pixPerUnit: 100f)!;
        UnbreakablePenetrate = Hermes.CreateSprite(LoadBytes("unbreakable_penetrate.png"), pixPerUnit: 100f)!;
        UnbreakableHit = Hermes.CreateSprite(LoadBytes("unbreakable_hit.png"), pixPerUnit: 100f)!;
    }

    private static byte[] LoadBytes(string name)
    {
        var resourceName = $"DeviceOfHermes.public.{name}";

        var asm = Assembly.GetAssembly(typeof(HermesConstants));
        using var stream = asm.GetManifestResourceStream(resourceName)
            ?? throw new FileNotFoundException($"{resourceName} is not found");

        using var ms = new MemoryStream();

        stream.CopyTo(ms);

        return ms.ToArray();
    }

    public static Sprite RevengeDiceSlash;

    public static Sprite RevengeDicePenetrate;

    public static Sprite RevengeDiceHit;

    public static Sprite UnbreakableSlash;

    public static Sprite UnbreakablePenetrate;

    public static Sprite UnbreakableHit;
}
