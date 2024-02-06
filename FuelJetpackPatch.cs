using Assets.Scripts.UI;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using Assets.Scripts.Objects.Items;
using Assets.Scripts.Atmospherics;
using Assets.Scripts.Objects;
using Assets.Scripts;
using System.Collections.Generic;

namespace FuelJetpack
{
    [HarmonyPatch(typeof(Jetpack))]
    class PatchJetpack
    {
        [HarmonyPatch("get_HasPropellent")]
        [HarmonyPrefix]
        [UsedImplicitly]
        static private bool HasPropellentPatch(Jetpack __instance, ref bool __result)
        {
            GasCanister gasCanister;
            if (!__instance.PropellentSlot.Contains<GasCanister>(out gasCanister))
            {
                __result = false;
                return false; 
            }

            if (gasCanister == null || gasCanister.InternalAtmosphere == null)
            {
                __result = false;
                return false; 
            }

            Atmosphere internalAtmosphere = gasCanister.InternalAtmosphere;
            __result = internalAtmosphere.GasMixture.Volatiles.Quantity > 0.01f &&
                       (internalAtmosphere.GasMixture.Oxygen.Quantity > 0.01f ||
                        internalAtmosphere.GasMixture.NitrousOxide.Quantity > 0.01f);
            return false; 
        }

        [HarmonyPatch("OnAtmosphericTick")]
        [HarmonyPrefix]
        [UsedImplicitly]
        static private bool OnAtmosphericTickPatch(Jetpack __instance)
        {
            if (!GameManager.RunSimulation)
            {
                return false; //exit if game is paused or if jetpack is not activated
            }

            FJ.InitInternalAtmosphere(__instance); // Initialize internal atmosphere if not already done

            // check if the jetpack is off (no emissions):
            if (__instance.CurrentEmission <= 0)
            {
                return false; //exit if no jetpack emissions exists
            }

            // Check for fuel canister presence and fuel
            GasCanister gasCanister;
            if (!__instance.PropellentSlot.Contains<GasCanister>(out gasCanister) || gasCanister.InternalAtmosphere == null || !__instance.HasPropellent)
            {
                Thing.Interact(__instance.InteractActivate, 0); //turn off jetpack
                return false; //and exit if the canister doesn't exists or is empty
            }

            // If there's a jetpack emission, burn some fuel
            if (__instance.CurrentEmission > 0f)
            {
                float gravityfactor;
                if (ConfigFile.GravityFuelImpact)
                {
                    gravityfactor = Mathf.Clamp((WorldSetting.Current.Gravity / -5f), 0.5f, 5f);
                }
                else
                {
                    gravityfactor = 1f;
                }
                float fuelToConsume = __instance.MolesToUse * (__instance.OutputSetting * gravityfactor * ConfigFile.FuelUsageMultiplier);
                if (fuelToConsume > 0f )
                {              
                    // Add the removed gas to the internal atmosphere of the jetpack and handle combustion
                    __instance.InternalAtmosphere.Add(gasCanister.InternalAtmosphere.Remove(fuelToConsume, AtmosphereHelper.MatterState.Gas));
                    __instance.InternalAtmosphere.Sparked = true;
                    __instance.InternalAtmosphere.ManualCombust(1f);
                    //And then eject the waste from the jetpack
                    FJ.EjectInternalAtmosphere(__instance);
                }
            }
            return false; // Skip the original method
        }

