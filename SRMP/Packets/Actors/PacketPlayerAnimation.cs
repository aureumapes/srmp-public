using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMultiplayer.Packets
{
    [Packet(PacketType.PlayerAnimation)]
    public class PacketPlayerAnimation : Packet
    {
        public enum AnimationType : int
        {
            Layer,
            Speed,
            Parameters

        }

        public byte ID;
        public byte Type;
        public NetBuffer internalData;
        /// <summary>
        /// mark construction inheritance incase we need it
        /// </summary>
        public PacketPlayerAnimation():base() { }
        /// <summary>
        /// mark construction inheritance so the deserialization automatically happens for is
        /// since the base decalres this
        /// </summary>
        public PacketPlayerAnimation(NetIncomingMessage im):base(im) { }
    }

}
