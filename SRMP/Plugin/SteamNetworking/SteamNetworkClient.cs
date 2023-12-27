using Lidgren.Network;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMultiplayer.Plugin.SteamNetworking
{
    public class SteamNetClient : NetClient
    {
        public CSteamID steamID;
        public SteamNetClient(CSteamID ID, NetPeerConfiguration config) : base(config) { steamID = ID; }
    }
}
