using Mono.Cecil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

public static class NewFields
{
    public static IEnumerable<string> TargetDLLs { get; } = new[] { "Assembly-CSharp.dll" };

    public static void Initialize() { }

    public static void Patch(AssemblyDefinition assembly)
    {
        Console.WriteLine("Cecil patch works!");
        ModuleDefinition gameModule = assembly.MainModule;

        string modsDirectory = Path.GetFullPath("Mods" + Path.DirectorySeparatorChar);
        UriBuilder thisPatchDllPathUri = new UriBuilder(Assembly.GetExecutingAssembly().CodeBase);
        string thisPatchDllPath = Uri.UnescapeDataString(thisPatchDllPathUri.Path);
        string thisPatchDir = Path.GetDirectoryName(thisPatchDllPath);
        string modDir = Directory.GetParent(thisPatchDir).FullName;
        string pluginDllPath = $"{modDir}{Path.DirectorySeparatorChar}plugins{Path.DirectorySeparatorChar}ModReferenceField-Harmony.dll";
        Console.WriteLine(pluginDllPath);

        TypeDefinition minEffectGroupType = gameModule.Types.First(t => t.Name == nameof(MinEffectGroup));
        // Do cecil stuff
    }

    public static void Finish() { }
}