        private static Dictionary<Jetpack, float> jetpackOldValues = new Dictionary<Jetpack, float>();
        private static Dictionary<Jetpack, bool> jetpackBoostStates = new Dictionary<Jetpack, bool>();
        [HarmonyPatch("LateUpdate")]
        [HarmonyPostfix]
        [UsedImplicitly]
        static private void LateUpdatehPatch(Jetpack __instance)
        {
            if (!jetpackBoostStates.ContainsKey(__instance))
            {
                jetpackBoostStates[__instance] = false;
            }

            if (!jetpackOldValues.ContainsKey(__instance))
            {
                jetpackOldValues[__instance] = __instance.OutputSetting;
            }

            // Boost the jetpack OutputSetting to max when shift is pressed.
            if (!jetpackBoostStates[__instance] && KeyManager.GetButtonDown(KeyCode.LeftShift))
            {
                jetpackOldValues[__instance] = __instance.OutputSetting;
                jetpackBoostStates[__instance] = true;
                __instance.OutputSetting = Jetpack.MaxSetting;
            }

            // And back to the previous value when shift is released
            if (jetpackBoostStates[__instance] && KeyManager.GetButtonUp(KeyCode.LeftShift))
            {
                __instance.OutputSetting = jetpackOldValues[__instance];
                jetpackBoostStates[__instance] = false;
            }
            // This changes the jetpack speed
            float baseJetpackSpeed;
            switch (__instance.PrefabHash) //get the current jetpack prefabhast and the vanilla speed value for it
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
            // If the propellant canister contains NitrousOxide and Volatiles, increase the base speed by 25%
            GasCanister gasCanister;
            __instance.PropellentSlot.Contains<GasCanister>(out gasCanister);
            if (gasCanister != null &&
                gasCanister.InternalAtmosphere != null &&
                gasCanister.InternalAtmosphere.GasMixture.NitrousOxide.Quantity > 0.01f &&
                gasCanister.InternalAtmosphere.GasMixture.Volatiles.Quantity > 0.01f)
            {
                baseJetpackSpeed *= 1.25f;
            }
            // Also multiply the speed by the OutputSetting of the jetpack (the Thrust up and down buttons setting) to a max of 250% speed boost
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
                GasCanister gasCanister;
                __instance.PropellentSlot.Contains<GasCanister>(out gasCanister);
                __result = gasCanister.InternalAtmosphere.PressureGassesAndLiquidsInPa < 1000f;                
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
            GasCanister gasCanister;
            __instance.PropellentSlot.Contains<GasCanister>(out gasCanister);
            if (gasCanister && gasCanister.InternalAtmosphere != null)
            {
                //Jetpack Critical alert when pressure is below 500kpa
                __result = gasCanister.InternalAtmosphere.PressureGassesAndLiquidsInPa < 500f;
                return false;
            }
            __result = false;
            return false;
        }
    }
 /*
    // This patches the Propellent slot of the jetpack to also allow liquid canisters. 
    // WIP, this code allows liquid canisters inside if the slot is empty, but not if there's another canister inside already.

    [HarmonyPatch(typeof(Slot))]
    public class PatchSlot
    {
        [HarmonyPatch(typeof(Slot), nameof(Slot.AllowSwap), new System.Type[] { typeof(Slot), typeof(Slot) })]
        [HarmonyPostfix]
        public static void SwapPostfix(Slot sourceSlot, Slot destinationSlot, ref bool __result)
        {
            if (sourceSlot.Occupant.CanEnter(destinationSlot))
            {
                if ((destinationSlot.Type == Slot.Class.GasCanister && sourceSlot.Occupant.SlotType == Slot.Class.LiquidCanister) ||
                (destinationSlot.Type == Slot.Class.LiquidCanister && sourceSlot.Occupant.SlotType == Slot.Class.GasCanister))
                {
                    __result = true;
                }
                //Debug.Log("Slot.AllowSwap: " + sourceSlot + " sourceSlot.occupant: " + sourceSlot.Occupant + " DestinationSlot: " + destinationSlot + " __result: " + __result);
            }
        }

        [HarmonyPatch("AllowMove")]
        [HarmonyPostfix]
        public static void MovePostfix(DynamicThing thing, Slot destinationSlot, ref bool __result)
        {
            if (thing.CanEnter(destinationSlot))
            {
                if (destinationSlot.Type == Slot.Class.GasCanister && thing.SlotType == Slot.Class.LiquidCanister && !(thing is DraggableThing))
                {
                    __result = true;
                }
            }
            Debug.Log("Slot.AllowMove: Thing: " + thing + " thing.CanEnter(destinationSlot): " + (thing.CanEnter(destinationSlot)) + " DestinationSlot: " + destinationSlot + " !(thing is DraggableThing)" + !(thing is DraggableThing) + " __result: " + __result);
        }        
    }
    */
}
