using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;
using Spiritfarer.EasyMinigames.Patches;

namespace Spiritfarer.EasyMinigames;

[BepInProcess("Spiritfarer.exe")]
[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public class EasyMinigames : BaseUnityPlugin
{
    internal const string PluginGuid = "org.ingwaz.spiritfarer.easy-minigames";
    internal const string PluginName = "Spiritfarer.EasyMinigames";
    internal const string PluginVersion = "1.0.0";

    internal static ManualLogSource logger;

    private void Awake()
    {
        logger = Logger;

        Logger.LogInfo($"Plugin {PluginName} {PluginVersion} loading...");
        var harmony = new Harmony(PluginName);

        MiningOverride.ApplyPatch(harmony);
        FishingOverride.ApplyPatch(harmony);
        CrusherOverride.ApplyPatch(harmony);

        Logger.LogInfo($"Plugin {PluginName} {PluginVersion} loaded!");
    }
}
