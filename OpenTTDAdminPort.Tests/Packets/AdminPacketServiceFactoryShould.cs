using OpenTTDAdminPort.Game;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTTDAdminPort.Tests.Packets
{
    public class AdminPacketServiceFactoryShould
    {
        [Fact]
        public void CreateAdminPacketService_ThatCanHandleAdminMessage()
        {
            IAdminMessage msg = new AdminChatMessage(NetworkAction.NETWORK_ACTION_CHAT, ChatDestination.DESTTYPE_BROADCAST, 0, "MSG");
            Packet packet = new AdminPacketServiceFactory().Create().CreatePacket(in msg);

            packet.PrepareToSend();
            Assert.Equal((byte)AdminMessageType.ADMIN_PACKET_ADMIN_CHAT, packet.ReadByte());
            Assert.Equal((byte)NetworkAction.NETWORK_ACTION_CHAT, packet.ReadByte());
            Assert.Equal((byte)ChatDestination.DESTTYPE_BROADCAST, packet.ReadByte());
            Assert.Equal(0u, packet.ReadU32());
            Assert.Equal("MSG", packet.ReadString());
        }

        [Fact]
        public void CreateAdminPacketService_ThatCanHandlePacketWithMessage()
        {
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_CLIENT_JOIN);
            packet.SendU32(50u);
            packet.PrepareToSend();

            var msg = new AdminPacketServiceFactory()
                .Create().ReadPacket(packet) as AdminServerClientJoinMessage;

            Assert.Equal(50u, msg.ClientId);
        }
    }
}
