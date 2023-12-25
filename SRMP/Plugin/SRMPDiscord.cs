﻿using SRMultiplayer.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMultiplayer.Plugin
{
    internal static class SRMPDiscord
    {
        internal static DiscordRpc.RichPresence lastRP;
        public static bool isWritting;
        public static int GetPlayerCount()
        {
            if (Globals.IsServer)
            {
                return NetworkServer.Instance.m_Server.ConnectionsCount + 1;
            }
            else if (Globals.IsClient)
            {
                return NetworkClient.Instance.playerCount;
            }
            else return 0;
        }
        internal static void InjectIntoRP(DiscordRpc.RichPresence rp)
        {
            lastRP.state += $", Player {SceneContext.Instance.player.GetComponent<NetworkPlayer>().ID}/{GetPlayerCount()}";
        }
        
        internal static void InjectIntoLastRP()
        {
            lastRP.state += $", Player {SceneContext.Instance.player.GetComponent<NetworkPlayer>().ID}/{GetPlayerCount()}";

            isWritting = true;
            DiscordRpc.UpdatePresence(lastRP);
            isWritting = false;
        }

    }
}