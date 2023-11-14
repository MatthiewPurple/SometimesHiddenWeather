using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using HarmonyLib;
using BepInEx.Logging;

namespace HiddenWeather.Patches
{
    [HarmonyPatch(typeof(Terminal), "TextPostProcess")]
    public class TextPostProcess
    {
        private static bool Prefix(Terminal __instance, ref string modifiedDisplayText, ref TerminalNode node)
        {
            ManualLogSource l = Logger.CreateLogSource("TextPostProcess_Prefix");
            l.LogInfo(modifiedDisplayText);

            int num = modifiedDisplayText.Split("[planetTime]").Length - 1;
            if (num > 0)
            {
                Regex regex = new Regex(Regex.Escape("[planetTime]"));
                for (int i = 0; i < num && __instance.moonsCatalogueList.Length > i; i++)
                {                   
                    string replacement = "";
                    modifiedDisplayText = regex.Replace(modifiedDisplayText, replacement, 1);
                }
            }

            if (node.displayPlanetInfo != -1)
            {
                string replacement = "mild weather";
                modifiedDisplayText = modifiedDisplayText.Replace("[currentPlanetTime]", replacement);
            }
            return true;
        }
    }
}
