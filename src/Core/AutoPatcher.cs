using System.Reflection;
using Mod;
using HarmonyLib;

namespace DeviceOfHermes;

internal static class AutoPatcher
{
    public static void PatchAll(ModContentInfo mod)
    {
        var dir = Path.Combine(mod.dirInfo.FullName, "Assemblies");

        if (!Directory.Exists(dir))
        {
            return;
        }

        var assemblies = Directory.GetFiles(dir).Filter(file => Path.GetExtension(file) == ".dll");
        var loaded = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
        {
            var asmName = AssemblyName.GetAssemblyName(assembly);

            var asm = loaded.FirstOrDefault(asm => AssemblyName.ReferenceMatchesDefinition(asm.GetName(), asmName));

            if (asm is not null && !patched.Contains(asm))
            {
                var harmony = new Harmony($"DeviceOfHermes.AutoPatcher.{asmName.Name}");

                harmony.PatchAll(asm);

                patched.Add(asm);
            }
        }
    }

    static List<Assembly> patched = new();
}
