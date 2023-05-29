using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRMultiplayer.Packets
{
    public interface IPacket
    {
        PacketType GetPacketType();
        void Serialize(NetOutgoingMessage om);
        void Deserialize(NetIncomingMessage im);
    }
}
