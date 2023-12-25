using HarmonyLib;
using SRMultiplayer.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMultiplayer.Patches
{
    [HarmonyPatch(typeof(DiscordRpc), nameof(DiscordRpc.UpdatePresence))]
    static class DiscordRPCPatch
    {
        static void Prefix(DiscordRpc __instance, DiscordRpc.RichPresence presence)
        {
            if (Globals.IsMultiplayer && !SRMPDiscord.isWritting)
                SRMPDiscord.InjectIntoRP(presence);
        }
        static void Postfix(DiscordRpc __instance, DiscordRpc.RichPresence presence)
        {
            SRMPDiscord.lastRP = presence;
        }
    }
}