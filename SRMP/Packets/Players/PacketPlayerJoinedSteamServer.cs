using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRMultiplayer.Packets
{
    [Packet(PacketType.SteamPlayerJoined)]
    public class PacketPlayerJoinedSteamServer : Packet
    {
        public int version;
        public string Username;
        public ulong SteamID;
        public Guid uuid;
        public List<string> mods;
        public List<DLCPackage.Id> DLCs;

        public PacketPlayerJoinedSteamServer() { }
        public PacketPlayerJoinedSteamServer(NetIncomingMessage im) { Deserialize(im); }
    }
}
