using System.Reflection;
using UnityEngine;
using DeviceOfHermes.Resource;

namespace DeviceOfHermes;

/// <summary>The constants used by DoH</summary>
public static class HermesConstants
{
    static HermesConstants()
    {
        RevengeDiceSlash = Artwork.CreateSprite(LoadBytes("revenge_slash.png"))!;
        RevengeDicePenetrate = Artwork.CreateSprite(LoadBytes("revenge_penetrate.png"))!;
        RevengeDiceHit = Artwork.CreateSprite(LoadBytes("revenge_hit.png"))!;
        UnbreakableSlash = Artwork.CreateSprite(LoadBytes("unbreakable_slash.png"), pixPerUnit: 100f)!;
        UnbreakablePenetrate = Artwork.CreateSprite(LoadBytes("unbreakable_penetrate.png"), pixPerUnit: 100f)!;
        UnbreakableHit = Artwork.CreateSprite(LoadBytes("unbreakable_hit.png"), pixPerUnit: 100f)!;
        UnbreakableSlashBeh = Artwork.CreateSprite(LoadBytes("unbreakable_slash_beh.png"), pixPerUnit: 100f)!;
        UnbreakablePenetrateBeh = Artwork.CreateSprite(LoadBytes("unbreakable_penetrate_beh.png"), pixPerUnit: 100f)!;
        UnbreakableHitBeh = Artwork.CreateSprite(LoadBytes("unbreakable_hit_beh.png"), pixPerUnit: 100f)!;
        SecondlySlash = Artwork.CreateSprite(LoadBytes("secondly_slash.png"), pixPerUnit: 100f)!;
        SecondlyPenetrate = Artwork.CreateSprite(LoadBytes("secondly_penetrate.png"), pixPerUnit: 100f)!;
        SecondlyHit = Artwork.CreateSprite(LoadBytes("secondly_hit.png"), pixPerUnit: 100f)!;
        SecondlyGuard = Artwork.CreateSprite(LoadBytes("secondly_guard.png"), pixPerUnit: 100f)!;
        SecondlyEvasion = Artwork.CreateSprite(LoadBytes("secondly_evade.png"), pixPerUnit: 100f)!;
        EqualSlash = Artwork.CreateSprite(LoadBytes("equal_slash.png"), pixPerUnit: 100f)!;
        EqualPenetrate = Artwork.CreateSprite(LoadBytes("equal_penetrate.png"), pixPerUnit: 100f)!;
        EqualHit = Artwork.CreateSprite(LoadBytes("equal_hit.png"), pixPerUnit: 100f)!;
        EqualGuard = Artwork.CreateSprite(LoadBytes("equal_guard.png"), pixPerUnit: 100f)!;
        EqualEvasion = Artwork.CreateSprite(LoadBytes("equal_evade.png"), pixPerUnit: 100f)!;
        EqualSlashBeh = Artwork.CreateSprite(LoadBytes("equal_slash_beh.png"), pixPerUnit: 100f)!;
        EqualPenetrateBeh = Artwork.CreateSprite(LoadBytes("equal_penetrate_beh.png"), pixPerUnit: 100f)!;
        EqualHitBeh = Artwork.CreateSprite(LoadBytes("equal_hit_beh.png"), pixPerUnit: 100f)!;
        EqualGuardBeh = Artwork.CreateSprite(LoadBytes("equal_guard_beh.png"), pixPerUnit: 100f)!;
        EqualEvasionBeh = Artwork.CreateSprite(LoadBytes("equal_evade_beh.png"), pixPerUnit: 100f)!;
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

    /// A sprite used by <see cref="DeviceOfHermes.CustomDice.RevengeDice"/>
    public static Sprite RevengeDiceSlash;

    /// A sprite used by <see cref="DeviceOfHermes.CustomDice.RevengeDice"/>
    public static Sprite RevengeDicePenetrate;

    /// A sprite used by <see cref="DeviceOfHermes.CustomDice.RevengeDice"/>
    public static Sprite RevengeDiceHit;

    /// A sprite used by <see cref="DeviceOfHermes.CustomDice.UnbreakableDice"/>
    public static Sprite UnbreakableSlash;

    /// A sprite used by <see cref="DeviceOfHermes.CustomDice.UnbreakableDice"/>
    public static Sprite UnbreakablePenetrate;

    /// A sprite used by <see cref="DeviceOfHermes.CustomDice.UnbreakableDice"/>
    public static Sprite UnbreakableHit;

    /// A sprite used by <see cref="DeviceOfHermes.CustomDice.UnbreakableDice"/>
    public static Sprite UnbreakableSlashBeh;

    /// A sprite used by <see cref="DeviceOfHermes.CustomDice.UnbreakableDice"/>
    public static Sprite UnbreakablePenetrateBeh;

    /// A sprite used by <see cref="DeviceOfHermes.CustomDice.UnbreakableDice"/>
    public static Sprite UnbreakableHitBeh;

    /// A sprite used by <see cref="DeviceOfHermes.CustomDice.SecondlyDice"/>
    public static Sprite SecondlySlash;

    /// A sprite used by <see cref="DeviceOfHermes.CustomDice.SecondlyDice"/>
    public static Sprite SecondlyPenetrate;

    /// A sprite used by <see cref="DeviceOfHermes.CustomDice.SecondlyDice"/>
    public static Sprite SecondlyHit;

    /// A sprite used by <see cref="DeviceOfHermes.CustomDice.SecondlyDice"/>
    public static Sprite SecondlyGuard;

    /// A sprite used by <see cref="DeviceOfHermes.CustomDice.SecondlyDice"/>
    public static Sprite SecondlyEvasion;

    /// A sprite used by <see cref="DeviceOfHermes.CustomDice.EqualDice"/>
    public static Sprite EqualSlash;

    /// A sprite used by <see cref="DeviceOfHermes.CustomDice.EqualDice"/>
    public static Sprite EqualPenetrate;

    /// A sprite used by <see cref="DeviceOfHermes.CustomDice.EqualDice"/>
    public static Sprite EqualHit;

    /// A sprite used by <see cref="DeviceOfHermes.CustomDice.EqualDice"/>
    public static Sprite EqualGuard;

    /// A sprite used by <see cref="DeviceOfHermes.CustomDice.EqualDice"/>
    public static Sprite EqualEvasion;

    /// A sprite used by <see cref="DeviceOfHermes.CustomDice.EqualDice"/>
    public static Sprite EqualSlashBeh;

    /// A sprite used by <see cref="DeviceOfHermes.CustomDice.EqualDice"/>
    public static Sprite EqualPenetrateBeh;

    /// A sprite used by <see cref="DeviceOfHermes.CustomDice.EqualDice"/>
    public static Sprite EqualHitBeh;

    /// A sprite used by <see cref="DeviceOfHermes.CustomDice.EqualDice"/>
    public static Sprite EqualGuardBeh;

    /// A sprite used by <see cref="DeviceOfHermes.CustomDice.EqualDice"/>
    public static Sprite EqualEvasionBeh;
}
