using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;

namespace OpenTTDAdminPort.Packets.PacketTransformers
{
    internal class AdminServerAuthRequestTransformer : IPacketTransformer<AdminServerAuthRequest>
    {
        public AdminMessageType SupportedMessageType => AdminMessageType.ADMIN_PACKET_SERVER_AUTH_REQUEST;

        public IAdminMessage Transform(Packet packet)
        {
            // Ignore PAKE byte
            packet.ReadByte();
            return new AdminServerAuthRequest(
                packet.ReadBytes(AdminPortCrypto.X25519_KEY_SIZE),
                packet.ReadBytes(AdminPortCrypto.X25519_NONCE_SIZE));
        }
    }
}