using System.Reflection;
using System.Diagnostics;
using HarmonyExtension;
using UI;
using Mod;

namespace DeviceOfHermes.Boot;

internal static class HermesPreloader
{
    public static void PreloadAssemblies()
    {
        Dictionary<string, (Version, string)> latest = new();

        foreach (var mod in ActiveMods)
        {
            try
            {
                var dir = Path.Combine(mod.dirInfo.FullName, "Assemblies", "HermesAssemblies");

                if (!Directory.Exists(dir))
                {
                    continue;
                }

                foreach (var file in Directory.GetFiles(dir))
                {
                    if (Path.GetExtension(file) != ".dll")
                    {
                        continue;
                    }

                    var name = AssemblyName.GetAssemblyName(file).Name;
                    var version = Version.Parse(FileVersionInfo.GetVersionInfo(file).FileVersion);

                    if (!latest.ContainsKey(name) || version > latest[name].Item1)
                    {
                        latest[name] = (version, file);
                    }
                }

            }
            catch (Exception)
            {
                Hermes.Say($"HermesPreloader(pid: {mod?.invInfo?.workshopInfo?.uniqueId}): 'FileVersion' access failed", MessageLevel.Warn);

                continue;
            }
        }

        List<Assembly> loaded = new();

        foreach (var (_, path) in latest.Values)
        {
            LoadAssembly(path)?.Let(asm => loaded.Add(asm));
        }

        foreach (var ty in loaded.Map(asm => asm.GetTypes()).Flatten())
        {
            try
            {
                if (typeof(HermesInitializer).IsAssignableFrom(ty))
                {
                    var init = (HermesInitializer)Activator.CreateInstance(ty);

                    init.OnInitMod();
                }
            }
            catch (Exception e)
            {
                var exc = e.InnerException;

                ModContentManager.Instance.AddErrorLog($"{exc.Message ?? $"OnInitMod by {ty.Name}"}");

                Hermes.Say($"{exc}", MessageLevel.Error);

                continue;
            }
        }

        Hermes.Say($"HermesPreloader: Successfully preloaded {loaded.Count} files");
    }

    public static Assembly? LoadAssembly(string path)
    {
        if (!File.Exists(path) || Path.GetExtension(path) != ".dll")
        {
            Hermes.Say($"Path '{path}' is not dll file", MessageLevel.Warn);

            return null;
        }

        var addsAsm = AssemblyName.GetAssemblyName(path);

        var find = AppDomain.CurrentDomain.GetAssemblies().ToList()
            .Find(asm => AssemblyName.ReferenceMatchesDefinition(asm.GetName(), addsAsm));

        if (find is null)
        {
            var asm = Assembly.LoadFrom(path);

            Hermes.Say($"Load new Assembly by DeviceOfHermes: {asm.GetName().Name}:{asm.GetName().Version}");

            return asm;
        }
        else
        {
            return find;
        }
    }

    public static List<ModContentInfo> ActiveMods =
        ((List<ModSlotData>)typeof(UIModPopup).Field("dataList").GetValue(UnityObject.FindObjectOfType<EntryScene>().modPopup))
            .Filter(mod => mod is ModSlotData slot && slot.IsActivated).Map(slot => slot.ModInfo).Collect();
}
