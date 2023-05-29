using HarmonyLib;
using SRMultiplayer.Networking;
using SRMultiplayer.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMultiplayer.Patches
{
    [HarmonyPatch(typeof(PlortCollector))]
    [HarmonyPatch("DoCollection")]
    class PlortCollector_DoCollection
    {
        static bool Prefix(PlortCollector __instance)
        {
            if (!Globals.IsMultiplayer) return true;

            var netLandplot = __instance.model.gameObj.GetComponent<NetworkLandplot>();
            return (netLandplot != null && netLandplot.IsLocal);
        }

        static void Postfix(PlortCollector __instance)
        {
            if (!Globals.IsMultiplayer) return;

            var netLandplot = __instance.model.gameObj.GetComponent<NetworkLandplot>();
            if (netLandplot != null && netLandplot.IsLocal)
            {
                new PacketLandPlotCollect()
                {
                    ID = __instance.model.gameObj.GetComponent<LandPlotLocation>().id,
                    collectorNextTime = __instance.model.collectorNextTime,
                    endCollectAt = __instance.endCollectAt,
                    forceCollectUntil = __instance.forceCollectUntil
                }.Send();
            }
        }
    }

    [HarmonyPatch(typeof(PlortCollector))]
    [HarmonyPatch("StartCollection")]
    class PlortCollector_StartCollection
    {
        static void Prefix(PlortCollector __instance)
        {
            if (!Globals.IsMultiplayer || Globals.HandlePacket) return;

            if (__instance.joints.Count == 0 && __instance.timeDir.HasReached(__instance.forceCollectUntil))
            {
                new PacketLandPlotStartCollection()
                {
                    ID = __instance.model.gameObj.GetComponent<LandPlotLocation>().id
                }.Send();
            }
        }
    }
}