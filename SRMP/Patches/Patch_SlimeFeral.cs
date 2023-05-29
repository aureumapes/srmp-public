using HarmonyLib;
using MonomiPark.SlimeRancher.DataModel;
using SRMultiplayer;
using SRMultiplayer.Networking;
using SRMultiplayer.Packets;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SRMultiplayer.Patches
{
    [HarmonyPatch(typeof(SlimeFeral))]
    [HarmonyPatch("MakeFeral")]
    class SlimeFeral_MakeFeral
    {
        static void Prefix(SlimeFeral __instance)
        {
            if (!Globals.IsMultiplayer || Globals.HandlePacket) return;

            var netActor = __instance.GetComponentInParent<NetworkActor>();
            if (netActor != null)
            {
                new PacketActorFeral()
                {
                    ID = netActor.ID,
                    Feral = true,
                    Deagitate = false
                }.Send();
            }
        }
    }

    [HarmonyPatch(typeof(SlimeFeral))]
    [HarmonyPatch("MakeNotFeral")]
    class SlimeFeral_MakeNotFeral
    {
        static void Prefix(SlimeFeral __instance, bool deagitate)
        {
            if (!Globals.IsMultiplayer || Globals.HandlePacket) return;

            var netActor = __instance.GetComponentInParent<NetworkActor>();
            if (netActor != null)
            {
                new PacketActorFeral()
                {
                    ID = netActor.ID,
                    Feral = false,
                    Deagitate = deagitate
                }.Send();
            }
        }
    }
}