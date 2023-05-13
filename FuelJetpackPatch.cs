using Assets.Scripts.UI;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using Assets.Scripts.Objects.Items;
using Assets.Scripts.Atmospherics;
using Assets.Scripts.Objects;

namespace FuelJetpack.Scripts
{
    [HarmonyPatch(typeof(Jetpack))]
    class PatchJetpack
    {
        [HarmonyPatch("get_HasPropellent")]
        [HarmonyPrefix]
        [UsedImplicitly]
        static private bool HasPropellentPatch(Jetpack __instance, ref bool __result)
        {
            __result = __instance.PropellentCanister && __instance.PropellentCanister.InternalAtmosphere != null && (__instance.PropellentCanister.InternalAtmosphere.GasMixture.Volatiles.Quantity > 0.01f && (__instance.PropellentCanister.InternalAtmosphere.GasMixture.Oxygen.Quantity > 0.01f || __instance.PropellentCanister.InternalAtmosphere.GasMixture.NitrousOxide.Quantity > 0.01f));
            return false;
        }
        
        [HarmonyPatch("OnAtmosphericTick")]
        [HarmonyPrefix]
        [UsedImplicitly]
        static private bool OnAtmosphericTickPatch(Jetpack __instance)
        {
            FJ.InitInternalAtmosphere(__instance); // Check if the internal atmosphere exists. If not, then create.
            if (!__instance.JetPackActivate)
            {
                if (__instance.InternalAtmosphere.TotalMoles > 0.001f)
                {
                    FJ.EjectInternalAtmosphere(__instance);
                }
                return false;
            }
            // If Jetpack activated, and no canister or invalid fuel. Deactivate jetpack and exit.
            if (__instance.JetPackActivate && (__instance.PropellentCanister == null || !__instance.HasPropellent))
            {
                Thing.Interact(__instance.InteractActivate, 0);
                return false;
            }
            // If there's a jetpack emission, burn some fuel
            if (__instance.CurrentEmission > 0f)
            {
                FJ.EjectInternalAtmosphere(__instance);
                float gravityfactor = Mathf.Clamp((WorldManager.CurrentWorldSetting.Gravity / -5f), 0.5f, 5f);
                GasMixture gasMixture = __instance.PropellentCanister.InternalAtmosphere.Remove(__instance.MolesToUse * (__instance.OutputSetting * gravityfactor));
                __instance.InternalAtmosphere.Add(gasMixture);
                __instance.InternalAtmosphere.Sparked = true;
                __instance.InternalAtmosphere.ManualCombust(1f);
            }
            return false;
        }

        private static float oldvalue;
        private static bool speedboosted = false;
        [HarmonyPatch("LateUpdate")]
        [HarmonyPostfix]
        [UsedImplicitly]
        static private void LateUpdatehPatch(Jetpack __instance)
        {
            // Boost the jetpack OutputSetting to max when shift is pressed. This doesn't work in BepInEx, only in Addons. I had to move this logic from patching the InteractWith to  LateUpdate
            // due to a strange error that happened just in BepInEx when patching InteractWith and, as a consequence, the logic to detect keypresses stopped working. If you know how to solve this,
            // please send a pull request.
            if (!speedboosted && KeyManager.GetButtonDown(KeyCode.LeftShift))
            {
                oldvalue = __instance.OutputSetting;
                speedboosted = true;
                __instance.OutputSetting = Jetpack.MaxSetting;
            }
            // And back to the previous value when shift is released
            if (speedboosted && KeyManager.GetButtonUp(KeyCode.LeftShift))
            {
                __instance.OutputSetting = oldvalue;
                speedboosted = false;
            }
            // Change the jetpack speed based on the OutputSetting
            float baseJetpackSpeed;
            switch (__instance.PrefabHash)
            {
                case 1969189000: 
                    baseJetpackSpeed = 3f; // Jetpack Basic
                    break;
                case -1260618380: 
                    baseJetpackSpeed = 5f; // Spacepack
                    break;
                case -412551656: 
                    baseJetpackSpeed = 8f; // Hardsuit Jetpack
                    break;
                default:
                    baseJetpackSpeed = 3f;
                    break;
            }
            __instance.JetPackSpeed = baseJetpackSpeed * Mathf.Clamp(__instance.OutputSetting + 0.5f, 0.6f, 2.5f);
        }

        [HarmonyPatch("get_PropellantLow")]
        [HarmonyPrefix]
        [UsedImplicitly]
        static private bool PropellantLowPatch(Jetpack __instance, ref bool __result)
        {
            if (__instance.HasPropellent)
            {
                //Jetpack Low alert when pressure is below 1000kpa
                __result = __instance.PropellentCanister.InternalAtmosphere.PressureGassesAndLiquids < 1000f;                
                return false;
            }
            __result = false;
            return false;
        }
        
        [HarmonyPatch("get_PropellantCritical")]
        [HarmonyPrefix]
        [UsedImplicitly]
        static private bool PropellantCriticalPatch(Jetpack __instance, ref bool __result)
        {
            if (!__instance.HasPropellent)
            {
                __result = true; //Jetpack Critical alert when no canister or incorrect fuel is present
                return false;
            }
            if (__instance.PropellentCanister && __instance.PropellentCanister.InternalAtmosphere != null)
            {
                //Jetpack Critical alert when pressure is below 500kpa
                __result = __instance.PropellentCanister.InternalAtmosphere.PressureGassesAndLiquids < 500f;
                return false;
            }
            __result = false;
            return false;
        }
    }
}
