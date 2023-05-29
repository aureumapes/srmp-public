using HarmonyLib;
using MonomiPark.SlimeRancher.Regions;
using SRMultiplayer.Networking;
using SRMultiplayer.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SRMultiplayer.Patches
{
    [HarmonyPatch(typeof(Region))]
    [HarmonyPatch("Proxy")]
    class Region_Proxy
    {
        static void Postfix(Region __instance)
        {
            if (!Globals.IsMultiplayer) return;

            var netRegion = __instance.GetComponent<NetworkRegion>();
            if (Globals.IsClient)
            {
                new PacketRegionChange()
                {
                    ID = netRegion.ID,
                    Load = false
                }.Send();
            }
            else if (Globals.LocalPlayer != null)
            {
                netRegion.RemovePlayer(Globals.LocalPlayer);
            }

            if (netRegion.IsLocal)
            {
                netRegion.DropOwnership();
            }
        }
    }

    [HarmonyPatch(typeof(Region))]
    [HarmonyPatch("Unproxy")]
    class Region_Unproxy
    {
        static void Postfix(Region __instance)
        {
            if (!Globals.IsMultiplayer) return;

            var netRegion = __instance.GetComponent<NetworkRegion>();
            if (Globals.IsClient)
            {
                new PacketRegionChange()
                {
                    ID = netRegion.ID,
                    Load = true
                }.Send();
            }
            else if(Globals.LocalPlayer != null)
            {
                netRegion.AddPlayer(Globals.LocalPlayer);
            }

            if (netRegion.Owner == 0 || Globals.IsServer)
            {
                netRegion.TakeOwnership();
            }
        }
    }
}