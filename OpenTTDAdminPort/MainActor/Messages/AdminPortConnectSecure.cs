namespace OpenTTDAdminPort.MainActor.Messages
{
    public record AdminPortConnectSecure(
        ServerInfo ServerInfo,
        string ClientName);
}