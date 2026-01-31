using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerEnableEncryptionMessageTransformer : IPacketTransformer<AdminServerEnableEncryptionMessage>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_ENABLE_ENCRYPTION;

        public IAdminMessage Transform(Packet packet) => new AdminServerEnableEncryptionMessage(packet.ReadBytes(24));
    }
}