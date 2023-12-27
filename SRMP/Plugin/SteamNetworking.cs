﻿using Lidgren.Network;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Steamworks.ISteamMatchmakingServerListResponse;

namespace SRMultiplayer.Plugin
{
    public static class SteamNetworking
    {
        internal static bool inServer;
        public const int SRAppID = 433340;

        public static Callback<LobbyMatchList_t> lobbyList;

        public static void Init()
        {
            lobbyList = Callback<LobbyMatchList_t>.Create(LobbyListFunction);
        }

        internal static List<CSteamID> lobbies;

        public static void GetLobbies()
        {
#if SRML
            SteamMatchmaking.AddRequestLobbyListStringFilter("ModLoader", "SRML", ELobbyComparison.k_ELobbyComparisonEqual);
#else
            SteamMatchmaking.AddRequestLobbyListStringFilter("ModLoader", "Standalone", ELobbyComparison.k_ELobbyComparisonEqual);
#endif
            SteamMatchmaking.RequestLobbyList();
        }
        public static void LobbyListFunction(LobbyMatchList_t list)
        {
            for (int i = 0; i < list.m_nLobbiesMatching; i++)
            {
                lobbies = new List<CSteamID>();
                lobbies.Add(SteamMatchmaking.GetLobbyByIndex(i));
            }
        }
        public static CSteamID GetServerByCode(string code)
        {
            foreach (var lobby in lobbies)  
            {
                if (SteamMatchmaking.GetLobbyData(lobby, "lobbyCode") == code)
                {
                    return lobby;
                }
            }
            return CSteamID.Nil;
        }
    }
}
