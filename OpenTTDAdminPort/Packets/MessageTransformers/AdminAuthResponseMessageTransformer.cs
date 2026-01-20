using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.MessageTransformers
{
    internal class AdminAuthResponseMessageTransformer : IMessageTransformer<AdminAuthResponseMessage>
    {
        /// <summary>
        /// Gets the type of the supported message.
        /// </summary>
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_ADMIN_AUTH_RESPONSE;

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

            var msg = (AdminAuthResponseMessage)message;
            packet.SendBytes(msg.ClientPublicKey);
            packet.SendBytes(msg.Mac);
            packet.SendBytes(msg.CipherText);

            return packet;
        }
    }
}