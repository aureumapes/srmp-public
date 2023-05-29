using HarmonyLib;
using SRMultiplayer.Networking;
using SRMultiplayer.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRMultiplayer.Patches
{
    [HarmonyPatch(typeof(SlimeEat))]
    [HarmonyPatch("Produce")]
    class SlimeEat_Produce
    {
        static bool Prefix(SlimeEat __instance, int count)
        {
            if (!Globals.IsMultiplayer || Globals.HandlePacket) return true;

            var netActor = __instance.GetComponent<NetworkActor>();
            if (netActor != null && netActor.IsLocal)
            {
                new PacketActorFX()
                {
                    ID = netActor.ID,
                    Type = (byte)PacketActorFX.FXType.SlimeProduceFX,
                    Count = count
                }.Send();
                return true;
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(SlimeEat))]
    [HarmonyPatch("EatAndProduce")]
    class SlimeEat_EatAndProduce
    {
        static void Prefix(SlimeEat __instance, GameObject target, SlimeDiet.EatMapEntry em, bool immediateMode, bool skipDelays, bool skipProduction)
        {
            if (!Globals.IsMultiplayer || Globals.HandlePacket) return;

            var netActor = __instance.GetComponent<NetworkActor>();
            if (netActor != null && netActor.IsLocal)
            {
                if (!immediateMode)
                {
                    new PacketActorFX()
                    {
                        ID = netActor.ID,
                        Type = em.isFavorite ? (byte)PacketActorFX.FXType.SlimeEatFavoriteFX : (byte)PacketActorFX.FXType.SlimeEatFX,
                    }.Send();
                }
            }
        }
    }

    [HarmonyPatch(typeof(SlimeEat))]
    [HarmonyPatch("EatAndTransform")]
    class SlimeEat_EatAndTransform
    {
        static void Prefix(SlimeEat __instance, GameObject target, SlimeDiet.EatMapEntry em, bool immediateMode)
        {
            if (!Globals.IsMultiplayer || Globals.HandlePacket) return;

            var netActor = __instance.GetComponent<NetworkActor>();
            if (netActor != null && netActor.IsLocal)
            {
                if (!immediateMode)
                {
                    new PacketActorFX()
                    {
                        ID = netActor.ID,
                        Type = (byte)PacketActorFX.FXType.SlimeTransformFX,
                    }.Send();
                }
            }
        }
    }
}