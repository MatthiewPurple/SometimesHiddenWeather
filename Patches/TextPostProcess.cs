using System.Text.RegularExpressions;
using HarmonyLib;

namespace UnreliableWeatherReports.Patches
{
    [HarmonyPatch(typeof(Terminal), "TextPostProcess")]
    public class TextPostProcess
    {
        private static bool Prefix(Terminal __instance, ref string modifiedDisplayText, ref TerminalNode node)
        {
            int num = modifiedDisplayText.Split("[planetTime]").Length - 1;
            if (num > 0)
            {
                Regex regex = new Regex(Regex.Escape("[planetTime]"));
                for (int i = 0; i < num && __instance.moonsCatalogueList.Length > i; i++)
                {
                    //BEGINNING-OF-NEW-CODE-----------------------------------------------------------------------------------------------------------------------
                    // Get text for the correct forecast (terminal selection)
                    string correctWeather = (__instance.moonsCatalogueList[i].currentWeather == LevelWeatherType.None)
                        ? ""
                        : "(" + __instance.moonsCatalogueList[i].currentWeather.ToString() + ")";

                    // Get text for the fake forecast (terminal selection)
                    string incorrectWeather = (Globals.incorrectWeatherReports[__instance.moonsCatalogueList[i].levelID] == LevelWeatherType.None)
                        ? ""
                        : "(" + Globals.incorrectWeatherReports[__instance.moonsCatalogueList[i].levelID].ToString() + ")";

                    // Get displayed texte depending on the state of the moon's forecast (Correct, Incorrect or Unknown) (terminal selection)
                    string replacement = (GameNetworkManager.Instance.isDemo && __instance.moonsCatalogueList[i].lockedForDemo)
                        ? "(Locked)"
                        : (Globals.unknownWeatherMoons[__instance.moonsCatalogueList[i].levelID])
                            ? "(Unknown)"
                            : (Globals.incorrectWeatherMoons[__instance.moonsCatalogueList[i].levelID])
                                ? incorrectWeather
                                : correctWeather;
                    //END-OF-NEW-CODE-----------------------------------------------------------------------------------------------------------------------------
                    modifiedDisplayText = regex.Replace(modifiedDisplayText, replacement, 1);
                }
            }

            if (node.displayPlanetInfo != -1)
            {
                //BEGINNING-OF-NEW-CODE---------------------------------------------------------------------------------------------------------------------------
                // Get text for the correct forecast (terminal confirmation)
                string correctWeather = (StartOfRound.Instance.levels[node.displayPlanetInfo].currentWeather != LevelWeatherType.None)
                    ? StartOfRound.Instance.levels[node.displayPlanetInfo].currentWeather.ToString().ToLower() ?? ""
                    : "mild weather";

                // Get text for the fake forecast (terminal confirmation)
                string incorrectWeather = (Globals.incorrectWeatherReports[StartOfRound.Instance.levels[node.displayPlanetInfo].levelID] != LevelWeatherType.None)
                    ? Globals.incorrectWeatherReports[StartOfRound.Instance.levels[node.displayPlanetInfo].levelID].ToString().ToLower() ?? ""
                    : "mild weather";

                // Get displayed texte depending on the state of the moon's forecast (Correct, Incorrect or Unknown) (terminal confirmation)
                string replacement = Globals.unknownWeatherMoons[StartOfRound.Instance.levels[node.displayPlanetInfo].levelID]
                    ? "????"
                    : (Globals.incorrectWeatherMoons[StartOfRound.Instance.levels[node.displayPlanetInfo].levelID])
                        ? incorrectWeather
                        : correctWeather;
                //END-OF-NEW-CODE---------------------------------------------------------------------------------------------------------------------------------
                modifiedDisplayText = modifiedDisplayText.Replace("[currentPlanetTime]", replacement);
            }
            return true;
        }
    }
}
