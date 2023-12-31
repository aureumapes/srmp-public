using Lidgren.Network;
using SRMultiplayer.Plugin;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SRMultiplayer.Networking
{
    public class NetworkMasterServer : SRSingleton<NetworkMasterServer>
    {
        public const string MasterServerIP = "srmp.saty.dev";
        public const int MasterServerPort = 7777;

        public ConnectionStatus Status;
        public long UniqueIdentifier { get { return m_Client.UniqueIdentifier; } }

        private NetClient m_Client;
        private float m_ReconnectTime = 30;

        public enum ConnectionStatus
        {
            Disconnected,
            Connecting,
            Connected
        }

        public enum MessageType
        {
            CreateServer,
            DeleteServer,
            JoinServer,
            UpdateServer,
            UpdateName,
            CreatePublicServer,
            ServerList
        }

        private void Start()
        {
            TryConnect();
        }

        private void TryConnect()
        {
            if (Status != ConnectionStatus.Disconnected) return;

            Status = ConnectionStatus.Connecting;
            NetPeerConfiguration config = new NetPeerConfiguration("srmp-master-1");
            config.EnableMessageType(NetIncomingMessageType.NatIntroductionSuccess);

            m_Client = new NetClient(config);
            m_Client.Start();

            NetOutgoingMessage hail = CreateMessage();
            hail.Write(Globals.Username);
            hail.Write(Globals.Version);
            m_Client.Connect(MasterServerIP, MasterServerPort, hail);
        }

        private void Update()
        {
            if (m_Client == null || m_Client.ConnectionStatus == NetConnectionStatus.Disconnected)
            {
                m_ReconnectTime -= Time.deltaTime;
                if (m_ReconnectTime <= 0)
                {
                    m_ReconnectTime = 30;
                    TryConnect();
                }
            }
            NetIncomingMessage im;
            while ((im = m_Client?.ReadMessage()) != null)
            {
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        {
                            string text = im.ReadString();
                            //Debug.Log("[MasterServer] " + text);
                        }
                        break;
                    case NetIncomingMessageType.NatIntroductionSuccess:
                        {
                            string token = im.ReadString();
                            Debug.Log("[MasterServer] Nat introduction success to " + im.SenderEndPoint + " token is: " + token);

                            SRSingleton<NetworkClient>.Instance.Connect(im.SenderEndPoint, Globals.Username);
                        }
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        {
                            NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                            string reason = im.ReadString();
                            Debug.Log("[MasterServer] " + NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);

                            if (status == NetConnectionStatus.Connected)
                            {
                                Status = ConnectionStatus.Connected;
                            }
                            else if (status == NetConnectionStatus.Disconnected)
                            {
                                
                                // Globals.ServerCode = "";
                                Status = ConnectionStatus.Disconnected;
                                m_ReconnectTime = 30;
                            }
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        {
                            MessageType type = (MessageType)im.ReadByte();

                            if (type == MessageType.CreateServer)
                            {
                                // Globals.ServerCode = im.ReadString();
                            }
                            else if (type == MessageType.JoinServer)
                            {
                                bool success = im.ReadBoolean();

                                if (success)
                                {
                                    SRSingleton<NetworkClient>.Instance.Connect(im.ReadString(), im.ReadInt32(), Globals.Username);
                                }
                                else
                                {
                                    MultiplayerUI.Instance.ConnectResponse(MultiplayerUI.ConnectError.InvalidServerCode);
                                }
                            }
                        }
                        break;
                    default:
                        Debug.Log("[MasterServer] " + "Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes " + im.DeliveryMethod + "|" + im.SequenceChannel);
                        break;
                }
                m_Client.Recycle(im);
            }
        }

        public void UpdateName(string name)
        {
            NetOutgoingMessage om = CreateMessage();
            om.Write((byte)MessageType.UpdateName);
            om.Write(name);

            SendMessage(om);
        }

        public void JoinServer(string code)
        {
            if (code.StartsWith("STEAM"))
            {
                if (SteamMain.FinishedSetup)
                {
                    SteamNetworkingClass.GetLobbies();
                    var server = SteamNetworkingClass.GetServerByCode(code);
                    Debug.Log(server.ToString());
                    SteamMatchmaking.JoinLobby(server);
                }
                return;
            }
            NetOutgoingMessage regMsg = CreateMessage();
            regMsg.Write((byte)MessageType.JoinServer);
            IPAddress mask;
            IPAddress adr = NetUtility.GetMyAddress(out mask);
            regMsg.Write(UniqueIdentifier);
            regMsg.Write(code);
            regMsg.Write(new IPEndPoint(adr, NetworkClient.Instance.Port));
            Debug.Log("Sending join msg to master server");
            NetworkClient.Instance.SendUnconnected(regMsg, NetUtility.Resolve(MasterServerIP, MasterServerPort));
        }

        public void CreateServer(int port)
        {
            NetOutgoingMessage regMsg = CreateMessage();
            regMsg.Write((byte)MessageType.CreateServer);
            IPAddress mask;
            IPAddress adr = NetUtility.GetMyAddress(out mask);
            regMsg.Write(UniqueIdentifier);
            regMsg.Write(port);
            regMsg.Write(new IPEndPoint(adr, NetworkServer.Instance.Port));
            Debug.Log("Sending create msg to master server");
            NetworkServer.Instance.SendUnconnected(regMsg, NetUtility.Resolve(MasterServerIP, MasterServerPort));
        }

        public void UpdateServer(int players)
        {
            NetOutgoingMessage om = CreateMessage();
            om.Write((byte)MessageType.UpdateServer);
            om.Write(players);

            SendMessage(om);
        }

        public void DeleteServer()
        {
            NetOutgoingMessage om = CreateMessage();
            om.Write((byte)MessageType.DeleteServer);

            SendMessage(om);
        }

        private void SendMessage(NetOutgoingMessage om)
        {
            if (m_Client == null || m_Client.ConnectionStatus != NetConnectionStatus.Connected) return;

            m_Client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);
        }

        private NetOutgoingMessage CreateMessage()
        {
            return m_Client?.CreateMessage();
        }
    }
}
