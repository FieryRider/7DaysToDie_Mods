using BepInEx;
using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
public class Init : BaseUnityPlugin
{
    public const string pluginGuid = "com.fieryrider.7daystodie.mods.customconsolecommandsloader";
    public const string pluginName = "CustomConsoleCommandsLoader";
    public const string pluginVersion = "1.0.0.0";

    public void Start()
    {
        Debug.Log("Loading Patch: " + GetType().ToString());
        Harmony harmony = new Harmony(pluginGuid);
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}