using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMultiplayer.Packets
{
    [Packet(PacketType.DroneAmmoAdd)]
    public class PacketDroneAmmoAdd : Packet
    {
        public string ID;
        public ushort Ident;

        public PacketDroneAmmoAdd() { }
        public PacketDroneAmmoAdd(NetIncomingMessage im) { Deserialize(im); }
    }
}
