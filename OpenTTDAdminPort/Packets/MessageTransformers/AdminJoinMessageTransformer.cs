using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.MessageTransformers
{
    internal class AdminJoinMessageTransformer : IMessageTransformer<AdminJoinMessage>
    {
        /// <summary>
        /// Gets the type of the supported message.
        /// </summary>
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_ADMIN_JOIN;

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

            var msg = (AdminJoinMessage)message;
            packet.SendString(msg.Password, 33);
            packet.SendString(msg.AdminName, 25);
            packet.SendString(msg.AdminVersion, 33);

            return packet;
        }
    }
}
