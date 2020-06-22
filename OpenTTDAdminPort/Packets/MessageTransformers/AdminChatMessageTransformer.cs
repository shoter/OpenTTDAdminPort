using OpenTTDAdminPort.Messaging;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.MessageTransformers
{
    internal class AdminChatMessageTransformer : IMessageTransformer<AdminChatMessage>
    {
        /// <summary>
        /// Gets the type of the supported message.
        /// </summary>
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_ADMIN_CHAT;

        /// <summary>
        /// Reads the admin message and transforms it into packet.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>
        /// Transformed message
        /// </returns>
        public Packet Transform(in IAdminMessage message)
        {
            Packet packet = new Packet();
            packet.SendByte((byte)message.MessageType);

            var msg = (AdminChatMessage)message;
            packet.SendByte((byte)msg.NetworkAction);
            packet.SendByte((byte)msg.ChatDestination);
            packet.SendU32(msg.Destination);
            packet.SendString(msg.Message, 900);

            return packet;
        }
    }
}
