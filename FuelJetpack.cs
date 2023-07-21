using System;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace FuelJetpack.Scripts
{
    [BepInPlugin("net.ThndrDev.stationeers.FuelJetpack.Scripts", "Fuel Jetpack", "0.3.0.0")]    
    public class FuelJetpackPlugin : BaseUnityPlugin
    {
        public static FuelJetpackPlugin Instance;
        public void Log(string line)
        {
            Debug.Log("[FuelJetpack]: " + line);
        }

        private void Awake()
        {
            FuelJetpackPlugin.Instance = this;
            Log("Hello World");
            var harmony = new Harmony("net.ThndrDev.stationeers.FuelJetpack.Scripts");
            harmony.PatchAll();
            Log("Patch succeeded");
        }
    }
}