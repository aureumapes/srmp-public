using SRMultiplayer.Networking;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.UI.Image;
using Lidgren.Network;
using SRMultiplayer.Packets;
namespace SRMultiplayer.Plugin
{
    public class SRMPSteam : SRSingleton<SRMPSteam>
    {
        public static string GenerateServerCode()
        {
            System.Random random = new System.Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return "STEAM" + new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public Dictionary<NetDeliveryMethod, EP2PSend> sendModeType = new Dictionary<NetDeliveryMethod, EP2PSend>()
        {
            { NetDeliveryMethod.Unreliable, EP2PSend.k_EP2PSendUnreliableNoDelay },
            { NetDeliveryMethod.UnreliableSequenced, EP2PSend.k_EP2PSendUnreliable },
            { NetDeliveryMethod.Unknown, EP2PSend.k_EP2PSendUnreliable },
            { NetDeliveryMethod.ReliableUnordered, EP2PSend.k_EP2PSendReliable },
            { NetDeliveryMethod.ReliableSequenced, EP2PSend.k_EP2PSendReliableWithBuffering },
            { NetDeliveryMethod.ReliableOrdered, EP2PSend.k_EP2PSendReliableWithBuffering },
        };

        public bool isHost { get; internal set; }
        internal Callback<LobbyCreated_t> successfulHost;
        internal Callback<LobbyInvite_t> invited;
        internal Callback<GameLobbyJoinRequested_t> attemptJoin;
        internal Callback<LobbyEnter_t> join;
        public void Start()
        {
            if (SteamMain.FinishedSetup)
            {
                successfulHost = Callback<LobbyCreated_t>.Create(CheckSteamHostSuccess);
                invited = Callback<LobbyInvite_t>.Create(DetectInvite);
                attemptJoin = Callback<GameLobbyJoinRequested_t>.Create(AttemptJoin);
                join = Callback<LobbyEnter_t>.Create(JoinLobby);
                join = Callback<LobbyEnter_t>.Create(JoinLobby);
            }
        }
        public void Update()
        {
            if (SteamMain.FinishedSetup)
            {
                SteamAPI.RunCallbacks();
            }
        }

        public void HostSteamGame(ELobbyType type)
        {
            SteamMatchmaking.CreateLobby(type, 255);
            isHost = true;
        }
        static string GetPublicIP()
        {
            using (WebClient webClient = new WebClient())
            {
                string response = webClient.DownloadString("http://api.ipify.org/");
                return response;
            }
        }

        internal void CheckSteamHostSuccess(LobbyCreated_t callback)
        {
            if (callback.m_eResult == EResult.k_EResultOK)
            {
                Globals.ServerCode = GenerateServerCode();

                currLobbyID = new CSteamID(callback.m_ulSteamIDLobby);
                SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "lobbyCode", Globals.ServerCode.ToString());
                SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "hostID", SteamUser.GetSteamID().m_SteamID.ToString());
#if SRML 
                SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "ModLoader", "SRML");
#else
                SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "ModLoader", "Standalone");
#endif

                SteamNetworkingClass.inServer = true;
                Debug.Log("Hosted game is now set up for steam invites!");
            }
        }

        internal void DetectInvite(LobbyInvite_t callback)
        {
            Debug.Log($"You have been invited to a game!");
        }

        internal void AttemptJoin(GameLobbyJoinRequested_t callback)
        {
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }
        internal void JoinLobby(LobbyEnter_t callback)
        {
            if (isHost)
                return;

            NetworkClient.Instance.hostSteamID = new CSteamID(ulong.Parse(SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "hostID")));
            currLobbyID = new CSteamID(callback.m_ulSteamIDLobby);
            string name;
            if (string.IsNullOrEmpty(Globals.Username))
                name = SteamFriends.GetPersonaName();
            else
                name = Globals.Username;

            var joinPacket = new PacketPlayerJoinedSteamServer()
            {
                Username = Globals.Username,
                uuid = Globals.UserData.UUID,
                mods = Globals.Mods,
                DLCs = SRSingleton<GameContext>.Instance.DLCDirector.Installed.ToList(),
                SteamID = SteamUser.GetSteamID().m_SteamID,
                version = Globals.Version   
            };
            NetworkClient.Instance.Send(joinPacket, NetDeliveryMethod.ReliableUnordered, 0);

            SteamNetworkingClass.inServer = true;
        }

        public CSteamID currLobbyID;
    }
}
