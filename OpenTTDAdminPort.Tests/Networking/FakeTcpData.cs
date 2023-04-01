using OpenTTDAdminPort.Messages;

namespace OpenTTDAdminPort.Tests.Networking;

internal record FakeTcpData
    (
        AdminServerProtocolMessage ProtocolMessage,
        AdminServerWelcomeMessage WelcomeMessage
    );
