using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.MessageTransformers
{
    internal class AdminUpdateFrequencyTransformer : IMessageTransformer<AdminUpdateFrequencyMessage>
    {
        /// <summary>
        /// Gets the type of the supported message.
        /// </summary>
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_ADMIN_UPDATE_FREQUENCY;

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

            var msg = (AdminUpdateFrequencyMessage)message;
            packet.SendU16((ushort)msg.UpdateType);
            packet.SendU16((ushort)msg.UpdateFrequency);

            return packet;
        }
    }
}
