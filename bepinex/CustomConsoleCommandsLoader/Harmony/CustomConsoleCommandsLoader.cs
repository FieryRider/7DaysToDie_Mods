using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HarmonyLib;

[HarmonyPatch]
public class SdtdConsoleReversePatch
{
    [HarmonyReversePatch]
    [HarmonyPatch(typeof(SdtdConsole), "RegisterCommand")]
    public static void RegisterCommand(SdtdConsole instance, SortedList<string, IConsoleCommand> _commandsList, string _className, IConsoleCommand _command)
    {
        throw new NotImplementedException("It's a stub");
    }
}

[HarmonyPatch(typeof(SdtdConsole))]
[HarmonyPatch("RegisterCommands")]
class RegisterCustomCommands
{
    static bool Prefix(SdtdConsole __instance, List<IConsoleCommand> ___m_Commands, Dictionary<string, IConsoleCommand> ___m_CommandsAllVariants)
    {
        SortedList<string, IConsoleCommand> commandsList = new SortedList<string, IConsoleCommand>();
        foreach (Mod mod in ModManager.GetLoadedMods())
        {
            Log.Warning($"Checking mod {mod.Path}");
            List<string> modFolders = new List<string>() { "Scripts", "Harmony", "plugins" };
            foreach (string modFolder in modFolders)
            {
                string modSubDir = $"{mod.Path}{Path.DirectorySeparatorChar}{modFolder}";
                Log.Warning($"Checking folder {modSubDir}");
                if (!Directory.Exists(modSubDir))
                    continue;
                string[] modAssembliesPaths = Directory.GetFiles($"{mod.Path}{Path.DirectorySeparatorChar}{modFolder}");
                foreach (string modAssemblyPath in modAssembliesPaths)
                {
                    Log.Out($"Checking file {modAssemblyPath}");
                    Assembly modAssembly = Assembly.LoadFrom(modAssemblyPath);
                    Log.Warning("Types:");
                    foreach (Type type in modAssembly.GetTypes())
                    {
                        Log.Out(type.Name);
                        if (type.IsClass && !type.IsAbstract && typeof(IConsoleCommand).IsAssignableFrom(type))
                        {
                            Log.Out("ConsoleCommand");
                            IConsoleCommand command = ReflectionHelpers.Instantiate<IConsoleCommand>(type);
                            SdtdConsoleReversePatch.RegisterCommand(__instance, commandsList, type.Name, command);
                        }
                    }
                }
            }
        }

        try
        {
            foreach (IConsoleCommand consoleCommand in commandsList.Values)
            {
                ___m_Commands.Add(consoleCommand);
                foreach (string consoleCommandName in consoleCommand.GetCommands())
                {
                    if (!string.IsNullOrEmpty(consoleCommandName))
                    {
                        IConsoleCommand existingConsoleCommand;
                        if (___m_CommandsAllVariants.TryGetValue(consoleCommandName, out existingConsoleCommand))
                            Log.Warning($"Command with alias \"{consoleCommandName}\" already registered from {existingConsoleCommand.GetType().Name}, not registering for class {consoleCommand.GetType().Name}");
                        else
                            ___m_CommandsAllVariants.Add(consoleCommandName, consoleCommand);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Log.Error("Error registering custom commands");
            Log.Exception(e);
        }
        return true;
    }
}