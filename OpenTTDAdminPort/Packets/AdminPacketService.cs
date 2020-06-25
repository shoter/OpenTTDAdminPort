using OpenTTDAdminPort.Assemblies;
using OpenTTDAdminPort.Common;
using OpenTTDAdminPort.Common.Assemblies;
using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets.MessageTransformers;
using OpenTTDAdminPort.Packets.PacketTransformers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenTTDAdminPort.Packets
{
    internal class AdminPacketService : IAdminPacketService
    {
        private Dictionary<AdminMessageType, IPacketTransformer> packetReaders { get; } 
        private Dictionary<AdminMessageType, IMessageTransformer> packetCreators { get; }

        public AdminPacketService(IEnumerable<IPacketTransformer> packetTransformers, IEnumerable<IMessageTransformer> messageTransformers)
        {
            packetReaders = packetTransformers.ToDictionary(pt => pt.SupportedMessageType);
            packetCreators = messageTransformers.ToDictionary(mt => mt.SupportedMessageType);
        }

        public Packet CreatePacket(in IAdminMessage message)
        {
            if (!packetCreators.ContainsKey(message.MessageType))
                throw new AdminPortException($"Reading message {message.MessageType} is currently not handled by Admin Port Client");

            Packet packet = packetCreators[message.MessageType].Transform(message);
            packet.PrepareToSend();
            return packet;
        }

        public IAdminMessage ReadPacket(Packet packet)
        {
            var type = (AdminMessageType)packet.ReadByte();

            if (!packetReaders.ContainsKey(type))
                throw new AdminPortException($"Creating message {type} is currently not handled by Admin Port Client");

            return this.packetReaders[type].Transform(packet);
        }
    }
}
