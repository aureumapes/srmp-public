using Lidgren.Network;
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
        public const int SRAppID = 433340;

        internal static ISteamMatchmakingServerListResponse serverListResponse;

        public static MatchMakingKeyValuePair_t[] filters = {
                new MatchMakingKeyValuePair_t { m_szKey = "appid", m_szValue = SRAppID.ToString() },
                new MatchMakingKeyValuePair_t { m_szKey = "gamever", m_szValue = Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString() },
#if SRML
                new MatchMakingKeyValuePair_t { m_szKey = "modloader", m_szValue = "srml"},
#else
                new MatchMakingKeyValuePair_t { m_szKey = "modloader", m_szValue = "standalone"},
#endif
        };


        private static void OnServerResponded(HServerListRequest hRequest, int iServer)
        {
            Debug.Log($"Debug -- Server Responded: {hRequest} - {iServer}");
        }

        private static void OnServerFailedToRespond(HServerListRequest hRequest, int iServer)
        {
            Debug.Log($"Debug -- Server Failed to respond: {hRequest} - {iServer}");
        }

        private static void OnRefreshComplete(HServerListRequest hRequest, EMatchMakingServerResponse response)
        {
            Debug.Log($"Debug -- Refresh complete: {hRequest} - {response}");

        }
        internal static HServerListRequest serverListRequest;
        public static void Init()
        {
            serverListResponse = new ISteamMatchmakingServerListResponse(OnServerResponded, OnServerFailedToRespond, OnRefreshComplete);
            serverListRequest = SteamMatchmakingServers.RequestInternetServerList(new AppId_t(SRAppID), filters, (uint)filters.Length, serverListResponse);
        }

        public static void RefreshServerList()
        {
            serverListRequest = SteamMatchmakingServers.RequestInternetServerList(new AppId_t(SRAppID), filters, (uint)filters.Length, serverListResponse);
        }


        public static CSteamID GetServerByCode(string code)
        {
            for (int i = 0; SteamMatchmakingServers.GetServerCount(serverListRequest) <= i; i++)
            {
                var server = SteamMatchmaking.GetLobbyByIndex(i);
                if (SteamMatchmaking.GetLobbyData(server, "serverCode") == code)
                {
                    return server;
                }
            }
            return CSteamID.Nil;
        }
    }
}
