using HarmonyLib;
using System;
using System.Linq;
using System.Collections.Generic;

namespace UnreliableWeatherReports.Patches
{
    [HarmonyPatch(typeof(StartOfRound), "SetPlanetsWeather")]
    public class SetPlanetsWeather
    {
        // After the game has picked the weather of each moon
        private static void Postfix(StartOfRound __instance)
        {
            // Clear all lists
            Globals.unknownWeatherMoons.Clear();
            Globals.incorrectWeatherMoons.Clear();
            Globals.incorrectWeatherReports.Clear();

            // Create randomness variable
            System.Random r = new(__instance.randomMapSeed);    // It's important for the randomness to be based on the map seed
                                                                // so that every player gets the same result
            // Roll for the "Unknown Day"
            double unknownDayChance = r.NextDouble();

            // For each of the moons
            for (int i = 0; i < 128 /*__instance.levels.Length*/; i++) // I don't know how to get the number of total moons including custom ones
            {
                // Roll for the unknown weather
                double unknownWeatherChance = r.NextDouble();

                // Determine if the moon's weather is unknown based on the configurable parameters
                bool isUnknown = (unknownDayChance < Config.unknownDayChance.Value/100d || unknownWeatherChance < Config.unknownWeatherChance.Value/100d) && i != 3;    // 3 is the ID of 71-Gordion (The Company Building)
                Globals.unknownWeatherMoons.Add(isUnknown);

                if (isUnknown)
                {
                    // An unknown weather is considered correct
                    Globals.incorrectWeatherMoons.Add(false);
                    Globals.incorrectWeatherReports.Add(LevelWeatherType.None); 
                }
                else
                {
                    // Roll for the incorrect weather
                    double incorrectWeatherChance = r.NextDouble();

                    // Determine if the moon's weather is incorrect based on the configurable parameter
                    bool isIncorrect = incorrectWeatherChance < Config.incorrectWeatherChance.Value / 100d && i != 3;   // 3 is the ID of 71-Gordion (The Company Building)
                    Globals.incorrectWeatherMoons.Add(isIncorrect);

                    if (!isIncorrect)
                    {
                        // No need to make make up a fake weather for a correct forecast
                        Globals.incorrectWeatherReports.Add(LevelWeatherType.None);
                    }
                    else
                    {
                        // Randomly pick a weather that is different from the correct one (and DustClouds)
                        List<LevelWeatherType> weatherList = Enum.GetValues(typeof(LevelWeatherType)).OfType<LevelWeatherType>().ToList();
                        int j;
                        do
                        {
                            j = r.Next(weatherList.Count);
                        } while (weatherList[j] == __instance.levels[i].currentWeather || weatherList[j] == LevelWeatherType.DustClouds);
                        Globals.incorrectWeatherReports.Add(weatherList[j]);
                    }
                }
            }
        }
    }
}