using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using System;
using System.Collections.Generic;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerProtocolPacketTransformer : IPacketTransformer<AdminServerProtocolMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_PROTOCOL;

        public IAdminMessage Transform(Packet packet)
        {
            var dic = new Dictionary<AdminUpdateType, UpdateFrequency>();
            byte version = packet.ReadByte();

            while (packet.ReadBool())
            {
                AdminUpdateType updateType = (AdminUpdateType)packet.ReadU16();
                ushort frequency = packet.ReadU16();
                dic.Add(updateType, (UpdateFrequency)frequency);
            }

            return new AdminServerProtocolMessage(version, dic);
        }
    }
}
