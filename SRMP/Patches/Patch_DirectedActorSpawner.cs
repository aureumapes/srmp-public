using HarmonyLib;
using SRMultiplayer.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRMultiplayer.Patches
{
    [HarmonyPatch(typeof(DirectedActorSpawner))]
    [HarmonyPatch("CanSpawn")]
    class DirectedActorSpawner_CanSpawn
    {
        static bool Prefix(DirectedActorSpawner __instance, ref bool __result)
        {
            if (!Globals.IsMultiplayer) return true;

            var spawner = __instance.gameObject.GetComponent<NetworkDirectedActorSpawner>();
            if(spawner != null && spawner.IsLocal)
            {
                return true;
            }
            __result = false;
            return false;
        }
    }

    [HarmonyPatch(typeof(DirectedActorSpawner))]
    [HarmonyPatch("CanContinueSpawning")]
    class DirectedActorSpawner_CanContinueSpawning
    {
        static bool Prefix(DirectedActorSpawner __instance, ref bool __result)
        {
            if (!Globals.IsMultiplayer) return true;

            var spawner = __instance.gameObject.GetComponent<NetworkDirectedActorSpawner>();
            if (spawner != null && spawner.IsLocal)
            {
                return true;
            }
            __result = false;
            return false;
        }
    }
}