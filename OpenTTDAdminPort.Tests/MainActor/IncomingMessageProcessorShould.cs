using AutoFixture;
using OpenTTDAdminPort.MainActor;
using OpenTTDAdminPort.MainActor.StateData;
using OpenTTDAdminPort.Messages;
using OpenTTDAdminPort.Tests.Cusomizations;
using Xunit;

namespace OpenTTDAdminPort.Tests.MainActor
{
    public class IncomingMessageProcessorShould
    {
        private IFixture fix = new DefaultFixture();
        private IIncomingMessageProcessor messageProcessor = new IncomingMessageProcessor();

        [Fact]
        public void Should_ProcessServerDateMessage()
        {
            var initialData = fix.Create<ConnectedData>();
            var adminServerDateMessage = fix.Create<AdminServerDateMessage>();

            var newData = messageProcessor.ProcessAdminMessage(
                initialData,
                adminServerDateMessage);

            Assert.NotEqual(initialData, newData);
            Assert.Equal(adminServerDateMessage.Date,
                newData.AdminServerInfo.Date);
        }
    }
}