using HarmonyLib;
using SRMultiplayer.Networking;
using SRMultiplayer.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRMultiplayer.Patches
{
    [HarmonyPatch(typeof(DroneStationAnimator))]
    [HarmonyPatch("SetEnabled")]
    class DroneStationAnimator_SetEnabled
    {
        static bool Prefix(DroneStationAnimator __instance, bool enabled)
        {
            if (!Globals.IsMultiplayer || Globals.HandlePacket) return true;

            var netDrone = __instance.GetComponentInParent<NetworkDrone>();
            if (netDrone != null && netDrone.IsLocal)
            {
                new PacketDroneStationEnabled()
                {
                    ID = netDrone.Drone.droneModel.siteId,
                    Enabled = enabled
                }.Send();
                return true;
            }
            return false;
        }
    }
}