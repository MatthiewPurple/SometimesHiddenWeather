using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;

namespace UnreliableWeatherReports
{
    public class Config
    {
        // Configurable variable declarations
        public static ConfigEntry<double> unknownWeatherChance;
        public static ConfigEntry<double> unknownDayChance;
        public static ConfigEntry<double> incorrectWeatherChance;

        // Configuration file shenanigans
        public Config(ConfigFile cfg)
        {
            unknownWeatherChance = cfg.Bind(
                    "General",                                                      // Category
                    "UnknownWeatherChance",                                         // Variable name
                    35d,                                                            // Default value
                    "Chance of unknown weather per moon"                            // Description
            );

            incorrectWeatherChance = cfg.Bind(
                    "General",                                                      // Category
                    "IncorrectWeatherChance",                                       // Variable name
                    20d,                                                            // Default value
                    "Chance of incorrect weather report per moon (if not unknown)"  // Description
            );

            unknownDayChance = cfg.Bind(
                    "General",                                                      // Category
                    "UnknownDayChance",                                             // Variable name
                    10d,                                                            // Default value
                    "Chance of unknown weather everywhere for an entire day"        // Description
            );
        }
    }

    public static class Globals
    {
        // Global variable declarations
        public static List<bool> unknownWeatherMoons = new();                   // List of each moon's unknown status
        public static List<bool> incorrectWeatherMoons = new();                 // List of each moon's incorrect status
        public static List<LevelWeatherType> incorrectWeatherReports = new();   // List of fake weather reports for each moon
    }

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Config MyConfig { get; internal set; }

        private void Awake()
        {
            // Plugin startup logic
            MyConfig = new(Config);
            Harmony h = new(PluginInfo.PLUGIN_GUID);
            h.PatchAll();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
