using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;

namespace FuelJetpack
{
    internal class ConfigFile
    {
        private static ConfigEntry<float> configFuelUsageMultiplier;
        private static ConfigEntry<bool> configGravityFuelImpact;

        public static float FuelUsageMultiplier;
        public static bool GravityFuelImpact;
        public static void HandleConfig(FuelJetpackPlugin fj) // Create and manage the configuration file parameters
        {
            configFuelUsageMultiplier = fj.Config.Bind("0 - General configuration",
             "FuelUsageMultiplier",
             1f,
             "Set the fuel usage multiplier of the jetpack. If you increase this value, the jetpack fuel will be spent faster, making it harder to keep the Jetpack" +
             " fueled. Decrease it to reduce the fuel usage. If you increase this value too much, keep in mind that the combustion waste of the jetpack will also" +
             " increase proportionally. Needs to be a positive value between 0.01 and 10");
            FuelUsageMultiplier = Mathf.Clamp(configFuelUsageMultiplier.Value, 0.01f, 10f);

            configGravityFuelImpact = fj.Config.Bind("0 - General configuration",
             "GravityFuelImpact",
             true,
             "Adjusts the influence of gravitational forces on jetpack fuel consumption. When enabled, the jetpack's fuel usage will vary based on the intensity" +
             " of gravity – higher in stronger gravitational fields and lower in weaker ones. This setting allows for a more realistic simulation of fuel dynamics " +
             "in different environmental conditions.");
            GravityFuelImpact = configGravityFuelImpact.Value;
        }

    }
}
