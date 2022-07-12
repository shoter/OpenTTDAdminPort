using System.Collections.Generic;
using System.Linq;

using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets.MessageTransformers;
using OpenTTDAdminPort.Packets.PacketTransformers;

namespace OpenTTDAdminPort.Packets
{
    internal class AdminPacketService : IAdminPacketService
    {
        private Dictionary<AdminMessageType, IPacketTransformer> PacketReaders { get; }

        private Dictionary<AdminMessageType, IMessageTransformer> PacketCreators { get; }

        public AdminPacketService(IEnumerable<IPacketTransformer> packetTransformers, IEnumerable<IMessageTransformer> messageTransformers)
        {
            PacketReaders = packetTransformers.ToDictionary(pt => pt.SupportedMessageType);
            PacketCreators = messageTransformers.ToDictionary(mt => mt.SupportedMessageType);
        }

        public Packet CreatePacket(in IAdminMessage message)
        {
            if (!PacketCreators.ContainsKey(message.MessageType))
            {
                throw new AdminPortException($"Reading message {message.MessageType} is currently not handled by Admin Port Client");
            }

            Packet packet = PacketCreators[message.MessageType].Transform(message);
            packet.PrepareToSend();
            return packet;
        }

        public IAdminMessage ReadPacket(Packet packet)
        {
            var type = (AdminMessageType)packet.ReadByte();

            if (!PacketReaders.ContainsKey(type))
            {
                throw new AdminPortException($"Creating message {type} is currently not handled by Admin Port Client");
            }

            return this.PacketReaders[type].Transform(packet);
        }
    }
}
