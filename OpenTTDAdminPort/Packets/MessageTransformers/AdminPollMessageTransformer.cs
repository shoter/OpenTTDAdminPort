using OpenTTDAdminPort.Messaging;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.MessageTransformers
{
    internal class AdminPollMessageTransformer : IMessageTransformer<AdminPollMessage>
    {
        /// <summary>
        /// Gets the type of the supported message.
        /// </summary>
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_ADMIN_POLL;

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

            var msg = (AdminPollMessage)message;
            packet.SendByte((byte)msg.UpdateType);
            packet.SendU32(msg.Argument);

            return packet;
        }
    }
}
