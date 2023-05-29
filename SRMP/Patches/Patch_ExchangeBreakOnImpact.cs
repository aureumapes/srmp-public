using HarmonyLib;
using SRMultiplayer.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRMultiplayer.Patches
{
    [HarmonyPatch(typeof(ExchangeBreakOnImpact))]
    [HarmonyPatch("BreakOpen")]
    class ExchangeBreakOnImpact_BreakOpen
    {
        static bool Prefix(ExchangeBreakOnImpact __instance)
        {
            if (!Globals.IsMultiplayer) return true;

            var entity = __instance.GetComponent<NetworkActor>();
            return (entity != null && entity.IsLocal);
        }
    }
}