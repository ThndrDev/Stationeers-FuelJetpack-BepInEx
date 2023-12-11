using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace FuelJetpack
{
    [BepInPlugin("FuelJetpack", "Fuel Jetpack", "0.4.0.0")]    
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
            ConfigFile.HandleConfig(this);     // read (or create) the configuration file parameters
            var harmony = new Harmony("net.ThndrDev.stationeers.FuelJetpack.Scripts");
            harmony.PatchAll();
            Log("Patch succeeded");
        }
    }
}