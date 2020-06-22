using OpenTTDAdminPort.Messaging;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.MessageTransformers
{
    internal class AdminRconMessageTransformer : IMessageTransformer<AdminRconMessage>
    {
        /// <summary>
        /// Gets the type of the supported message.
        /// </summary>
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_ADMIN_RCON;

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

            var msg = (AdminRconMessage)message;
            packet.SendString(msg.Command, 500);

            return packet;
        }
    }
}
