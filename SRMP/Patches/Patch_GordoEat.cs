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
    [HarmonyPatch(typeof(GordoEat))]
    [HarmonyPatch("MaybeEat")]
    class GordoEat_MaybeEat
    {
        static bool Prefix(GordoEat __instance, ref bool __result, Collider col)
        {
            if (!Globals.IsMultiplayer || Globals.HandlePacket) return true;

            var netActor = col.GetComponent<NetworkActor>();
            if (netActor != null && netActor.IsLocal)
            {
                return true;
            }
            __result = false;
            return false;
        }

        static void Postfix(GordoEat __instance, ref bool __result, Collider col)
        {
            if (!Globals.IsMultiplayer || Globals.HandlePacket) return;

            if(__result)
            {
                Identifiable component = col.GetComponent<Identifiable>();
                List<SlimeDiet.EatMapEntry> eatMap = __instance.slimeDefinition.Diet.EatMap;
                for (int i = 0; i < eatMap.Count; i++)
                {
                    SlimeDiet.EatMapEntry eatMapEntry = eatMap[i];
                    if (eatMapEntry.eats == component.id)
                    {
                        new PacketGordoEat()
                        {
                            ID = __instance.GetComponent<NetworkGordo>().ID,
                            Position = col.transform.position,
                            Rotation = col.transform.localRotation,
                            Count = eatMapEntry.NumToProduce(),
                            Favorite = eatMapEntry.isFavorite
                        }.Send();
                        return;
                    }
                }
            }
        }
    }
}