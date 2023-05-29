using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using UnityEngine;

namespace SRMultiplayer.Packets
{
    public abstract class Packet : IPacket
    {
        public virtual void Deserialize(NetIncomingMessage im)
        {
            im.ReadAllFields(this, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        }

        public virtual void Serialize(NetOutgoingMessage om)
        {
            om.Write((ushort)GetPacketType());
            om.WriteAllFields(this, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        }

        public PacketType GetPacketType()
        {
            PacketAttribute packet = (PacketAttribute)Attribute.GetCustomAttribute(GetType(), typeof(PacketAttribute));
            if (packet != null)
            {
                return packet.Type;
            }
            else
            {
                SRMP.Log($"Packet {GetType()} has no attribute");
                return PacketType.Unknown;
            }
        }
    }
}
