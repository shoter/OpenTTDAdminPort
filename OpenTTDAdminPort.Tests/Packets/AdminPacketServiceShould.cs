using Moq;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Networking;
using OpenTTDAdminPort.Packets;
using OpenTTDAdminPort.Packets.MessageTransformers;
using OpenTTDAdminPort.Packets.PacketTransformers;
using Xunit;

namespace OpenTTDAdminPort.Tests.Packets
{
    public class AdminPacketServiceShould
    {
        [Fact]
        public void ThrowException_WhenCreatePacketGotPacketsThatItCannotHandle()
        {
            // no transformers? then it cannot handle anything xD
            var adminPacketService = new AdminPacketService(new IPacketTransformer[0], new IMessageTransformer[0]);

            var adminMessageMock = new Mock<IAdminMessage>();
            adminMessageMock.SetupGet(x => x.MessageType).Returns(AdminMessageType.ADMIN_PACKET_ADMIN_CHAT);

            Assert.Throws<AdminPortException>(() => adminPacketService.CreatePacket(adminMessageMock.Object));
        }

        [Fact]
        public void ThrowException_WhenReadPacketGotPacketItCannotHandle()
        {
            var adminPacketService = new AdminPacketService(new IPacketTransformer[0], new IMessageTransformer[0]);

            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_BANNED);
            packet.PrepareToSend();

            Assert.Throws<AdminPortException>(() => adminPacketService.ReadPacket(packet));
        }

        [Fact]
        public void ReadPacket_WhenItCanCreateMessageBasedOnPacket()
        {
            var messageTransformer = new Mock<IPacketTransformer>();
            messageTransformer.SetupGet(x => x.SupportedMessageType).Returns(AdminMessageType.ADMIN_PACKET_SERVER_BANNED);
            IAdminMessage adminMessage = Mock.Of<IAdminMessage>();
            messageTransformer.Setup(x => x.Transform(It.IsAny<Packet>())).Returns(adminMessage);

            var adminPacketService = new AdminPacketService(new IPacketTransformer[] { messageTransformer.Object }, new IMessageTransformer[0]);
            Packet packet = new Packet();
            packet.SendByte((byte)AdminMessageType.ADMIN_PACKET_SERVER_BANNED);
            packet.PrepareToSend();

            Assert.Same(adminMessage, adminPacketService.ReadPacket(packet));
        }

        [Fact]
        public void CreatePacket_WhenItCanReadSpecificMessageType()
        {
            var messageTransformer = new Mock<IMessageTransformer>();
            Packet packet = new Packet();

            messageTransformer.SetupGet(x => x.SupportedMessageType).Returns(AdminMessageType.ADMIN_PACKET_ADMIN_CHAT);
            messageTransformer.Setup(x => x.Transform(It.Ref<IAdminMessage>.IsAny)).Returns(packet);

            var adminPacketService = new AdminPacketService(new IPacketTransformer[0], new IMessageTransformer[] { messageTransformer.Object });
            var adminMessageMock = new Mock<IAdminMessage>();
            adminMessageMock.SetupGet(x => x.MessageType).Returns(AdminMessageType.ADMIN_PACKET_ADMIN_CHAT);


            Assert.Same(packet, adminPacketService.CreatePacket(adminMessageMock.Object));
        }
    }
}
