using System.Text.RegularExpressions;
using HarmonyLib;
using BepInEx.Logging;
using UnityEngine;
using Unity.Netcode;
using TMPro;

namespace HiddenWeather.Patches
{
    [HarmonyPatch(typeof(StartOfRound), "SetMapScreenInfoToCurrentLevel")]
    public class SetMapScreenInfoToCurrentLevel
    {
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

            string text = "";
            string levelDescription = __instance.currentLevel.LevelDescription;
            ((TMP_Text)__instance.screenLevelDescription).SetText("Orbiting: " + __instance.currentLevel.PlanetName + "\n" + levelDescription + "\n" + text);
            __instance.mapScreen.overrideCameraForOtherUse = true;
            __instance.mapScreen.cam.transform.position = new Vector3(0f, 100f, 0f);
            ((Behaviour)(object)__instance.screenLevelDescription).enabled = true;
            if (__instance.currentLevel.videoReel != null)
            {
                __instance.screenLevelVideoReel.enabled = true;
                __instance.screenLevelVideoReel.gameObject.SetActive(value: true);
                __instance.screenLevelVideoReel.Play();
            }

            return false;
        }
    }
}