using Assets.Scripts.Atmospherics;
using Assets.Scripts.Objects.Items;
using UnityEngine;

namespace FuelJetpack.Scripts
{
    public class FJ
    {
        public static void EjectInternalAtmosphere(Jetpack jetpack)
        {
            bool inflamed = jetpack.InternalAtmosphere.Inflamed;
            jetpack.WorldAtmosphere.Add(jetpack.InternalAtmosphere.GasMixture);
            if (jetpack.WorldAtmosphere != null)
            {
                jetpack.WorldAtmosphere.Sparked = true;
            }
            jetpack.InternalAtmosphere.GasMixture.Reset();
        }

        public static void InitInternalAtmosphere(Jetpack jetpack)
        {
            if (jetpack.InternalAtmosphere == null)
            {
                jetpack.InternalAtmosphere = new Atmosphere(jetpack, 1f, 0L);
            }
        }
    }
}
