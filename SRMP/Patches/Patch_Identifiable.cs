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
    [HarmonyPatch(typeof(Identifiable))]
    [HarmonyPatch("OnDestroy")]
    class Identifiable_OnDestroy
    {
        static void Postfix(Identifiable __instance)
        {
            if (!Globals.IsMultiplayer || Globals.HandlePacket || __instance.id == Identifiable.Id.NONE || Identifiable.SCENE_OBJECTS.Contains(__instance.id)) return;

            var netActor = __instance.GetComponent<NetworkActor>();
            if (netActor != null)
            {
                if (Globals.Actors.ContainsKey(netActor.ID))
                {
                    Globals.Actors.Remove(netActor.ID);
                    new PacketActorDestroy()
                    {
                        ID = netActor.ID
                    }.Send();
                }
            }
        }
    }
}