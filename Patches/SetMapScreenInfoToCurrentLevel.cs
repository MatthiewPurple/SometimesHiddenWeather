using HarmonyLib;
using UnityEngine;
using TMPro;

namespace UnreliableWeatherReports.Patches
{
    [HarmonyPatch(typeof(StartOfRound), "SetMapScreenInfoToCurrentLevel")]
    public class SetMapScreenInfoToCurrentLevel
    {
        // Before the game displays the current moon's info on the main monitor
        private static bool Prefix(StartOfRound __instance)
        {
            __instance.screenLevelVideoReel.enabled = false;
            __instance.screenLevelVideoReel.gameObject.SetActive(value: false);
            __instance.screenLevelVideoReel.clip = __instance.currentLevel.videoReel;
            TimeOfDay timeOfDay = UnityEngine.Object.FindObjectOfType<TimeOfDay>();
            if (timeOfDay.totalTime == 0f)
            {
                timeOfDay.totalTime = (float)timeOfDay.numberOfHours * timeOfDay.lengthOfHours;
            }

            //BEGINNING-OF-NEW-CODE--------------------------------------------------------------------------------------------------------------------------------------------------------
            // Get text for the correct forecast (main monitor)
            string correctWeather = (__instance.currentLevel.currentWeather == LevelWeatherType.None)
                ? ""
                : "Weather: " + __instance.currentLevel.currentWeather;

            // If the list of incorrectWeatherMoons has been filled (because this method is called before AND after SetPlanetsWeather for some reason...)
            if (Globals.incorrectWeatherMoons.Count != 0)
            {
                // Get text for the fake forecast (main monitor)
                string incorrectWeather = (Globals.incorrectWeatherReports[__instance.currentLevel.levelID] == LevelWeatherType.None)
                ? ""
                : "Weather: " + Globals.incorrectWeatherReports[__instance.currentLevel.levelID];

                // Get displayed texte depending on the state of the moon's forecast (Correct, Incorrect or Unknown) (main monitor)
                string text = (Globals.unknownWeatherMoons[__instance.currentLevel.levelID] && __instance.currentLevel.levelID != 3)    // 3 is the ID of 71-Gordion (The Company Building)
                    ? "Weather: Unknown"
                    : (Globals.incorrectWeatherMoons[__instance.currentLevel.levelID] && __instance.currentLevel.levelID != 3)          // 3 is the ID of 71-Gordion (The Company Building)
                        ? incorrectWeather
                        : correctWeather;
            //END-OF-NEW-CODE--------------------------------------------------------------------------------------------------------------------------------------------------------------
                string levelDescription = __instance.currentLevel.LevelDescription;
                ((TMP_Text)__instance.screenLevelDescription).SetText("Orbiting: " + __instance.currentLevel.PlanetName + "\n" + levelDescription + "\n" + text);
            }
            __instance.mapScreen.overrideCameraForOtherUse = true;
            __instance.mapScreen.cam.transform.position = new Vector3(0f, 100f, 0f);
            ((Behaviour)(object)__instance.screenLevelDescription).enabled = true;
            if (__instance.currentLevel.videoReel != null)
            {
                __instance.screenLevelVideoReel.enabled = true;
                __instance.screenLevelVideoReel.gameObject.SetActive(value: true);
                __instance.screenLevelVideoReel.Play();
            }

            return false;   // Skips the vanilla method
        }
    }
}